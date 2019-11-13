using UnityEngine;
using UnityEngine.UI;
using adolli.Engine;
using adolli.Engine.Network;
using System.Collections;
using System.Collections.Generic;

namespace adolli
{
    public class GameController : MonoBehaviour
    {

        private static GameController instance_;
        public static GameController Instance
        {
            get { return instance_; }
        }

        private TextMesh console_;
        private int promptIndex_ = 0;
        private static string[] screenWords_ =
        {
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#_",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#",
            "{0:G}@lobitus:~#c_",
            "{0:G}@lobitus:~#co_",
            "{0:G}@lobitus:~#con_",
            "{0:G}@lobitus:~#conn_",
            "{0:G}@lobitus:~#conn_",
            "{0:G}@lobitus:~#conn_",
            "{0:G}@lobitus:~#conn",
            "{0:G}@lobitus:~#conn",
            "{0:G}@lobitus:~#conn",
            "{0:G}@lobitus:~#conn\n_",
            "{0:G}@lobitus:~#conn\n\n欢迎来到lobitus\n正在与lobitus进行连接，请不要将脸太贴近透明板。\n今天lobitus将开放以下功能，明天的话，我也不敢保证。\n祝你顺利！\n_",
            "{0:G}@lobitus:~#conn\n\n欢迎来到lobitus\n正在与lobitus进行连接，请不要将脸太贴近透明板。\n今天lobitus将开放以下功能，明天的话，我也不敢保证。\n祝你顺利！\n",
            "{0:G}@lobitus:~#conn\n\n欢迎来到lobitus\n正在与lobitus进行连接，请不要将脸太贴近透明板。\n今天lobitus将开放以下功能，明天的话，我也不敢保证。\n祝你顺利！\n\n| 普通模式 | 挑战模式 | 英雄天梯 | 退出游戏 |\n\n",
            "{0:G}@lobitus:~#conn\n\n欢迎来到lobitus\n正在与lobitus进行连接，请不要将脸太贴近透明板。\n今天lobitus将开放以下功能，明天的话，我也不敢保证。\n祝你顺利！\n\n| 普通模式 | 挑战模式 | 英雄天梯 | 退出游戏 |\n\n{0:G}@lobitus:~#_"
        };

        private const float ResponseTimeOutThredshold = 3f;

        private bool challengeMode_;
        private bool logedin_;
        private string ip_;
        private string port_;
        private string username_;
        private string password_;

        // 保存前10条挑战榜的记录
        public class ChallengeRecordType
        {
            public int No;
            public string username;
            public int seconds;
            public int steps;
            public string initRandomSeq;
            public ChallengeRecordType(int no, string user, int seconds, int steps, string initSeq)
            {
                No = no;
                username = user;
                this.seconds = seconds;
                this.steps = steps;
                initRandomSeq = initSeq;
            }
        }
        private ChallengeRecordType[] challengeRecord_;
        private bool challengeListDownloaded_;

        private const int DefaultPageRecordsCount = 10;
        private int highScoreListPageNum_; // index
        private int heroLadderListPageNum_; // index
        private int highScoreListTotalRecords_; // 数量
        private int heroLadderListTotalRecords_; // 数量

