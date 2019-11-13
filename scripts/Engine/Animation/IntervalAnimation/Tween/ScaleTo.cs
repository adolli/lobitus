using UnityEngine;

namespace adolli.Engine
{
    public class ScaleTo : Tween
    {
        protected Vector3 startScaler_;
        protected Vector3 endScaler_;
        protected double duration_;
        //protected RectTransform rectTransform_;

        public ScaleTo(GameObject target, Vector3 scaler, double duration)
        {
            endScaler_ = scaler;
            duration_ = duration;
            target_ = target;
            startScaler_ = target.transform.localScale;
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && target_ != null)
            {
                timeElapse_ += dt;
                Vector3 newScale = Vector3.Lerp(startScaler_, endScaler_, ease_.f((float)(timeElapse_ / duration_)));
                target_.transform.localScale = newScale;
                if (timeElapse_ >= duration_)
                {
                    target_.transform.localPosition = endScaler_;
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
            startScaler_ = target.transform.localScale;
        }
    }
}

