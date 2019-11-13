using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class RotationControlPaneR : RotationControlPane
    {
        public RotationControlPaneR()
        {
            TouchTag = "RotationControlPaneR";
        }

        public override bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            if (rubicCube_.IsSteady())
            {
                pickEffect_.Play();
                lastPosition_ = touch.position;
                rubicCube_.ActiveFace.SetCurrentFace(new FaceR(rubicCube_));
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


