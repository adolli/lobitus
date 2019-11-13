using UnityEngine;
using System.Collections;

namespace adolli
{
    public class FaceContextAdapter : IFace
    {
        private RubicCube rcube_;
        private SmallCubeGroup currentFace_;
        private bool binded_;

        public FaceContextAdapter(RubicCube rcube)
        {
            rcube_ = rcube;
            currentFace_ = null;
            binded_ = false;
        }

        public FaceContextAdapter(SmallCubeGroup face, RubicCube rcube)
        {
            rcube_ = rcube;
            currentFace_ = face;
            binded_ = false;
        }

        public Vector3 NormalVector
        {
            get { return currentFace_.NormalVector; }
        }

        public void SetCurrentFace(SmallCubeGroup face)
        {
            if (currentFace_ == null || currentFace_.GetFaceName() != face.GetFaceName())
            {
                Unbind();
                currentFace_ = face;
                Bind();
            }
        }

        public void Bind()
        {
            currentFace_.Bind();
            binded_ = true;
        }

        public void Unbind()
        {
            binded_ = false;
            if (currentFace_ != null)
            {
                currentFace_.Unbind();
                currentFace_ = null;
            }
        }

        public bool Binded()
        {
            return binded_;
        }

        public string GetFaceName()
        {
            if (currentFace_ == null)
            {
                return "";
            }
            else
            {
                return currentFace_.GetFaceName();
            }
        }

        public void AnimatedRotate(int n)
        {
            if (currentFace_ != null)
            {
                currentFace_.AnimatedRotate(n);
            }
        }

        public void Update()
        {
            if (currentFace_ != null)
            {
                bool steady = currentFace_.Step(Time.deltaTime);
                if (steady)
                {
                    OnActionDone();
                    Unbind();
                    OnUnbinded();
                }
            }
        }

        public bool IsAnimating()
        {
            return currentFace_ != null && !currentFace_.ActionDone();
        }

        public void Rotate(Vector3 rotNormal, float angle)
        {
            if (currentFace_ != null)
            {
                int dir = Vector3.Angle(rotNormal, currentFace_.NormalVector) <= float.Epsilon ? -1 : 1;
                currentFace_.SetRotation(dir * angle);
            }
        }

        public void SmoothDock()
        {
            if (currentFace_ != null)
            {
                currentFace_.SmoothDock();
            }
        }


        public void Stop()
        {
            if (currentFace_ != null)
            {
                currentFace_.Stop();
            }
        }

        public delegate void VoidFnType();
        private VoidFnType actionDoneCallback_;
        private VoidFnType unbindedCallback_;
        public void SetOnActionDone(VoidFnType callback)
        {
            actionDoneCallback_ = callback;
        }
        public void SetOnUnbinded(VoidFnType callback)
        {
            unbindedCallback_ = callback;
        }

        public void OnActionDone()
        {
            if (actionDoneCallback_ != null)
            {
                actionDoneCallback_();
            }
            currentFace_.OnActionDone();
            GameObject.Find("StepCounter").GetComponent<StepCounterText>().CountUp();
        }

        public void OnUnbinded()
        {
            int rollbackCount = rcube_.GetRollbackCount();
            if (rollbackCount == 0)
            {
                rcube_.RollbackMode = false;
            }
            if (unbindedCallback_ != null)
            {
                unbindedCallback_();
            }
        }

    }
}
