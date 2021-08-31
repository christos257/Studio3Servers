using System;
using UnityEngine;

[Serializable]
public class MovementPacket : BasePacket
{
    public Vec3 position;
    public Vec3 rotation;

    public MovementPacket() 
    {
        packetType = Type.MovementPacket;
    }
}