        // Use this for initialization
        void Start()
        {
            instance_ = this;

            challengeMode_ = false;
            logedin_ = false;
            ip_ = "";
            port_ = "";
            username_ = "root";
            password_ = "";
            console_ = GameObject.Find("prompt").GetComponent<TextMesh>();
            console_.text = "";
            GameObject.Find("LoginTips").GetComponent<Text>().text = "";
            challengeRecord_ = new ChallengeRecordType[10];
            challengeListDownloaded_ = false;

            // 从第0页开始计数
            highScoreListPageNum_ = 0;
            heroLadderListPageNum_ = 0;
            highScoreListTotalRecords_ = 0;
            heroLadderListTotalRecords_ = 0;

            // 初始化时禁止旋转视角
            ViewingControl.enable = false;

            // UI button play offline
            Button playOffline = GameObject.Find("PlayOfflineButton").GetComponent<Button>();
            playOffline.onClick.AddListener(() =>
            {
                MoveBy GUIGroupLeave = new MoveBy(GameObject.Find("GUIGroup"), new Vector3(0, -3000, 0), 0.7f);
                GUIGroupLeave.Start();
                StartPrompt();
            });

            Button loginButton = GameObject.Find("LoginButton").GetComponent<Button>();
            loginButton.onClick.AddListener(() =>
            {
                ip_ = GameObject.Find("InputIP").GetComponent<InputField>().text;
                port_ = GameObject.Find("InputPort").GetComponent<InputField>().text;
                username_ = GameObject.Find("Username").GetComponent<InputField>().text;
                if (username_.Length > 15)
                {
                    username_ = username_.Substring(0, 15);
                }
                password_ = GameObject.Find("Password").GetComponent<InputField>().text;
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username_);
                data.Add("password", password_);
                HttpRequest req = new HttpRequest("http://" + ip_ + ":" + port_ + "/login", data);
                req.OnResponsed = (Hashtable result) =>
                {
                    string loginTips = "";
                    if ((bool)result["authsuccess"])
                    {
                        logedin_ = true;
                        MoveBy GUIGroupLeave = new MoveBy(GameObject.Find("GUIGroup"), new Vector3(0, -3000, 0), 0.7f);
                        GUIGroupLeave.Start();
                        StartPrompt();
                        loginTips = "Login success!";
                    }
                    else
                    {
                        loginTips = "密码或者账号不对哦~";
                    }
                    GameObject.Find("LoginTips").GetComponent<Text>().text = loginTips;
                };
                req.OnTimeOut = () =>
                {
                    Debug.Log("超时了咯");
                    GameObject.Find("LoginTips").GetComponent<Text>().text = "服务器居然没有反应，正在尝试重新登录";
                };
                req.OnError = (string msg) =>
                {
                    GameObject.Find("LoginTips").GetComponent<Text>().text = "连接错误啦！可能ip地址或者端口不对";
                };
                req.OnMaxRetried = () =>
                {
                    GameObject.Find("LoginTips").GetComponent<Text>().text = "超过重试次数，请检查一下路由器是否被偷走。";
                };
                req.Post();
                GameObject.Find("LoginTips").GetComponent<Text>().text = "正在登陆...";
            });

            Button closeHighScorePaneButton = GameObject.Find("CloseHighScorePaneButton").GetComponent<Button>();
            closeHighScorePaneButton.onClick.AddListener(HideHighScorePane);

            Button closeHeroLadderPaneButton = GameObject.Find("CloseHeroLadderPaneButton").GetComponent<Button>();
            closeHeroLadderPaneButton.onClick.AddListener(HideHeroLadderPane);

            Button highScoreListNextPage = GameObject.Find("HighScoreListNextPage").GetComponent<Button>();
            highScoreListNextPage.onClick.AddListener(GetHighScoreListNextPage);
            Button highScoreListPrevPage = GameObject.Find("HighScoreListPrevPage").GetComponent<Button>();
            highScoreListPrevPage.onClick.AddListener(GetHighScoreListPrevPage);

            Button heroLadderListNextPage = GameObject.Find("HeroLadderListNextPage").GetComponent<Button>();
            heroLadderListNextPage.onClick.AddListener(GetHeroLadderListNextPage);
            Button heroLadderListPrevPage = GameObject.Find("HeroLadderListPrevPage").GetComponent<Button>();
            heroLadderListPrevPage.onClick.AddListener(GetHeroLadderListPrevPage);

            // 设置挑战按钮监听器
            for (int i = 0; i < 10; ++i)
            {
                Button challengeButton = GameObject.Find("/Canvas/HighScorePane/HighScoreList/Button (" + i + ")").GetComponent<Button>();
                challengeButton.name += i;
                challengeButton.onClick.AddListener(() =>
                {
                    if (logedin_ && challengeListDownloaded_)
                    {
                        string buttonName = challengeButton.name;
                        string indexStr = challengeButton.name.Remove(0, buttonName.Length - 1);
                        int buttonIndex = System.Convert.ToInt32(indexStr);
                        if (challengeRecord_[buttonIndex] != null)
                        {
                            string initSeq = challengeRecord_[buttonIndex].initRandomSeq;
                            if (initSeq.Length > 3 && initSeq.EndsWith(","))
                            {
                                StartNewGame(initSeq);
                                HideHighScorePane();
                            }
                            else
                            {
                                // 建议去附近的海鲜酒家
                                PopoTips.ShowTips("  下来5楼找李翔请你吃饭！  ");
                            }
                        }
                    }
                });
            }
        }


