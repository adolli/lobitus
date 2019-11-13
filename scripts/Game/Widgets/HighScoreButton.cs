using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class HighScoreButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == GetTag())
            {
                GameController.Instance.ShowHighScorePane();
            }
        }

        public override string GetTag()
        {
            return "HighScoreButton";
        }

    }
}
