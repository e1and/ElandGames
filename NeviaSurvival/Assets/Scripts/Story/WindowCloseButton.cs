using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCloseButton : MonoBehaviour
{
    [SerializeField] GameObject window;
    Links links;

    private void Start()
    {
        links = FindObjectOfType<Links>();
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        links.mousePoint.isPointUI = false;
        
    }
}
