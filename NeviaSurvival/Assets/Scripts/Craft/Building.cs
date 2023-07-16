using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Blueprint 
{ 
	None = 0,
	Campfire = 1,
	GrassBed = 2,
	Fence
}

public class Building : MonoBehaviour 
{
	public List<BuildData> Buildings;
	public Dictionary<string, BuildData> BuildList;

	public List<GameObject> allBuildIcons;
	
	[Header("Иконки чертежей")]
	public GameObject campfireBuildIcon;
	public GameObject grassBedBuildIcon;

	[Header("Костёр")]
	public GameObject campFireBluePrint;
	public GameObject campFirePrefab;
	public int sticksNeedForCampfire;
	public QuestData campFireQuestData;
	public bool isCampFireBuilding;

	[Header("Настил из травы")]
	public GameObject grassBedBluePrint;
	public GameObject grassBedPrefab;
	public QuestData grassBedQuestData;
	public bool isGrassBedBuilding;
	
	[Header("Ограда")]
	public GameObject fenceBluePrint;
	public GameObject fencePrefab;
	public QuestData fenceQuestData;
	public bool isFenceBuilding;

	[Header("Паренты для строительства")]
	public GameObject parentObject;
	public Transform playerBuildings;
	
	[Header("Место строительства")]
	public bool isBlueprintActive;
	public Vector3 buildingPlace;
	public Vector3 buildingNormal;
	Vector3 chosenPlace;

	public bool isAbleToBuild;
	public bool isConstructing;


	QuestWindow questWindow;
	private QuestHandler questHandler;
	Player player;
	Inventory inventory;
	Links links;
	UILinks ui;

    private void Awake()
    {
		links = FindObjectOfType<Links>();
		ui = FindObjectOfType<UILinks>();

		Buildings.Clear();
		Buildings.AddRange(Resources.LoadAll<BuildData>("ScriptableObjects"));

		BuildList = new Dictionary<string, BuildData>();

		foreach (BuildData build in Buildings)
        {
			BuildList.Add(build.id, build);
        }
    }

    void Start () 
	{
		inventory = links.inventoryWindow.inventory;
		questWindow = links.questWindow;
		questHandler = links.questHandler;
		player = links.player;
	}

	public void LearningBlueprint(Blueprint blueprint)
    {
		if (blueprint == Blueprint.Campfire) campfireBuildIcon.SetActive(true);
		if (blueprint == Blueprint.GrassBed) grassBedBuildIcon.SetActive(true);
		if (blueprint == Blueprint.Fence) grassBedBuildIcon.SetActive(true);
	}

	public void BuildingActivator(int index)
    {
		isBlueprintActive = false;
		if (index == 0)
		{
			if (player.Wood >= sticksNeedForCampfire) Campfire();
			else links.mousePoint.Comment("Для костра нужна хотя бы одна ветка!");
		}

		if (index == 1)
		{
			if (links.mousePoint.carryObject != null && links.mousePoint.carryObject.GetComponent<ItemInfo>().type == ItemType.GrassStack)
				GrassBed();
			else links.mousePoint.Comment("Для настила из листьев нужно сначала собрать кучу листьев!");
		}
		
		if (index == 2)
		{
			if (links.inventoryWindow.ItemCount("woodplank") >= 6)
				Fence();
			else links.mousePoint.Comment("Не хватает досок - нужно 6!");
		}
    }

	void BuildingQuest(QuestData questData)
    {
		if (questHandler.takenQuestList.Contains(questData))
		{
			for (int i = 0; i < questWindow.questBlocksList.Count; i++)
			{
				if (questWindow.questBlocksList[i].questData == questData)
				{
					questWindow.questBlocksList[i].isComplete = true;
					questWindow.questBlocksList[i].checkMarkImage.gameObject.SetActive(true);
					questWindow.QuestStatusUpdate();
					break;
				}
			}
		}
	}

	Coroutine buildingCoroutine;
	IEnumerator BlueprintPlace(GameObject blueprint, GameObject prefab, QuestData questData, float buildingTime)
    {
		blueprint.SetActive(true);
		isBlueprintActive = true;
		blueprint.transform.SetParent(parentObject.transform);

		while (true)
		{
			blueprint.transform.position = buildingPlace;
		
			if (Input.GetKey(KeyCode.E)) blueprint.transform.Rotate(0, 1, 0, 0);
			if (Input.GetKey(KeyCode.Q)) blueprint.transform.Rotate(0, -1, 0, 0);

			if (Input.GetMouseButtonDown(1) || !isBlueprintActive)
			{
				campFireBluePrint.SetActive(false);
				grassBedBluePrint.SetActive(false);
				fenceBluePrint.SetActive(false);
				isBlueprintActive = false;
				isCampFireBuilding = false;
				isGrassBedBuilding = false;
				isFenceBuilding = false;
				yield break;
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (isAbleToBuild)
				{
					if (Vector3.Distance(transform.position, buildingPlace) > 2)
					{
						links.mousePoint.Comment("Надо подойти поближе!");
					}
					else break;
				}
					else links.mousePoint.Comment("Тут не получится ничего построить!");
			}
			yield return null;
		}

		player.PlayerControl(false);
		isConstructing = true;
		chosenPlace = buildingPlace;
		float timer = 0;
		ui.progressIndicator.transform.parent.gameObject.SetActive(true);
		links.player.gameObject.transform.rotation = Quaternion.LookRotation(chosenPlace - player.transform.position); 
		links.player.animator.SetBool("CollectGrass", true);
		while (timer < buildingTime)
		{
			timer += Time.deltaTime * links.time.timeFactor / 60;
			ui.progressIndicator.fillAmount = timer / buildingTime;
			yield return null;
			if (Input.GetKeyDown(KeyCode.Space) || player.isDead)
			{
				campFireBluePrint.SetActive(false);
				grassBedBluePrint.SetActive(false);
				fenceBluePrint.SetActive(false);
				isBlueprintActive = false;
				isCampFireBuilding = false;
				isGrassBedBuilding = false;
				player.PlayerControl(true);
				ui.progressIndicator.transform.parent.gameObject.SetActive(false);
				links.player.animator.SetBool("CollectGrass", false);
				yield break;
			}
		}
		
		isConstructing = false;
		links.player.animator.SetBool("CollectGrass", false);
		ui.progressIndicator.fillAmount = 0;
		ui.progressIndicator.transform.parent.gameObject.SetActive(false);

		blueprint.SetActive(false);
		newObject = Instantiate(prefab);
		newObject.transform.position = chosenPlace;
		newObject.transform.rotation = blueprint.transform.rotation;
		newObject.transform.SetParent(playerBuildings, true);

		isBlueprintActive = false;

		BuildingQuest(questData);

		if (isCampFireBuilding) CampfireBuild();
		if (isGrassBedBuilding) GrassBedBuild();
		if (isFenceBuilding) FenceBuild();

		player.PlayerControl(true);

		yield break;
	}

