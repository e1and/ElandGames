using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DamageTrigger : MonoBehaviour
{
    public NPC_Move Monster;
    public BoxCollider box;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip fallSound;
    public float damageCooldown = 1;
    private bool _isDamageCooldown;

    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out WeaponCollider weaponCollider) && !_isDamageCooldown && Monster.currentHP > 0)
        {
            StartCoroutine(Cooldown());
            Monster.GetDamage(weaponCollider);
            Monster.player.inventoryWindow.WeaponRandomDamage(weaponCollider.weapon);
            PlayHitSound();
        }
    }

    IEnumerator Cooldown()
    {
        _isDamageCooldown = true;
        yield return new WaitForSeconds(damageCooldown);
        _isDamageCooldown = false;
    }
    
    void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }
    
    public void PlayFallSound()
    {
        StartCoroutine(FallSoundDelay());
    }

    IEnumerator FallSoundDelay()
    {
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(fallSound);
    }
}
