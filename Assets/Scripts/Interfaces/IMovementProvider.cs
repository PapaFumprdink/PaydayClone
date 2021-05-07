using System;
using UnityEngine;

public interface IMovementProvider
{
    event Action JumpEvent;
    Vector3 MovementDirection { get; }
    bool IsSprinting { get; }
    bool IsCrouching { get; }
}
