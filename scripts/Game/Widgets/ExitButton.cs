using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class ExitButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == "ExitButton")
            {
                Application.Quit();
            }
        }

        public override string GetTag()
        {
            return "ExitButton";
        }

    }
}
