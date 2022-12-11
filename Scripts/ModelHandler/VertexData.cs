using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VertexData : ScriptableObject
{
    public struct Vertex {
        public Vector3 pos;
        public Vector2 uv;
        public Vector3 normal;
        public Vector3 tangent;

        public static int GetStride() {
            return sizeof(float) * (3 + 2 + 3 + 3);
        }
    }
    public Vertex[] vertices;
    public int[] indexs;

}
