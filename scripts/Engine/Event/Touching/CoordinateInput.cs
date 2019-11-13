using UnityEngine;

namespace adolli.Engine
{
    public interface CoordinateInput
    {
        Vector3 GetPosition();
        Vector3 GetScreenPosition();
        TouchPhase GetTouchPhase();
        Vector3 GetScreenDeltaPosition();
    }
}


