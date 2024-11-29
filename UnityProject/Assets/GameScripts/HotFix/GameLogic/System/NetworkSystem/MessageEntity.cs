using System;
using UnityEngine;

namespace GameLogic
{
    [Serializable]
    public class MessageEntity 
    {
        //后续测试
        public string Platform { get; set; } //平台
        public string UserName { get; set; } //登录名
        public string CallName { get; set; } //呼叫用户
    }
    [Serializable]
    public enum MessageInstruction
    {
        OnLine = 0,      //上线
        OffLine = 1,     //下线       
        Heartbeat = 2,   //心跳
    }
}
