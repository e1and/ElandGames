using UnityEngine;
using System.Collections;

public class Torchlight : MonoBehaviour {
	
	public GameObject TorchLight;
	public GameObject MainFlame;
	public GameObject BaseFlame;
	public GameObject Etincelles;
	public GameObject Fumee;
	public float MaxLightIntensity;
	public float IntensityLight;
	public Color color;
	public bool isBurn;
	public float burningTime = 10;

	void Start () 
	{
		TorchLight.GetComponent<Light>().intensity=IntensityLight;
		MainFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*20f;
		BaseFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*15f;	
		Etincelles.GetComponent<ParticleSystem>().emissionRate=IntensityLight*7f;
		Fumee.GetComponent<ParticleSystem>().emissionRate=IntensityLight*12f;
	}
	

	void Update () 
	{
		if (IntensityLight<0) IntensityLight=0;
		if (IntensityLight>MaxLightIntensity) IntensityLight=MaxLightIntensity;		

		TorchLight.GetComponent<Light>().intensity =
		IntensityLight / 2f + Mathf.Lerp(IntensityLight, IntensityLight + 0.02f, Mathf.Cos(Time.time * 30));

		TorchLight.GetComponent<Light>().color = color;
			//new Color(Mathf.Min(IntensityLight/1.5f,1f),Mathf.Min(IntensityLight/2f,1f),0f);
		MainFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*20f;
		BaseFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*15f;
		Etincelles.GetComponent<ParticleSystem>().emissionRate=IntensityLight*7f;
		Fumee.GetComponent<ParticleSystem>().emissionRate=IntensityLight*12f;

		if (isBurn && burningTime > 0) burningTime -= Time.deltaTime;
		else if (isBurn) { burningTime = 0; }

		if (burningTime == 0) { isBurn = false; TorchLight.SetActive(false); }
		else TorchLight.SetActive(true);

		IntensityLight = burningTime * 0.1f;
		if (IntensityLight >= MaxLightIntensity) IntensityLight = MaxLightIntensity;

	}
}
