using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] Collider useTrigger;
    
    public float raycastLength = 500;
    public bool isPointUI;
    public GameObject Player;
    public int stickLimit;
    public GameObject Target;
    private Animator Animator;
    private Vector3 lookDirection;
    float _distanceToTarget;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemComment;
    Coroutine commentCoroutine;
    [SerializeField] Item Stick;
    [SerializeField] Item Axe;
    [SerializeField] Item Torch;
    [SerializeField] Item Key;
    [SerializeField] Item Cauldron;
    Inventory inventory;
    [SerializeField] InventoryWindow inventoryWindow;

    private void Start()
    {
        Animator = Player.GetComponent<Animator>();
        inventory = Player.GetComponent<Inventory>();
    }

    void Update()
    {
        GameObject Target = GameObject.Find("Target");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!isPointUI && Physics.Raycast(ray, out hit, raycastLength, 3))
        {
            _distanceToTarget = Vector3.Distance(Player.transform.position, hit.transform.position);

            if (hit.collider.TryGetComponent<ItemInfo>(out ItemInfo item))
            {
                itemNameText.text = item.itemName + " (" + _distanceToTarget.ToString("0.0") + "м)";

                if (Input.GetMouseButtonDown(0))
                {
                    Comment(item.itemComment);

                    if (item.isCollectible && _distanceToTarget < 1.5f)
                    {
                        if (inventory.filledSlots < inventory.size)
                        {
                            if (item.type == Items.Stick)
                            {
                                GetComponent<Inventory>().AddItem(Stick);
                                Player.GetComponent<Player>().Sticks += 1;
                            }
                            if (item.type == Items.Axe) GetComponent<Inventory>().AddItem(Axe);
                            if (item.type == Items.Torch) GetComponent<Inventory>().AddItem(Torch);
                            if (item.type == Items.Key) GetComponent<Inventory>().AddItem(Key);
                            if (item.type == Items.Cauldron) GetComponent<Inventory>().AddItem(Cauldron);

                            Animator.SetTrigger("Grab");
                            Destroy(hit.collider.gameObject, 1);
                            inventory.Recount();
                            inventoryWindow.Redraw();
                        }
                        else
                        {
                            Comment("Больше не унесу");
                        }
                    }
                }


            }
            else
            {
                itemNameText.text = "";
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.TryGetComponent<DoorTrigger>(out DoorTrigger trigger))
                {
                    if (_distanceToTarget <= 3 && !trigger.doorScript.isMoving)
                    {
                        Animator.SetTrigger("Use");
                        trigger.OpenDoor();
                    }
                }
                //if (hit.collider.CompareTag("Ground"))
                //{
                //    Debug.Log("New hit " + hit.collider.tag);
                //    //Target.transform.rotation = new Quaternion(45, 100, 45, 0);
                //    Target.GetComponent<MeshRenderer>().enabled = true;
                //    Target.transform.position = hit.point;
                //}
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.TryGetComponent(out Container container))
                {
                    if (_distanceToTarget <= 3)
                    {
                        Animator.SetTrigger("Use");
                        container.Click();
                    }
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * raycastLength, Color.yellow);


        }
        
        if (isPointUI) itemNameText.text = "";


        if (Target.GetComponent<MeshRenderer>().enabled == true)
        {
            if (hit.transform != null)
            {
                Animator.SetFloat("Speed", 2);
                lookDirection = new Vector3(Target.transform.position.x, Player.transform.position.y, Target.transform.position.z);
                transform.LookAt(lookDirection);
                Player.transform.position = Vector3.MoveTowards(Player.transform.position, Target.transform.position, 2 * Player.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed * Time.deltaTime); 
                if (Vector3.Distance(Player.transform.position, Target.transform.position) < 1f || GetComponent<StarterAssets.StarterAssetsInputs>().v2m)
                { Target.GetComponent<MeshRenderer>().enabled = false;
                    Animator.SetFloat("Speed", 0);
                }
            }
        }
    }

    void Comment(string comment)
    {
        if (commentCoroutine != null) StopCoroutine(commentCoroutine);
        itemComment.gameObject.SetActive(false);
        itemComment.text = comment;
        commentCoroutine = StartCoroutine(ShowComment());
    }

    IEnumerator ShowComment()
    {
        itemComment.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        itemComment.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<DoorTrigger>(out DoorTrigger trigger))
        {
            if (Input.GetKeyDown(KeyCode.E) && !trigger.doorScript.isMoving)
            {
                Animator.SetTrigger("Use");
                trigger.OpenDoor();
            }
        }
    }


}
