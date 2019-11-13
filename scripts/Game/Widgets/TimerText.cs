using UnityEngine;

namespace adolli
{
    public class TimerText : MonoBehaviour
    {

        private static TimerText instance_;
        public static TimerText Instance
        {
            get { return instance_; }
        }

        private float clock_;
        private int totalSeconds_;
        private bool activated_;

        void Start()
        {
            instance_ = this;
            clock_ = 0;
            totalSeconds_ = 0;
            activated_ = false;
        }

        void Update()
        {
            if (activated_)
            {
                clock_ += Time.deltaTime;
                if ((int)clock_ > totalSeconds_)
                {
                    totalSeconds_ = (int)clock_;
                    int seconds = totalSeconds_ % 60;
                    int minutes = totalSeconds_ / 60;
                    GetComponent<TextMesh>().text = string.Format("Time {0:D2}:{1:D2}", minutes, seconds);
                }
            }
        }

        public int GetTimeSeconds()
        {
            return totalSeconds_;
        }

        public void Reset()
        {
            clock_ = 0;
            totalSeconds_ = 0;
            GetComponent<TextMesh>().text = string.Format("Time {0:D2}:{1:D2}", 0, 0);
        }

        public void StartTiming()
        {
            activated_ = true;
        }

        public void Stop()
        {
            activated_ = false;
        }

        public bool IsRunning()
        {
            return activated_;
        }

    }
}

