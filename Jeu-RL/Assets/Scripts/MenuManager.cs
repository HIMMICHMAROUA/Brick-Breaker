using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LancerJeuHumain()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LancerJeuAgent()
    {
        SceneManager.LoadScene("Level1 1-RL");
    }
}