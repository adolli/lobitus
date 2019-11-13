using UnityEngine;
using adolli.Engine;

namespace adolli
{
    public class FragmentRotationControlPane : RotationControlPane
    {

        class Line2D
        {
            public Vector3 p1, p2;
            public float DistanceToPoint(Vector3 point)
            {
                float k = (p1.y - p2.y) / (p1.x - p2.x);
                float A = k;
                float B = 1;
                float C = p1.y - k * p2.x;
                return Mathf.Abs(A * point.x + B * point.y + C) / Mathf.Sqrt(A * A + B * B);
            }
        }

        GameObject targetCube_;
        Vector3 touchStartNormal_;
        Vector3 touchStart_;
        Vector3 touchRun_;
        Vector3 normalDir_;
        float angleDir_;

        public FragmentRotationControlPane()
        {
            TouchTag = "SmallCube";
            angleDir_ = 0;
        }

        public override bool OnTouchBegan(Collider target, TouchInfo touch)
        {
            if (rubicCube_.IsSteady())
            {
                pickEffect_.Play();
                lastPosition_ = touch.position;

                targetCube_ = target.gameObject;
                touchStartNormal_ = touch.normal;
                touchStart_ = touch.position;

                return true;
            }
            return false;
        }

        public override void OnTouchMoved(Collider target, TouchInfo touch)
        {
            if (rubicCube_.ActiveFace.Binded() && !rubicCube_.ActiveFace.IsAnimating())
            {
                Vector3 cameraRot = GameObject.Find("CameraCenter").transform.localEulerAngles;
                Debug.Log(cameraRot);
                float sgny = cameraRot.y - 360 < -60 || cameraRot.y - 360 > 150 ? -1 : 1;
                float sgnx = 1;
                float sgn = Mathf.Sign(normalDir_.x + normalDir_.y + normalDir_.z);
                Debug.Log("rn=" + normalDir_ + "  an=" + rubicCube_.ActiveFace.NormalVector + "  dsx=" + touch.deltaScreenPosition.x + " dsy=" + touch.deltaScreenPosition.y);
                rubicCube_.ActiveFace.Rotate(normalDir_, sgn * 10 * (sgnx * touch.deltaScreenPosition.x - sgny * touch.deltaScreenPosition.y));
            }
            else if (!rubicCube_.ActiveFace.IsAnimating() && ReferenceEquals(target.gameObject, targetCube_))
            {
                Vector3 currentPos = touch.position;
                Debug.DrawRay(Vector3.zero, (target.transform.position + touch.position) * 5);

                touchRun_ = touch.position - touchStart_;
                if (touchRun_.sqrMagnitude > 0.05f)
                {
                    Vector3 cross = Vector3.Cross(touchStartNormal_, touchRun_).normalized;
                    normalDir_ = GetRotationDirection(cross);
                    Vector3 loc = rubicCube_.GetSmallCubeLocation(target.gameObject.GetComponent<SmallCube>());
                    if (normalDir_.x != 0)
                    {
                        if (loc.x == 1)
                        {
                            rubicCube_.ActiveFace.SetCurrentFace(new FaceF(rubicCube_));
                            angleDir_ = Mathf.Sign(normalDir_.x);
                        }
                        else if (loc.x == -1)
                        {
                            rubicCube_.ActiveFace.SetCurrentFace(new FaceB(rubicCube_));
                        }
                    }
                    else if (normalDir_.y != 0)
                    {
                        if (loc.y == 1)
                        {
                            rubicCube_.ActiveFace.SetCurrentFace(new FaceU(rubicCube_));
                        }
                        else if (loc.y == -1)
                        {
                            rubicCube_.ActiveFace.SetCurrentFace(new FaceD(rubicCube_));
                        }
                    }
                    else if (normalDir_.z != 0)
                    {
                        if (loc.z == 1)
                        {
                            rubicCube_.ActiveFace.SetCurrentFace(new FaceR(rubicCube_));
                        }
                        else if (loc.z == -1)
                        {
                            rubicCube_.ActiveFace.SetCurrentFace(new FaceL(rubicCube_));
                        }
                    }
                    targetCube_ = null;
                }
            }
        }

        public override void OnTouchEnded(Collider target, TouchInfo touch)
        {
            if (!rubicCube_.ActiveFace.IsAnimating())
            {
                rubicCube_.ActiveFace.SmoothDock();
            }
        }

        Vector3 GetRotationDirection(Vector3 cross)
        {
            if (Mathf.Abs(cross.x) > Mathf.Abs(cross.y))
            {
                if (Mathf.Abs(cross.x) > Mathf.Abs(cross.z))
                {
                    // x axis
                    return new Vector3(Mathf.Sign(cross.x) * 1, 0, 0);
                }
                else
                {
                    // z axis
                    return new Vector3(0, 0, Mathf.Sign(cross.z) * 1);
                }
            }
            else
            {
                if (Mathf.Abs(cross.y) > Mathf.Abs(cross.z))
                {
                    // y axis
                    return new Vector3(0, Mathf.Sign(cross.y) * 1, 0);
                }
                else
                {
                    // z axis
                    return new Vector3(0, 0, Mathf.Sign(cross.z) * 1);
                }
            }
        }


    }
}

