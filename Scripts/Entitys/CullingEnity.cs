using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOOPipeline
{
    public class CullingEnity : Entity
    {
        public CullingShader cullingShader;

        public FrustumCullingCom frustumCullingCom;



        
        public override bool Initialize()
        {
            frustumCullingCom = new FrustumCullingCom();
            frustumCullingCom.planes = new Vector4[6];
            frustumCullingCom.corners = new Vector3[4];
            components.Add(frustumCullingCom);

            cullingShader = new CullingShader();
            components.Add(cullingShader);
            

            return base.Initialize();
        }
        public override bool Release() {


            return base.Release();
        }

    }
}

