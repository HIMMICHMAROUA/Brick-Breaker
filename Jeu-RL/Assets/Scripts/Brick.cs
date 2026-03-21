using UnityEngine;

public class Brick : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] staates;

    public bool unbreakable;

    public int points = 100;
    public int health { get; private set; }

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (!this.unbreakable)
        {
            if (this.staates == null || this.staates.Length == 0)
            {
                Debug.LogError("Staates array is empty on " + gameObject.name);
                return;
            }
            this.health = this.staates.Length;
            this.spriteRenderer.sprite = this.staates[this.health - 1];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ball")
        {
            Hit();
        }
    }

    private void Hit()
    {
        
        if (this.unbreakable) return;

        --this.health;

        if (health <= 0)
        {
            
            this.gameObject.SetActive(false);

            
            PaddleAgent agent = FindObjectOfType<PaddleAgent>();
            if (agent != null)
            {
                agent.RewardForBreakingBrick(); 
                agent.CheckIfWon();             
            }
        }
        else
        {
            this.spriteRenderer.sprite = this.staates[this.health - 1];
        }

        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Hit(this);
        }
    }
}