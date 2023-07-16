using System.Collections;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraMouseDrag : MonoBehaviour
{
    [SerializeField] private Player mainPlayerCharacter;

    [Header("Camera distance")] [SerializeField]
    private float distance = 10f;

    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 20f;

    public CinemachineVirtualCamera vCam;
    private CinemachineComponentBase componentBase;

    public PlayerInput _playerInput;


    private void OnEnable()
    {
        _playerInput = mainPlayerCharacter.GetComponent<PlayerInput>();
        componentBase = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);

        if (componentBase is CinemachineFramingTransposer)
            _playerInput.actions["Scroll"].performed += OnScroll;
    }

    private void OnDisable()
    {
        _playerInput.actions["Scroll"].performed -= OnScroll;
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        distance -= context.ReadValue<Vector2>().y;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        ((CinemachineFramingTransposer) componentBase).m_CameraDistance = distance;
    }

}