using UnityEngine;

[DisallowMultipleComponent]
public class PlayerMovement : EntityMovement
{
    [Space]
    [SerializeField] private float m_SprintSpeedMultiplyer;
    [SerializeField] private float m_SprintDot;
    [SerializeField] private float m_SprintFOVScale;

    [Space]
    [SerializeField] private float m_CrouchSpeedMultiplyer;
    [SerializeField] private float m_CrouchFOVScale;
    [SerializeField] private float m_CrouchCameraOffset;

    private IFPSCameraProvider m_CameraProvider;

    protected override void Awake()
    {
        m_CameraProvider = GetComponent<IFPSCameraProvider>();

        base.Awake();
    }

    protected override void ApplyMovement()
    {
        float inputDot = Vector3.Dot(m_InputProvider.MovementDirection, transform.forward); 
        if (m_InputProvider.IsSprinting && inputDot > m_SprintDot)
        {
            MovementSpeedScalar *= m_SprintSpeedMultiplyer;
            m_CameraProvider.FOVScale *= m_SprintFOVScale;
        }
        else if (m_InputProvider.IsCrouching)
        {
            MovementSpeedScalar *= m_CrouchSpeedMultiplyer;
            m_CameraProvider.FOVScale *= m_CrouchFOVScale;
            m_CameraProvider.CameraOffset += Vector3.up * m_CrouchCameraOffset;
        }

        base.ApplyMovement();
    }
}
