using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace IOOPipeline
{
   
    public class CullingBuffer : Component
    { 
        public ComputeBuffer clusterBuffer;
        public ComputeBuffer resultBuffer;
        public ComputeBuffer argsBuffer;
        public ComputeBuffer local2WorldMatrixBuffer;
        public CullingBuffer() {
            this.needToDispose = true;
        }
        ~CullingBuffer() {
            Dispose();
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
            //Debug.Log("Release CullingBuffer");
            if (clusterBuffer != null) {
                clusterBuffer.Release();
                clusterBuffer = null;
            }
            if (resultBuffer != null) {
                resultBuffer.Release();
                resultBuffer = null;
            }
            if (argsBuffer != null) {
                argsBuffer.Release();
                argsBuffer = null;
            }

            if (local2WorldMatrixBuffer != null) {
                local2WorldMatrixBuffer.Release();
                local2WorldMatrixBuffer = null;
            }

        }
    }

    public class SceneBuffer : Component
    { 
        public ComputeBuffer vertexBuffer;
        public GraphicsBuffer indexBuffer;
        public ComputeBuffer renderObjectBuffer;
        public ComputeBuffer local2WorldMatrixBuffer;
        public SceneBuffer() {
            this.needToDispose = true;
        }
        ~SceneBuffer() {
            Dispose(disposing: false);
           
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
            //Debug.Log("Release SceneBuffer");
            if (vertexBuffer != null) {
                vertexBuffer.Release();
                vertexBuffer = null;
            } 
            if (indexBuffer != null) {
                indexBuffer.Release();
                indexBuffer = null;
            }
            if (renderObjectBuffer != null) {
                renderObjectBuffer.Release();
                renderObjectBuffer = null;
            }

        }
    }
   
        
}
