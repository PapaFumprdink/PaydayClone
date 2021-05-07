using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFPSCameraProvider
{
    Quaternion CameraRotation { get; }
    float FOVScale { get; set; }
    Vector3 CameraOffset { get; set; }
}
