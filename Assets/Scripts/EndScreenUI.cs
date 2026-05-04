using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public string homeScene = "Home";

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene("RaceLevel"); // or your race scene name
    }

    public void LoadHome()
    {
        SceneManager.LoadScene(homeScene);
    }
}
