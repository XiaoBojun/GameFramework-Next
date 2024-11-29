using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEditor.Experimental.GraphView;

namespace GameLogic
{    
    /// <summary>
     /// �Զ������ýӿڷ��صĹ�����ʽ
     /// </summary>
     /// <typeparam name="T"></typeparam>
    public class ResponseResult<T>
    {
        /// <summary>
        /// ״̬��
        /// </summary>
        public int code;

        /// <summary>
        /// ��Ϣ
        /// </summary>
        public string message;

        /// <summary>
        /// ��������
        /// </summary>
        public List<T> data { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public int count { get; set; }
    }
    public class HTTPSystem: MonoBehaviour
    {
        int timeOut = 10;
        /// <summary>
        /// ���õ�ǰ��ʱʱ��
        /// </summary>
        public int TimeOut
        {
            set { timeOut = value; }
            get { return timeOut; }
        }
        string token = "";
        public string Token
        {
            set { token = value; }
            get { return token; }
        }
        private static HTTPSystem instance = null;
        private static object oLock = new object();
        public static HTTPSystem Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (oLock)
                    {
                        GameObject obj = new GameObject();
                        obj.name = "HttpTool";
                        HTTPSystem hTTPSystem = obj.AddComponent<HTTPSystem>();
                        HTTPSystem.instance = hTTPSystem;
                    }
                }
                return instance;
            }
        }
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        #region ʾ��
        //public void Init()
        //{
        //    ip = GameEntry.Config.GetString("IP");
        //    port = GameEntry.Config.GetString("Port");
        //    httpTool = HttpTool.Instance;
        //}
        //public void Net_GetUserListBy_role<T>(Action<T> callback, string role)
        //{
        //    url = "http://" + ip + ":" + port + "/api/Rmt/GetUserByRole?role=" + role;

        //    httpTool.Get<T>(url, callback);
        //}
        //public void Net_SelectSysLayoutInfo(Action<SelectSysLayoutInfoResult> callback)
        //{
        //    url = "http://" + ip_hzcj + ":" + port_hzcj + "/equExhibition/selectSysLayoutInfo";

        //    //url = "http://192.168.110.147:9000/equExhibition/selectSysLayoutInfo";
        //    httpTool.Post<SelectSysLayoutInfoResult>(url, "{}", callback);
        //    //http://192.168.110.147:9000/equExhibition/sselectSysLayoutInfo
        //}
        #endregion

        #region �ӿ�
        /// <summary>
        /// GET����
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public void Get<T>(string url, Action<T> callback, bool needToken = true)
        {
            StartCoroutine(GetRequest<T>(url, callback, needToken));
        }
        private IEnumerator GetRequest<T>(string url, Action<T> callback, bool needToken = true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                //����header
                this.SetHeader(webRequest, needToken);
                webRequest.timeout = timeOut;
                yield return webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
#elif UNITY_2017_1_OR_NEWER
                if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callback(default);
                }
                else
                {
                    if (null != callback)
                    {
                        string requestText = webRequest.downloadHandler.text;
                        //print(requestText.Length);
                        //if (requestText.Length>18600)
                        //{
                        //    string str = requestText;
                        //    string str1 = requestText;
                        //    GameObject.Find("Canvas/Text").GetComponent<UnityEngine.UI.Text>().text = str.Substring(0, 9346);
                        //    print(str1.Length);
                        //    GameObject.Find("Canvas/Text1").GetComponent<UnityEngine.UI.Text>().text = str1.Substring(9347);
                        //    //GameObject.Find("Canvas/Text2").GetComponent<UnityEngine.UI.Text>().text = requestText.Substring(12465, 18680);
                        //}

                        Debug.Log("url = " + url + "\n����ֵ = " + requestText + "\nToken = " + needToken);
                        T requestData = JsonUtility.FromJson<T>(requestText);
                        if (requestData == null)
                        {
                            Debug.LogError("get�����ֵΪ��");
                        }
                        else
                        {
                            callback(requestData);
                        }
                    }
                }
            }
        }
        public void Get(string url, Action<string> callback, bool needToken = true)
        {
            StartCoroutine(GetRequest(url, callback, needToken));
        }
        private IEnumerator GetRequest(string url, Action<string> callback, bool needToken = true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                //����header
                this.SetHeader(webRequest, needToken);
                webRequest.timeout = timeOut;
                yield return webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
#elif UNITY_2017_1_OR_NEWER
                if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callback(default);
                }
                else
                {
                    if (null != callback)
                    {
                        string requestText = webRequest.downloadHandler.text;
                        Debug.Log("url = " + url + "\n����ֵ = " + requestText + "\nToken = " + needToken);
                        if (requestText == null)
                        {
                            Debug.LogError("get�����ֵΪ��");
                        }
                        else
                        {
                            callback(requestText);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Post����
        /// </summary>
        /// <param name="url">url��ַ</param>
        /// <param name="jsonString">Ϊjson�ַ�����post�ύ�����ݰ�Ϊjson</param>
        /// <param name="callback">���ݷ���</param>
        public void Post<T>(string url, object jsonString, Action<T> callback, bool needToken = true)
        {
            StartCoroutine(PostRequest<T>(url, jsonString, callback, needToken));
        }
        private IEnumerator PostRequest<T>(string url, object jsonString, Action<T> callback, bool needToken = true)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonString));
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = timeOut;
                this.SetHeader(webRequest, needToken);
                yield return webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
#elif UNITY_2017_1_OR_NEWER
                if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callback(default);
                }
                else
                {
                    if (callback != null)
                    {
                        string requestText = webRequest.downloadHandler.text;
                        T requestData = JsonUtility.FromJson<T>(requestText);
                        Debug.Log("url = " + url + "\njsonString = " + jsonString + "\n����ֵ = " + requestText + "\nToken = " + needToken);
                        if (null == requestData)
                        {
                            Debug.LogError("���ݷ���ֵΪ��");
                        }
                        else
                        {
                            callback(requestData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Put����
        /// </summary>
        /// <param name="url">url��ַ</param>
        /// <param name="jsonString">Ϊjson�ַ�����post�ύ�����ݰ�Ϊjson</param>
        /// <param name="callback">���ݷ���</param>
        public void Put<T>(string url, object jsonString, Action<ResponseResult<T>> callback, bool needToken = true)
        {
            StartCoroutine(PutRequest(url, jsonString, callback, needToken));
        }
        private IEnumerator PutRequest<T>(string url, object jsonString, Action<ResponseResult<T>> callback, bool needToken = true)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonString));
            using (UnityWebRequest webRequest = UnityWebRequest.Put(url, bodyRaw))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.timeout = timeOut;
                this.SetHeader(webRequest, needToken);
                yield return webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
#elif UNITY_2017_1_OR_NEWER
                if (webRequest.isHttpError || webRequest.isNetworkError)
#endif
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callback(default);
                }
                else
                {
                    string requestText = webRequest.downloadHandler.text;
                    ResponseResult<T> requestData = JsonUtility.FromJson<ResponseResult<T>>(requestText);
                    Debug.Log("url = " + url + "\njsonString = " + jsonString + "\n����ֵ = " + requestText + "\nToken = " + needToken);
                    if (null == requestData)
                    {
                        Debug.LogError("���ݷ���ֵΪ��");
                    }
                    else
                    {
                        callback(requestData);
                    }
                }
            }
        }
        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="path">����·��</param>
        /// <param name="fileName">�ļ���</param>
        /// <param name="fun">�ص����ɻ�����ؽ���</param>
        /// <param name="needToken">�Ƿ���Ҫtoken</param>
        public void DownloadFile(string url, string path, string fileName, Action<float> fun, bool needToken = true)
        {
            StartCoroutine(DownloadRequestData(url, path, fileName, fun, needToken));
        }
        IEnumerator DownloadRequestData(string url, string path, string fileName, Action<float> fun, bool needToken = true)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
            {
                string filePath = Path.Combine(path, fileName);
                webRequest.downloadHandler = new DownloadHandlerFile(filePath);
                this.SetHeader(webRequest, needToken);
                webRequest.SendWebRequest();
                while (!webRequest.isDone)
                {
                    if (fun != null)
                    {
                        fun(webRequest.downloadProgress);
                    }
                    yield return null;
                }
                fun(1);
            }
        }
        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="url">���ص�ַ</param>
        /// <param name="receiveFunction"></param>
        public void DownloadFile(string url, string path, Action<float> fun, bool needToken = true)
        {
            StartCoroutine(DownloadRequestData(url, path, fun, needToken));
        }
        IEnumerator DownloadRequestData(string url, string path, Action<float> fun, bool needToken = true)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET))
            {
                webRequest.downloadHandler = new DownloadHandlerFile(path);
                this.SetHeader(webRequest, needToken);
                webRequest.SendWebRequest();
                while (!webRequest.isDone)
                {
                    if (fun != null)
                    {
                        fun(webRequest.downloadProgress);
                    }
                    yield return null;
                }

                fun(1);
            }
        }
        /// <summary>
        /// ��������ͷ
        /// </summary>
        /// <param name="webRequest"></param>
        private void SetHeader(UnityWebRequest webRequest, bool needToken = true)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            if (needToken)
            {
                webRequest.SetRequestHeader("Authorization", token);
            }
        }
        private void SetHeader(UnityWebRequest webRequest, string jsonStr, bool needToken = true)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            if (needToken)
            {
                webRequest.SetRequestHeader("Authorization", token);
            }
            // webRequest.SetRequestHeader("Authorization", jsonStr);
        }
        #endregion

    }
}

