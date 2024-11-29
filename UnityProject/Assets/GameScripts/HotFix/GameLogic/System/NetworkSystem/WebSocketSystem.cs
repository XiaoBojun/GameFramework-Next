
using System.Text;
using System;
using UnityEngine;
using BestHTTP.WebSocket;
using LitJson;
using static UnityEditor.ShaderData;

namespace GameLogic
{

    public class WebSocketSystem
    {
        private WebSocket webSocket = null;
        string address = "wss://echo.websocket.org";

        public void Init()
        {
            //地址可更新 todo
            if (webSocket == null)
            {
                webSocket = new WebSocket(new Uri(address));
#if !UNITY_WEBGL
                 webSocket.StartPingThread = true;
#endif
                // Subscribe to the WS events
                webSocket.OnOpen += OnOpen;
                webSocket.OnMessage += OnMessageReceived;
                webSocket.OnBinary += OnBinaryReceived;
                webSocket.OnClosed += OnClosed;
                webSocket.OnError += OnError;
                // Start connecting to the server
                webSocket.Open();
            }
        }

        public void Destroy()
        {
            if (webSocket != null)
            {
                webSocket.Close();
                webSocket = null;
            }
        }

        void OnOpen(WebSocket ws)
        {
            Debug.Log("WebSocket开始:"+ address);
            //Test
            MessageEntity message = new MessageEntity();
            message.UserName = "11";
            message.Platform = "PC";
            SendMessageEntity(message);
        }

        void OnMessageReceived(WebSocket ws, string message)
        {
            Debug.LogFormat("OnMessageRecv: msg={0}", message);
        }

        void OnBinaryReceived(WebSocket ws, byte[] data)
        {
            var _str = System.Text.Encoding.UTF8.GetString(data);
            Debug.LogFormat("OnBinaryRecv: len={0}", data.Length+":"+_str);
        }

        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            Debug.LogFormat("OnClosed: code={0}, msg={1}", code, message);
            webSocket = null;
        }

        void OnError(WebSocket ws, Exception ex)
        {
            string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
            if (ws.InternalRequest.Response != null)
            {
                errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
            }
#endif
            Debug.LogFormat("OnError: error occured: {0}\n", (ex != null ? ex.Message : "Unknown Error " + errorMsg));
            webSocket = null;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void Send(string msg)
        {
            if (webSocket == null)
                return;
            webSocket.Send(msg);
        }
        public void SendMessageEntity(MessageEntity message)
        {
            string msgJson = JsonMapper.ToJson(message);
            Send(msgJson);
        }
        public MessageEntity ReceivedEntity(string message)
        {
            MessageEntity msg = JsonMapper.ToObject<MessageEntity>(message.ToString());
            return msg;
        }


        /// <summary>
        /// 发送字节流
        /// 数据包长度【4 字节Init 32】 数据类型【4字节 Int32】 实际数据包：用户名【字符串 100】  数据体
        /// </summary>
        /// <param name="str"></param>
        public void Send_Binary(byte[] data, MessageInstruction messagePicks)
        {
            //byte[] packet = new byte[binaryDataLen + binaryTypeLen + binaryNameLen + data.Length];

            //int len = packet.Length;
            //var lenArrays = BitConverter.GetBytes(len);

            //int _type = (int)messagePicks;
            //var typeArrays = BitConverter.GetBytes(_type);

            //string callName = mCallName;
            //byte[] stringBytes = Encoding.UTF8.GetBytes(callName);
            //byte[] nameArray = new byte[100];
            //int bytesToCopy = Math.Min(stringBytes.Length, nameArray.Length);
            //Array.Copy(stringBytes, 0, nameArray, 0, bytesToCopy);

            //int index = 0;
            //Array.Copy(lenArrays, 0, packet, 0, lenArrays.Length);
            //index += binaryDataLen;
            //Array.Copy(typeArrays, 0, packet, index, typeArrays.Length);
            //index += binaryTypeLen;
            //Array.Copy(nameArray, 0, packet, index, nameArray.Length);
            //index += binaryNameLen;
            //Array.Copy(data, 0, packet, index, data.Length);

            //if (webSocket != null)
            //    webSocket.Send(packet);
        }
    }
}
