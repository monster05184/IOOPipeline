using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IOOPipeline
{
    public class SceneInfoHandler 
    {
        public static string GetSceneName()
        {
            Scene scene = SceneManager.GetActiveScene();
        
            return scene.name;
        }
    }
}

