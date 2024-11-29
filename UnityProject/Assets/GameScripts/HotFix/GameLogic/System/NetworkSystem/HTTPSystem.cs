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
     /// 自定义设置接口返回的公共格式
     /// </summary>
     /// <typeparam name="T"></typeparam>
    public class ResponseResult<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int code;

        /// <summary>
        /// 消息
        /// </summary>
        public string message;

        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> data { get; set; }

        /// <summary>
        /// 返回数量
        /// </summary>
        public int count { get; set; }
    }
    public class HTTPSystem: MonoBehaviour
    {
        int timeOut = 10;
        /// <summary>
        /// 设置当前超时时间
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

        #region 示例
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

        #region 接口
        /// <summary>
        /// GET请求
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
                //设置header
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

                        Debug.Log("url = " + url + "\n返回值 = " + requestText + "\nToken = " + needToken);
                        T requestData = JsonUtility.FromJson<T>(requestText);
                        if (requestData == null)
                        {
                            Debug.LogError("get请求的值为空");
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
                //设置header
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
                        Debug.Log("url = " + url + "\n返回值 = " + requestText + "\nToken = " + needToken);
                        if (requestText == null)
                        {
                            Debug.LogError("get请求的值为空");
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
        /// Post请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">为json字符串，post提交的数据包为json</param>
        /// <param name="callback">数据返回</param>
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
                        Debug.Log("url = " + url + "\njsonString = " + jsonString + "\n返回值 = " + requestText + "\nToken = " + needToken);
                        if (null == requestData)
                        {
                            Debug.LogError("数据返回值为空");
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
        /// Put请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="jsonString">为json字符串，post提交的数据包为json</param>
        /// <param name="callback">数据返回</param>
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
                    Debug.Log("url = " + url + "\njsonString = " + jsonString + "\n返回值 = " + requestText + "\nToken = " + needToken);
                    if (null == requestData)
                    {
                        Debug.LogError("数据返回值为空");
                    }
                    else
                    {
                        callback(requestData);
                    }
                }
            }
        }
        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="path">下载路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fun">回调，可获得下载进度</param>
        /// <param name="needToken">是否需要token</param>
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
        /// 下载资源
        /// </summary>
        /// <param name="url">下载地址</param>
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
        /// 设置请求头
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

