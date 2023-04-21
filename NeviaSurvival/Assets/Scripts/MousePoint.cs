using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Cinemachine;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;

    [SerializeField] float useDistance = 2;
    public float raycastLength = 500;
    public bool isPointUI;
    public bool isUIDescription;
    public bool isCarry;

    public Transform carryParent;
    public Transform carryDropParent;
    public GameObject carryObject;
    public float carryWeight;
    float carryCoolDown;

    public GameObject Player;
    Player player;
    QuestWindow questWindow;
    public int stickLimit;
    public GameObject Target;
    private Animator Animator;
    private Vector3 lookDirection;
    float _distanceToTarget;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] TMP_Text itemComment;
    [SerializeField] Vector3 itemNameOffset;

    Vector3 mousePosition;
    [SerializeField] Vector3 itemPanelInventoryOffset;
    [SerializeField] Vector3 itemPanelInventoryOffsetUp;
    [SerializeField] Vector3 itemPanelInventoryOffsetDown;
    [SerializeField] Vector3 itemPanelInventoryOffsetLeft;
    [SerializeField] Vector3 itemPanelInventoryOffsetRight;
    [Space]
    public GameObject itemInfoPanel;
    public TMP_Text itemNamePanelText;
    public TMP_Text itemDescriptionPanelText;
    public TMP_Text itemCommentPanelText;
    public TMP_Text itemActionPanelText;
    [Space]
    public GameObject item3dInfoPanel;
    public TMP_Text item3dNamePanelText;
    public TMP_Text item3dDescriptionPanelText;
    public TMP_Text item3dActionPanelText;
    [Space]
    public GameObject IconHighLight;
    [Space]
    public ItemInfo pointedIcon;

    Coroutine commentCoroutine;
    [Space]
    [SerializeField] Item Stick;
    [SerializeField] Item Axe;
    [SerializeField] Item Torch;
    [SerializeField] Item Key;
    [SerializeField] Item BlueKey;
    [SerializeField] Item Cauldron;
    [SerializeField] Item RedMushroom;
    [SerializeField] Item BackPack;
    [SerializeField] Item Scroll;
    [Space]

    Inventory inventory;
    [SerializeField] InventoryWindow inventoryWindow;
    [SerializeField] RectTransform inventoryRect;
    [SerializeField] Image progressIndicator;
    Coroutine openingCoroutine;
    public StarterAssetsInputs inputs;
    Building building;

    [SerializeField] GameObject buildPlaceParent;

    private NavMeshAgent agent;
    private CharacterController controller;

    [SerializeField] GameObject scrollPanel;
    [SerializeField] TMP_Text scrollTitle;
    [SerializeField] TMP_Text scrollText;

    CinemachineVirtualCamera cinemachine;
    Camera _camera;

    [SerializeField] Texture2D cursorDefault;
    [SerializeField] Texture2D cursorAction;

    float objectHeight = 0;

    Links links;

    private void Awake()
    {
        links = FindObjectOfType<Links>();
    }

    private void Start()
    {
        Animator = links.personController._animator;
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        inputs = links.inputs;
        building = GetComponent<Building>();
        player = links.player;
        questWindow = links.questWindow;
        _camera = links.mainCamera;
        cinemachine = links.cinemachine;
        progressIndicator = links.ui.progressIndicator;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) scrollPanel.SetActive(false);

        // Атака правой рукой
        if (Input.GetKeyDown(KeyCode.F))
        {
            Animator.SetTrigger("Attack");
        }

        // Перетаскивание больших предметов (отпускание)
        if (Input.GetKeyDown(KeyCode.G) && isCarry)
        {
            CarryRelease();
        }

        mousePosition = Input.mousePosition;

        ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (!Input.GetMouseButton(1) && !player.isLay && Physics.Raycast(ray, out hit, raycastLength, 3))
        {
            if (!isPointUI)
            {
                building.buildingPlace = hit.point;

                itemInfoPanel.SetActive(false);

                _distanceToTarget = Vector3.Distance(Player.transform.position, hit.transform.position);

                if (Input.GetMouseButton(1))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        DropObject();
                    }
                }

                if (_distanceToTarget <= useDistance)
                {
                    // Индикация предметов
                    if (hit.collider.TryGetComponent(out ItemInfo item))
                    {
                        Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);

                        if (!Input.GetMouseButton(1)) // Выводим название предмета рядом с курсором
                        {

                            Descripion3d(hit, item);

                        }
                        else
                        { item3dInfoPanel.SetActive(false); }

                        // По нажатию кнопки мыши комментируем предмет
                        if (Input.GetMouseButtonDown(0))
                        {
                            //Comment(item.itemComment); // Пока отключил комментирование (не факт что нужно)
                            if (item.TryGetComponent(out Campfire campfire)) Comment(campfire.burningTimeText);
                        }

                        if (Input.GetMouseButtonDown(0) && item.isCollectible) // Если предмет собираемый, то запускаем корутину подбора
                        {
                            if (!isCarry) Collect(item, hit.collider.gameObject);
                            else Comment("Вот бы у меня была третья рука!");
                        }

                        // Перетаскивание больших предметов в 2 руках
                        if (Input.GetKeyDown(KeyCode.G))
                        {
                            if (player.isAbleCarry)
                            {
                                if (inventoryWindow.LeftHandItem == null && inventoryWindow.RightHandItem == null)
                                Carry(item);
                                else Comment("Чтобы что-то поднять двумя руками, надо чтобы в них ничего не было!");
                            }
                            else
                            {
                                player.animator.SetTrigger("Tired");
                                Comment("Я так устал, что ничего тяжелого поднять уже не могу!");
                            }
                        }
                        

                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            if (item.isFirePlace)
                            {
                                if (player.Wood > 0)
                                {
                                    item.GetComponent<Campfire>().AddFireWood();
                                    player.Wood--;
                                    links.building.SpendWood(1);
                                    Animator.SetTrigger("Use");
                                    inventoryWindow.Redraw();
                                }
                                else Comment("Нечего подложить - надо поискать дрова!");
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            if (item.isBed)
                            {
                                player.Lay();
                                player.transform.position = item.GetComponent<Bed>().layPosition.transform.position;
                                player.transform.rotation = item.GetComponent<Bed>().layPosition.transform.rotation;
                                player.ui.pressToSleep.gameObject.SetActive(true);
                                player.sleepPlace = item.GetComponent<Bed>().layPosition;
                            }
                        }
                    }
                    else
                    {
                        item3dInfoPanel.SetActive(false);
                        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);

                    }



                    // Нажатие рычага/кнопки
                    if (hit.collider.TryGetComponent(out DoorTrigger trigger))
                    {
                        Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (_distanceToTarget <= 3 && !trigger.doorScript.isMoving)
                            {
                                Animator.SetTrigger("Use");
                                trigger.OpenDoor();
                            }
                        }
                    }

                    // Открытие сундуков
                    else if (hit.collider.TryGetComponent(out Container container))
                    {
                        Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);
                        Debug.Log("111");

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (_distanceToTarget <= 3 && openingCoroutine == null)
                            {
                                Animator.SetTrigger("Use");
                                openingCoroutine = StartCoroutine(OpeningContainer(container));
                            }
                        }
                    }
                }
                if (_distanceToTarget < useDistance + 0.5f)
                {
                    if (hit.collider.TryGetComponent(out ItemInfo item) && item.isOpenable) Descripion3d(hit, item);

                    // Открытие двери
                    if (hit.collider.TryGetComponent(out Door door))
                    {
                        Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (_distanceToTarget <= 3)
                            {
                                Animator.SetTrigger("Use");

                                if (door.SearchingKey())
                                    openingCoroutine = StartCoroutine(OpeningDoor(door));

                            }
                        }
                    }
                }
                else
                {
                    Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                    item3dInfoPanel.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E) && !player.isLay && !player.isSit && !isCarry)
                {
                    if (player.isAbleToGrabGrass)
                    {
                        if (player.animator.GetBool("CollectGrass")) player.animator.SetBool("CollectGrass", false);
                        else
                        {
                            StartCoroutine(player.CollectGrass());
                            player.animator.SetBool("CollectGrass", true);
                        }
                    }
                }
                
            }
        }
        else
        {
            Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
            item3dInfoPanel.SetActive(false);
        }

        if (isPointUI)
        {
            item3dInfoPanel.SetActive(false);
            if (!Input.GetMouseButton(1))
            {
                Descripion();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Поедание еды
                if (pointedIcon != null && pointedIcon.item.isFood && pointedIcon.gameObject.TryGetComponent(out InventoryIcon icon))
                {
                    Debug.Log("Eat Food");
                    GetComponent<Player>().Food += pointedIcon.item.foodValue;
                    icon.RemoveFromInventory();
                    icon.item3dObject.SetActive(false);
                    icon.item3dObject.transform.parent = links.objectPool;
                    Destroy(pointedIcon.gameObject);
                }

                // Зажигание факела
                else if (pointedIcon != null && pointedIcon.item.Type == ItemType.Torch)
                {
                    if (pointedIcon.GetComponentInParent<InventorySlot>().indexSlot == 100 || pointedIcon.GetComponentInParent<InventorySlot>().indexSlot == 101)
                    {
                        Animator.SetTrigger("Use");
                        if (pointedIcon.GetComponent<InventoryIcon>().item3dObject.TryGetComponent(out Torchlight torch))
                        {
                            torch.isBurn = !torch.isBurn;
                        }
                    }
                    else Comment("Чтобы зажечь факел надо взять его в руки!");
                }

                // Чтение свитка
                else if (pointedIcon != null && pointedIcon.GetComponent<InventoryIcon>().item3dObject.TryGetComponent(out Scroll scroll))
                {
                    scrollPanel.SetActive(true);
                    scrollTitle.text = scroll.title;
                    scrollText.text = scroll.text;

                    if (scroll.quest != null)
                    {
                        if (!questWindow.questList.Contains(scroll.quest) && !questWindow.completedQuests.Contains(scroll.quest))
                        {
                            questWindow.questList.Add(scroll.quest);
                            questWindow.QuestUpdate();
                        }
                    }

                }
            }
        }


    }

    public void Carry(ItemInfo item)
    {
        if (carryCoolDown <= 0 && item.isCarrying && !isCarry)
        {
            isCarry = true;
            carryObject = item.gameObject;
            if (carryObject.TryGetComponent(out ItemInfo itemInfo)) carryWeight = itemInfo.weight;
            Animator.SetTrigger("CarryTrigger");
            Animator.SetBool("Carry", true);
            carryObject.layer = 3;
            carryObject.transform.parent = carryParent;
            carryObject.transform.position = Vector3.zero;
            carryObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            carryObject.transform.localPosition = item.carryPosition;
            carryObject.transform.localRotation = item.carryRotation;
            carryObject.GetComponent<Rigidbody>().isKinematic = true;
            if (TryGetComponent(out BoxCollider box)) box.enabled = false;
            if (TryGetComponent(out MeshCollider mesh)) mesh.enabled = false;
            Player.GetComponent<CharacterController>().radius = 0.6f;
            Player.GetComponent<CharacterController>().center = new Vector3(0, 0.94f, 0.49f);
        }
    }    

    public void CarryRelease()
    {
        carryWeight = 0;
        carryObject.transform.localPosition += new Vector3(-0.2f, 0f, 0f);
        carryObject.transform.parent = carryDropParent;
        carryObject.GetComponent<Rigidbody>().isKinematic = false;
        if (carryObject.TryGetComponent(out BoxCollider box)) box.enabled = true;
        if (carryObject.TryGetComponent(out MeshCollider mesh)) mesh.enabled = true;
        carryObject.layer = 0;

        Animator.SetBool("Carry", false);
        Player.GetComponent<CharacterController>().radius = 0.28f;
        Player.GetComponent<CharacterController>().center = new Vector3(0, 0.94f, 0f);
        StartCoroutine(CarryCoolDown());
    }

    IEnumerator CarryCoolDown()
    {
        carryCoolDown = 0.2f;
        while (carryCoolDown > 0)
        {
            yield return new WaitForSeconds(0.1f);
            carryCoolDown -= 0.1f;
        }
        isCarry = false;
    }

    IEnumerator OpeningContainer(Container container)
    {
        player.PlayerControl(false);
        float time = 0;
        progressIndicator.transform.parent.gameObject.SetActive(true);
        while (time < container.openingTime)
        {
            if (container.isOpen)
            {
                player.PlayerControl(true); break;
            }
            time += Time.deltaTime * links.time.timeFactor / 60;
            progressIndicator.fillAmount = time / container.openingTime;
            yield return null;
        }
        container.Click();
        progressIndicator.fillAmount = 0;
        progressIndicator.transform.parent.gameObject.SetActive(false);
        openingCoroutine = null;
        player.PlayerControl(true);
    }

    IEnumerator OpeningDoor(Door door)
    {
        float time = 0;
        progressIndicator.transform.parent.gameObject.SetActive(true);
        player.PlayerControl(false);
        while (time < door.openingTime && Input.GetMouseButton(0))
        {
            if (!door.isLocked) { player.PlayerControl(true); break; }
            time += Time.deltaTime * links.time.timeFactor / 60;
            progressIndicator.fillAmount = time / door.openingTime;
            yield return null;
        }
        if (time >= door.openingTime || !door.isLocked)
        { 
            door.isLocked = false;
            door.OpenClose();
        }
        player.PlayerControl(true);
        progressIndicator.fillAmount = 0;
        progressIndicator.transform.parent.gameObject.SetActive(false);
        openingCoroutine = null;
    }

    Vector3 iconOffset = new Vector3(-220, 145);
    Vector3 bagOffset = new Vector3(-220, -145);
    Vector3 statusOffset = new Vector3(0, 140);
    Vector3 craftOffset = new Vector3(200, 100);

    void Descripion()
    {
        if (isUIDescription)
        {
            itemInfoPanel.SetActive(true);

            //if (Input.mousePosition.y < 540) itemPanelInventoryOffset = itemPanelInventoryOffsetDown;
            //else itemPanelInventoryOffset = itemPanelInventoryOffsetUp;

            //if (Input.mousePosition.x < 150) itemPanelInventoryOffset += itemPanelInventoryOffsetRight;
            if (Input.mousePosition.x > 960)
            {
                if (Input.mousePosition.y > 800) itemPanelInventoryOffset = bagOffset;
                else itemPanelInventoryOffset = iconOffset;
            }
            if (Input.mousePosition.y < 200 && Input.mousePosition.x > 300)
                itemPanelInventoryOffset = statusOffset;
            if (Input.mousePosition.x < 300) itemPanelInventoryOffset = craftOffset;

            itemInfoPanel.transform.position = pointedIcon.transform.position + itemPanelInventoryOffset;
        }
        else
        {
            itemInfoPanel.SetActive(false);
        }
    }

    void Descripion3d(RaycastHit hit, ItemInfo item)
    {
        item3dNamePanelText.text = item.itemName;
        item3dDescriptionPanelText.text = item.itemDescription;
        string actions = null;
        if (item.isCollectible) actions = "ЛКМ - подобрать";
        if (item.isCarrying) actions = "G - поднять/поставить";
        if (item.isUsable) actions = "ЛКМ - открыть";
        if (item.isOpenable) actions = "ЛКМ - открыть";
        if (item.isFirePlace) actions = "E - подложить дров";
        if (item.isBed) { if (player.isLay) actions = "Q - встать"; else actions = "E - прилечь"; }
        item3dActionPanelText.text = actions;

        if (hit.collider.TryGetComponent(out ItemInfo info))
        {
            item3dInfoPanel.SetActive(true);
        }
        else
        {
            item3dInfoPanel.SetActive(false);
        }

        if (item.TryGetComponent(out BoxCollider box))
        {
            objectHeight = box.size.y;
            Debug.Log(box.size.y);
        }
        item3dInfoPanel.transform.position = _camera.WorldToScreenPoint(hit.collider.gameObject.transform.position)
        + new Vector3(_camera.WorldToScreenPoint(hit.collider.gameObject.transform.position).x > 960 ? 200 : -200, 150 /* + objectHeight * 100 */, 0);

    }

    IEnumerator NavMeshMove()
        {
            if (Target.GetComponent<MeshRenderer>().enabled == true)
            {
                //if (hit.transform != null)
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

    void Collect(ItemInfo item, GameObject itemObject)
    {
        if (inventoryWindow.RightHandItem == null || inventoryWindow.LeftHandItem == null)
        {
            Item collectingItem = null;
            if (item.item.Type == ItemType.Wood) { links.player.Wood += 1; }
            collectingItem = item.item;

            if (inventoryWindow.RightHandItem == null)
            {
                inventoryWindow.RightHandItem = collectingItem;
                inventoryWindow.RightHandObject = itemObject;
                itemObject.transform.parent = inventoryWindow.rightHandParent;
                itemObject.layer = 3;
            }
            else
            {
                inventoryWindow.LeftHandItem = collectingItem;
                inventoryWindow.LeftHandObject = itemObject;
                itemObject.transform.parent = inventoryWindow.leftHandParent;
                itemObject.layer = 3;
            }
                
            Animator.SetTrigger("Grab");
            if (inventoryWindow.inventory != null) inventoryWindow.inventory.Recount();
            inventoryWindow.Redraw();

            itemInfoPanel.SetActive(false);

            questWindow.onCollectItem?.Invoke();
        }
        else
        {
            Comment("Руки заняты!");
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

    void DropObject()
    {
        if (inventoryWindow.RightHandObject != null)
        {
            inventoryWindow.RightHandObject.GetComponent<Rigidbody>().isKinematic = false;
            inventoryWindow.RightHandObject.GetComponent<BoxCollider>().enabled = true;
            inventoryWindow.RightHandObject.GetComponent<Rigidbody>().AddForce(transform.up * 500);
            inventoryWindow.RightHandObject.GetComponent<Rigidbody>().AddForce(transform.forward * 700);
            inventoryWindow.RightHandObject.transform.SetParent(inventoryWindow.dropParent);
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryIcon>().RemoveFromInventory();
            return;
        }
        if (inventoryWindow.LeftHandObject != null)
        {
            inventoryWindow.LeftHandObject.GetComponent<Rigidbody>().isKinematic = false;
            inventoryWindow.LeftHandObject.GetComponent<BoxCollider>().enabled = true;
            inventoryWindow.LeftHandObject.GetComponent<Rigidbody>().AddForce(transform.up * 500);
            inventoryWindow.LeftHandObject.GetComponent<Rigidbody>().AddForce(transform.forward * 700);
            inventoryWindow.LeftHandObject.transform.SetParent(inventoryWindow.dropParent);
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryIcon>().RemoveFromInventory();
            return;
        }
    }


} 

