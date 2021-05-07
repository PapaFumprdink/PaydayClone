using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FPSCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_CameraRotor;
    [SerializeField] private Vector3 m_DefaultCameraOffset;
    [SerializeField] private float m_CameraPositionalSmoothing;

    [Space]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_VirtualCamera;
    [SerializeField] private float m_BaseFOV;
    [SerializeField] private float m_FOVSmoothing;

    private IFPSCameraProvider m_InputProvider;

    private void Awake()
    {
        m_InputProvider = GetComponent<IFPSCameraProvider>();

        m_InputProvider.CameraOffset = m_DefaultCameraOffset;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        float targetFov = m_BaseFOV / m_InputProvider.FOVScale;
        float fovDifference = targetFov - m_VirtualCamera.m_Lens.FieldOfView;

        m_VirtualCamera.m_Lens.FieldOfView += fovDifference / m_FOVSmoothing * Time.deltaTime;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, m_InputProvider.CameraRotation.eulerAngles.y, 0f);
        m_CameraRotor.rotation = m_InputProvider.CameraRotation;

        Vector3 targetPosition = m_InputProvider.CameraOffset;
        Vector3 positionDifference = targetPosition - m_CameraRotor.localPosition;

        m_CameraRotor.localPosition += positionDifference * m_CameraPositionalSmoothing * Time.deltaTime;
        m_InputProvider.CameraOffset = m_DefaultCameraOffset;
    }
}
