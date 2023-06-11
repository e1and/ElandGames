using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] MeshRenderer lampOn;
    [SerializeField] MeshRenderer lampOff;
    [SerializeField] Light _light;
    public bool isOn;
    bool isProcess;

    private void Start()
    {
        if (isOn) TurnOn(); else TurnOff();
    }

    public void Switch(bool isOn)
    {
        if (!isProcess) StartCoroutine(Switching(isOn));
    }

    IEnumerator Switching(bool isOn)
    {
        isProcess = true;
        yield return new WaitForSeconds(1);
        if (isOn) TurnOn(); else TurnOff();
        isProcess = false;
    }
    
    public void TurnOn()
    {
        isOn = true;
        lampOn.enabled = true;
        lampOff.enabled = false;
        _light.enabled = true;
    }

    public void TurnOff()
    {
        isOn = false;
        lampOn.enabled = false;
        lampOff.enabled = true;
        _light.enabled = false;
    }
}
