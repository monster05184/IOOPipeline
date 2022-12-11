using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.Rendering;

namespace IOOPipeline
{
    public class TextureArray : Component
    {
        public string path;
        public string fieldName;
        public TextureList textureList;
        public Texture2DArray texture2DArray;
        private int size;

        public TextureArray(string path, string fieldName, int size)
        {
            this.path = path;
            this.fieldName = fieldName;
            needToDispose = true;
            this.size = size;
        }

        public override void Load()
        {
            if (path != null)
            {
                textureList = AssetDatabase.LoadAssetAtPath<TextureList>(path);
            }

            texture2DArray = new Texture2DArray(size, size, textureList.textures.Count, TextureFormat.RGBA32, true);
            
            for (int i = 0; i < textureList.textures.Count; i++)
            {
                if (textureList.textures[i] != null)
                {
                    texture2DArray.SetPixels(textureList.textures[i].GetPixels(),i);
                }
                
            }

            texture2DArray.wrapMode = TextureWrapMode.Repeat;
            texture2DArray.filterMode = FilterMode.Bilinear;
            texture2DArray.Apply();
        }
        
        public override void Dispose() {
            base.Dispose();

            Dispose(disposing: true);
            
            GC.SuppressFinalize(this);
        }
        
        protected void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    Close();
                }
            }
            this.disposed = true;
        }
        private void Close() {
            //TODO
            if (textureList != null)
            {
                
            }

        }
        
    }

}
