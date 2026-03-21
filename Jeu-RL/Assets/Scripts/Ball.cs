using UnityEngine;

public class Ball : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public float speed = 500f;
    public float maxBounceAngle = 75f;
    public float minY = -8.5f;
    public float maxVelocity = 15f;

    public PaddleAgent paddleAgent;

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();

        if (transform.parent != null)
        {
            this.paddleAgent = transform.parent.GetComponentInChildren<PaddleAgent>();
        }
    }

    private void Start()
    {
        ResetBall();
    }

    private void Update()
    {
        if (this.transform.localPosition.y < minY)
        {
            if (paddleAgent != null)
            {
                paddleAgent.ResetBall();
            }
        }
    }

    public void ResetBall()
    {
        this.transform.localPosition = Vector2.zero;
        this.rigidbody.velocity = Vector2.zero;
        Invoke(nameof(setRandomTrajectory), 1f);
    }

    public void ResetPaddle()
    {
        this.transform.localPosition = new Vector2(0f, this.transform.localPosition.y);
        this.rigidbody.velocity = Vector2.zero;
    }

    private void setRandomTrajectory()
    {
        Vector2 force = Vector2.zero;
        force.x = Random.Range(-1f, 1f);
        force.y = -1f;

        this.rigidbody.AddForce(force.normalized * this.speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            if (paddleAgent != null)
            {
                paddleAgent.RewardForBreakingBrick();
            }
            collision.gameObject.SetActive(false);
        }

        Paddle humanPaddle = collision.gameObject.GetComponent<Paddle>();
        PaddleAgent aiPaddle = collision.gameObject.GetComponent<PaddleAgent>();

        if (humanPaddle != null || aiPaddle != null)
        {
            Vector3 paddlePosition = collision.transform.position;
            Vector2 contactPoint = collision.GetContact(0).point;

            float offset = paddlePosition.x - contactPoint.x;
            float width = collision.collider.bounds.size.x / 2;

            float currentAngle = Vector2.SignedAngle(Vector2.up, this.rigidbody.velocity);
            float bounceAngle = (offset / width) * this.maxBounceAngle;

            float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -this.maxBounceAngle, this.maxBounceAngle);
            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);

            this.rigidbody.velocity = rotation * Vector2.up * this.rigidbody.velocity.magnitude;
        }

        float currentSpeed = this.rigidbody.velocity.magnitude;
        Vector2 direction = this.rigidbody.velocity;

        if (Mathf.Abs(direction.y) < 2f)
        {
            direction.y = (direction.y >= 0) ? 2f : -2f;
        }

        if (Mathf.Abs(direction.x) < 2f)
        {
            direction.x = (direction.x >= 0) ? 2f : -2f;
        }

        if (currentSpeed > 0.1f)
        {
            this.rigidbody.velocity = direction.normalized * currentSpeed;
        }
    }
}