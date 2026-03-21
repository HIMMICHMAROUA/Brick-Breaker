using UnityEngine;

public class LoseZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Ball") || collision.gameObject.name == "Ball")
        {
            
            PaddleAgent agent = FindObjectOfType<PaddleAgent>();

            if (agent != null)
            {
                
                agent.ResetBall();
            }
        }
    }
}