using UnityEngine;
using System.Collections;


public enum Blueprint 
{ 
	None = 0,
	Campfire = 1,
	GrassBed = 2
}

public class Building : MonoBehaviour 
{
	[Header("Иконки чертежей")]
	public GameObject campfireBuildIcon;
	public GameObject grassBedBuildIcon;

	[Header("Костёр")]
	public GameObject campFireBluePrint;
	public GameObject campFirePrefab;
	public int sticksNeedForCampfire;
	public Quest campFireQuest;
	public bool isCampFireBuilding;

	[Header("Настил из травы")]
	public GameObject grassBedBluePrint;
	public GameObject grassBedPrefab;
	public Quest grassBedQuest;
	public bool isGrassBedBuilding;

	[Header("Паренты для строительства")]
	public GameObject parentObject;
	public Transform playerBuildings;
	
	[Header("Место строительства")]
	public bool isBlueprintActive;
	public Vector3 buildingPlace;
	Vector3 chosenPlace;


	QuestWindow questWindow;
	Player player;
	Inventory inventory;
	Links links;
	UILinks ui;

    private void Awake()
    {
		links = FindObjectOfType<Links>();
		ui = FindObjectOfType<UILinks>();
    }

    void Start () 
	{
		inventory = links.inventoryWindow.inventory;
		questWindow = links.questWindow;
		player = links.player;
	}

	public void LearningBlueprint(Blueprint blueprint)
    {
		if (blueprint == Blueprint.Campfire) campfireBuildIcon.SetActive(true);
		if (blueprint == Blueprint.GrassBed) grassBedBuildIcon.SetActive(true);
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
    }

	void BuildingQuest(Quest quest)
    {
		if (questWindow.questList.Contains(quest))
		{
			for (int i = 0; i < questWindow.questBlocksList.Count; i++)
			{
				if (questWindow.questBlocksList[i].quest == quest)
				{
					questWindow.questBlocksList[i].isComplete = true;
					questWindow.questBlocksList[i].checkMarkImage.gameObject.SetActive(true);
					break;
				}
			}
		}
	}

	Coroutine buildingCoroutine;
	IEnumerator BlueprintPlace(GameObject blueprint, GameObject prefab, Quest quest, float buildingTime)
    {
		blueprint.SetActive(true);
		isBlueprintActive = true;
		blueprint.transform.SetParent(parentObject.transform);

		while (!Input.GetMouseButtonDown(0))
		{
			blueprint.transform.position = buildingPlace;
			if (Input.GetKey(KeyCode.E)) blueprint.transform.Rotate(0, 1, 0, 0);
			if (Input.GetKey(KeyCode.Q)) blueprint.transform.Rotate(0, -1, 0, 0);

			if (Input.GetMouseButtonDown(1) || !isBlueprintActive)
			{
				campFireBluePrint.SetActive(false);
				isBlueprintActive = false;
				isCampFireBuilding = false;
				isGrassBedBuilding = false;
				yield break;
			}
			yield return null;
		}

		player.PlayerControl(false);
		chosenPlace = buildingPlace;
		float timer = 0;
		ui.progressIndicator.transform.parent.gameObject.SetActive(true);
		while (timer < buildingTime)
		{
			timer += Time.deltaTime * links.time.timeFactor / 60;
			ui.progressIndicator.fillAmount = timer / buildingTime;
			yield return null;
			if (Input.GetMouseButtonDown(1) || player.isDead)
			{
				campFireBluePrint.SetActive(false);
				isBlueprintActive = false;
				isCampFireBuilding = false;
				isGrassBedBuilding = false;
				player.PlayerControl(true);
				ui.progressIndicator.transform.parent.gameObject.SetActive(false);
				yield break;
			}
		}
		ui.progressIndicator.fillAmount = 0;
		ui.progressIndicator.transform.parent.gameObject.SetActive(false);

		blueprint.SetActive(false);
		newObject = Instantiate(prefab);
		newObject.transform.position = chosenPlace;
		newObject.transform.rotation = blueprint.transform.rotation;
		newObject.transform.SetParent(playerBuildings, true);

		isBlueprintActive = false;

		BuildingQuest(quest);

		if (isCampFireBuilding) CampfireBuild();
		if (isGrassBedBuilding) GrassBedBuild();

		player.PlayerControl(true);

		yield break;
	}

	public void Campfire()
    {
		isCampFireBuilding = true;
		buildingCoroutine = StartCoroutine(BlueprintPlace(campFireBluePrint, campFirePrefab, campFireQuest, 20));
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
		}
		if (woodAmount == amount) return;
		if (links.inventoryWindow.LeftHandItem != null && links.inventoryWindow.LeftHandItem.Type == ItemType.Wood && woodAmount < amount)
		{
			woodAmount++;
			links.inventoryWindow.LeftHandItem = null;
			Destroy(links.inventoryWindow.LeftHandObject);
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
		buildingCoroutine = StartCoroutine(BlueprintPlace(grassBedBluePrint, grassBedPrefab, grassBedQuest, 30));
	}

	void GrassBedBuild()
    {
		Destroy(links.mousePoint.carryObject);
		links.mousePoint.CarryRelease();
		isGrassBedBuilding = false;
	}		

	GameObject newObject;
}