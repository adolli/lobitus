using UnityEngine;

namespace adolli
{
    public class StepCounterText : MonoBehaviour
    {
        private static StepCounterText instance_;
        public static StepCounterText Instance
        {
            get { return instance_; }
        }
        private int count_;
        private bool changed_;

        public bool enable
        {
            get;
            set;
        }

        void Start()
        {
            instance_ = this;
            count_ = 0;
            changed_ = false;
            enable = true;
        }

        void Update()
        {
            if (changed_)
            {
                changed_ = false;
                GetComponent<TextMesh>().text = "Step " + count_;
            }
        }

        public void CountUp()
        {
            if (enable)
            {
                ++count_;
                changed_ = true;
            }
        }


        public void Reset()
        {
            count_ = 0;
            GetComponent<TextMesh>().text = "Step 0";
        }

        public int GetCount()
        {
            return count_;
        }
    }
}

