using UnityEngine;

namespace adolli.Engine
{
    public class MouseInput : CoordinateInput
    {

        //private Vector3 lastMousePosition_;

        public Vector3 GetPosition()
        {
            return Vector3.zero;
        }

        public Vector3 GetScreenPosition()
        {
            return Input.mousePosition;
        }

        public TouchPhase GetTouchPhase()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return TouchPhase.Began;
            }
            else if (Input.GetMouseButton(0))
            {
                return TouchPhase.Moved;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                return TouchPhase.Ended;
            }
            return TouchPhase.Canceled;
        }

        public Vector3 GetScreenDeltaPosition()
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            return new Vector3(x, y, 0);
        }
    }
}


