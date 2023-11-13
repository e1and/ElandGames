using UnityEngine;

public class MiniMapObjectY : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] private float distance;
    
    void Update()
    {
        gameObject.transform.position = new Vector3(Player.position.x, distance, Player.position.z);
    }
}
