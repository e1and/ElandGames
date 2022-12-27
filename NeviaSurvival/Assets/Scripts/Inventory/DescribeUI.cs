using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescribeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public MousePoint mousePoint;
    
    void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    { Debug.Log("Function UI");}

    public void OnPointerEnter(PointerEventData eventData)
    {
        mousePoint.isPointUI = true;
        Debug.Log("Describe UI"); 
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        mousePoint.isPointUI = false;
    }
}
