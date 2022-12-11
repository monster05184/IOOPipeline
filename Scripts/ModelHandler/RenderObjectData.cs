using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IOOPipeline
{
    public class RenderObjectData:ScriptableObject
    {
        [System.Serializable]
        public struct ObjectInfo{
            public MaterialProperty materialProperty;
            public Matrix4x4 local2WordMatrix;
            public static int GetStride() {
                
                return MaterialProperty.GetStride() + sizeof(float) * (4 * 4) ;
            }
        }
        [System.Serializable]
        public struct MaterialProperty 
        {
            public Color mainColor;
            public static int GetStride()
            {
                return sizeof(float) * (4);
            }
        }
    
        public ObjectInfo[] renderObjects;
        public int renderObjectCount = 0;
        
        public bool AddRenderObject(ObjectInfo renderObject)
        {
            int formerLength = 0;
            if (renderObjects == null)
            {
                
            }
            else
            {
                formerLength = renderObjects.Length;
            }
    
           
            ObjectInfo[] newInfos = new ObjectInfo[formerLength + 1];
            for (int i = 0; i < formerLength; i++)
            {
                newInfos[i] = renderObjects[i];
            }
            newInfos[formerLength] = renderObject;
            renderObjects = newInfos;
            renderObjectCount++;
            return true;
        }
    }
}

