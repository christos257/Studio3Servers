using System;

[Serializable]
public class ScenePacket : BasePacket
{
    public int sceneIndex;
    public ScenePacket() 
    {
        packetType = Type.ScenePacket;
    }
}
