using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IOOPipeline
{
    public class TextureList : ScriptableObject
    {
        public List<Texture2D> textures = new List<Texture2D>();
    
        private void AddTexture(Texture2D texture)
        {
            textures.Add(texture);
        }

        public void ChangeTextureAt(Texture2D texture, int index)
        {
            textures.RemoveAt(index);
            textures.Insert(index, texture);
        }
    

        public Texture2D GetTextureAt(int index)//如果某个位置有图片，取用图片，如果没有，添加图片
        {
            if (index >= textures.Count)
            {
                Texture2D addOne = null;
                AddTexture(addOne);
            }
            return textures[index];
        }

    }

}