	public void Campfire()
    {
		isCampFireBuilding = true;
		buildingCoroutine = StartCoroutine(BlueprintPlace(campFireBluePrint, campFirePrefab, campFireQuestData, 5));
	}

	void CampfireBuild()
    {
		player.Wood -= sticksNeedForCampfire;
		isCampFireBuilding = false;
		SpendWood(sticksNeedForCampfire);
		links.inventoryWindow.Redraw();
	}

	public void SpendWood(int amount)
    {
		int woodAmount = 0;
		if (links.inventoryWindow.RightHandItem != null && links.inventoryWindow.RightHandItem.Type == ItemType.Wood)
		{
			woodAmount++;
			links.inventoryWindow.RightHandItem = null;
			Destroy(links.inventoryWindow.RightHandObject);
			links.inventoryWindow.RightHandObject = null;
		}
		if (woodAmount == amount) return;
		if (links.inventoryWindow.LeftHandItem != null && links.inventoryWindow.LeftHandItem.Type == ItemType.Wood && woodAmount < amount)
		{
			woodAmount++;
			links.inventoryWindow.LeftHandItem = null;
			Destroy(links.inventoryWindow.LeftHandObject);
			links.inventoryWindow.LeftHandObject = null;
		}
		if (woodAmount == amount) return;
		if (links.inventoryWindow.inventory != null)
			for (int i = 0; i < links.inventoryWindow.inventory.inventoryItems.Count; i++)
			{
				if (links.inventoryWindow.inventory.inventoryItems[i] != null)
					if (links.inventoryWindow.inventory.inventoryItems[i].Type == ItemType.Wood && woodAmount < amount)
					{
						links.inventoryWindow.inventory.inventoryItems[i] = null;
						woodAmount++;
					}
			}
	}

	public void GrassBed()
	{
		isGrassBedBuilding = true;
		buildingCoroutine = StartCoroutine(BlueprintPlace(grassBedBluePrint, grassBedPrefab, grassBedQuestData, 8));
	}

	void GrassBedBuild()
    {
		Destroy(links.mousePoint.carryObject);
		links.mousePoint.CarryRelease();
		isGrassBedBuilding = false;
	}	
	
	public void Fence()
	{
		isFenceBuilding = true;
		buildingCoroutine = StartCoroutine(BlueprintPlace(fenceBluePrint, fencePrefab, fenceQuestData, 8));
	}

	void FenceBuild()
	{
		isGrassBedBuilding = false;
		SpendItemByID("woodplank", 6);
		links.inventoryWindow.Redraw();
	}
	
	public void SpendItemByID(string id, int amount)
	{
		int itemAmount = 0;
		if (links.inventoryWindow.RightHandItem != null && links.inventoryWindow.RightHandItem.id == id)
		{
			itemAmount++;
			links.inventoryWindow.RightHandItem = null;
			Destroy(links.inventoryWindow.RightHandObject);
			links.inventoryWindow.RightHandObject = null;
		}
		if (itemAmount == amount) return;
		if (links.inventoryWindow.LeftHandItem != null && links.inventoryWindow.LeftHandItem.id == id && itemAmount < amount)
		{
			itemAmount++;
			links.inventoryWindow.LeftHandItem = null;
			Destroy(links.inventoryWindow.LeftHandObject);
			links.inventoryWindow.LeftHandObject = null;
		}
		if (itemAmount == amount) return;
		if (links.inventoryWindow.inventory != null)
			for (int i = 0; i < links.inventoryWindow.inventory.inventoryItems.Count; i++)
			{
				if (links.inventoryWindow.inventory.inventoryItems[i] != null)
					if (links.inventoryWindow.inventory.inventoryItems[i].id == id && itemAmount < amount)
					{
						links.inventoryWindow.inventory.inventoryItems[i] = null;
						itemAmount++;
					}
			}
		if (links.inventoryWindow.LeftHandItem != null && links.inventoryWindow.LeftHandItem.Type == ItemType.Bag)
		{
			if (links.inventoryWindow.LeftHandObject != null)
				foreach (Item item in links.inventoryWindow.LeftHandObject.GetComponent<Inventory>().inventoryItems)
				{
					if (item != null && item.id == id) itemAmount++;
				}
		}
	}

	GameObject newObject;
}