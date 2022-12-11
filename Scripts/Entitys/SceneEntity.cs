using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOOPipeline
{
    public class SceneEntity : Entity
    {
        private static SceneEntity instance;

        public static SceneEntity GetInstance()
        {
            if (instance == null)
            {
                instance = new SceneEntity();
            }
            
            return instance;
        }

        public CullingBuffer cullingBuffer;
        public SceneBuffer sceneBuffer;
        public SceneInfo sceneInfo;
        
        public override bool Initialize()
        {
            cullingBuffer = new CullingBuffer();
            components.Add(cullingBuffer);

            sceneBuffer = new SceneBuffer();
            components.Add(sceneBuffer);

            sceneInfo = new SceneInfo();
            components.Add(sceneInfo);
            sceneInfo.initialized = true;
            return base.Initialize();
            
        }

        public override bool Release()
        {
            sceneInfo.initialized = false;
            instance = null;
            
            return base.Release();
            
        }
    }
}

