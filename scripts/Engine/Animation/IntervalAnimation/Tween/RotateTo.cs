using UnityEngine;

namespace adolli.Engine
{
    public class RotateTo : Tween
    {
        protected Vector3 startRotation_;
        protected Vector3 endRotation_;
        protected double duration_;

        public RotateTo(GameObject target, Vector3 rotation, double duration)
        {
            endRotation_ = rotation;
            duration_ = duration;

            target_ = target;
            startRotation_ = target.transform.rotation.eulerAngles;
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && target_ != null)
            {
                timeElapse_ += dt;
                target_.transform.rotation = Quaternion.Euler(Vector3.Slerp(startRotation_, endRotation_, ease_.f((float)(timeElapse_ / duration_))));
                if (timeElapse_ >= duration_)
                {
                    target_.transform.rotation = Quaternion.Euler(endRotation_);
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
