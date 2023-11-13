using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private Vector3 shift;
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            
            transform.position = Input.mousePosition + shift;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        shift = transform.position - Input.mousePosition;
    }
}
