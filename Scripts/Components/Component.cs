using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace IOOPipeline
{
    public class Component:IDisposable
    {
        protected bool disposed = false;
        public bool needToDispose = false;

        public virtual void Load()
        {
            
        }

        public virtual void Dispose() {
           
        }
    }
}