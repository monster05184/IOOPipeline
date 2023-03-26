using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace IOOPipeline
{
    
    struct ShaderProperty
    {
        public struct CullingShader
        {
            public static int clusterBufferId = Shader.PropertyToID("clusterBuffer");
            public static int resultBufferId = Shader.PropertyToID("resultBuffer");
            public static int argsBufferId = Shader.PropertyToID("argsBuffer");
            public static int planesId = Shader.PropertyToID("planes");
            public static int clusterCountId = Shader.PropertyToID("clusterCount");
            public static int camPosId = Shader.PropertyToID("camPosition");
            public static int local2WorldMatrixBufferId = Shader.PropertyToID("localToWorldMatrixBuffer");
        }
    
    }
}

