using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Ball ball { get; private set; }
    public Paddle paddle { get; private set; }
    public Brick[] bricks { get; private set; }
    public int score = 0;
    public int lives = 3;
    public int level = 1;

    [Header("Mode Intelligence Artificielle")]
    public bool isTrainingAI = true;

    private void Awake()
    {
        Instance = this;

        
        if (!isTrainingAI)
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnLevelLoaded;
        }
    }

    private void Start()
    {
        if (isTrainingAI)
        {
            
            this.ball = transform.parent.GetComponentInChildren<Ball>();
            this.paddle = transform.parent.GetComponentInChildren<Paddle>();
            this.bricks = transform.parent.GetComponentsInChildren<Brick>();
        }
        else
        {
            
            NewGame();
        }
    }

    private void NewGame()
    {
        this.score = 0;
        this.lives = 3;
        LoadLevel(1);
    }

    private void LoadLevel(int level)
    {
        this.level = level;
        SceneManager.LoadScene("Level" + level);
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        this.ball = FindObjectOfType<Ball>();
        this.paddle = FindObjectOfType<Paddle>();
        this.bricks = FindObjectsOfType<Brick>();
    }

    private void ResetLevel()
    {
        this.ball.ResetBall();
        this.ball.ResetPaddle();
    }

    public void Hit(Brick brick)
    {
        this.score += brick.points;

        if (Cleared())
        {
            if (isTrainingAI)
            {
                
                PaddleAgent agent = transform.parent.GetComponentInChildren<PaddleAgent>();
                if (agent != null) agent.EndEpisode();
            }
            else
            {
                LoadLevel(this.level + 1);
            }
        }
    }

    private bool Cleared()
    {
        foreach (Brick b in bricks)
        {
            if (b.gameObject.activeInHierarchy && !b.unbreakable)
            {
                return false;
            }
        }
        return true;
    }

    public void Miss()
    {
        if (isTrainingAI) return; 

        --this.lives;

        if (this.lives > 0)
        {
            ResetLevel();
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        NewGame();
    }

    
    public void ResetForAI()
    {
        this.score = 0;

        if (this.bricks != null)
        {
            foreach (Brick b in this.bricks)
            {
                if (b != null)
                {
                    b.gameObject.SetActive(true);
                }
            }
        }

        if (this.ball != null)
        {
            this.ball.ResetBall();
        }
    }
}