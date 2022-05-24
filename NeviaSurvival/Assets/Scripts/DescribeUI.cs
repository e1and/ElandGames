using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescribeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    
    
    void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    { Debug.Log("Function UI");}

    public void OnPointerEnter(PointerEventData eventData)
    { Debug.Log("Describe UI"); }



    void Update()
    {

    }
    
}