        // Update is called once per frame
        void Update()
        {
        }


        void StartPrompt()
        {
            Wait waitMoment = new Wait(0.1f);
            Sequence seq = new Sequence();
            for (int i = 0; i < screenWords_.Length; ++i)
            {
                seq.AddToQueue(waitMoment.Clone());
                seq.AddToQueue(new CallFunc(() =>
                {
                    console_.text = string.Format(screenWords_[promptIndex_], username_);
                    ++promptIndex_;
                }));
            }
            seq.AddToQueue(new CallFunc(() =>
            {
                GameObject.Find("NewGameButton").GetComponent<NewGameButton>().enable = true;
                GameObject.Find("ExitButton").GetComponent<ExitButton>().enable = true;
                GameObject.Find("HighScoreButton").GetComponent<HighScoreButton>().enable = true;
                GameObject.Find("HeroLadderButton").GetComponent<HeroLadderButton>().enable = true;
            }));
            seq.Start();
        }

        public void UploadScore(string initRandomSeq)
        {
            if (logedin_)
            {
                int steps = GameObject.Find("StepCounter").GetComponent<StepCounterText>().GetCount();
                int seconds = GameObject.Find("Timer").GetComponent<TimerText>().GetTimeSeconds();
                Debug.Log("upload initSeq=" + initRandomSeq + "  steps=" + steps + "  seconds=" + seconds);
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("username", username_);
                data.Add("password", password_);
                data.Add("steps", "" + steps);
                data.Add("seconds", "" + seconds);
                data.Add("initRandomSeq", initRandomSeq);
                if (challengeMode_)
                {
                    // 挑战模式下就更新记录
                    Debug.Log("challenge mode. update the highest score.");
                    HttpRequest req = new HttpRequest("http://" + ip_ + ":" + port_ + "/updateHighScore", data);
                    req.OnResponsed = (Hashtable result) =>
                    {
                        if ((bool)result["success"])
                        {
                            if ((bool)result["betterRecord"])
                            {
                                PopoTips.ShowTips("恭喜，挑战成功啦！个人分数又增加了哦！");
                            }
                            else
                            {
                                PopoTips.ShowTips("恭喜，挑战完成！但是还不够快哦，要不再来一次？");
                            }
                        }
                        else
                        {
                            PopoTips.ShowTips("成绩似乎没有上传成功，身份验证出了问题，我帮你去把程序员哥哥吊打一顿。");
                        }
                    };
                    req.OnError = (string message) =>
                    {
                        // 网络错误重传
                        Debug.Log("update score error!");
                        PopoTips.ShowTips("网络又开小差啦，稍后等网络恢复后会自动重传");
                    };
                    req.OnTimeOut = () =>
                    {
                        // 超时重传
                        Debug.Log("update score timeout!");
                        PopoTips.ShowTips("网络又开小差啦，稍后等网络恢复后会自动重传");
                    };
                    req.Post();
                }
                else
                {
                    // 普通模式就按正常上传
                    HttpRequest req = new HttpRequest("http://" + ip_ + ":" + port_ + "/uploadScore", data);
                    req.OnResponsed = (Hashtable result) =>
                    {
                        // 显示排行榜
                        Debug.Log("upload score responsed!");
                        if ((bool)result["success"])
                        {
                            if ((bool)result["newRecord"])
                            {
                                PopoTips.ShowTips("恭喜，成绩上传成功啦！占领了新挑战哦。");
                            }
                            else if ((bool)result["betterRecord"])
                            {
                                PopoTips.ShowTips("恭喜，成绩上传成功啦！而且超过了几个人呢，个人分数有所增加。");
                            }
                            else
                            {
                                PopoTips.ShowTips("继续加油，还有人更快。");
                            }
                        }
                        else
                        {
                            PopoTips.ShowTips("成绩似乎没有上传成功，身份验证出了问题，我帮你去把程序员哥哥吊打一顿。");
                        }
                    };
                    req.OnError = (string message) =>
                    {
                        // 网络错误重传
                        Debug.Log("upload score error!");
                        PopoTips.ShowTips("网络又开小差啦，稍后等网络恢复后会自动重传");
                    };
                    req.OnTimeOut = () =>
                    {
                        // 超时重传
                        Debug.Log("upload score timeout!");
                        PopoTips.ShowTips("网络又开小差啦，稍后等网络恢复后会自动重传");
                    };
                    req.Post();
                }
            }
            else
            {
                // 没有登录的单机模式下则弹出胜利和再来一局的对话框
                PopoTips.ShowTips("祝贺你完成了复原\n离线模式的话，就不上传服务器啦~");
            }

        }

