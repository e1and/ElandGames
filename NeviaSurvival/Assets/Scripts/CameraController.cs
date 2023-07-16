using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
    public class CameraController : MonoBehaviour
    {
        Cinemachine3rdPersonFollow cinemachine;
        CinemachineVirtualCamera cinemachineVirtualCamera;
        private Links links;
        [SerializeField] Vector3 defaultPosition;
        [SerializeField] Quaternion defaultRotation;
        [SerializeField] GameObject player;
        [SerializeField] GameObject cameraRoot;
        [SerializeField] ThirdPersonController thirdPerson;
        public float maxCameraDistance = 8;
        public float minCameraDistance = 2;
        float cameraY;
        float cameraX;
        public Vector3 rightHandParentPositionThirdPerson;
        public Vector3 rightHandParentPositionFirstPerson;
        public Vector3 leftHandParentPositionThirdPerson;
        public Vector3 leftHandParentPositionFirstPerson;

        void Start()
        {
            cinemachine = FindObjectOfType<Cinemachine3rdPersonFollow>();
            cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            links = FindObjectOfType<Links>();

            rightHandParentPositionThirdPerson = new Vector3(-0.15f, 0.018f, -0.076f);
            rightHandParentPositionFirstPerson = new Vector3(0.352f, 0.518f, 0.01f);
            leftHandParentPositionThirdPerson = new Vector3(0.13f, 0.062f, 0.051f);
            leftHandParentPositionFirstPerson = new Vector3(-0.2f, -0.62f, -0.2f);

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
            { if (cinemachine.CameraDistance < maxCameraDistance) cinemachine.CameraDistance += 0.2f; }
            if (Input.mouseScrollDelta.y > 0)
            { if (cinemachine.CameraDistance > minCameraDistance) cinemachine.CameraDistance -= 0.2f; }
        }

        public void CameraMode1()
        {
            cinemachineVirtualCamera.m_Lens.FieldOfView = 60;
            cinemachine.ShoulderOffset = new Vector3(0.7f, -0.4f, 0);
            cinemachine.CameraDistance = 2.6f;
            maxCameraDistance = 8f;
            minCameraDistance = 2f;
            links.PlayerMesh.SetActive(true);
            links.inventoryWindow.headParent.gameObject.SetActive(true);
            links.inventoryWindow.backpackParent.gameObject.SetActive(true);
            links.inventoryWindow.rightHandParent.localPosition = rightHandParentPositionThirdPerson;
            links.inventoryWindow.leftHandParent.localPosition = leftHandParentPositionThirdPerson;
        }
        
        public void CameraMode2()
        {
            cinemachine.ShoulderOffset = new Vector3(0f, 0f, 0);
            cinemachine.CameraDistance = 4f;
            maxCameraDistance = 5f;
            minCameraDistance = 2f;
            links.PlayerMesh.SetActive(true);
            links.inventoryWindow.backpackParent.gameObject.SetActive(true);
        }
        
        public void CameraMode3()
        {
            cinemachineVirtualCamera.m_Lens.FieldOfView = 70;
            cinemachine.ShoulderOffset = new Vector3(0f, 0.2f, 0);
            cinemachine.CameraDistance = 0.1f;
            maxCameraDistance = 0.1f;
            minCameraDistance = 0.1f;
            links.PlayerMesh.SetActive(false);
            links.inventoryWindow.backpackParent.gameObject.SetActive(false);
            links.inventoryWindow.headParent.gameObject.SetActive(false);
            links.inventoryWindow.rightHandParent.localPosition = rightHandParentPositionFirstPerson;
            links.inventoryWindow.leftHandParent.localPosition = leftHandParentPositionFirstPerson;

        }
    }
}
