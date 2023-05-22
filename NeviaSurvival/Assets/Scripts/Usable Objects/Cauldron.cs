using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public Campfire campfire;
    public bool isOnFire;
    public bool isWater;
    public bool isSoup;
    public bool isCooking;
    public Storage cauldron;
    public float cookingTime;
    public float cookingTimer;
    public GameObject water;
    public GameObject soup;

    public string cauldronName = "Котелок";
    Links links;
    Cooking cooking;
    ItemInfo cauldronInfo;

    void Start()
    {
        links = FindObjectOfType<Links>();
        cooking = links.cooking;
        cauldronInfo = cauldron.gameObject.GetComponent<ItemInfo>();
        
        
        if (cauldron.size == 3) cookingTime = 0.5f;
        if (cauldron.size == 6) cookingTime = 1f;
        if (cauldron.size == 9) cookingTime = 2f;

        UpdateCauldron();
        StartRandom();
    }



    void StartRandom()
    {
        if (!isSoup && !isWater && cauldron.storageItems[0] == null) cauldron.storageItems[0] = links.cooking.randomFood[Random.Range(0, 9)];
        if (!isSoup && !isWater && cauldron.storageItems[1] == null) cauldron.storageItems[1] = links.cooking.randomFood[Random.Range(0, 9)];
        if (!isSoup && !isWater && cauldron.storageItems[2] == null) cauldron.storageItems[2] = links.cooking.randomFood[Random.Range(0, 9)];
    }

    
    void Update()
    {
        if (campfire != null && campfire.burningTime > 0) isOnFire = true; else isOnFire = false;
        if (isOnFire && isWater && !isCooking && !isSoup && cauldron.filledSlots > 0)
        {
            StartCoroutine(CookingProcess());
        }
    }

    IEnumerator CookingProcess()
    {
        cauldronInfo.itemName = cauldronName + " (Варится)";
        isCooking = true;
        cookingTimer = 0;
        links.ui.CookingIndicator.maxValue = cookingTime * 100;
        links.ui.CookingIndicator.value = 0;
        links.ui.CookingIndicator.gameObject.SetActive(true);

        links.mousePoint.OpenContainer(gameObject.GetComponent<Container>());

        // Процесс готовки
        while (cookingTimer < cookingTime)
        {
            cookingTimer += Time.deltaTime * links.time.timeFactor / 3600 * campfire.burningTime;
            links.ui.CookingIndicator.value = cookingTimer * 100;
            yield return null;
        }

        int redMushroomCount = 0;
        int rawPotatoCount = 0;
        int soupCount = 0;
        for (int i = 0; i < cauldron.storageItems.Count; i++)
        {
            if (cauldron.storageItems[i] == cooking.redMushroom) redMushroomCount++;
            if (cauldron.storageItems[i] == cooking.rawPotato) rawPotatoCount++;
        }

        if (redMushroomCount > 0 && rawPotatoCount > 0)  // Рецепт грибного супа
        {
            soupCount = redMushroomCount + rawPotatoCount;
            RemoveAll();
            for (int i = 0; i < soupCount; i++) cauldron.storageItems[i] = cooking.mushroomSoup;
            Water(false); Soup(true);
            cauldronInfo.itemName = cauldronName + " с супом";
        }
        else if (redMushroomCount > 0 && rawPotatoCount == 0)  // Рецепт вареных грибов
        {
            RemoveAll();
            for (int i = 0; i < redMushroomCount; i++)
            {
                cauldron.storageItems[i] = cooking.cookedMushroom;
            }
            Water(false);
            cauldronInfo.itemName = cauldronName + " с грибами";
        }
        else if (redMushroomCount == 0 && rawPotatoCount > 0)  // Рецепт вареной картошки
        {
            RemoveAll();
            for (int i = 0; i < rawPotatoCount; i++)
            {
                cauldron.storageItems[i] = cooking.cookedPotato;
            }
            Water(false);
            cauldronInfo.itemName = cauldronName + " с картошкой";
        }
        
        links.storageWindow.Redraw();

        links.ui.BurningIndicator.maxValue = cookingTime * 100;
        links.ui.BurningIndicator.value = 0;
        links.ui.BurningIndicator.gameObject.SetActive(true);

        // Процесс сгорания
        while (cookingTimer < cookingTime * 2 && campfire != null && campfire.burningTime > 0)
        {
            cookingTimer += Time.deltaTime * links.time.timeFactor / 3600 * campfire.burningTime;
            links.ui.BurningIndicator.value = cookingTimer * 100 - cookingTime * 100;
            yield return null;
        }

        if (cookingTimer > cookingTime * 2)
        {
            redMushroomCount = 0;
            rawPotatoCount = 0;
            soupCount = 0;
            for (int i = 0; i < cauldron.storageItems.Count; i++)
            {
                if (cauldron.storageItems[i] == cooking.redMushroom || cauldron.storageItems[i] == cooking.cookedMushroom) redMushroomCount++;
                if (cauldron.storageItems[i] == cooking.rawPotato || cauldron.storageItems[i] == cooking.cookedPotato) rawPotatoCount++;
                if (cauldron.storageItems[i] == cooking.mushroomSoup) soupCount++;
            }

            RemoveAll();
            int count = 0;
            for (int j = 0; j < redMushroomCount; j++)
            {
                cauldron.storageItems[count] = cooking.burnedMushroom;
                count++;
            }
            for (int j = 0; j < rawPotatoCount; j++)
            {
                cauldron.storageItems[count] = cooking.burnedPotato;
                count++;
            }
            for (int j = 0; j < soupCount; j++)
            {
                if (Random.Range(0, 2) < 1) cauldron.storageItems[count] = cooking.burnedPotato;
                else cauldron.storageItems[count] = cooking.burnedMushroom;
                count++;
            }

            cauldronInfo.itemName = cauldronName + " (Всё сгорело)";
            Water(false);
            Soup(false);
            
            links.storageWindow.Redraw();
        }
        isCooking = false;
        links.ui.CookingIndicator.value = 0;
        links.ui.BurningIndicator.value = 0;
        links.ui.CookingIndicator.gameObject.SetActive(false);
        links.ui.BurningIndicator.gameObject.SetActive(false);
        cookingTimer = 0;
    }

    void RemoveAll()
    {
        for (int i = 0; i < cauldron.storageItems.Count; i++)
        {
            if (cauldron.storageItems[i] != null)
            {
                cauldron.storageItems[i] = null;
            }
        }
        for (int i = 0; i < cauldron.storageItemObjects.Count; i++)
        {
            if (cauldron.storageItemObjects[i] != null)
            {
                Destroy(cauldron.storageItemObjects[i]);
                cauldron.storageItemObjects[i] = null;
            }
        }
    }

    public void PlaceCauldron(Campfire newCampfire)
    {
        campfire = newCampfire;
        campfire.gameObject.GetComponent<SphereCollider>().radius = 0.3f;
        gameObject.layer = 0;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.transform.parent = campfire.cauldronPlace;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void GrabCauldron()
    {
        campfire = null;
        isCooking = false;
        isOnFire = false;
        gameObject.layer = 0;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public string withWaterName = "с водой";
    public string withSoupName = "с супом";

    public void Water(bool isFilled)
    {
        if (isFilled)
        {  
            water.SetActive(true);
            isWater = true;
            if (links.mousePoint.pointedIcon != null)
                links.mousePoint.pointedIcon.gameObject.GetComponent<DescribeUI>().EnterUI();
        }
        else
        {
            water.SetActive(false);
            isWater = false;
            if (links.mousePoint.pointedIcon != null)
            links.mousePoint.pointedIcon.gameObject.GetComponent<DescribeUI>().EnterUI();
        }
    }
    public void Soup(bool isFilled)
    {
        if (isFilled)
        {
            soup.SetActive(true);
            isSoup = true;
            cauldronInfo.itemName = cauldronName + " с супом";
            if (links.mousePoint.pointedIcon != null)
                links.mousePoint.pointedIcon.gameObject.GetComponent<DescribeUI>().EnterUI();
            cauldronInfo.isCollectible = false;
            cauldronInfo.isCarrying = true;
        }
        else
        {
            soup.SetActive(false);
            isSoup = false;
            cauldronInfo.itemName = cauldronName;
            if (links.mousePoint.pointedIcon != null)
                links.mousePoint.pointedIcon.gameObject.GetComponent<DescribeUI>().EnterUI();
            cauldronInfo.isCollectible = true;
            cauldronInfo.isCarrying = false;
        }
        cauldron.gameObject.GetComponent<ItemInfo>().itemName = cauldronInfo.itemName;
        links.storageWindow.storageTitle.text = cauldronInfo.itemName;
        links.inventoryWindow.Redraw();

    }

    public void UpdateCauldron()
    {
        cauldron.Recount();
        if (cauldron.filledSlots == 0) Soup(false);
        if (cauldron.storageItems.Contains(links.cooking.mushroomSoup)) Soup(true);
    }
}
