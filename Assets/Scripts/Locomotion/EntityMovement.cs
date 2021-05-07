using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EntityMovement : MonoBehaviour
{
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_AccelerationTime;
    [SerializeField] private float m_AirAccelerationTime;

    [Space]
    [SerializeField] private float m_JumpPower;

    [Space]
    [SerializeField] private Transform m_GroundCheckPoint;
    [SerializeField] private float m_GroundCheckRadius;
    [SerializeField] private LayerMask m_GroundCheckMask;

    protected IMovementProvider m_InputProvider;
    protected Rigidbody m_Rigidbody;

    public float MovementSpeedScalar { get; set; }

    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_InputProvider = GetComponent<IMovementProvider>();

        m_InputProvider.JumpEvent += Jump;

        MovementSpeedScalar = 1f;
    }

    private void Update()
    {
        ApplyMovement();
    }

    protected virtual void ApplyMovement()
    {
        Vector3 currentVelocity = m_Rigidbody.velocity;
        Vector3 targetVelocity = m_InputProvider.MovementDirection * m_MovementSpeed * MovementSpeedScalar;
        Vector3 velocityDifference = (targetVelocity - currentVelocity);
        Vector3 acceleration = velocityDifference / (GetGrounded() ? m_AccelerationTime : m_AirAccelerationTime);

        m_Rigidbody.velocity += acceleration * Time.deltaTime;

        MovementSpeedScalar = 1f;
    }

    private void Jump()
    {
        if (GetGrounded())
        {
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
        }    
    }

    public bool GetGrounded ()
    {
        Collider[] queryList = new Collider[2];
        Physics.OverlapSphereNonAlloc(m_GroundCheckPoint.position, m_GroundCheckRadius, queryList, m_GroundCheckMask);

        foreach (Collider query in queryList)
        {
            if (query)
            {
                if (query.gameObject != gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_GroundCheckPoint)
        {
            Gizmos.color = GetGrounded() ? Color.green : Color.red;
            Gizmos.DrawSphere(m_GroundCheckPoint.position, m_GroundCheckRadius);
            Gizmos.color = Color.white;
        }
    }
}
