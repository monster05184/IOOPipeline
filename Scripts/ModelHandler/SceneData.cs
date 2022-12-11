using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SceneData : ScriptableObject
{
    public int maxRenderObjectId = 0;
    public static int ClusterSize = 64;

    public int GetRenderObjectId()
    {
        return maxRenderObjectId+1;
    }
}


