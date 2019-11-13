using UnityEngine;

namespace adolli.Engine
{
    public class Wait : Tween
    {
        private double duration_;

        public Wait(double duration)
        {
            duration_ = duration;
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_)
            {
                timeElapse_ += dt;
                if (timeElapse_ >= duration_)
                {
                    actionDone_ = true;
                    if (actionDoneListener_ != null)
                    {
                        actionDoneListener_();
                    }
                }
            }
            return actionDone_;
        }

        public Wait Clone()
        {
            Wait ret = new Wait(duration_);
            ret.actionDoneListener_ = actionDoneListener_;
            return ret;
        }
    }
}
