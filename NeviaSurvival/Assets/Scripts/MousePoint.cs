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

    [SerializeField] float lookDistance = 10;
    [SerializeField] float useDistance = 3.5f;
    [SerializeField] float grabDistance = 2;
    float _distanceToTarget;
    bool longDistanceItem;
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
    private QuestHandler questHandler;
    public int stickLimit;
    public GameObject Target;
    private Animator Animator;
    private Vector3 lookDirection;

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
    public TMP_Text itemDurabilityPanelText;
    public TMP_Text itemWeightPanelText;
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
    public Coroutine openingCoroutine;
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

    public Links links;

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
        questHandler = links.questHandler;
        _camera = links.mainCamera;
        cinemachine = links.cinemachine;
        progressIndicator = links.ui.progressIndicator;

    }

    void ShowDescription3D(ItemInfo item)
    {
        if (!Input.GetMouseButton(1)) // Выводим название предмета рядом с курсором
        {

            Descripion3d(hit, item);

        }
        else
        { item3dInfoPanel.SetActive(false); }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) scrollPanel.SetActive(false);

        // Атака правой рукой
        if (Input.GetMouseButton(1) && !links.player.isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
                Animator.SetTrigger("Attack");

            if (Input.GetKeyDown(KeyCode.F))
                Animator.SetTrigger("Hack");
        }

        // Перетаскивание больших предметов (отпускание)
        if (Input.GetKeyDown(KeyCode.G) && isCarry)
        {
            CarryRelease();
        }

        MousePoint3D();

        MousePointUI();
    }

    void MousePoint3D()
    {
        if (!Input.GetMouseButton(1))
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!isPointUI && !player.isLay && Physics.Raycast(ray, out hit, raycastLength))
            {
                // Точка строительства только на террейне или слое default
                if (hit.collider.gameObject.layer == 12 || hit.collider.gameObject.layer == 15)
                {
                    building.buildingPlace = hit.point;
                    building.buildingNormal = hit.normal;
                }

                itemInfoPanel.SetActive(false);
                
                

                if (Input.GetMouseButton(1))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        DropObject();
                    }
                }
                
                if (hit.collider.gameObject.layer == 0)
                {
                    _distanceToTarget = Vector3.Distance(Player.transform.position, hit.transform.position);
                    
                    if (_distanceToTarget <= lookDistance && hit.collider.TryGetComponent(out ItemInfo item)) 
                    {
                        // По нажатию кнопки мыши комментируем предмет
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (item.TryGetComponent(out Campfire campfire) && _distanceToTarget < lookDistance)
                                Comment(campfire.burningTimeText);
                            else if ((item.isCollectible || item.isCarrying) && _distanceToTarget > grabDistance
                                     || _distanceToTarget > useDistance)
                                Comment(item.itemComment);
                        }

                        // Зажигание фонаря
                        if (hit.collider.TryGetComponent(out Lamp lamp) && _distanceToTarget <= useDistance)
                        {
                            Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);

                            ShowDescription3D(item);

                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                StartCoroutine(LampIgnition(lamp));
                            }
                        }
                        // Нажатие рычага/кнопки
                        else if (hit.collider.TryGetComponent(out DoorTrigger trigger) && _distanceToTarget <= grabDistance)
                        {
                            Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);
                            ShowDescription3D(item);

                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                if (_distanceToTarget <= 3 && !trigger.doorScript.isMoving)
                                {
                                    Animator.SetTrigger("Use");
                                    trigger.OpenDoor();
                                }
                            }
                        }

                        // Открытие сундуков
                        else if (hit.collider.TryGetComponent(out Container container) && _distanceToTarget <= grabDistance)
                        {
                            Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);
                            ShowDescription3D(item);

                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                if (openingCoroutine == null)
                                {
                                    Animator.SetTrigger("Use");
                                    openingCoroutine = StartCoroutine(OpeningContainer(container));
                                }
                            }

                            if (Input.GetMouseButtonDown(0) && hit.collider.TryGetComponent(out Cauldron cauldron))
                            {
                                TryCollect(item, hit.collider.gameObject);
                            }

                            // Перетаскивание сундуков в 2 руках
                            if (Input.GetKeyDown(KeyCode.G))
                            {
                                TryCarry(item);
                            }
                        }
                        // Открытие двери
                        else if (hit.collider.TryGetComponent(out Door door) && _distanceToTarget < useDistance)
                        {
                            if (item.isOpenable) Descripion3d(hit, item);

                            Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);

                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                Animator.SetTrigger("Use");

                                if (door.key != null && door.isLocked && door.SearchingKey())
                                    openingCoroutine = StartCoroutine(OpeningDoor(door));
                                else
                                {
                                    door.OpenClose();
                                }

                                if (door.isBlocked)
                                {
                                    Comment("Дверь заблокирована!");
                                }
                            }
                        }
                        // Индикация предметов
                        else if (_distanceToTarget < grabDistance)
                        {
                            Cursor.SetCursor(cursorAction, Vector2.zero, CursorMode.Auto);

                            ShowDescription3D(item);

                            if (Input.GetMouseButtonDown(0)) // Если предмет собираемый, то запускаем корутину подбора
                            {
                                TryCollect(item, hit.collider.gameObject);
                            }

                            // Перетаскивание больших предметов в 2 руках
                            if (Input.GetKeyDown(KeyCode.G))
                            {
                                TryCarry(item);
                            }

                            // Установка котелка на костёр и подкладывание дров в костёр
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                if (item.isFirePlace)
                                {
                                    if (inventoryWindow.RightHandItem != null &&
                                        inventoryWindow.RightHandItem.Type == ItemType.Cauldron ||
                                        inventoryWindow.LeftHandItem != null &&
                                        inventoryWindow.LeftHandItem.Type == ItemType.Cauldron)
                                    {
                                        if (inventoryWindow.RightHandItem != null &&
                                            inventoryWindow.RightHandItem.Type == ItemType.Cauldron)
                                        {
                                            if (inventoryWindow.RightHandObject.TryGetComponent(out Cauldron cauldron))
                                            {
                                                cauldron.PlaceCauldron(item.gameObject.GetComponent<Campfire>());
                                                inventoryWindow.rightHandSlot.GetChild(0).gameObject
                                                    .GetComponent<InventoryIcon>().RemoveFromInventory();
                                            }
                                        }
                                        else if (inventoryWindow.LeftHandItem != null &&
                                                 inventoryWindow.LeftHandItem.Type == ItemType.Cauldron)
                                        {
                                            if (inventoryWindow.LeftHandObject.TryGetComponent(out Cauldron cauldron))
                                            {
                                                cauldron.PlaceCauldron(item.gameObject.GetComponent<Campfire>());
                                                inventoryWindow.leftHandSlot.GetChild(0).gameObject
                                                    .GetComponent<InventoryIcon>().RemoveFromInventory();
                                            }
                                        }
                                    }
                                    else
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

                                if (item.isCollectible && item.gameObject.TryGetComponent(out Cauldron cauldronOnFire))
                                {
                                    if (cauldronOnFire.campfire != null &&
                                        item.gameObject.TryGetComponent(out Container _container))
                                    {
                                        if (openingCoroutine == null)
                                        {
                                            openingCoroutine = StartCoroutine(OpeningContainer(_container));
                                        }
                                    }
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
                                    item3dInfoPanel.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                            item3dInfoPanel.SetActive(false);
                        }
                    }
                    else
                    {
                        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                        item3dInfoPanel.SetActive(false);
                    }
                }
                else
                {
                    Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                    item3dInfoPanel.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.T) && !player.isLay && !player.isSit && !isCarry)
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
            else
            {
                Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
                item3dInfoPanel.SetActive(false);
            }
        }
        else
        {
            Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
            item3dInfoPanel.SetActive(false);
        }
    }

    void MousePointUI()
    {
        if (isPointUI)
        {
            item3dInfoPanel.SetActive(false);
            if (!Input.GetMouseButton(1))
            {
                Description();
            }

            if (Input.GetMouseButton(1) && pointedIcon != null && !pointedIcon.GetComponent<InventoryIcon>().isInInventory &&
                !pointedIcon.GetComponent<InventoryIcon>().isEquiped)
            {
                TryCollect(pointedIcon.GetComponent<ItemInfo>(), pointedIcon.GetComponent<InventoryIcon>().item3dObject);
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Поедание еды
                if (pointedIcon != null && pointedIcon.item != null && pointedIcon.item.isFood && 
                    pointedIcon.gameObject.TryGetComponent(out InventoryIcon icon) && !player.isSleep && !player.isLay && !player.isSwim)
                {
                    Debug.Log("Eat Food");
                    if (pointedIcon.item.foodValue > player.maxFood - player.Food && player.Food > player.maxFood - 5)
                        Comment("Я пока не голоден!");
                    else
                    {
                        player.Food += pointedIcon.item.foodValue;
                        if (player.Food > player.maxFood) player.Food = player.maxFood;
                        player.Health -= pointedIcon.item.poisonValue;
                        icon.RemoveFromInventory();
                        icon.item3dObject.SetActive(false);
                        icon.item3dObject.transform.parent = links.objectPool;
                        pointedIcon.gameObject.SetActive(false);
                    }
                }

                // Зажигание факела
                else if (pointedIcon != null && pointedIcon.item != null && pointedIcon.item.Type == ItemType.Torch)
                {
                    if (pointedIcon.GetComponentInParent<InventorySlot>().indexSlot == 100 || pointedIcon.GetComponentInParent<InventorySlot>().indexSlot == 101)
                    {
                        Animator.SetTrigger("Use");
                        if (pointedIcon.GetComponent<InventoryIcon>().item3dObject.TryGetComponent(out Torchlight torch))
                        {
                            if (!torch.isBurn) torch.TorchOn();
                            else torch.TorchOff();
                        }
                    }
                    else Comment("Чтобы зажечь факел надо взять его в руки!");
                }

                // Чтение свитка
                else if (pointedIcon != null && pointedIcon.item != null && pointedIcon.GetComponent<InventoryIcon>().item3dObject.TryGetComponent(out Scroll scroll))
                {
                    scrollPanel.SetActive(true);
                    scrollTitle.text = scroll.title;
                    scrollText.text = scroll.text;

                    if (scroll.questData != null)
                    {
                        if (!questHandler.takenQuestList.Contains(scroll.questData) && !questHandler.completedQuests.Contains(scroll.questData))
                        {
                            questHandler.takenQuestList.Add(scroll.questData);
                            questWindow.QuestUpdate();
                        }
                    }

                }

                // Открывание окна содержимого котелка
                else if (pointedIcon != null && pointedIcon.item != null &&
                    pointedIcon.GetComponent<InventoryIcon>().item3dObject.TryGetComponent(out Cauldron cauldron))
                {
                    if (openingCoroutine == null)
                    {
                        openingCoroutine = StartCoroutine(OpeningContainer(cauldron.gameObject.GetComponent<Container>()));
                    }
                }
            }
            // Наполнение котелка водой
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (pointedIcon != null && pointedIcon.item != null && pointedIcon.TryGetComponent(out InventoryIcon icon) &&
                icon.item3dObject.TryGetComponent(out Cauldron cauldron))
                {
                    if (!cauldron.isWater)
                    {
                        if (!cauldron.isSoup) cauldron.Water(true);
                        else Comment("В котелке еще что-то осталось!");
                    }
                    else
                    {
                        cauldron.Water(false);
                    }
                }
            }
        }
    }

    void TryCollect(ItemInfo item, GameObject itemObject)
    {
        if (item.isCollectible)
        {
            if (!isCarry) Collect(item, itemObject);
            else Comment("Вот бы у меня была третья рука!");
        }
    }

    void TryCarry(ItemInfo item)
    {
        if (player.isAbleCarry)
        {
            if (inventoryWindow.LeftHandItem == null && inventoryWindow.RightHandItem == null)
            {
                Carry(item);
                if (item.type == ItemType.Cauldron && !item.isCollectible) item.GetComponent<Cauldron>().GrabCauldron();
            }
            else Comment("Чтобы что-то поднять двумя руками, надо чтобы в них ничего не было!");
        }
        else
        {
            player.animator.SetTrigger("Tired");
            Comment("Я так устал, что ничего тяжелого поднять уже не могу!");
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
            carryObject.layer = 10;
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
            links.ui.pressToDrop.gameObject.SetActive(true);
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
        links.ui.pressToDrop.gameObject.SetActive(false);
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

    public void OpenContainer(Container container)
    {
        openingCoroutine = StartCoroutine(OpeningContainer(container));
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

    IEnumerator LampIgnition(Lamp lamp)
    {
        player.PlayerControl(false);
        float time = 0;
        bool isTorch = false;

        if ((inventoryWindow.RightHandItem != null &&
            inventoryWindow.RightHandObject.TryGetComponent(out Torchlight torch) && torch.isBurn) ||
            (inventoryWindow.LeftHandItem != null &&
             inventoryWindow.LeftHandObject.TryGetComponent(out Torchlight torch2) && torch2.isBurn))
        {
            isTorch = true;
        }

        if (!lamp.isOn && !isTorch)
        {
            Comment("Мне нужен горящий факел, чтобы зажечь фонарь!");
            player.PlayerControl(true);
            yield break;
        }

        progressIndicator.transform.parent.gameObject.SetActive(true);
        if (isTorch || lamp.isOn) Animator.SetTrigger("Use");

        while (time < 1)
        {
            time += Time.deltaTime * links.time.timeFactor / 60;
            progressIndicator.fillAmount = time / 1;
            yield return null;
        }

        if (lamp.isOn) lamp.Switch(false);
        else lamp.Switch(true);

        player.PlayerControl(true);
        progressIndicator.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator OpeningDoor(Door door)
    {
        float time = 0;
        progressIndicator.transform.parent.gameObject.SetActive(true);
        player.PlayerControl(false);
        door.PlayKeySound();
        while (time < door.openingTime && Input.GetKey(KeyCode.E))
        {
            if (!door.isLocked) { player.PlayerControl(true); break; }
            time += Time.deltaTime * links.time.timeFactor / 60;
            progressIndicator.fillAmount = time / door.openingTime;
            yield return null;
        }
        door.StopSound();
        if (time >= door.openingTime || !door.isLocked)
        { 
            door.isLocked = false;
            door.OpenClose();
            links.questWindow.QuestEventRecount();
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

    void Description()
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
        if (item.isCollectible)
        {
            if (item.gameObject.TryGetComponent(out Cauldron cauldron) && cauldron.campfire != null)
            actions = "Е - открыть, ЛКМ - взять";
            else
            actions = "ЛКМ - подобрать";
        }
        if (item.isCarrying) actions = "G - поднять";
        if (item.isUsable) actions = "ЛКМ - открыть";
        if (item.isOpenable) actions = "E - открыть";
        if (item.TryGetComponent(out Lamp lamp)) actions = "E - зажечь/потушить";
        if (item.isOpenable && item.gameObject.TryGetComponent(out Door door) && door.isLocked) actions = "Удерживать E - отпереть";
        if (item.isOpenable && item.isCollectible) actions = "E - открыть, ЛКМ - взять";
        if (item.isOpenable && item.isCarrying) actions = "E - открыть, G - поднять";
        if (item.isFirePlace)
        {
            if (inventoryWindow.RightHandItem != null && inventoryWindow.RightHandItem.Type == ItemType.Cauldron ||
                inventoryWindow.LeftHandItem != null && inventoryWindow.LeftHandItem.Type == ItemType.Cauldron)
            {
                actions = "E - поставить котелок";
            }
            else actions = "E - подложить дров";
        }
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
        int offsetY = 150;
        if (item.isLamp) offsetY = -150;
        item3dInfoPanel.transform.position = _camera.WorldToScreenPoint(hit.collider.gameObject.transform.position)
        + new Vector3(_camera.WorldToScreenPoint(hit.collider.gameObject.transform.position).x > 960 ? 200 : -200, offsetY /* + objectHeight * 100 */, 0);

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
        if (inventoryWindow.RightHandItem == null || inventoryWindow.LeftHandItem == null ||
            inventoryWindow.Backpack != null && inventoryWindow.Backpack.TryGetComponent(out Inventory bag2) &&
            bag2.filledSlots < bag2.inventoryItems.Count)
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
            else if (inventoryWindow.LeftHandItem == null)
            {
                inventoryWindow.LeftHandItem = collectingItem;
                inventoryWindow.LeftHandObject = itemObject;
                itemObject.transform.parent = inventoryWindow.leftHandParent;
                itemObject.layer = 3;
            }
            else if (inventoryWindow.Backpack.TryGetComponent(out Inventory bag))
            {
                for (int i = 0; i < bag.inventoryItems.Count; i++)
                {
                    if (bag.inventoryItems[i] == null)
                    {
                        bag.inventoryItems[i] = collectingItem;
                        bag.inventoryItemObjects[i] = itemObject;
                        itemObject.SetActive(false);
                        itemObject.layer = 3;
                        break;
                    }
                }
            }
            
            if (itemObject.TryGetComponent(out Cauldron cauldron))
            {
                cauldron.isOnFire = false;
                cauldron.campfire = null;
            }
        
            if (Game.links.storageWindow.targetStorage != null && pointedIcon != null) pointedIcon.GetComponent<InventoryIcon>().RemoveFromInventory();
            
            Animator.SetTrigger("Grab");
            if (inventoryWindow.inventory != null) inventoryWindow.inventory.Recount();
            inventoryWindow.Redraw();

            itemInfoPanel.SetActive(false);
            if (links.ui.inventoryPanel.activeSelf == false) links.ui.inventoryPanel.SetActive(true);

            questWindow.onCollectItem?.Invoke();
        }
        else
        {
            Comment("Нет места!");
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

