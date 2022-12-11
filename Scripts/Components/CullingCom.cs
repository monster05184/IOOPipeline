using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IOOPipeline {
    public class FrustumCullingCom : Component {
        public Camera cam;
        public Vector3[] corners;
        public Vector4[] planes;
    }

}
