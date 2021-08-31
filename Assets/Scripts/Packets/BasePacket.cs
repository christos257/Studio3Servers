using System;

[Serializable]
public class BasePacket
{
    public string username;
    public enum Type { BasePacket, ChatPacket, MovementPacket, InstantiatePacket, ScenePacket }
    public Type packetType;

    public BasePacket() 
    {
        packetType = Type.BasePacket;
    }

}
