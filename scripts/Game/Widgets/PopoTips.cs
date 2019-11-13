using UnityEngine;
using UnityEngine.UI;
using adolli.Engine;

namespace adolli
{
    public class PopoTips : MonoBehaviour
    {

        private static PopoTips instance_;

        // Use this for initialization
        void Start()
        {
            instance_ = this;
            GetComponent<CanvasGroup>().alpha = 0;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public static void ShowTips(string tips)
        {
            GameObject.Find("PopoTipsText").GetComponent<Text>().text = tips;

            AlphaTo fadeIn = new AlphaTo(instance_.gameObject, 1, 0.5f);
            AlphaTo fadeOut = new AlphaTo(instance_.gameObject, 0, 0.5f);

            Sequence seq = new Sequence();
            seq.AddToQueue(fadeIn);
            seq.AddToQueue(new Wait((float)tips.Length / 10));
            seq.AddToQueue(fadeOut);
            seq.Start();
        }
    }

}
