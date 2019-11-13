using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class HighScoreList : GameObjectButton
    {

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (target.tag == GetTag())
            {
                Debug.Log("click this!");
            }
        }

        public override string GetTag()
        {
            return "HighScoreList";
        }

    }
}
