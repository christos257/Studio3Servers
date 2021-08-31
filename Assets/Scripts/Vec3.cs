using System;
using UnityEngine;

[Serializable]
public class Vec3
{
    public float x;
    public float y;
    public float z;


    public Vec3() 
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Vec3(float x, float y, float z) 
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vec3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
    public Vector3 GetVector3() 
    {
        return new Vector3(x, y, z);
    }
}
