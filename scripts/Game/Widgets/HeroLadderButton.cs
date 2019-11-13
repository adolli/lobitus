using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class HeroLadderButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == GetTag())
            {
                GameController.Instance.ShowHeroLadderPane();
            }
        }

        public override string GetTag()
        {
            return "HeroLadderButton";
        }

    }
}
