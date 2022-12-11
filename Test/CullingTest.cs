using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IOOPipeline
{
    public class CullingTest : MonoBehaviour
    {
        public int interval = 1;
        public Color testColor = Color.blue;
        public void TestResourse()
        {
            Quaternion quaternion = Quaternion.identity;
            Vector3 scale = new Vector3(1, 1, 1);
            RenderObjectData renderObjectData = ScriptableObject.CreateInstance<RenderObjectData>();
            RenderObjectData.ObjectInfo[] renderObjects = new RenderObjectData.ObjectInfo[1000];
            renderObjectData.renderObjectCount = 1000;
            ClusterData clusterData = ScriptableObject.CreateInstance<ClusterData>();
            ClusterData.ClusterInfo[] clusterInfos = new ClusterData.ClusterInfo[1000];
            clusterData.clusterCount = 1000;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        Vector3 pos = new Vector3(i * interval, j * interval, k * interval);

                        Matrix4x4 local2WorldMatrix = Matrix4x4.TRS(pos, quaternion, scale);

                        renderObjects[i * 10 * 10 + j * 10 + k].materialProperty.mainColor = testColor;
                        renderObjects[i * 10 * 10 + j * 10 + k].local2WordMatrix = local2WorldMatrix;
                        clusterInfos[i * 10 * 10 + j * 10 + k].objIndex = i * 10 * 10 + j * 10 + k;
                        clusterInfos[i * 10 * 10 + j * 10 + k].bounds.extent = new Vector3(0.5f, 0.5f, 0.5f);
                        clusterInfos[i * 10 * 10 + j * 10 + k].bounds.center = pos;

                    }
                }
            }

            renderObjectData.renderObjects = renderObjects;
            clusterData.clusterInfos = clusterInfos;
            AssetDatabase.CreateAsset(clusterData, "Assets/Resourses/Cluster.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AssetDatabase.CreateAsset(renderObjectData, "Assets/Resourses/RenderObjects.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

}
