using UnityEngine;
using System.Collections.Generic;

namespace adolli.Engine
{
    public class AnimationScheduler : MonoBehaviour
    {


        private static LinkedList<Tween> animationList_ = new LinkedList<Tween>();

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            LinkedList<Tween> toBeRemoved = new LinkedList<Tween>();
            foreach (Tween tween in animationList_)
            {
                bool done = tween.Step(Time.deltaTime);
                if (done)
                {
                    toBeRemoved.AddLast(tween);
                }
            }
            foreach (Tween tween in toBeRemoved)
            {
                animationList_.Remove(tween);
            }
        }

        public static void AddTween(Tween tween)
        {
            animationList_.AddLast(tween);
        }

    }
}
