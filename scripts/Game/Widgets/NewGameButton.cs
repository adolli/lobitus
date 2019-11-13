using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class NewGameButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == "NewGameButton")
            {
                GameController.Instance.StartNewGame(null);
            }
        }

        public override string GetTag()
        {
            return "NewGameButton";
        }
    }
}
