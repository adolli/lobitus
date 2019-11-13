using UnityEngine;

namespace adolli.Engine
{
    public class MoveTo : Tween
    {
        protected Vector3 startPosition_;
        protected Vector3 endPosition_;
        protected double duration_;
        protected RectTransform rectTransform_;

        public MoveTo(GameObject target, Vector3 position, double duration)
        {
            endPosition_ = position;
            duration_ = duration;

            target_ = target;
            startPosition_ = target.transform.localPosition;

            // 对于canvas对象的处理
            rectTransform_ = target_.GetComponent<RectTransform>();
            if (rectTransform_ != null)
            {
                startPosition_ = rectTransform_.anchoredPosition3D;
            }
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && target_ != null)
            {
                timeElapse_ += dt;
                Vector3 newPos = Vector3.Lerp(startPosition_, endPosition_, ease_.f((float)(timeElapse_ / duration_)));
                if (rectTransform_ != null)
                {
                    rectTransform_.anchoredPosition3D = newPos;
                }
                else
                {
                    target_.transform.localPosition = newPos;
                }
                if (timeElapse_ >= duration_)
                {
                    if (rectTransform_ != null)
                    {
                        rectTransform_.anchoredPosition3D = endPosition_;
                    }
                    else
                    {
                        target_.transform.localPosition = endPosition_;
                    }
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
            startPosition_ = target.transform.localPosition;

            // 对于canvas对象的处理
            rectTransform_ = target_.GetComponent<RectTransform>();
            if (rectTransform_ != null)
            {
                startPosition_ = rectTransform_.anchoredPosition3D;
            }
        }

        public override void Start()
        {
            activated_ = true;
            if (rectTransform_ != null)
            {
                startPosition_ = rectTransform_.anchoredPosition3D;
            }
            else
            {
                startPosition_ = target_.transform.localPosition;
            }
        }
    }
}
