using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MousePointTerrain : MonoBehaviour, IPointerClickHandler
{
    public GameObject Player;
    public GameObject Target;
    private Vector3 lookDirection;
    public event UnityAction<Vector3> OnClick;

    private void Start()
    {
        OnClick += (pos) =>
        {
            Target.transform.position = pos;
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick.Invoke(eventData.pointerPressRaycast.worldPosition);
    }
}
