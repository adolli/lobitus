using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class RestartButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == GetTag())
            {
                GameController.Instance.RestartGame();
                this.enable = false;
            }

        }

        public override string GetTag()
        {
            return "RestartButton";
        }

    }
}
