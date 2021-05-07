using System;
using UnityEngine;

public interface IWeaponInputProvider
{
    event Action FireEvent;
    event Action ReloadEvent;
    bool WantsToFire { get; }
    bool IsAimingDownSights { get; }
    float FOVScale { get; }
}
