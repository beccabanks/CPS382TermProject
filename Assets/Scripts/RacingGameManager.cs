using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class RacingGameManager : MonoBehaviour
{
    public List<RacerProgress> racers = new List<RacerProgress>();
    public RacerProgress player;

    private bool raceFinished = false;

    void Update()
    {
        if (raceFinished) return;

        // Sort racers by progress
        racers = racers.OrderByDescending(r => r.GetProgressScore()).ToList();

        // Check if player finished
        if (player.HasFinished())
        {
            raceFinished = true;

            int playerRank = racers.IndexOf(player) + 1;

            Debug.Log("Player finished in place: " + playerRank);

            if (playerRank == 1)
            {
                Invoke(nameof(LoadNextLevel), 2f);
            }
            else
            {
                Invoke(nameof(RestartLevel), 2f);
            }
        }
    }

    void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}