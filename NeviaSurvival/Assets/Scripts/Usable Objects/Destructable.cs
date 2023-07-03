using System.Collections;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public int health = 30;
    public GameObject dropLoot;
    public int lootAmount;
    public bool isDropItems;
    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip destructionSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WeaponCollider weapon))
        {
            health -= weapon.weapon.damage;
            
            if (health <= 0)
            {
                PlayDestructionSound();
                StartCoroutine(Destruct());
            }
            else PlayHitSound();
        }
    }

    IEnumerator Destruct()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        DropLoot();
        if (isDropItems) DropRandomLoot();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    void DropLoot()
    {
        for (int i = 0; i < lootAmount; i++)
        {
            GameObject loot = Instantiate(dropLoot);
            RandomPlaceAround(loot);
        }
    }
    
    void RandomPlaceAround(GameObject item)
    {
        item.transform.position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), 0.5f, Random.Range(-1.5f, 1.5f));
        item.transform.eulerAngles = new Vector3(Random.Range(-90, 90f), Random.Range(-90, 90f), Random.Range(-90, 90f));
    }

    void DropRandomLoot()
    {
        int itemCount = Random.Range(0, 5);
        for (int i = 0; i < itemCount; i++)
        {
            Item randomItem = Game.links.itemRandomizer
                .randomItems[Random.Range(0, Game.links.itemRandomizer.randomItems.Count)];
            if (randomItem.rarity > Random.Range(0, 101))
            {
                GameObject item = Instantiate(randomItem.Prefab);
                RandomPlaceAround(item);
            }
        }
    }

    void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }
    
    void PlayDestructionSound()
    {
        audioSource.PlayOneShot(destructionSound);
    }
}
