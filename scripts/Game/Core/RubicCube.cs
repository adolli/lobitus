using UnityEngine;
using adolli.Engine;
using System.Text;
using System.Collections.Generic;

namespace adolli
{
    public class RubicCube : MonoBehaviour
    {
        private static RubicCube instance_;
        public static RubicCube Instance
        {
            get { return instance_; }
        }


        private static readonly int[,,,] DyeTable =
        {
            {
                {
		//U  B  L  F  R  D
					{ 0, 1, 1, 0, 0, 1 }, // 000
					{ 0, 1, 0, 0, 0, 1 }, // 001
					{ 0, 1, 0, 0, 1, 1 }, // 002
				},
                {
		//U  B  L  F  R  D
					{ 0, 1, 1, 0, 0, 0 }, // 010
					{ 0, 1, 0, 0, 0, 0 }, // 011
					{ 0, 1, 0, 0, 1, 0 }, // 012
				},
                {
		//U  B  L  F  R  D
					{ 1, 1, 1, 0, 0, 0 }, // 020
					{ 1, 1, 0, 0, 0, 0 }, // 021
					{ 1, 1, 0, 0, 1, 0 }, // 022
				},
            },
            {
                {
		//U  B  L  F  R  D
					{ 0, 0, 1, 0, 0, 1 }, // 100
					{ 0, 0, 0, 0, 0, 1 }, // 101
					{ 0, 0, 0, 0, 1, 1 }, // 102
				},
                {
		//U  B  L  F  R  D
					{ 0, 0, 1, 0, 0, 0 }, // 110
					{ 0, 0, 0, 0, 0, 0 }, // 111
					{ 0, 0, 0, 0, 1, 0 }, // 112
				},
                {
		//U  B  L  F  R  D
					{ 1, 0, 1, 0, 0, 0 }, // 120
					{ 1, 0, 0, 0, 0, 0 }, // 121
					{ 1, 0, 0, 0, 1, 0 }, // 122
				},
            },
            {
                {
		//U  B  L  F  R  D
					{ 0, 0, 1, 1, 0, 1 }, // 200
					{ 0, 0, 0, 1, 0, 1 }, // 201
					{ 0, 0, 0, 1, 1, 1 }, // 202
				},
                {
		//U  B  L  F  R  D
					{ 0, 0, 1, 1, 0, 0 }, // 210
					{ 0, 0, 0, 1, 0, 0 }, // 211
					{ 0, 0, 0, 1, 1, 0 }, // 212
				},
                {
		//U  B  L  F  R  D
					{ 1, 0, 1, 1, 0, 0 }, // 220
					{ 1, 0, 0, 1, 0, 0 }, // 221
					{ 1, 0, 0, 1, 1, 0 }, // 222
				},
            },
        };

        public GameObject cubeCenter_;
        private SmallCube[,,] smallCubes_;

        // 以下两个公有成员需要从Inspector面板中赋予对象实例
        public GameObject rotationControlRoot_;
        public RotationControlPane[] rotationControlPanes_ = new RotationControlPane[6];

        private FaceContextAdapter ActiveFace_ = null;
        public FaceContextAdapter ActiveFace
        {
            get { return ActiveFace_; }
            set { ActiveFace_ = value; }
        }

        /**
		 * @brief 魔方动作命令序列
		 */
        private class Action
        {
            public delegate void VoidFnType();

            public VoidFnType task;
            public SmallCubeGroup face;
            public int direction;

            public Action(SmallCubeGroup f, int dir)
            {
                face = f;
                direction = dir;
            }
            public Action(VoidFnType task)
            {
                this.task = task;
            }
        };

