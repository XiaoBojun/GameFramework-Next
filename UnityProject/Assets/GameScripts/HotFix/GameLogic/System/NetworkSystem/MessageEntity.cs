using System;
using UnityEngine;

namespace GameLogic
{
    [Serializable]
    public class MessageEntity 
    {
        //��������
        public string Platform { get; set; } //ƽ̨
        public string UserName { get; set; } //��¼��
        public string CallName { get; set; } //�����û�
    }
    [Serializable]
    public enum MessageInstruction
    {
        OnLine = 0,      //����
        OffLine = 1,     //����       
        Heartbeat = 2,   //����
    }
}
