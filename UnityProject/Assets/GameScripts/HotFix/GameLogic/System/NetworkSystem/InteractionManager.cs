using UnityEngine;

namespace GameLogic
{
    public class InteractionManager : MonoBehaviour
    {
#if UNITY_EDITOR
        //�������������ɾ��
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
        #region//ǰ�˵���Unity******************************************//

        public void OnEvent_VueMessageToUnity(string key,string value)
        {
            switch (key)
            {
                default:
                    break;
            }
        }
        #endregion

        #region//Unity����ǰ��******************************************//
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SendUnityMessageToVue(string key,string msg);
#else
        private void SendUnityMessageToVue(string key,string msg)
        {
            Debug.Log($"��ǰ�˴�����Ϣ����{key}��{msg}");
        }
#endif
        public void SendUnityMessageToVue_Web(string key, string msg)
        {
            SendUnityMessageToVue(key,msg);
        }
        #endregion

    }
}

