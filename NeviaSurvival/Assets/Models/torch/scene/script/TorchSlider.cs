using UnityEngine;
using System.Collections;

public class TorchSlider : MonoBehaviour {
	public GameObject TorchObj;
	public GUISkin SkinSlider;
	
    void OnGUI() {
		GUI.Label(new Rect(25,25,150,30),"Light Intensity",SkinSlider.label);
        TorchObj.GetComponent<Torchlight>().IntensityLight = GUI.HorizontalSlider(new Rect(25, 50, 150, 30), TorchObj.GetComponent<Torchlight>().IntensityLight, 0.0F, TorchObj.GetComponent<Torchlight>().MaxLightIntensity,SkinSlider.horizontalSlider,SkinSlider.horizontalSliderThumb);
    }
}