using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOOPipeline
{
    public class SceneInfo : Component
    {
        public Camera[] Cameras;
        public int clusterCount = 0;
        public int renderObjectCount = 0;
        public bool initialized = false;
        public bool dataSended = false;
    }
}

