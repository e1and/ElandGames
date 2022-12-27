using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    [SerializeField] NPC_Move npc_move;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            npc_move.isObstacle = true;
            npc_move.obstacleHits++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            npc_move.isObstacle = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            npc_move.isObstacle = true;
            npc_move.obstacleHits++;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
