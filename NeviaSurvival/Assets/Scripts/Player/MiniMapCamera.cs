using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] private float distance;
    
    void Update()
    {
        gameObject.transform.position = new Vector3(Player.position.x, distance, Player.position.z);
    }
}
