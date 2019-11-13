using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class BackToHomeButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == "BackToHomeButton")
            {
                // 清空魔方动作序列
                GameObject.Find("RubicCube").GetComponent<RubicCube>().ClearAllActions();

                Sequence seq = new Sequence();

                // 切换视角
                GameObject cameraView = GameObject.Find("CameraCenter");
                MoveTo move = new MoveTo(cameraView, new Vector3(13.17f, 0.2f, 0f), 1.5);
                move.Start();
                RotateTo rotate = new RotateTo(cameraView, new Vector3(0f, 0f, 54.3f), 1.5);

                GameObject timer = GameObject.Find("Timer");
                GameObject counter = GameObject.Find("StepCounter");
                timer.GetComponent<TimerText>().Stop();

                seq.AddToQueue(rotate);
                seq.AddToQueue(new CallFunc(() =>
               {
                    // 禁止触摸旋转
                    ViewingControl.enable = false;
               }));

                // 定时器，计步器退场
                MoveTo counterEnter = new MoveTo(counter, new Vector3(-2.31f, -5.46f, 36.11f), 0.5);
                MoveTo timerEnter = new MoveTo(timer, new Vector3(-0.7f, -5.46f, 39.45f), 0.5);

                seq.AddToQueue(counterEnter);
                seq.AddToQueue(timerEnter);

                seq.Start();

                // 同时整个home page进入
                MoveTo homePageLeave = new MoveTo(GameObject.Find("HomePage"), new Vector3(14.34694f, -12.79404f, -2.630041f), 2);
                homePageLeave.Start();
            }
        }

        public override string GetTag()
        {
            return "BackToHomeButton";
        }

    }
}
