using UnityEngine;
using System.Collections;

public class BuildCampFire : MonoBehaviour 
{
	public GameObject Maket;
	public GameObject campFirePrefab;
	GameObject campfire;
	public GameObject parentObject;
	public Transform playerBuildings;
	public Vector3 buildingPlace;
	Inventory inventory;
	
	private bool maket;
	public int sticksTotal;


	void Start () 
	{
		inventory = GetComponent<Inventory>();
	}
	
	void Update () 
	{
		parentObject.transform.position = buildingPlace;

		if (Input.GetKeyDown (KeyCode.B)) 
		{
			Maket.SetActive (true);
			Maket.transform.SetParent(parentObject.transform);
			maket = true;
		}

		if (maket == true) 
		{
			Maket.transform.position = buildingPlace;

			if (gameObject.GetComponent<Player>().Sticks >= sticksTotal)
			{
				if (Input.GetMouseButton(0))
				{
					Maket.SetActive(false);
					campfire = Instantiate(campFirePrefab);
					campfire.transform.position = buildingPlace;
					campfire.transform.SetParent(playerBuildings, true);
					gameObject.GetComponent<Player>().Sticks -= sticksTotal;
					maket = false;
				}
			}

			if (Input.GetKeyDown (KeyCode.Escape)) 
			{
				Maket.SetActive (false);
				maket = false;
			}
		}

	}
}