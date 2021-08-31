using System;

[Serializable]
public class ChatPacket : BasePacket
{
    public string message;

    public ChatPacket() 
    {
        packetType = Type.ChatPacket;
    }
}
