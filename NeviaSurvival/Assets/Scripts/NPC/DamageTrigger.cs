using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DamageTrigger : MonoBehaviour
{
    public NPC_Move Monster;
    public BoxCollider box;

    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out WeaponCollider weaponCollider) && Monster.currentHP > 0) 
        {
            Monster.GetDamage(weaponCollider);
        }
    }
}
