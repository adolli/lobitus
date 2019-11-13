using UnityEngine;
using System.Collections.Generic;

namespace adolli.Engine
{
    public class TouchDispatcher : MonoBehaviour
    {

#if MOBILE_INPUT
        private static CoordinateInput input_ = new TouchInput();
#else
		private static CoordinateInput input_ = new MouseInput ();
#endif

        private static LinkedList<Touchable> registeredListener_ = new LinkedList<Touchable>();
        private Touchable activeListener_ = null;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (input_.GetTouchPhase() == TouchPhase.Began)
            {
                activeListener_ = null;
                Ray ray = Camera.main.ScreenPointToRay(input_.GetScreenPosition());
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500))
                {
                    foreach (Touchable listener in registeredListener_)
                    {
                        Debug.Log("colloder=" + hit.collider.tag + "   listener=" + listener.GetTag());
                        if (hit.collider.tag == listener.GetTag())
                        {
                            TouchInfo touch = new TouchInfo();
                            touch.screenPosition = input_.GetScreenPosition();
                            touch.position = hit.point - hit.collider.transform.position;
                            touch.normal = hit.normal;
                            bool handled = listener.OnTouchBegan(hit.collider, touch);
                            if (handled)
                            {
                                activeListener_ = listener;
                                break;
                            }
                        }
                    }
                }
            }
            else if (input_.GetTouchPhase() == TouchPhase.Moved)
            {
                if (activeListener_ != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(input_.GetScreenPosition());
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 500))
                    {
                        TouchInfo touch = new TouchInfo();
                        touch.screenPosition = input_.GetScreenPosition();
                        touch.deltaScreenPosition = input_.GetScreenDeltaPosition();
                        touch.position = hit.point - hit.collider.transform.position;
                        touch.normal = hit.normal;
                        activeListener_.OnTouchMoved(hit.collider, touch);
                    }
                }
            }
            else if (input_.GetTouchPhase() == TouchPhase.Ended)
            {
                if (activeListener_ != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(input_.GetScreenPosition());
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 500))
                    {
                        TouchInfo touch = new TouchInfo();
                        touch.screenPosition = input_.GetScreenPosition();
                        touch.deltaScreenPosition = input_.GetScreenDeltaPosition();
                        touch.position = hit.point;
                        touch.normal = hit.normal;
                        activeListener_.OnTouchEnded(hit.collider, touch);
                    }
                }
            }
        }

        public static void AddTouchListener(Touchable listener)
        {
            registeredListener_.AddLast(listener);
        }

    }
}
