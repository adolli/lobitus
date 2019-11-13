using UnityEngine;
using System.Collections;

namespace adolli
{
    public class FaceB : SmallCubeGroup
    {

        public FaceB(RubicCube cube) : base(cube, new Vector3(-1f, 0, 0))
        {
        }

        public override void Bind()
        {
            const int x = -1;
            for (int y = -1; y <= 1; ++y)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    cube_.GetSmallCube(x, y, z).transform.parent = cube_.cubeCenter_.transform;
                }
            }
            cube_.rotationControlPanes_[1].transform.parent = cube_.cubeCenter_.transform;
        }

        public override void Unbind()
        {
            const int x = -1;
            for (int y = -1; y <= 1; ++y)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    cube_.GetSmallCube(x, y, z).transform.parent = cube_.transform;
                }
            }
            cube_.rotationControlPanes_[1].transform.parent = cube_.rotationControlRoot_.transform;
            cube_.ResetCubeCenterRotation();
        }

        public override void OnActionDone()
        {
            // 计算旋转后的smallCube的数据存储位置并解除坐标绑定
            const int x = -1;
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotationTo_, Vector3.one);
            SmallCube[,] newlayer = new SmallCube[3, 3];
            for (int y = -1; y <= 1; ++y)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    Vector3 oldPos = new Vector3(x, y, z);
                    Vector3 newPos = m.MultiplyPoint3x4(oldPos);
                    newlayer[System.Convert.ToInt32(newPos.y) + 1, System.Convert.ToInt32(newPos.z) + 1] = cube_.GetSmallCube(oldPos);
                }
            }
            for (int y = -1; y <= 1; ++y)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    cube_.SetSmallCube(x, y, z, newlayer[y + 1, z + 1]);
                }
            }
        }

        public override string GetFaceName()
        {
            return "B";
        }
    }
}
