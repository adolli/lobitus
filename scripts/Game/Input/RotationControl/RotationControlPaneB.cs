using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class RotationControlPaneB : RotationControlPane
    {
        public RotationControlPaneB()
        {
            TouchTag = "RotationControlPaneB";
        }

        public override bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            if (rubicCube_.IsSteady())
            {
                pickEffect_.Play();
                lastPosition_ = touch.position;
                rubicCube_.ActiveFace.SetCurrentFace(new FaceB(rubicCube_));
                this.transform.localScale += TouchFaceExpand;
            }
            else if ("RotationControlPane" + rubicCube_.ActiveFace.GetFaceName() == TouchTag && !rubicCube_.RollbackMode)
            {
                pickEffect_.Play();
                rubicCube_.ActiveFace.Stop();
                lastPosition_ = touch.position;
                this.transform.localScale += TouchFaceExpand;
            }
            return true;
        }
    }
}

