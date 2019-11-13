using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace adolli.Engine.Network
{
    public class HttpRequest
    {
        public const float TimeOutThredshold = 1.4f;
        public const float ErrorWaitForRetryThredshold = 3f;    // 出错或超时则等待3s后重试
        public const int RetryMax = 3;  // 最大重试次数

        public enum RequestStatus
        {
            NewSpawn,
            Ready,
            Waiting,
            TimeOut,
            WaitForRetry,
            Disposed
        }

        protected string url_;
        protected Dictionary<string, string> data_;
        private float clock_;
        private int retryTimes_;
        private RequestStatus status_;
        public WWW request_;

        public HttpRequest(string url, Dictionary<string, string> data)
        {
            url_ = url;
            data_ = data;
            clock_ = 0;
            retryTimes_ = 0;
            status_ = RequestStatus.NewSpawn;
            RequestEventDispatcher.AddRequest(this);
        }

        public void Post()
        {
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> kv in data_)
            {
                form.AddField(kv.Key, kv.Value);
            }
            request_ = new WWW(url_, form);
            status_ = RequestStatus.Waiting;
        }

        public bool IsDone()
        {
            return request_ != null && request_.isDone;
        }


        public RequestStatus GetStatus()
        {
            return status_;
        }

        /**
		 * @brief 记录请求发起开始经历的时间，并根据时间判断当前状态
		 * @return 超时次数，根据超时次数判断状态
		 */
        public RequestStatus AddTimeElapse(float deltaTime)
        {
            clock_ += deltaTime;
            if (status_ == RequestStatus.WaitForRetry)
            {
                if (clock_ > ErrorWaitForRetryThredshold)
                {
                    ++retryTimes_;
                    if (retryTimes_ > RetryMax)
                    {
                        status_ = RequestStatus.Disposed;
                        if (OnMaxRetried != null)
                        {
                            OnMaxRetried();
                        }
                    }
                    else
                    {
                        status_ = RequestStatus.Ready;
                    }
                    clock_ = 0;
                }
            }
            else
            {
                if (clock_ > TimeOutThredshold)
                {
                    status_ = RequestStatus.TimeOut;
                    clock_ = 0;
                }
            }
            return status_;
        }

        public delegate void ResponseType(Hashtable resp);
        public delegate void ResponseErrorType(string message);
        public delegate void TimeOutType();

        public ResponseType OnResponsed
        {
            set;
            get;
        }

        private ResponseErrorType onError_;
        public ResponseErrorType OnError
        {
            set { onError_ = value; }
            get
            {
                status_ = RequestStatus.WaitForRetry;
                return onError_;
            }
        }

        private TimeOutType onTimeOut_;
        public TimeOutType OnTimeOut
        {
            set { onTimeOut_ = value; }
            get
            {
                status_ = RequestStatus.WaitForRetry;
                return onTimeOut_;
            }
        }

        public TimeOutType OnMaxRetried
        {
            set;
            get;
        }
    }
}

