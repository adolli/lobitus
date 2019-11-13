using UnityEngine;
using System.Collections;

namespace adolli
{
    public abstract class SmallCubeGroup : IFace
    {

        public static float AnimationIntervalMax = 1.5f;

        protected RubicCube cube_;
        public RubicCube parentCube
        {
            get { return cube_; }
        }

        public Vector3 NormalVector;

        protected bool animating_;
        protected float startTime_;
        protected Quaternion rotationTo_;

        private bool dockingEffectPlayed_;

        public SmallCubeGroup()
        {
            cube_ = null;
            animating_ = false;
            dockingEffectPlayed_ = false;
        }

        public SmallCubeGroup(RubicCube cube, Vector3 normal)
        {
            cube_ = cube;
            NormalVector = normal;
            animating_ = false;
            dockingEffectPlayed_ = false;
        }


        public abstract void Bind();
        public abstract void Unbind();
        public abstract void OnActionDone();
        public abstract string GetFaceName();


        /**
		 * @brief
		 */
        public virtual void AnimatedRotate(int n)
        {
            animating_ = true;
            startTime_ = Time.time;
            rotationTo_ = Quaternion.Euler(NormalVector * n * 90);
        }

        public bool ActionDone()
        {
            return !animating_;
        }

        public bool Binded()
        {
            return cube_.cubeCenter_.transform.childCount > 0;
        }

        public bool Step(float dt)
        {
            bool steady = false;
            if (!ActionDone())
            {
                Quaternion oldrot = cube_.cubeCenter_.transform.rotation;
                Quaternion newrot = Quaternion.Slerp(oldrot, rotationTo_, (Time.time - startTime_) / AnimationIntervalMax);
                cube_.cubeCenter_.transform.rotation = newrot;
                float dAngle = Mathf.Abs(newrot.x - rotationTo_.x) + Mathf.Abs(newrot.y - rotationTo_.y) + Mathf.Abs(newrot.z - rotationTo_.z);
                if ((dAngle < 0.1 || dAngle > 2 - 0.1) && !dockingEffectPlayed_)
                {
                    DockingEffect.Instance.Play();
                    dockingEffectPlayed_ = true;
                }
                if (dAngle < 0.001 || dAngle > 2 - 0.001)
                {
                    cube_.cubeCenter_.transform.rotation = rotationTo_;
                    animating_ = false;
                    steady = true;
                    dockingEffectPlayed_ = false;
                }
            }
            return steady;
        }

        public void Stop()
        {
            animating_ = false;
        }

        public void SetRotation(float angle)
        {
            cube_.cubeCenter_.transform.Rotate(NormalVector * angle);
        }

        public void SmoothDock()
        {
            float angle1;
            Vector3 axis;
            cube_.cubeCenter_.transform.rotation.ToAngleAxis(out angle1, out axis);
            int dir = Vector3.Angle(axis, NormalVector) <= float.Epsilon ? 1 : -1;
            int rotdir = ((int)angle1 + 45) / 90 * dir;
            AnimatedRotate(rotdir);
        }

        public void Start()
        {
        }

        public void Pause()
        {
        }
    }
}
