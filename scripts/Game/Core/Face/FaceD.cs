using UnityEngine;
using System.Collections;

namespace adolli
{
    public class FaceD : SmallCubeGroup
    {

        public FaceD(RubicCube cube) : base(cube, new Vector3(0, -1f, 0))
        {
        }

        public override void Bind()
        {
            const int y = -1;
            for (int x = -1; x <= 1; ++x)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    cube_.GetSmallCube(x, y, z).transform.parent = cube_.cubeCenter_.transform;
                }
            }
            cube_.rotationControlPanes_[5].transform.parent = cube_.cubeCenter_.transform;
        }

        public override void Unbind()
        {
            const int y = -1;
            for (int x = -1; x <= 1; ++x)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    cube_.GetSmallCube(x, y, z).transform.parent = cube_.transform;
                }
            }
            cube_.rotationControlPanes_[5].transform.parent = cube_.rotationControlRoot_.transform;
            cube_.ResetCubeCenterRotation();
        }

        public override void OnActionDone()
        {
            // 计算旋转后的smallCube的数据存储位置并解除坐标绑定
            const int y = -1;
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotationTo_, Vector3.one);
            SmallCube[,] newlayer = new SmallCube[3, 3];
            for (int x = -1; x <= 1; ++x)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    Vector3 oldPos = new Vector3(x, y, z);
                    Vector3 newPos = m.MultiplyPoint3x4(oldPos);
                    newlayer[System.Convert.ToInt32(newPos.x) + 1, System.Convert.ToInt32(newPos.z) + 1] = cube_.GetSmallCube(oldPos);
                }
            }
            for (int x = -1; x <= 1; ++x)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    cube_.SetSmallCube(x, y, z, newlayer[x + 1, z + 1]);
                }
            }
        }

        public override string GetFaceName()
        {
            return "D";
        }

    }
}