        public void ShowHighScorePane()
        {
            if (logedin_)
            {
                // TODO 加入回弹的ease
                GameObject highScorePane = GameObject.Find("HighScorePane");
                AlphaTo fade = new AlphaTo(highScorePane, 1f, 0.1f);
                ScaleTo size = new ScaleTo(highScorePane, new Vector3(1f, 1f, 1f), 0.3f);
                fade.Start();
                size.Start();

                // 打开后开始加载
                DownloadHighScore(highScoreListPageNum_);
            }
            else
            {
                PopoTips.ShowTips("离线模式下还不可以挑战哦");
            }
        }

        public void HideHighScorePane()
        {
            GameObject highScorePane = GameObject.Find("HighScorePane");
            AlphaTo fade = new AlphaTo(highScorePane, 0f, 0.1f);
            ScaleTo size = new ScaleTo(highScorePane, Vector3.zero, 0.3f);
            fade.Start();
            size.Start();
        }

        public void DownloadHighScore(int pageIndex)
        {
            challengeListDownloaded_ = false;
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("startNo", "" + pageIndex * DefaultPageRecordsCount);
            form.Add("endNo", "" + (pageIndex + 1) * DefaultPageRecordsCount);
            HttpRequest req = new HttpRequest("http://" + ip_ + ":" + port_ + "/getHighScore", form);
            req.OnResponsed = (Hashtable result) =>
            {
                Text scoreList = GameObject.Find("HighScoreList").GetComponent<Text>();
                scoreList.text = string.Format("{0:G2} {1,-10} {2,5}  {3,8}  {4,3}\n", "No.", "user", "时长", "步数", "复杂度");
                int index = 0;
                challengeRecord_ = new ChallengeRecordType[10];
                ArrayList list = (ArrayList)result["data"];
                foreach (Hashtable table in list)
                {
                    string initSeq = (string)table["initRandomSeq"];
                    challengeRecord_[index++] = new ChallengeRecordType(
                        System.Convert.ToInt32(table["No"]),
                        (string)table["username"],
                        System.Convert.ToInt32(table["seconds"]),
                        System.Convert.ToInt32(table["steps"]),
                        (string)table["initRandomSeq"]);
                    string line = string.Format("{0:G} {1,-10} {2,5}s {3,8}步 {4,3}\n", table["No"], table["username"], table["seconds"], table["steps"], initSeq.Length);
                    scoreList.text += line;
                    if (index > 10)
                    {
                        break;
                    }
                }
                highScoreListTotalRecords_ = System.Convert.ToInt32(result["totalRecords"]);
                challengeListDownloaded_ = true;
            };
            req.OnTimeOut = () =>
            {
                Text scoreList = GameObject.Find("HighScoreList").GetComponent<Text>();
                scoreList.text = "(服务器貌似又连不上了...)";
            };
            req.OnError = (string msg) =>
            {
                Text scoreList = GameObject.Find("HighScoreList").GetComponent<Text>();
                scoreList.text = "(服务器貌似又连不上了...)\n但留下了一句话：\nerror" + msg + "\n\n后面的字都看不清了";
            };
            req.Post();
        }

        public void GetHighScoreListNextPage()
        {
            // 如果上次的记录已经下载了，才能翻页，不然
            if ((highScoreListPageNum_ + 1) * DefaultPageRecordsCount < highScoreListTotalRecords_)
            {
                ++highScoreListPageNum_;
                DownloadHighScore(highScoreListPageNum_);
            }
            else
            {
                PopoTips.ShowTips("已经到最后一页啦");
            }
        }

        public void GetHighScoreListPrevPage()
        {
            if (highScoreListPageNum_ > 0)
            {
                --highScoreListPageNum_;
                DownloadHighScore(highScoreListPageNum_);
            }
            else
            {
                PopoTips.ShowTips("已经是第一页了");
            }
        }



