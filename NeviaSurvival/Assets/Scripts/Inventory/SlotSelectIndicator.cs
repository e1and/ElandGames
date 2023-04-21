using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotSelectIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public MousePoint mousePoint;
    Image image;

    private void Start()
    {
        mousePoint = FindAnyObjectByType<MousePoint>();
        image = mousePoint.IconHighLight.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterSlot();
    }
    public void EnterSlot()
    {
        mousePoint.IconHighLight.SetActive(true);
        mousePoint.IconHighLight.transform.position = gameObject.transform.position;
        image.enabled = true;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        mousePoint.IconHighLight.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            image.enabled = false;
        }    
        else
        { image.enabled = true; }
    }
}
