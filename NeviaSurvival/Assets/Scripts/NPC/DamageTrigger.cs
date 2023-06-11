using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DamageTrigger : MonoBehaviour
{
    public int MonsterHP = 2;
    public NPC_Move Monster;
    private Animator Animator;
    public BoxCollider box;
    float timer = 0;

    void Start()
    {
        Animator = Monster.GetComponent<Animator>();
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out WeaponCollider weaponCollider) && MonsterHP > 0) 
        { 
            MonsterHP -= weaponCollider.weapon.damage;
            Debug.Log("Take Damage");
            Animator.SetTrigger("Damage");
            if (MonsterHP <= 0) 
            { 
                Animator.SetTrigger("Death"); 
                Monster.enabled = false;
                Monster.GetComponent<NavMeshAgent>().enabled = false;
                StopAllCoroutines();
                StartCoroutine(MonsterCoolDown());
            } 
        }
    }

    IEnumerator MonsterCoolDown()
    {
        timer = 0;
        while (timer < 10)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Animator.SetTrigger("GetUp");
        Monster.enabled = true;
        Monster.GetComponent<NavMeshAgent>().enabled = true;
        MonsterHP = 2;
        Monster._isAttack = false;
        box.enabled = false;
    }
}
