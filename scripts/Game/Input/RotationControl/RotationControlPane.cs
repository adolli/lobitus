using UnityEngine;
using adolli.Engine;
using System.Collections;

namespace adolli
{
    public abstract class RotationControlPane : MonoBehaviour, Touchable
    {
        public string TouchTag;

        protected readonly Vector3 TouchFaceExpand = new Vector3(20f, 0, 20f);
        protected readonly Vector3 TouchFaceInitialScale = new Vector3(0.28f, 0.28f, 0.28f);

        protected RubicCube rubicCube_;
        protected Vector3 lastPosition_;
        protected AudioSource pickEffect_;

        // Use this for initialization
        void Start()
        {
            rubicCube_ = GameObject.Find("RubicCube").GetComponent<RubicCube>();
            TouchDispatcher.AddTouchListener(this);

            pickEffect_ = gameObject.AddComponent<AudioSource>();
            AudioClip ac = Resources.Load("effect/pick", typeof(AudioClip)) as AudioClip;
            pickEffect_.clip = ac;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public virtual bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            return false;
        }

        public virtual void OnTouchMoved(Collider target, TouchInfo touch)
        {
            if (!rubicCube_.ActiveFace.IsAnimating())
            {
                Vector3 currentPos = touch.position;
                Debug.DrawRay(target.transform.position, lastPosition_);
                Debug.DrawRay(target.transform.position, currentPos);

                float angle = Vector3.Angle(lastPosition_, currentPos);
                rubicCube_.ActiveFace.Rotate(Vector3.Cross(currentPos, lastPosition_), angle);
                lastPosition_ = currentPos;
            }
        }

        public virtual void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (!rubicCube_.ActiveFace.IsAnimating())
            {
                rubicCube_.ActiveFace.SmoothDock();
            }
            this.transform.localScale = TouchFaceInitialScale;
        }

        public string GetTag()
        {
            return TouchTag;
        }

    }
}
