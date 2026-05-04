using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class RacingGameManager : MonoBehaviour
{
    [Header("Racers")]
    public List<RacerProgress> racers = new List<RacerProgress>();
    public RacerProgress player;

    [Header("Scenes")]
    public string homeScene = "Home";
    public string winScene = "Win";
    public string loseScene = "Lose";

    private bool raceFinished = false;
    bool isRaceScene = false;
    private List<RacerProgress> finishOrder = new List<RacerProgress>();

    void Update()
    {
        if (!isRaceScene) return;
        if (raceFinished) return;

        // Sort racers by progress
        racers = racers
            .OrderByDescending(r => r.GetProgressScore())
            .ToList();

        // Check if player finished (last waypoint + lap logic)
        if (player.HasFinished())
        {
            foreach (var racer in racers)
            {
                if (racer.HasFinished() && !finishOrder.Contains(racer))
                {
                    finishOrder.Add(racer);
                    Debug.Log(racer.name + " finished at position " + finishOrder.Count);
                }
            }
        }

        if (player.HasFinished() && !raceFinished)
        {
            raceFinished = true;

            int playerRank = finishOrder.IndexOf(player) + 1;

            Debug.Log("Player finished in place: " + playerRank);

            if (playerRank == 1)
                HandleWin();
            else
                HandleLose();
        }
    }

    // =========================
    // WIN / LOSE HANDLERS
    // =========================

    void HandleWin()
    {
        Debug.Log("WIN → loading Win scene");
        SceneManager.LoadScene(winScene);
    }

    void HandleLose()
    {
        Debug.Log("LOSE → loading Lose scene");
        SceneManager.LoadScene(loseScene);
    }

    // =========================
    // UI BUTTON FUNCTIONS
    // =========================

    // 🏠 Home button (from Win/Lose scenes)

    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Level 1 build index
    }

    public void LoadHome()
    {
        SceneManager.LoadScene(homeScene);
    }

    // 🔁 Retry button (from Lose scene)
    public void RetryLevel()
    {
        int previousLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        SceneManager.LoadScene(previousLevel);
    }

    // ➡️ Next Level button (from Win scene)
    public void LoadNextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        int nextLevel = currentLevel + 1;

        if (nextLevel <= 3) // levels 1–3
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            Debug.Log("All levels complete → returning to Home");
            SceneManager.LoadScene(homeScene);
        }
    }

    // =========================
    // TRACK CURRENT LEVEL
    // =========================

    void Start()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        // Assuming:
        // 0 = Home, 1–3 = Levels, 4 = Win, 5 = Lose
        isRaceScene = (currentIndex >= 1 && currentIndex <= 3);

        // Store level for retry/next
        if (isRaceScene)
        {
            PlayerPrefs.SetInt("LastPlayedLevel", currentIndex);
        }
    }
}