using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class RollbackButton : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == GetTag())
            {
                RubicCube rcube = GameObject.Find("RubicCube").GetComponent<RubicCube>();
                rcube.RollbackMode = true;
            }
        }

        public override string GetTag()
        {
            return "RollbackButton";
        }

    }
}
