using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenDown : MonoBehaviour
{
    [SerializeField] float openTime = 2;
    [SerializeField] bool isOpen = false;
    [SerializeField] float doorSpeed = 1;
    float _speed;
    public bool isMoving;

    public void StartOpenDoor()
    {
        if (!isOpen && !isMoving)
        {
            _speed = -doorSpeed;
            StartCoroutine(OpenDoor());
        }
        if (isOpen && !isMoving)
        {
            _speed = doorSpeed;
            StartCoroutine(OpenDoor());
        }
    }
    
    private IEnumerator OpenDoor()
    {
        isMoving = true;
        float timer = 0;
        while (timer < openTime)
        {
            yield return null;
            transform.Translate(new Vector3(0, _speed, 0) * Time.deltaTime);
            timer += Time.deltaTime;
        }
        if (_speed == doorSpeed) isOpen = false;
        else isOpen = true;
        isMoving = false;
    }
}