        public void ShowHeroLadderPane()
        {
            if (logedin_)
            {
                GameObject highScorePane = GameObject.Find("HeroLadderPane");
                AlphaTo fade = new AlphaTo(highScorePane, 1f, 0.1f);
                ScaleTo size = new ScaleTo(highScorePane, new Vector3(1f, 1f, 1f), 0.3f);
                fade.Start();
                size.Start();

                DownloadHeroLadder(heroLadderListPageNum_);
            }
            else
            {
                PopoTips.ShowTips("你就是英雄！其实是你没有登陆...");
            }
        }

        public void HideHeroLadderPane()
        {
            GameObject highScorePane = GameObject.Find("HeroLadderPane");
            AlphaTo fade = new AlphaTo(highScorePane, 0f, 0.1f);
            ScaleTo size = new ScaleTo(highScorePane, Vector3.zero, 0.3f);
            fade.Start();
            size.Start();
        }


        public void DownloadHeroLadder(int pageIndex)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("startNo", "" + pageIndex * DefaultPageRecordsCount);
            form.Add("endNo", "" + (pageIndex + 1) * DefaultPageRecordsCount);
            HttpRequest req = new HttpRequest("http://" + ip_ + ":" + port_ + "/getHeroLadder", form);
            req.OnResponsed = (Hashtable result) =>
            {
                Text scoreList = GameObject.Find("HeroLadderList").GetComponent<Text>();
                scoreList.text = string.Format("{0:G2} {1,-12} {2,8} \n", "No.", "user", "总积分");
                int index = 0;
                ArrayList list = (ArrayList)result["data"];
                foreach (Hashtable table in list)
                {
                    string line = string.Format("{0:G} {1,-12} {2,8}\n", table["No"], table["username"], table["score"]);
                    scoreList.text += line;
                    if (index > 10)
                    {
                        break;
                    }
                }
                heroLadderListTotalRecords_ = System.Convert.ToInt32(result["totalRecords"]);
            };
            req.OnTimeOut = () =>
            {
                Text scoreList = GameObject.Find("HeroLadderList").GetComponent<Text>();
                scoreList.text = "(服务器貌似又连不上了...)";
            };
            req.OnError = (string msg) =>
            {
                Text scoreList = GameObject.Find("HeroLadderList").GetComponent<Text>();
                scoreList.text = "(服务器貌似又连不上了...)\n但留下了一句话：\nerror" + msg + "\n\n后面的字都看不清了";
            };
            req.Post();
        }

        public void GetHeroLadderListNextPage()
        {
            if ((heroLadderListPageNum_ + 1) * DefaultPageRecordsCount < heroLadderListTotalRecords_)
            {
                ++heroLadderListPageNum_;
                DownloadHighScore(heroLadderListPageNum_);
            }
            else
            {
                PopoTips.ShowTips("已经到最后一页啦");
            }
        }

        public void GetHeroLadderListPrevPage()
        {
            if (heroLadderListPageNum_ > 0)
            {
                --heroLadderListPageNum_;
                DownloadHighScore(heroLadderListPageNum_);
            }
            else
            {
                PopoTips.ShowTips("已经是第一页了");
            }
        }


        /**
		 * @brief 开始一场新的游戏，无论在什么地方开始新游戏，都需要经过此入口
		 */
        public void StartNewGame(string initSeq)
        {
            // 隐藏高分榜和英雄天梯
            HideHighScorePane();

            // 魔方复位
            RubicCube rcube1 = RubicCube.Instance;
            rcube1.PostResetCubesAction();

            Sequence seq = new Sequence();

            // 切换视角
            GameObject cameraView = GameObject.Find("CameraCenter");
            MoveTo move = new MoveTo(cameraView, Vector3.zero, 1);
            move.SetEasing(new ExpoIn());
            move.Start();
            RotateTo rotate = new RotateTo(cameraView, Vector3.zero, 1);
            rotate.SetEasing(new ExpoIn());

            seq.AddToQueue(rotate);
            seq.AddToQueue(new CallFunc(() =>
            {
                // 使能触摸视角控制
                ViewingControl.ResetViewDirection();
                ViewingControl.enable = true;

                // 使能BackToHome按钮
                GameObject.Find("BackButton").GetComponent<BackToHomeButton>().enable = true;
                GameObject.Find("RollbackButton").GetComponent<RollbackButton>().enable = true;
                GameObject.Find("RollbackButtonText").GetComponent<TextMesh>().text = "点我超神";

                // 随机乱序
                RubicCube rcube = RubicCube.Instance;
                rcube.OnShuffleDone = () =>
                {
                    SmallCubeGroup.AnimationIntervalMax = 1f;

                    // 定时器复位，计数器复位
                    TimerText timer = TimerText.Instance;
                    timer.Reset();
                    timer.StartTiming();
                    StepCounterText counter = StepCounterText.Instance;
                    counter.Reset();
                    counter.enable = true;

                    // 定时器，计步器进场
                    MoveTo counterEnter = new MoveTo(GameObject.Find("StepCounter"), new Vector3(-2.31f, -5.46f, 36.11f - 26f), 0.5);
                    counterEnter.SetEasing(new CubicIn());
                    counterEnter.Start();
                    MoveTo timerEnter = new MoveTo(timer.gameObject, new Vector3(-0.7f, -5.46f, 39.45f - 26f), 0.5);
                    timerEnter.SetEasing(new QuadIn());
                    timerEnter.Start();

                    GameObject.Find("RestartButton").GetComponent<RestartButton>().enable = true;
                };
                if (initSeq == null)
                {
                    challengeMode_ = false;
                    rcube.Shuffle();
                }
                else
                {
                    challengeMode_ = true;
                    rcube.ShuffleWithSequence(initSeq);
                }

            }));

            seq.Start();

            // 同时整个home page离开
            MoveTo homePageLeave = new MoveTo(GameObject.Find("HomePage"), new Vector3(39.34694f, -12.79404f, -2.630041f), 2);
            homePageLeave.Start();
        }


        /**
		 * @brief 在任何游戏模式中重新开始游戏，将魔方从初始状态重新打乱，进入普通模式
		 */
        public void RestartGame()
        {
            GameObject restartBanner = GameObject.Find("RestartBannerImage");
            restartBanner.transform.localPosition = new Vector3(-5500, 0, 0);
            restartBanner.transform.localScale = new Vector3(1f, 0.1f, 1f);
            GameObject restartBannerText = GameObject.Find("RestartBannerText");
            restartBannerText.transform.localPosition = new Vector3(-1500, 0, 0);

            MoveTo bannerAcross = new MoveTo(restartBanner, new Vector3(5500, 0, 0), 2.6f);
            MoveTo textAcross = new MoveTo(restartBannerText, new Vector3(1500, 0, 0), 2.4f);
            textAcross.SetEasing(new FunctorEase((float x) =>
            {
                float x2 = x - 0.5f;
                return 2f * Mathf.Sign(x2) * x2 * x2 + 0.5f;
            }));
            textAcross.Start();

            ScaleTo bannerScaleTo = new ScaleTo(restartBanner, new Vector3(1, 1, 1), 0.65f);
            bannerScaleTo.SetEasing(new QuadOut());
            bannerScaleTo.Start();

            Sequence seq = new Sequence();
            seq.AddToQueue(bannerAcross);
            seq.AddToQueue(new CallFunc(() =>
            {
                // 计数器禁止
                StepCounterText.Instance.enable = false;

                // 清空魔方动作序列
                RubicCube rcube = RubicCube.Instance;
                rcube.ClearAllActions();
                rcube.PostResetCubesAction();
            }));
            seq.AddToQueue(new Wait(0.8));
            seq.AddToQueue(new CallFunc(() =>
            {
                RubicCube rcube = RubicCube.Instance;
                rcube.OnShuffleDone = () =>
                {
                    SmallCubeGroup.AnimationIntervalMax = 1f;

                    // 定时器复位，计数器复位
                    TimerText timer = TimerText.Instance;
                    timer.Reset();
                    timer.StartTiming();
                    StepCounterText counter = StepCounterText.Instance;
                    counter.Reset();
                    counter.enable = true;

                    GameObject.Find("RestartButton").GetComponent<RestartButton>().enable = true;
                };
                rcube.Shuffle();
                PopoTips.ShowTips("当前为普通模式");
            }));
            seq.Start();
            RestartGameEffect.Instance.Play();
            challengeMode_ = false;
        }

    }
}
