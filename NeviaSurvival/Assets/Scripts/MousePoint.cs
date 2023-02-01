using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Cinemachine;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    [SerializeField] Collider useTrigger;

    [SerializeField] float useDistance = 2;
    public float raycastLength = 500;
    public bool isPointUI;
    public bool isUIDescription;
    public bool isCarry;
    public Transform carryParent;
    public Transform carryDropParent;
    public GameObject carryObject;

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
    [SerializeField] Item BlueKey;
    [SerializeField] Item Cauldron;
    Inventory inventory;
    [SerializeField] InventoryWindow inventoryWindow;
    [SerializeField] RectTransform inventoryRect;
    [SerializeField] Image openingIndicator;
    Coroutine openingCoroutine;
    public StarterAssetsInputs inputs;
    BuildCampFire campFire;

    [SerializeField] GameObject buildPlaceParent;

    private NavMeshAgent agent;
    private CharacterController controller;

    [SerializeField] CinemachineVirtualCamera cinemachine;

    private void Start()
    {
        Animator = Player.GetComponent<ThirdPersonController>()._animator;
        inventory = Player.GetComponent<Inventory>();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        inputs = GetComponent<StarterAssetsInputs>();
        campFire = GetComponent<BuildCampFire>();
            
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Plus))
        //    { cinemachine. CameraDistance++; }
        //if (Input.GetKeyDown(KeyCode.Minus))
        //{ cinemachine.CameraDistance--; }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.G) && isCarry)
        {
            carryObject.transform.parent = carryDropParent;
            carryObject.GetComponent<Rigidbody>().isKinematic = false;
            isCarry = false;
            Animator.SetBool("Carry", false);
            Player.GetComponent<CharacterController>().radius = 0.28f;
            Player.GetComponent<CharacterController>().center = new Vector3(0, 0.93f, 0f);
        }

        mousePosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!isPointUI && Physics.Raycast(ray, out hit, raycastLength, 3))
        {
            campFire.buildingPlace = hit.point;
            
            //itemInfoPanel.SetActive(false);

            _distanceToTarget = Vector3.Distance(Player.transform.position, hit.transform.position);

            if (_distanceToTarget <= useDistance)
            {
                // Индикация предметов
                if (hit.collider.TryGetComponent(out ItemInfo item))
                {
                    if (!Input.GetMouseButton(1)) // Выводим название предмета рядом с курсором
                    {
                        itemNamePanelText.text = item.itemName;
                        itemDescriptionPanelText.text = item.itemDescription;
                        Descripion();
                        //itemNameText.text = item.itemName + " (" + _distanceToTarget.ToString("0.0") + "м)";
                        //itemNameText.transform.position = Input.mousePosition + itemNameOffset;
                    }

                    if (Input.GetMouseButtonDown(0)) // По нажатию кнопки мыши комментируем предмет
                    {
                        //Comment(item.itemComment); // Пока отключил комментирование (не факт что нужно)

                        if (item.isCollectible && !isCarry) // Если предмет собираемый, то запускаем корутину подбора
                        {
                            Collect(item, hit.collider.gameObject);
                            //StartCoroutine(GoAndCollect(item, hit.collider.gameObject));
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.G))
                    { 
                        if (item.isCarrying && !isCarry)
                        {
                            isCarry = true;
                            carryObject = item.gameObject;
                            Animator.SetTrigger("CarryTrigger");
                            Animator.SetBool("Carry", true);
                            carryObject.transform.parent = carryParent;
                            carryObject.transform.position = Vector3.zero;
                            carryObject.transform.rotation = new Quaternion(0, 0, 0, 0);
                            carryObject.transform.localPosition = item.carryPosition;
                            carryObject.transform.localRotation = item.carryRotation;
                            carryObject.GetComponent<Rigidbody>().isKinematic = true;
                            Player.GetComponent<CharacterController>().radius = 0.6f;
                            Player.GetComponent<CharacterController>().center = new Vector3(0, 0.93f, 0.49f);
                        }
                    }
                }
                else
                {
                    itemNameText.text = ""; // Если курсор не попадает на предмет с инфой, то ее обнуляем
                    itemInfoPanel.SetActive(false);
                }

                // Нажатие рычага/кнопки
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

                // Открытие двери
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.TryGetComponent(out Door door))
                    {
                        if (_distanceToTarget <= 3)
                        {
                            Animator.SetTrigger("Use");

                            if (door.SearchingKey())
                            openingCoroutine = StartCoroutine(OpeningDoor(door));

                        }
                    }
                }

                // Открытие сундуков
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.TryGetComponent(out Container container))
                    {
                        if (_distanceToTarget <= 3 && openingCoroutine == null)
                        {
                            Animator.SetTrigger("Use");
                            openingCoroutine = StartCoroutine(OpeningContainer(container));
                        }
                    }
                }

                // Установка точки маршрута на земле
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

    IEnumerator OpeningContainer(Container container)
    {
        float time = 0;
        while (time < container.openingTime)
        {
            if (container.isOpen) break;
            time += Time.deltaTime;
            openingIndicator.fillAmount = time / container.openingTime;
            yield return null;
        }
        container.Click();
        openingIndicator.fillAmount = 0;
        openingCoroutine = null;
    }

    IEnumerator OpeningDoor(Door door)
    {
        float time = 0;
        while (time < door.openingTime)
        {
            if (!door.isLocked) break;
            time += Time.deltaTime;
            openingIndicator.fillAmount = time / door.openingTime;
            yield return null;
        }
        door.isLocked = false;
        door.OpenClose();
        openingIndicator.fillAmount = 0;
        openingCoroutine = null;
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
                        || inputs.isPlayerMove || inputs.jump)
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

    void SendObjectToSlot(Item item, GameObject itemObject)
    {
        for (int i = 0; i < inventory.inventoryItems.Count; i++)
        {
            if (inventory.inventoryItems[i] == item && inventory.inventoryItemObjects[i] == null)
            {
                inventory.inventoryItemObjects[i] = itemObject;
                inventory.inventoryItemObjects[i].SetActive(false);
                return;
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
                SendObjectToSlot(Stick, itemObject);
            }
            if (item.type == Items.Axe) { GetComponent<Inventory>().AddItem(Axe); SendObjectToSlot(Axe, itemObject); }
            if (item.type == Items.Torch) { GetComponent<Inventory>().AddItem(Torch); SendObjectToSlot(Torch, itemObject); }
            if (item.type == Items.Key) { GetComponent<Inventory>().AddItem(Key); SendObjectToSlot(Key, itemObject); }
            if (item.type == Items.BlueKey) { GetComponent<Inventory>().AddItem(BlueKey); SendObjectToSlot(BlueKey, itemObject); }
            if (item.type == Items.Cauldron) { GetComponent<Inventory>().AddItem(Cauldron); SendObjectToSlot(Cauldron, itemObject); }

        //Animator.SetTrigger("Grab");
        inventory.Recount();
            inventoryWindow.Redraw();

            itemInfoPanel.SetActive(false);
        }
        else
        {
            Comment("Больше не унесу");
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
                GetComponent<StarterAssetsInputs>().isPlayerMove ||
                GetComponent<StarterAssetsInputs>().jump)
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

