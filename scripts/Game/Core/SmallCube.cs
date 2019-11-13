using UnityEngine;
using System.Collections;

namespace adolli
{
    public class SmallCube : MonoBehaviour
    {

        // 从inspector中把prefabs拖进来，并且按照下面的FragmentIndex定义的顺序
        public GameObject[] PrefabFragments = new GameObject[6];

        public enum DirIndex
        {
            U, B, L, F, R, D
        };

        // 保存每个面的实例化小碎块
        private GameObject[] fragments_;

        // 每个面的颜色定义
        public static readonly Color[] ColorList = new Color[]
        {
            Color.black,
            new Color(51f / 255f, 204f / 255f, 51f / 255f),
            new Color(255f / 255f, 114f / 255f, 0f),
            new Color(51f / 255f, 102f / 255f, 204f / 255f),
            Color.white,
            Color.yellow,
            new Color(102f / 255f, 0f, 153f/ 255f),
        };


        void Start()
        {
        }

        void Update()
        {
        }


        /**
		 * @brief 初始化一个小方块的每个面，暴露在最表面的小碎块就要染色
		 * @param dyeFaces 染色开关，长度为6，为1的位需要染色
		 */
        public void Init(int[] dyeFaces)
        {
            fragments_ = new GameObject[6];

            for (int i = 0; i < fragments_.Length; ++i)
            {
                if (dyeFaces[i] != 0)
                {
                    fragments_[i] = Instantiate(PrefabFragments[i]);

                    // 锚点参考点绑定父坐标
                    fragments_[i].transform.parent = transform;

                    // 当前坐标定位
                    fragments_[i].transform.localPosition = fragments_[i].transform.position;

                    // 设置对应面小碎块颜色
                    SetFragmentColor((DirIndex)i, i);
                }
            }
        }


        /**
		 * @brief 设置每个面小碎块颜色，如果某个面不存在，则忽略
		 * @param index 要染色的面
		 * @param colorIndex 颜色表中的序号
		 */
        public void SetFragmentColor(DirIndex index, int colorIndex)
        {
            if (fragments_[(int)index] != null)
            {
                Renderer rend = fragments_[(int)index].GetComponent<Renderer>();
                rend.material.color = ColorList[colorIndex];
            }
        }


        public Color GetFragmentColor(DirIndex index)
        {
            if (fragments_[(int)index] != null)
            {
                Renderer rend = fragments_[(int)index].GetComponent<Renderer>();
                return rend.material.color;
            }
            return Color.black;
        }

    }
}
