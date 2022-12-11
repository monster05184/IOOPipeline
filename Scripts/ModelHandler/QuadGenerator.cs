using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace IOOPipeline
{
   public class QuadGenerator
   {
      struct Triangle
      {
         public int v1;
         public int v2;
         public int v3;
         public bool merged ;

         public Triangle(int v1, int v2, int v3)
         {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            merged = false;
         }
      }

      private List<Triangle> triangles = new List<Triangle>();
      

      public int[] MergeToQuad(int[] indexs)
      {
         for (int i = 0; i < indexs.Length; i+=3)
         {
            Triangle newTriangle = new Triangle(indexs[i], indexs[i + 1], indexs[i + 2]);
            triangles.Add(newTriangle);
         }

         List<int> resultIndex = new List<int>();
         for (int i = 0; i < triangles.Count; i++)
         {
            var triSource = triangles[i];
            if(triSource.merged) continue;
            bool found = false;
            for (int j = i + 1; j < triangles.Count; j++)
            {
               var triTarget = triangles[j];
               if(triTarget.merged) continue;
               //将所有的不重复的点加入数组。
               var quadIndexList = new List<int>();
               tryAddListUnique(quadIndexList , triSource.v1);
               tryAddListUnique(quadIndexList , triSource.v2);
               tryAddListUnique(quadIndexList , triSource.v3);
               tryAddListUnique(quadIndexList , triTarget.v1);
               tryAddListUnique(quadIndexList , triTarget.v2);
               tryAddListUnique(quadIndexList , triTarget.v3);
               if(quadIndexList.Count != 8) continue;
               
               
               triSource.merged = true;
               triTarget.merged = true;
               triangles[i] = triSource;
               triangles[j] = triTarget;
               
               
               resultIndex.Add(triSource.v1);
               if (quadIndexList[5] == 0) resultIndex.Add(quadIndexList[6]);
               resultIndex.Add(triSource.v2);
               if (quadIndexList[1] == 0) resultIndex.Add(quadIndexList[6]);
               resultIndex.Add(triSource.v3);
               if (quadIndexList[3] == 0) resultIndex.Add(quadIndexList[6]);
               found = true;

            }
         }

         return resultIndex.ToArray();

      }

      public void tryAddListUnique(List<int> quadIndexList, int triPoint)
      {
         for (int k = 0; k <= quadIndexList.Count / 2; k++)
         {
            if (quadIndexList.Count == 0 || k == quadIndexList.Count / 2)
            {
               quadIndexList.Add(triPoint);
               quadIndexList.Add(0);
               break;
            }
            else
            {
               if (quadIndexList[k * 2] == triPoint)
               {
                  quadIndexList[k * 2 + 1] = 1; 
                  break;
               }
            }


            
         }
      }
   }

}
