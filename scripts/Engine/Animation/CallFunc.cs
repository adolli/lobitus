using UnityEngine;

namespace adolli.Engine
{
    // Tween 这个类型后续要修改
    public class CallFunc : Tween
    {
        public delegate void FunctionType();
        private FunctionType func_;

        public CallFunc(FunctionType func)
        {
            func_ = func;
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_)
            {
                if (func_ != null)
                {
                    func_();
                }
                actionDone_ = true;
                if (actionDoneListener_ != null)
                {
                    actionDoneListener_();
                }
            }
            return actionDone_;
        }

    }
}


