using UnityEngine;

namespace adolli.Engine
{
    public abstract class Tween
    {
        protected GameObject target_;
        protected bool actionDone_;
        protected bool activated_;
        protected double timeElapse_;
        protected Ease ease_;

        public delegate void ActionDoneListener();
        protected ActionDoneListener actionDoneListener_;

        public Tween()
        {
            target_ = null;
            actionDone_ = false;
            activated_ = false;
            timeElapse_ = 0;
            ease_ = new Linear();
            actionDoneListener_ = null;

            //  向动画调度器注册该动画
            AnimationScheduler.AddTween(this);
        }

        public abstract bool Step(double dt);


        public virtual bool Playing()
        {
            return activated_;
        }

        public virtual bool ActionDone()
        {
            return actionDone_;
        }

        public virtual void Start()
        {
            activated_ = true;
        }

        public virtual void Pause()
        {
            activated_ = false;
        }

        public virtual void Stop()
        {
            actionDone_ = true;
            activated_ = false;
        }

        public virtual void SetTarget(GameObject target)
        {
            target_ = target;
        }

        public void SetOnActionDoneCallback(ActionDoneListener listener)
        {
            actionDoneListener_ = listener;
        }

        public void SetEasing(Ease ease)
        {
            ease_ = ease;
        }
    }
}
