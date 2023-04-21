using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
    public class CameraController : MonoBehaviour
    {
        Cinemachine3rdPersonFollow cinemachine;
        CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] Vector3 defaultPosition;
        [SerializeField] Quaternion defaultRotation;
        [SerializeField] GameObject player;
        [SerializeField] GameObject cameraRoot;
        [SerializeField] ThirdPersonController thirdPerson;
        float cameraY;
        float cameraX;

        void Start()
        {
            cinemachine = FindObjectOfType<Cinemachine3rdPersonFollow>();
            cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();


            RotateCameraToPlayer(false);
        }

        public void RotateCameraToPlayer(bool isDeath)
        {
            
            if (isDeath)
            {
                cameraY = player.transform.eulerAngles.y - 180;
                cameraX = 70;
            }
            else
            {
                cameraY = player.transform.eulerAngles.y;
                cameraX = 20;
            }

            thirdPerson.CustomRotateCamera(cameraY, cameraX);

        }


        void Update()
        {
            if (Input.mouseScrollDelta.y < 0)
            { if (cinemachine.CameraDistance < 8) cinemachine.CameraDistance += 0.2f; }
            if (Input.mouseScrollDelta.y > 0)
            { if (cinemachine.CameraDistance > 1) cinemachine.CameraDistance -= 0.2f; }
        }
    }
}
