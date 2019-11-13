using UnityEngine;

namespace adolli.Engine
{
    public class AlphaTo : Tween
    {
        protected float startAlpha_;
        protected float endAlpha_;
        protected double duration_;
        protected CanvasGroup canvasGroup_;

        public AlphaTo(GameObject target, float alpha, double duration)
        {
            endAlpha_ = alpha;
            duration_ = duration;
            target_ = target;
            canvasGroup_ = target.GetComponent<CanvasGroup>();
            if (canvasGroup_ != null)
            {
                startAlpha_ = canvasGroup_.alpha;
            }
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && canvasGroup_ != null)
            {
                timeElapse_ += dt;
                float newAlpha = Mathf.Lerp(startAlpha_, endAlpha_, ease_.f((float)(timeElapse_ / duration_)));
                canvasGroup_.alpha = newAlpha;
                if (timeElapse_ >= duration_)
                {
                    canvasGroup_.alpha = endAlpha_;
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
            canvasGroup_ = target.GetComponent<CanvasGroup>();
            if (canvasGroup_ != null)
            {
                startAlpha_ = canvasGroup_.alpha;
            }
        }

        public override void Start()
        {
            activated_ = true;
            if (canvasGroup_ != null)
            {
                startAlpha_ = canvasGroup_.alpha;
            }
        }
    }
}