        private static string[] EasterEggForRollbackPrepare =
        {
            "现在还不可以超神啦~",
            "都说还不可以超神啦~",
            "再等等嘛~",
            "很快就可以超神啦~",
            "再等一会儿，马上带你超神~",
            "呜，怎么还不能超神 >.< ",
            "其实我不是彩蛋啦~",
            "都说不是彩蛋啦~",
            "不是彩蛋你也戳吗？",
            "好啦，等一下就超神了嘛~",
        };
        private static string[] EasterEggForRollbacking =
        {
            "你已超神",
            "已经超神了哦~",
            "你已经是神了啦~",
            "神都喜欢戳来戳去的吗 >.<",
            "听说超神的人今晚会有xx~",
            "呜，神 >.< ",
            "我的主人，有何吩咐~ ",
            "喜欢这个奇怪的方块吗？",
            "如果你能把它拆了，那一定很好吃！",
            "呜，要睡着了~",
            "UBFLRD 什么来的？",
        };
        private int rollbackPrepareIndex_ = 0;
        private int rollbackingIndex_ = 0;

        private StringBuilder initialRandomSequence_;
        private LinkedList<Action> actionSequence_;
        private LinkedList<Action> rollbackSequence_;
        private bool stageCleared_;  // 记录本局中是否已经胜利过一次，在魔方复位时设置false，避免胜利后上传多次成绩
        private bool rollbackMode_;
        private bool rolledback_;
        public bool RollbackMode
        {
            get { return rollbackMode_; }
            set
            {
                if (value == true)
                {
                    // 加点彩蛋
                    if (actionSequence_.Count > 0 || (ActiveFace.Binded() && !rollbackMode_))
                    {
                        // 动作序列还没执行完或最后一个序列的动画还在进行中时不允许rollback
                        // 如果已经处于rollback状态了，则不允许暂停
                        rollbackMode_ = false;
                        GameObject.Find("RollbackButtonText").GetComponent<TextMesh>().text = EasterEggForRollbackPrepare[rollbackPrepareIndex_++];
                        if (rollbackPrepareIndex_ == EasterEggForRollbackPrepare.Length)
                        {
                            rollbackPrepareIndex_ = 0;
                        }
                    }
                    else if (IsVictory())
                    {
                        rollbackMode_ = false;
                        GameObject.Find("RollbackButtonText").GetComponent<TextMesh>().text = "已经不需要超神了T.T";
                    }
                    else
                    {
                        rollbackMode_ = true;
                        GameObject.Find("RollbackButtonText").GetComponent<TextMesh>().text = EasterEggForRollbacking[rollbackingIndex_++];
                        if (rollbackingIndex_ == EasterEggForRollbacking.Length)
                        {
                            rollbackingIndex_ = 1;
                        }
                    }
                }
                else
                {
                    rollbackMode_ = false;
                    GameObject.Find("RollbackButtonText").GetComponent<TextMesh>().text = "点我超神";
                    rollbackPrepareIndex_ = 0;
                    rollbackingIndex_ = 0;
                }

                // 记录是否rollback过，rollback过的记录不上传服务器
                if (rollbackMode_)
                {
                    rolledback_ = true;
                }
            }

        }


        private int shuffleStepsCountDown_;
        public delegate void ShuffleDoneCallback();
        public ShuffleDoneCallback OnShuffleDone
        {
            get;
            set;
        }


