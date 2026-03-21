using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PaddleAgent : Agent
{
    public Rigidbody2D rb;
    public float speed = 30f;

    [Header("Références")]
    public Transform ball;
    public GameManager gameManager;
    public GameObject texteVictoire;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();

         
        gameManager = transform.parent.GetComponentInChildren<GameManager>();

        
        if (ball == null)
        {
            ball = transform.parent.Find("Ball");
        }
    }

    public override void OnEpisodeBegin()
    {
        
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        
        if (texteVictoire != null) texteVictoire.SetActive(false);

        rb.velocity = Vector2.zero;
        this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);

        if (gameManager != null)
        {
            gameManager.ResetForAI();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);

        if (ball != null)
        {
            sensor.AddObservation(ball.localPosition);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0];
        Vector3 direction = Vector3.zero;

        if (moveAction == 1) direction = Vector3.left;
        else if (moveAction == 2) direction = Vector3.right;

        transform.Translate(direction * speed * Time.deltaTime);

        
        
        Vector3 positionCorrigee = transform.localPosition;

        
        positionCorrigee.x = Mathf.Clamp(positionCorrigee.x, -7.5f, 7.5f);

        transform.localPosition = positionCorrigee;
        

        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            discreteActions[0] = 2;
    }

    public void RewardForBreakingBrick()
    {
        AddReward(1.0f);
    }

    public void ResetBall()
    {
        SetReward(-1.0f);
        EndEpisode();
    }

    public void CheckIfWon()
    {
        // CORRECTION 4 : On compte uniquement les briques de SON arène
        Brick[] briquesRestantes = transform.parent.GetComponentsInChildren<Brick>();

        int briquesACasser = 0;

        foreach (Brick b in briquesRestantes)
        {
            if (!b.unbreakable)
            {
                briquesACasser++;
            }
        }

        if (briquesACasser == 0)
        {
            Debug.Log("Une IA A TOUT CASSÉ ! JACKPOT !");
            AddReward(10f);

            if (texteVictoire != null)
            {
                texteVictoire.SetActive(true);
            }

            

            EndEpisode();
        }
    }
}