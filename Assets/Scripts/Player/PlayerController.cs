using System;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour, IMovementProvider, IFPSCameraProvider, IWeaponInputProvider
{
    private const float Deadzone = 0.5f;

    public event Action JumpEvent;
    public event Action FireEvent;
    public event Action ReloadEvent;

    private Controls m_Controls;
    private Vector2 m_SSRotation;

    public Quaternion CameraRotation => Quaternion.Euler(m_SSRotation.y, m_SSRotation.x, 0f);
    public Vector3 MovementDirection
    {
        get
        {
            var localDirection = m_Controls.General.Movement.ReadValue<Vector2>();
            return transform.TransformDirection(localDirection.x, 0f, localDirection.y);
        }
    }

    public bool WantsToFire => m_Controls.General.Fire.ReadValue<float>() > Deadzone;
    public bool IsAimingDownSights => m_Controls.General.ADS.ReadValue<float>() > Deadzone;

    public bool IsSprinting => m_Controls.General.Sprinting.ReadValue<float>() > Deadzone;
    public bool IsCrouching => m_Controls.General.Crouching.ReadValue<float>() > Deadzone;

    public float FOVScale { get; set; }
    public Vector3 CameraOffset { get; set; }

    private void Awake()
    {
        m_Controls = new Controls();

        m_Controls.General.Jump.performed += (ctx) => JumpEvent?.Invoke();
        m_Controls.General.Fire.performed += (ctx) => FireEvent?.Invoke();
        m_Controls.General.Reload.performed += (ctx) => ReloadEvent?.Invoke();

        FOVScale = 1f;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        m_Controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        m_Controls.Disable();
    }

    private void Update()
    {
        m_SSRotation += m_Controls.General.CameraMovement.ReadValue<Vector2>();
        m_SSRotation.y = Mathf.Clamp(m_SSRotation.y, -90f, 90f);
    }

    private void LateUpdate()
    {
        FOVScale = 1f;
    }
}