        // Use this for initialization
        void Start()
        {
            instance_ = this;

            shuffleStepsCountDown_ = -1;
            cubeCenter_ = new GameObject();
            cubeCenter_.transform.parent = transform;
            cubeCenter_.transform.localPosition = cubeCenter_.transform.position = Vector3.zero;

            if (ActiveFace_ == null)
            {
                ActiveFace_ = new FaceContextAdapter(this);
            }
            ActiveFace_.SetOnUnbinded(() =>
            {
                if (IsVictory())
                {
                    if (!rolledback_ && !stageCleared_ && actionSequence_.Count == 0 && TimerText.Instance.IsRunning())
                    {
                        // 手工复原的情况
                        Debug.Log("true winner!");
                        stageCleared_ = true;
                        LevelClearEffect.Instance.Play();
                        PopoTips.ShowTips("通关啦！正在处理成绩哦~");
                        GameController gameController = GameController.Instance;
                        gameController.UploadScore(initialRandomSequence_.ToString());
                    }
                    // 计时器停止，计数器禁用
                    GameObject.Find("StepCounter").GetComponent<StepCounterText>().enable = false;
                    GameObject.Find("Timer").GetComponent<TimerText>().Stop();

                    // 赢了之后就不用再回滚了
                    rollbackSequence_.Clear();
                }
            });

            smallCubes_ = new SmallCube[3, 3, 3];
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    for (int z = -1; z <= 1; ++z)
                    {
                        // 使用名字定位方块
                        string cubeName = string.Format("Cube ({0:G},{1:G},{2:G})", x, y, z);
                        GameObject cube = GameObject.Find(cubeName);
                        SmallCube scube = cube.GetComponent<SmallCube>();
                        SetSmallCube(x, y, z, scube);

                        // make a copy of dye table of each face for one small cube
                        int[] dyeList = new int[6];
                        for (int i = 0; i < 6; ++i)
                        {
                            dyeList[i] = DyeTable[x + 1, y + 1, z + 1, i];
                        }
                        scube.Init(dyeList);
                        for (int i = 0; i < 6; ++i)
                        {
                            // 自动初始化每一面的颜色
                            scube.SetFragmentColor((SmallCube.DirIndex)i, DyeTable[x + 1, y + 1, z + 1, i] * (i + 1));
                        }

                    }
                }
            }

