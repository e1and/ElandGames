using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DamageTrigger : MonoBehaviour
{
    public int MonsterHP = 2;
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
        if(other.tag == "Player" || other.gameObject.TryGetComponent(out ItemInfo info) && info.type == ItemType.Axe) 
        { 
            MonsterHP--;
            Debug.Log("Take Damage");
            Animator.SetTrigger("Damage");
            if (MonsterHP <= 0) 
            { 
                Animator.SetTrigger("Death"); 
                Monster.GetComponent<NPC_Move>().enabled = false;
                Monster.GetComponent<NavMeshAgent>().enabled = false;
                //this.enabled = false;
                StartCoroutine(MonsterCoolDown());
            } 
        }
    }

    IEnumerator MonsterCoolDown()
    {
        float timer = 0;
        while (timer < 10)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Animator.SetTrigger("GetUp");
        Monster.GetComponent<NPC_Move>().enabled = true;
        Monster.GetComponent<NavMeshAgent>().enabled = true;
        //this.enabled = true;
        MonsterHP = 2;
    }
}
