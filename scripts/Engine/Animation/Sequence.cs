using System.Collections.Generic;

namespace adolli.Engine
{
    public class Sequence : Tween
    {
        private LinkedList<Tween> tweenQueue_;
        private Tween activatedTween_;

        public Sequence()
        {
            tweenQueue_ = new LinkedList<Tween>();
            activatedTween_ = null;
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && (activatedTween_ == null || activatedTween_.ActionDone()))
            {
                if (tweenQueue_.Count > 0)
                {
                    activatedTween_ = tweenQueue_.First.Value;
                    activatedTween_.Start();
                    tweenQueue_.RemoveFirst();
                }
                else
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

        public void AddToQueue(Tween tween)
        {
            tweenQueue_.AddLast(tween);
        }

    }
}


