using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class RotationControlPaneD : RotationControlPane
    {
        public RotationControlPaneD()
        {
            TouchTag = "RotationControlPaneD";
        }

        public override bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            if (rubicCube_.IsSteady())
            {
                pickEffect_.Play();
                lastPosition_ = touch.position;
                rubicCube_.ActiveFace.SetCurrentFace(new FaceD(rubicCube_));
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

