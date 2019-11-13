using UnityEngine;

namespace adolli.Engine
{
    public interface Touchable
    {
        bool OnTouchBegan(Collider target, TouchInfo touch);
        void OnTouchMoved(Collider target, TouchInfo touch);
        void OnTouchEnded(Collider target, TouchInfo touch);

        string GetTag();
    }
}
