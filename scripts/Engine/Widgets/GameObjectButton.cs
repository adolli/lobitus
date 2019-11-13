using UnityEngine;

namespace adolli.Engine
{

    /**
	 * @brief 场景中的3D对象按钮，由于3D场景的特殊性，使用前默认是没有使能的
	 */
    public abstract class GameObjectButton : MonoBehaviour, Touchable
    {

        protected bool enable_;
        public virtual bool enable
        {
            get { return enable_; }
            set { enable_ = value; }
        }

        protected virtual void Start()
        {
            enable_ = false;
            TouchDispatcher.AddTouchListener(this);
        }

        void Update()
        {
        }

        public virtual bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            return enable_;
        }

        public virtual void OnTouchMoved(Collider target, TouchInfo touch)
        {
        }

        public abstract void OnTouchEnded(Collider target, TouchInfo touch);
        public abstract string GetTag();

    }
}


