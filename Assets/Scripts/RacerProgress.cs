using UnityEngine;

public class RacerProgress : MonoBehaviour
{
    public Transform[] waypoints;

    public int currentWaypoint = 0;
    public int currentLap = 0;
    public int totalLaps = 3;

    public float distanceToNext;

    public bool finished = false;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform wp = waypoints[currentWaypoint];
        distanceToNext = Vector3.Distance(transform.position, wp.position);

        // Waypoint reached
        if (distanceToNext < 15f)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
                currentLap++;

                if (currentLap >= totalLaps)
                {
                    finished = true;
                }
            }
        }
    }

    public float GetProgressScore()
    {
        // Higher = further ahead
        return currentLap * 10000f + currentWaypoint * 100f - distanceToNext;
    }

    public bool HasFinished()
    {
        return finished;
    }
}

