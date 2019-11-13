using UnityEngine;

namespace adolli.Engine
{
    /**
	 * @brief 单点触摸坐标接口
	 */
    public class TouchInput : CoordinateInput
    {

        public Vector3 GetPosition()
        {
            return Vector3.zero;
        }

        public Vector3 GetScreenPosition()
        {
            if (Input.touchCount == 1)
            {
                Vector2 pos = Input.GetTouch(0).position;
                return new Vector3(pos.x, pos.y, 0);
            }
            return Vector3.zero;
        }

        public TouchPhase GetTouchPhase()
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                return TouchPhase.Began;
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                return TouchPhase.Moved;
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                return TouchPhase.Ended;
            }
            return TouchPhase.Canceled;
        }

        public Vector3 GetScreenDeltaPosition()
        {
            Vector2 delta = Input.GetTouch(0).deltaPosition / (Screen.dpi / 40);
            return new Vector3(delta.x, delta.y, 0);
        }
    }
}

