using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData : ScriptableObject
{
    [Serializable]
    public struct Vertex
    {
        public Vector3 pos;
        public Vector2 uv;
        public Vector3 normal;
        public Vector4 tangent;
        public static int GetStride() {
            return sizeof(float) * (3 + 2 + 3 + 4);
        }
    }

    public int vertexCount;
    public int indexCount;
    public Vertex[] vertices;

    public int[] indexs;
    
    
}
