using UnityEngine;
using UnityEngine.AI;

public class EnemyAI: MonoBehaviour
{
    private GameObject player = null;
    public NavMeshAgent agent;
    private void Start()
    {
        if (player == null) {
            Debug.Log("Finding player by Player tag!");
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player == null)
        {
            Debug.Log("No tagged player! Tag your player!");
        }
        /** Put the enemy on the closest NavMesh. 
         * Note that this will move the enemy's position. 
         * If you don't want to let the script the position of enemy, comment out this block.
         */
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(this.transform.position, out closestHit, 500, 1))
        {
            //Place the object to the NavMesh.
            this.transform.position = closestHit.position;
        }
        //setup agent if not assigned
        if (agent == null) {
            //If the object does not have NavMeshAgent component, add one
            if (this.gameObject.GetComponent<NavMeshAgent>() == null) {
                this.gameObject.AddComponent<NavMeshAgent>();
                Debug.Log("NavMeshAgent component added by script");
            }
            else
                Debug.Log("Using its existing NavMeshAgent component");

            //assign its own NavMeshAgent to agent
            agent = this.GetComponent<NavMeshAgent>();
            Debug.Log("Agent set to its own NavMeshAgent component");
        }
    }
    public void Update() {
        agent.destination = player.transform.position;
    }
}
