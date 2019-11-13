using UnityEngine;

namespace adolli.Engine
{
    public class PitchTo : Tween
    {
        protected float startPitch_;
        protected float endPitch_;
        protected double duration_;
        protected AudioSource audioSource_;

        public PitchTo(GameObject target, float alpha, double duration)
        {
            endPitch_ = alpha;
            duration_ = duration;
            target_ = target;
            audioSource_ = target.GetComponent<AudioSource>();
            if (audioSource_ != null)
            {
                startPitch_ = audioSource_.pitch;
            }
        }

        public override bool Step(double dt)
        {
            if (activated_ && !actionDone_ && audioSource_ != null)
            {
                timeElapse_ += dt;
                float newPitch = Mathf.Lerp(startPitch_, endPitch_, ease_.f((float)(timeElapse_ / duration_)));
                audioSource_.pitch = newPitch;
                if (timeElapse_ >= duration_)
                {
                    audioSource_.pitch = endPitch_;
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
            audioSource_ = target.GetComponent<AudioSource>();
            if (audioSource_ != null)
            {
                startPitch_ = audioSource_.pitch;
            }
        }

        public override void Start()
        {
            activated_ = true;
            if (audioSource_ != null)
            {
                startPitch_ = audioSource_.pitch;
            }
        }
    }
}

