using UnityEngine;
using Cinemachine;

public class Links : MonoBehaviour
{
    public Player player;
    public MousePoint mousePoint;
    public BuildingHandler buildingHandler;
    public InventoryWindow inventoryWindow;
    public StorageWindow storageWindow;
    public Cooking cooking;
    public SaveInventory saveInventory;
    public SaveObjects saveObjects;
    public QuestHandler questHandler;
    public QuestWindow questWindow;
    public DialogueHandler dialogueHandler;
    public ItemSpawner itemSpawner;
    public ItemRandomizer itemRandomizer;
    public UILinks ui;
    public GameMenu gameMenu;
    public PlayerSaveLoad saveLoad;
    public ThirdPersonController personController;
    public StarterAssetsInputs inputs;
    public DayNight dayNight;
    public Music music;
    public Sounds sounds;
    public TOD_Time time;
    public TOD_CycleParameters cycle;
    public TOD_Sky sky;
    public CameraController cameraController;
    public Camera mainCamera;
    public CinemachineVirtualCamera cinemachine;
    public Transform spawnItemsParent;
    public Transform savedDropedItemsParent;
    public Transform containersParent;
    public Transform itemsOnLocationsParent;
    public Transform objectPool;
    public Transform playerBuildings;

    public GameObject PlayerMesh;
    public Camera ScottyCamera;
    public Camera PlayerCamera;
    public Camera PlayerInfoCamera;

    void Awake()
    {
        Game.links = this;
        Game.Player = player;
        mainCamera = Camera.main;
    }
}
