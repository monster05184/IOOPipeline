using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterData : ScriptableObject
{
    [Serializable]
    public struct ClusterInfo
    {
        public Bounds bounds;
        public int objIndex;

        public static int GetStride()
        {
            return Bounds.GetStride() + sizeof(float) * (0) + sizeof(int) * 1;
        }
    }
    [Serializable]
    public struct Bounds 
    {
        public Vector3 extent;
        public Vector3 center;
        public static int GetStride() {
            return sizeof(float) * (3 + 3);
        }
    }



    public ClusterInfo[] clusterInfos;
    public int clusterCount = 0;

    public bool AddCluster(ClusterInfo cluster)
    {
        int formerLength = 0;
        if (clusterInfos == null)
        {
            
        }
        else
        {
            formerLength = clusterInfos.Length;
        }
        ClusterInfo[] newInfos = new ClusterInfo[formerLength + 1];
        for (int i = 0; i < formerLength; i++)
        {
            newInfos[i] = clusterInfos[i];
        }
        newInfos[formerLength] = cluster;
        clusterInfos = newInfos;
        clusterCount++;
        return true;
    }


}
