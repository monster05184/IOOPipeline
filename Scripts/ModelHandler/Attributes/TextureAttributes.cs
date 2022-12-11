using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOOPipeline
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NormalTexture : System.Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class TextureAsset : System.Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class TextureSize : System.Attribute
    {
        public enum SIZE
        {
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096
        }

        public SIZE size;

        public TextureSize(SIZE size)
        {
            this.size = size;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class sRGB : System.Attribute
    {
        public bool on;
        public sRGB( bool on )
        {
            this.on = on;
        }
    }
    
    
}

