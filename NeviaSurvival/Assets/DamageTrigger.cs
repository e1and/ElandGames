using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public int MonsterHP = 3;
    public GameObject Monster;
    private Animator Animator;
    private Component MonsterAI;
    void Start()
    {
        Animator = Monster.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") 
        { 
            MonsterHP--;
            Debug.Log("Take Damage");
            if (MonsterHP <= 0) 
            { Animator.SetTrigger("Death"); Monster.GetComponent<EnemyAI>().enabled = false; this.enabled = false; } 
        }
    }
}
