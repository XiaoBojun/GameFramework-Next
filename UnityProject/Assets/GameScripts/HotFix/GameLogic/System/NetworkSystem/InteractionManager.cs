using UnityEngine;

namespace GameLogic
{
    public class InteractionManager : MonoBehaviour
    {
#if UNITY_EDITOR
        //测试暂留，随便删除
        private string input_id1 = "10005070";
        private string input_id2 = "10005071";
        public void OnGUI()
        {

            if (GUI.Button(new Rect(100, 100, 100, 30), ""))
            {
                OnEvent_VueMessageToUnity("","");
            }
            input_id1 = GUI.TextField(new Rect(100, 170, 200, 30), input_id1, 25);
            input_id2 = GUI.TextField(new Rect(100, 200, 200, 30), input_id2, 25);
        }
#endif
        #region//前端调用Unity******************************************//

        public void OnEvent_VueMessageToUnity(string key,string value)
        {
            switch (key)
            {
                default:
                    break;
            }
        }
        #endregion

        #region//Unity调用前端******************************************//
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SendUnityMessageToVue(string key,string msg);
#else
        private void SendUnityMessageToVue(string key,string msg)
        {
            Debug.Log($"向前端传递信息——{key}：{msg}");
        }
#endif
        public void SendUnityMessageToVue_Web(string key, string msg)
        {
            SendUnityMessageToVue(key,msg);
        }
        #endregion

    }
}

