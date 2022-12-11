using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IOOPipeline
{
    public class MaterialPropertyData : ScriptableObject
    {
        [Serializable]
        public struct MaterialProperty 
        {
            public Color mainColor;
            public Color emissionColor;
            public float matallicStrengeth;
            public float smoothness;
            [TextureAsset] [sRGB(true)] [TextureSize(TextureSize.SIZE._2048)]public int abedoTexture;
            [NormalTexture] [sRGB(true)] [TextureSize(TextureSize.SIZE._2048)]public int normalTexture;
            [TextureAsset] [sRGB(true)] [TextureSize(TextureSize.SIZE._2048)]public int metallicMap;
            [TextureAsset] [sRGB(true)] [TextureSize(TextureSize.SIZE._2048)]public int emissionMap;
            
            public static int GetStride()
            {
                return sizeof(float) * (4);
            }
        }

        public MaterialProperty materialProperty;
        public int renderObjectId;
        
    }


}

