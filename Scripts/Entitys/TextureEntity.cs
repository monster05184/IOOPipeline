using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using IOOPipeline;
using UnityEngine;

namespace IOOPipeline
{
    public class TextureEntity : Entity
    {
        private void AnalyseTextures()
        {
            FieldInfo[] fields = typeof(MaterialPropertyData.MaterialProperty).GetFields();
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes();
                TextureSize.SIZE size = TextureSize.SIZE._2048;//默认尺寸为2048
                bool isTexture = false;
                foreach (var a in attributes)
                {
                    if (a.GetType() == typeof(TextureAsset) || a.GetType() == typeof(NormalTexture))//判断是否为图片
                    {
                        isTexture = true;
                    }

                    if (a.GetType() == typeof(TextureSize))//修正图片的尺寸
                    {
                        TextureSize ts = a as TextureSize;
                        size = ts.size;
                    }
                }
                
                if (isTexture)
                {
                    string path = "Assets/Resourses/Textures/" + SceneInfoHandler.GetSceneName() + "_" + field.Name + ".asset";;
                    components.Add(new TextureArray(path, field.Name, (int)size));
                }
            }
        }

        public override bool Initialize()
        {
            AnalyseTextures();
            return base.Initialize();
        }
    }

}