            initialRandomSequence_ = new StringBuilder();
            actionSequence_ = new LinkedList<Action>();
            rollbackSequence_ = new LinkedList<Action>();
            RollbackMode = false;
            rolledback_ = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (ActiveFace.Binded())
            {
                ActiveFace.Update();
            }
            else
            {
                if (!RollbackMode)
                {
                    PitchTo pitchReset = new PitchTo(GameObject.Find("BackgroundMusic"), 1f, 1f);
                    pitchReset.Start();
                    if (actionSequence_.Count > 0)
                    {
                        Action action = actionSequence_.First.Value;
                        if (action.face != null)
                        {
                            ActiveFace.SetCurrentFace(action.face);
                            ActiveFace.AnimatedRotate(action.direction);
                            --shuffleStepsCountDown_;
                        }
                        else if (action.task != null)
                        {
                            action.task();
                        }
                        actionSequence_.RemoveFirst();
                    }

                    // 当最后一个动作序列执行后，活动面已经稳定（非绑定状态），才算整个动作序列执行完毕
                    if (shuffleStepsCountDown_ == 0 && !ActiveFace.Binded() && OnShuffleDone != null)
                    {
                        OnShuffleDone();
                        OnShuffleDone = null;
                    }
                }
                else
                {
                    if (rollbackSequence_.Count > 0)
                    {
                        PitchTo pitchUp = new PitchTo(GameObject.Find("BackgroundMusic"), 1.35f, 1f);
                        pitchUp.Start();
                        Action action = rollbackSequence_.Last.Value;
                        ActiveFace.SetCurrentFace(action.face);
                        ActiveFace.AnimatedRotate(action.direction);
                        rollbackSequence_.RemoveLast();
                    }
                }
            }
        }


        /**
		 * @brief 复位用于旋转某个面的旋转中心的转角
		 */
        public void ResetCubeCenterRotation()
        {
            if (!RollbackMode)
            {
                float angle1;
                Vector3 axis;
                cubeCenter_.transform.rotation.ToAngleAxis(out angle1, out axis);
                int dir = Vector3.Angle(axis, ActiveFace.NormalVector) <= float.Epsilon ? 1 : -1;
                int rotdir = -((int)angle1 + 45) / 90 * dir;
                if (rotdir != 0)
                {
                    if (rotdir > 2)
                    {
                        rotdir -= 4;
                    }
                    if (rotdir <= -2)
                    {
                        rotdir += 4;
                    }
                    Debug.Log("face= " + ActiveFace.GetFaceName() + "  dir=" + rotdir);
                    switch (ActiveFace.GetFaceName())
                    {
                        case "U":
                            rollbackSequence_.AddLast(new Action(new FaceU(this), rotdir));
                            break;
                        case "F":
                            rollbackSequence_.AddLast(new Action(new FaceF(this), rotdir));
                            break;
                        case "L":
                            rollbackSequence_.AddLast(new Action(new FaceL(this), rotdir));
                            break;
                        case "R":
                            rollbackSequence_.AddLast(new Action(new FaceR(this), rotdir));
                            break;
                        case "B":
                            rollbackSequence_.AddLast(new Action(new FaceB(this), rotdir));
                            break;
                        case "D":
                            rollbackSequence_.AddLast(new Action(new FaceD(this), rotdir));
                            break;
                        default:
                            break;
                    }
                }
            }

            cubeCenter_.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public int GetRollbackCount()
        {
            return rollbackSequence_.Count;
        }


        /**
		 * @brief 使用动作命令让魔方自动按照序列执行
		 */
        public void AddAction(SmallCubeGroup face, int direction)
        {
            actionSequence_.AddLast(new Action(face, direction));
        }


        /**
		 * @brief 清除动作系列中未执行的动作
		 */
        public void ClearAllActions()
        {
            actionSequence_.Clear();
            rollbackSequence_.Clear();
        }


        /**
		 * @brief 判断魔方是否已经稳定，即旋转的面已经停靠完成
		 */
        public bool IsSteady()
        {
            return !ActiveFace.Binded();
        }


        /**
		 * @brief 判断游戏是否胜利，即魔方是否为复原状态
		 */
        public bool IsVictory()
        {
            // wait until steady
            if (!IsSteady())
            {
                return false;
            }

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    for (int z = -1; z <= 1; ++z)
                    {
                        // skip central cube
                        if (x != 0 || y != 0 || z != 0)
                        {
                            for (int dir = 0; dir < 6; ++dir)
                            {
                                SmallCube scube = GetSmallCube(x, y, z);
                                Color color = scube.GetFragmentColor((SmallCube.DirIndex)dir);
                                if (color != SmallCube.ColorList[DyeTable[x + 1, y + 1, z + 1, dir] * (dir + 1)])
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }


        /**
		 * @brief 恢复到初始状态
		 * @notice 只有在steady状态下才能call
		 */
        public void ResetCubes()
        {
            stageCleared_ = false;
            RollbackMode = false;
            rolledback_ = false;
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    for (int z = -1; z <= 1; ++z)
                    {
                        string cubeName = string.Format("Cube ({0:G},{1:G},{2:G})", x, y, z);
                        GameObject cube = GameObject.Find(cubeName);
                        SmallCube scube = cube.GetComponent<SmallCube>();
                        scube.transform.rotation = Quaternion.Euler(Vector3.zero);
                        scube.transform.position = new Vector3(x, y, z);
                        SetSmallCube(x, y, z, scube);
                    }
                }
            }
        }


        /**
		 * @brief 发送复位命令道动作序列中
		 * @notice 不受到steady限制，但是这是个异步的动作
		 */
        public void PostResetCubesAction()
        {
            actionSequence_.AddLast(new Action(() =>
            {
                ResetCubes();
            }));
        }


        /**
		 * @brief 使用特定序列初始化魔方
		 */
        public void ShuffleWithSequence(string initRandomSeq)
        {
            Debug.Log("initSeq=" + initRandomSeq);
            initialRandomSequence_.Remove(0, initialRandomSequence_.Length);
            initialRandomSequence_.Append(initRandomSeq);

            // 去掉最后一个逗号
            initRandomSeq = initRandomSeq.Remove(initRandomSeq.Length - 1);
            string[] actions = initRandomSeq.Split(',');

            // 设置shuffles时的旋转速度
            SmallCubeGroup.AnimationIntervalMax = 0.65f;
            shuffleStepsCountDown_ = actions.Length;
            Debug.Log("shuffleStepsCountDown_=" + shuffleStepsCountDown_);

            foreach (string action in actions)
            {
                string[] faceAndDir = action.Split('|');
                int dir = System.Convert.ToInt32(faceAndDir[1]);
                switch (faceAndDir[0])
                {
                    case "U":
                        actionSequence_.AddLast(new Action(new FaceU(this), dir));
                        break;
                    case "F":
                        actionSequence_.AddLast(new Action(new FaceF(this), dir));
                        break;
                    case "L":
                        actionSequence_.AddLast(new Action(new FaceL(this), dir));
                        break;
                    case "R":
                        actionSequence_.AddLast(new Action(new FaceR(this), dir));
                        break;
                    case "B":
                        actionSequence_.AddLast(new Action(new FaceB(this), dir));
                        break;
                    case "D":
                        actionSequence_.AddLast(new Action(new FaceD(this), dir));
                        break;
                    default:
                        break;
                }
            }
        }

        /**
		 * @brief 随机打乱魔方
		 */
        public void Shuffle()
        {
            const int ShuffleTimes = 5;

            // 设置shuffles时的旋转速度
            SmallCubeGroup.AnimationIntervalMax = 0.65f;
            shuffleStepsCountDown_ = ShuffleTimes;

            System.Random r = new System.Random();
            int face = 0;
            int temp1 = 0;

            // 随机旋转n次以打乱
            initialRandomSequence_.Remove(0, initialRandomSequence_.Length);
            for (int i = 0; i < ShuffleTimes; ++i)
            {
                // until a difference
                while (temp1 == face)
                {
                    temp1 = r.Next(6);
                }
                face = temp1;

                int dir = r.NextDouble() > 0.5 ? -1 : 1;
                switch (face)
                {
                    case 0:
                        actionSequence_.AddLast(new Action(new FaceU(this), dir));
                        initialRandomSequence_.Append(string.Format("U|{0:G},", dir));
                        break;
                    case 1:
                        actionSequence_.AddLast(new Action(new FaceL(this), dir));
                        initialRandomSequence_.Append(string.Format("L|{0:G},", dir));
                        break;
                    case 2:
                        actionSequence_.AddLast(new Action(new FaceR(this), dir));
                        initialRandomSequence_.Append(string.Format("R|{0:G},", dir));
                        break;
                    case 3:
                        actionSequence_.AddLast(new Action(new FaceF(this), dir));
                        initialRandomSequence_.Append(string.Format("F|{0:G},", dir));
                        break;
                    case 4:
                        actionSequence_.AddLast(new Action(new FaceB(this), dir));
                        initialRandomSequence_.Append(string.Format("B|{0:G},", dir));
                        break;
                    case 5:
                        actionSequence_.AddLast(new Action(new FaceD(this), dir));
                        initialRandomSequence_.Append(string.Format("D|{0:G},", dir));
                        break;
                    default:
                        break;
                }
            }
        }


        /**
		 * @brief 小方块的读写访问
		 */
        public void SetSmallCube(int x, int y, int z, SmallCube scube)
        {
            smallCubes_[x + 1, y + 1, z + 1] = scube;
        }

        public SmallCube GetSmallCube(int x, int y, int z)
        {
            return smallCubes_[x + 1, y + 1, z + 1];
        }

        public void SetSmallCube(Vector3 v, SmallCube scube)
        {
            smallCubes_[System.Convert.ToInt32(v.x) + 1, System.Convert.ToInt32(v.y) + 1, System.Convert.ToInt32(v.z) + 1] = scube;
        }

        public SmallCube GetSmallCube(Vector3 v)
        {
            return smallCubes_[System.Convert.ToInt32(v.x) + 1, System.Convert.ToInt32(v.y) + 1, System.Convert.ToInt32(v.z) + 1];
        }

        public Vector3 GetSmallCubeLocation(SmallCube scube)
        {
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    for (int z = -1; z <= 1; ++z)
                    {
                        if (ReferenceEquals(GetSmallCube(x, y, z), scube))
                        {
                            return new Vector3(x, y, z);
                        }
                    }
                }
            }
            return new Vector3(2, 2, 2);
        }

    }
}
