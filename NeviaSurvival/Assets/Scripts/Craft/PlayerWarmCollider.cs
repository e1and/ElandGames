using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWarmCollider : MonoBehaviour
{
    public Player player;
    void Start()
    {
        player = GetComponentInParent<Player>();
    }
}
