using UnityEngine;
using Cinemachine;

public class Links : MonoBehaviour
{
    public Player player;
    public MousePoint mousePoint;
    public Building building;
    public InventoryWindow inventoryWindow;
    public StorageWindow storageWindow;
    public SaveInventory saveInventory;
    public QuestWindow questWindow;
    public ItemSpawner itemSpawner;
    public ItemRandomizer itemRandomizer;
    public UILinks ui;
    public GameMenu gameMenu;
    public ThirdPersonController personController;
    public StarterAssetsInputs inputs;
    public DayNight dayNight;
    public Music music;
    public TOD_Time time;
    public TOD_CycleParameters cycle;
    public CameraController cameraController;
    public Camera mainCamera;
    public CinemachineVirtualCamera cinemachine;
    public Transform spawnItemsParent;
    public Transform savedDropedItemsParent;
    public Transform containersParent;
    public Transform itemsOnLocationsParent;
    public Transform objectPool;
    public Transform playerBuildings;

    public Camera ScottyCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }
}
