using UnityEngine;
using System.Collections;

public class CampFire : MonoBehaviour {
	public GameObject Maket;
	public GameObject campFire;
	public GameObject parentObject;

	private bool maket;
	public int sticksTotal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.B)) {
			Maket.SetActive (true);
			maket = true;
		}
		if (maket == true) {
			if (gameObject.GetComponent<StickInvent>().stick >= sticksTotal)
			{
				if (Input.GetMouseButton(0))
				{
					Maket.SetActive(false);
					campFire.SetActive(true);
					campFire.transform.SetParent(parentObject.transform, true);
					gameObject.GetComponent<StickInvent>().stick -= sticksTotal;
					gameObject.GetComponent<CampFire>().enabled = false;
				}
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Maket.SetActive (false);
				maket = false;
			}
		}

	}
}