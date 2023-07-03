
using UnityEngine;

public class Lamp : MonoBehaviour
{
    Links links;
    [SerializeField] MeshRenderer lampOn;
    [SerializeField] MeshRenderer lampOff;
    [SerializeField] Light _light;
    public bool isOn;

    private void Start()
    {
        links = FindObjectOfType<Links>();
        ConfigureLamp();
        CheckSaveList();
    }

    public void ConfigureLamp()
    {
        if (isOn) TurnOn(); else TurnOff();
    }
    
    void CheckSaveList()
    {
        if (!links.saveObjects.lamps.Contains(this))
            Debug.LogError($"Фонарь { gameObject.name } не добавлен в список сохраняемых объектов!");
    }

    public void Switch(bool isOn)
    {
        if (isOn) TurnOn(); else TurnOff();
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
