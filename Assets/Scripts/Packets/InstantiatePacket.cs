using System;
using UnityEngine;

[Serializable]
public class InstantiatePacket : BasePacket
{
    public string gameObjectName;
    public Vec3 position;
    public Vec3 rotation;
    public InstantiatePacket() 
    {
        packetType = Type.InstantiatePacket;
    }
}
