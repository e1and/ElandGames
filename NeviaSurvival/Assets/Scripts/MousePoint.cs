using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] Collider useTrigger;

    [SerializeField] float useDistance = 2;
    public float raycastLength = 500;
    public bool isPointUI;
    public bool isUIDescription;
    public GameObject Player;
    public int stickLimit;
    public GameObject Target;
    private Animator Animator;
    private Vector3 lookDirection;
    float _distanceToTarget;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemComment;
    [SerializeField] Vector3 itemNameOffset;

    [SerializeField] Vector3 mousePosition;
    [SerializeField] Vector3 itemPanelInventoryOffset;
    [SerializeField] Vector3 itemPanelInventoryOffsetUp;
    [SerializeField] Vector3 itemPanelInventoryOffsetDown;
    [SerializeField] Vector3 itemPanelInventoryOffsetLeft;
    [SerializeField] Vector3 itemPanelInventoryOffsetRight;

    public GameObject itemInfoPanel;
    public TMP_Text itemNamePanelText;
    public TMP_Text itemDescriptionPanelText;
    Coroutine commentCoroutine;
    [SerializeField] Item Stick;
    [SerializeField] Item Axe;
    [SerializeField] Item Torch;
    [SerializeField] Item Key;
    [SerializeField] Item Cauldron;
    Inventory inventory;
    [SerializeField] InventoryWindow inventoryWindow;
    [SerializeField] RectTransform inventoryRect;

    private NavMeshAgent agent;
    private CharacterController controller;

    private void Start()
    {
        Animator = Player.GetComponent<Animator>();
        inventory = Player.GetComponent<Inventory>();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!isPointUI && Physics.Raycast(ray, out hit, raycastLength, 3))
        {
            //itemInfoPanel.SetActive(false);

            _distanceToTarget = Vector3.Distance(Player.transform.position, hit.transform.position);
            
            if (_distanceToTarget <= useDistance)
            { 
                // ��������� ���������
                if (hit.collider.TryGetComponent(out ItemInfo item))
                {
                    if (!Input.GetMouseButton(1)) // ������� �������� �������� ����� � ��������
                    {
                        itemNamePanelText.text = item.itemName;
                        itemDescriptionPanelText.text = item.itemDescription;
                        Descripion();
                        //itemNameText.text = item.itemName + " (" + _distanceToTarget.ToString("0.0") + "�)";
                        //itemNameText.transform.position = Input.mousePosition + itemNameOffset;
                    }

                    if (Input.GetMouseButtonDown(0)) // �� ������� ������ ���� ������������ �������
                    {
                        //Comment(item.itemComment); // ���� �������� ��������������� (�� ���� ��� �����)

                        if (item.isCollectible) // ���� ������� ����������, �� ��������� �������� �������
                        {
                            Collect(item, hit.collider.gameObject);
                            //StartCoroutine(GoAndCollect(item, hit.collider.gameObject));
                        }
                    }
                }
                else
                {
                    itemNameText.text = ""; // ���� ������ �� �������� �� ������� � �����, �� �� ��������
                    itemInfoPanel.SetActive(false);
                }

                // ������� ������/������
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.TryGetComponent(out DoorTrigger trigger))
                    {
                        if (_distanceToTarget <= 3 && !trigger.doorScript.isMoving)
                        {
                            Animator.SetTrigger("Use");
                            trigger.OpenDoor();
                        }
                    }
                }

                // �������� �����
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.TryGetComponent(out Door door))
                    {
                        if (_distanceToTarget <= 3)
                        {
                            Animator.SetTrigger("Use");
                            door.OpenClose();
                            if (door.isLocked) Comment("����� �������!");
                        }
                    }
                }

                // �������� ��������
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

                // ��������� ����� �������� �� �����
                if (Input.GetMouseButtonDown(0))
                {
                    //if (hit.collider.CompareTag("Ground")) 
                    //{
                    //    Debug.Log("New hit " + hit.collider.tag);
                    //    //Target.transform.rotation = new Quaternion(45, 100, 45, 0);
                    //    Target.transform.position = hit.point;
                    //    Target.GetComponent<MeshRenderer>().enabled = true;

                    //    StartCoroutine(NavMeshMove());
                    //}
                }
            }
            else
            { 
                itemNameText.text = "";
                itemInfoPanel.SetActive(false);
            }

            Debug.DrawRay(ray.origin, ray.direction * raycastLength, Color.yellow);
        }

        if (isPointUI && !Input.GetMouseButton(1))
        {
            Descripion();
        }
    }

    void Descripion()
    {
            itemNameText.text = "";

            if (isUIDescription || hit.collider.TryGetComponent(out ItemInfo item))
            {
                itemInfoPanel.SetActive(true);

                if (Input.mousePosition.y < 540) itemPanelInventoryOffset = itemPanelInventoryOffsetDown;
                else itemPanelInventoryOffset = itemPanelInventoryOffsetUp;

                if (Input.mousePosition.x < 150) itemPanelInventoryOffset += itemPanelInventoryOffsetRight;
                if (Input.mousePosition.x > 1540) itemPanelInventoryOffset += itemPanelInventoryOffsetLeft;

                itemInfoPanel.transform.position = Input.mousePosition + itemPanelInventoryOffset;
            }
            else
            {
                itemInfoPanel.SetActive(false);
            }
    }

    IEnumerator NavMeshMove()
    {
        if (Target.GetComponent<MeshRenderer>().enabled == true)
        {
            if (hit.transform != null)
            {
                while (true)
                {
                    controller.enabled = false;
                    agent.enabled = true;
                    agent.SetDestination(Target.transform.position);

                    //Animator.SetFloat("Speed", 2);
                    Animator.SetFloat("Speed", agent.speed);
                    yield return null;

                    //lookDirection = new Vector3(Target.transform.position.x, Player.transform.position.y, Target.transform.position.z);
                    //transform.LookAt(lookDirection);
                    //Player.transform.position = Vector3.MoveTowards(Player.transform.position, Target.transform.position, 2 * Player.GetComponent<StarterAssets.ThirdPersonController>().MoveSpeed * Time.deltaTime); 

                    if (Vector3.Distance(Player.transform.position, Target.transform.position) < 0.5f 
                        || GetComponent<StarterAssets.StarterAssetsInputs>().isPlayerMove
                        || GetComponent<StarterAssets.StarterAssetsInputs>().jump)
                    {
                        Debug.Log("");
                        Target.GetComponent<MeshRenderer>().enabled = false;
                        Animator.SetFloat("Speed", 0);
                        controller.enabled = true;
                        agent.enabled = false;
                        break;
                    }
                }
            }
        }
    }

    void Collect(ItemInfo item, GameObject itemObject)
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

            //Animator.SetTrigger("Grab");
            Destroy(itemObject, 0);
            inventory.Recount();
            inventoryWindow.Redraw();

            itemInfoPanel.SetActive(false);
        }
        else
        {
            Comment("������ �� �����");
        }
    }

    IEnumerator GoAndCollect(ItemInfo item, GameObject itemObject)
    {
        Debug.Log("Start Coroutine");
        Target.transform.position = itemObject.transform.position;
        Target.GetComponent<MeshRenderer>().enabled = true;
        StartCoroutine(NavMeshMove());

        while (true)
        {
            Debug.Log("MoveTime");
            yield return null;
            if (Vector3.Distance(Player.transform.position, Target.transform.position) < 1.5f || 
                GetComponent<StarterAssets.StarterAssetsInputs>().isPlayerMove ||
                GetComponent<StarterAssets.StarterAssetsInputs>().jump)
                {
                    Debug.Log("Collect");
                    Collect(item, itemObject);
                    break;
                }
        }
    
    }

    public void Comment(string comment)
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
