using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace adolli.Engine.Network
{
    public class RequestEventDispatcher : MonoBehaviour
    {

        protected static LinkedList<HttpRequest> requestQueue_ = new LinkedList<HttpRequest>();

        void Start()
        {
        }

        void Update()
        {
            LinkedList<HttpRequest> toBeRemoved = new LinkedList<HttpRequest>();
            foreach (HttpRequest req in requestQueue_)
            {
                HttpRequest.RequestStatus sta = req.GetStatus();
                if (sta == HttpRequest.RequestStatus.Ready)
                {
                    req.Post();
                }
                else if (sta == HttpRequest.RequestStatus.Waiting && req.IsDone())
                {
                    Hashtable result = (Hashtable)NGUIJson.jsonDecode(req.request_.text);
                    if (result != null)
                    {
                        if (req.OnResponsed != null)
                        {
                            req.OnResponsed(result);
                        }
                        toBeRemoved.AddLast(req);
                    }
                    else
                    {
                        if (req.OnError != null)
                        {
                            req.OnError(req.request_.text);
                        }
                    }
                }
                else
                {
                    HttpRequest.RequestStatus status = req.AddTimeElapse(Time.deltaTime);
                    if (status == HttpRequest.RequestStatus.TimeOut)
                    {
                        if (req.OnTimeOut != null)
                        {
                            req.OnTimeOut();
                        }
                    }
                    else if (status == HttpRequest.RequestStatus.Disposed)
                    {
                        toBeRemoved.AddLast(req);
                    }
                }
            }
            foreach (HttpRequest req in toBeRemoved)
            {
                requestQueue_.Remove(req);
            }
            //Debug.Log ("queue length=" + requestQueue_.Count);
        }

        public static void AddRequest(HttpRequest req)
        {
            requestQueue_.AddLast(req);
        }
    }
}

