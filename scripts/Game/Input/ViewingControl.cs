using UnityEngine;
using adolli.Engine;
using System.Collections;

namespace adolli
{
    public class ViewingControl : MonoBehaviour, Touchable
    {
        public string TouchTag;

        private static float moveX_ = 0f;
        private static float moveY_ = 0f;
        private static float cameraMoveSpeed_ = 1.5f;

        public static bool enable
        {
            get;
            set;
        }

        public static void ResetViewDirection()
        {
            moveX_ = 0f;
            moveY_ = 0f;
        }

        // Use this for initialization
        void Start()
        {
            TouchDispatcher.AddTouchListener(this);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            return enable;
        }

        public void OnTouchMoved(Collider target, TouchInfo touch)
        {
            Vector3 delta = touch.deltaScreenPosition;
            moveX_ += cameraMoveSpeed_ * delta.x;
            moveY_ += cameraMoveSpeed_ * delta.y;
            Quaternion rotationTo = Quaternion.Euler(0, moveX_, -moveY_);
            Transform cam = Camera.main.transform.parent;
            cam.rotation = rotationTo;
        }

        public void OnTouchEnded(Collider target, TouchInfo touch)
        {
        }

        public string GetTag()
        {
            return TouchTag;
        }
    }
}
