using UnityEngine;

namespace adolli.Engine
{
    public class RotateBy : Tween
    {
        protected Vector3 startRotation_;
        protected Vector3 deltaRotation_;
        protected double duration_;

        public RotateBy(GameObject target, Vector3 rotation, double duration)
        {
            deltaRotation_ = rotation;
            duration_ = duration;
            target_ = target;
            startRotation_ = target.transform.rotation.eulerAngles;
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && target_ != null)
            {
                timeElapse_ += dt;
                target_.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation_, startRotation_ + deltaRotation_, ease_.f((float)(timeElapse_ / duration_))));
                if (timeElapse_ >= duration_)
                {
                    target_.transform.rotation = Quaternion.Euler(startRotation_ + deltaRotation_);
                    actionDone_ = true;
                    if (actionDoneListener_ != null)
                    {
                        actionDoneListener_();
                    }
                }
            }
            return actionDone_;
        }

        public override void SetTarget(GameObject target)
        {
            target_ = target;
            startRotation_ = target.transform.rotation.eulerAngles;
        }
    }
}