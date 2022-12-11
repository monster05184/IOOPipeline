using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace IOOPipeline {
    [CustomEditor(typeof(MaterialPropertyData))]
    public class MaterialPropertyEditor : Editor
    {

        private Editor materialEditor;

        private Texture texture;

        private SerializedProperty materialPropertySP;

        private List<Texture> materialTextures;//本材质使用的各种贴图 临时

        private List<TextureList> textureWarehouse;//贴图仓库 需加载存储

        private List<SerializedObject> textureWarehouseSO;

        private List<SerializedProperty> texturesSP;

        private bool textureLoaded = false;

        private int objectId;
        
        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();
            FieldInfo[] materialFileds;
            Type materialType = typeof(MaterialPropertyData.MaterialProperty);
            materialFileds = materialType.GetFields();

            GUILayout.BeginVertical("box");
            int texIndex = -1;
            foreach (var field in materialFileds)
            {
                

                bool isTexture = false;
                foreach (Attribute a in field.GetCustomAttributes())
                {
                    if (a.GetType() == typeof(NormalTexture) || a.GetType() == typeof(TextureAsset))//检验是否为图片
                    {
                        isTexture = true;
                        texIndex++;
                    }
                }

                if (isTexture)
                {
                    HandleTexture(field, texIndex);
                }
                else
                {
                    HandleField(field);
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
            textureLoaded = true;
        }

        private void HandleField(FieldInfo field)//处理不是Texture的field
        {
            if (field.FieldType == typeof(Color))
            {
                GUILayout.BeginHorizontal();
                    GUILayout.Label("颜色：  " + field.Name);

                    using (SerializedProperty colorSP = materialPropertySP.FindPropertyRelative(field.Name))
                    {
                        colorSP.colorValue = EditorGUILayout.ColorField(colorSP.colorValue, GUILayout.Width(65));
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }

                    GUILayout.EndHorizontal();
                return;
            }

            if (field.FieldType == typeof(int))
            {
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Int：  " + field.Name);
                    
                    using (SerializedProperty intSP = materialPropertySP.FindPropertyRelative(field.Name))
                    {
                        intSP.intValue = EditorGUILayout.IntField(intSP.intValue, GUILayout.Width(65));
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }

                GUILayout.EndHorizontal();
                return;
            }
            if (field.FieldType == typeof(float))
            {
                GUILayout.BeginHorizontal();
                    GUILayout.Label("Float：  " + field.Name);
                
                    using (SerializedProperty floatSP = materialPropertySP.FindPropertyRelative(field.Name))
                    {
                        floatSP.floatValue = EditorGUILayout.FloatField(floatSP.floatValue, GUILayout.Width(65));
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }

                GUILayout.EndHorizontal();
                return;
            }
            //
            if (field.FieldType == typeof(Vector4))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("颜色：  " + field.Name);
                EditorGUILayout.Vector4Field("向量:   " + field.Name, Vector4.one, GUILayout.Width(65));
                GUILayout.EndHorizontal();
                return;
            }
            if (field.FieldType == typeof(int))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("颜色：  " + field.Name);
                EditorGUILayout.IntField(0, GUILayout.Width(65));
                GUILayout.EndHorizontal();
                return;
            }

            GUILayout.BeginHorizontal("box", GUILayout.Width(320));
            GUILayout.Label("存在不合法的数据类型");
            GUILayout.EndHorizontal();

        }

        private void HandleTexture(FieldInfo field, int texIndex)
        {

            if (!textureLoaded)
            {
                LoadTextureWithField(field);
            }
            //materialTextures[texIndex] = EditorGUILayout.ObjectField("贴图：  " + field.Name, materialTextures[texIndex], typeof(Texture), true) as Texture;
            texturesSP[texIndex].objectReferenceValue = EditorGUILayout.ObjectField("贴图：  " + field.Name, texturesSP[texIndex].objectReferenceValue, typeof(Texture), true) as Texture;
            GUILayout.Space(3);
            textureWarehouseSO[texIndex].ApplyModifiedProperties();
            textureWarehouseSO[texIndex].Update();
            

            string path = AssetDatabase.GetAssetPath( texturesSP[texIndex].objectReferenceValue );
            TextureImporter textureImporter;
            if (materialTextures[texIndex] != null)
            {
                textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter != null)
                {
                    foreach (Attribute a in field.GetCustomAttributes())
                    {
                        Type type = a.GetType();
                        if (a.GetType() == typeof(NormalTexture))
                        {
                            NormalTexture(textureImporter, path);
                        }

                        if (a.GetType() == typeof(sRGB))
                        {
                            sRGB sRGBattribute = a as sRGB;
                            FixGamma(textureImporter, path, sRGBattribute.on);
                        }

                        if (a.GetType() == typeof(TextureSize))
                        {
                            TextureSize textureSizeAttribute = a as TextureSize;
                            FixSize(textureImporter, path, textureSizeAttribute.size);
                        }
                    }
                }

                
            }
        }

        private void NormalTexture(TextureImporter textureImporter, string path)
        {
            if (textureImporter.textureType != TextureImporterType.NormalMap)
            {
                GUILayout.BeginHorizontal("box",GUILayout.Width(320));
                GUILayout.Label("这个图片的格式不为法线图");
                if (GUILayout.Button("fix"))
                {
                    TextureImporterSettings importSetting = new TextureImporterSettings();
                    textureImporter.ReadTextureSettings(importSetting);
                    importSetting.ApplyTextureType(TextureImporterType.NormalMap);
                    textureImporter.SetTextureSettings(importSetting);
                    AssetDatabase.ImportAsset(path);
                }
                
                GUILayout.EndHorizontal();
            }
        }

        private void FixSize(TextureImporter textureImporter, string path, TextureSize.SIZE size)
        {
            int maxSize = 0;
            switch (size)
            {
                case TextureSize.SIZE._512 :
                    maxSize = 512;
                    break;
                case TextureSize.SIZE._1024:
                    maxSize = 1024;
                    break;
                case TextureSize.SIZE._2048:
                    maxSize = 2048;
                    break;
                case TextureSize.SIZE._4096:
                    maxSize = 4096;
                    break;

            }
            maxSize = (int) size;

            if (!(textureImporter.maxTextureSize == maxSize))
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label("该图片的尺寸应该为" + maxSize + "，当前为" + textureImporter.maxTextureSize);
                if (GUILayout.Button("fix"))
                {
                    TextureImporterSettings importSetting = new TextureImporterSettings();
                    textureImporter.ReadTextureSettings(importSetting);
                    importSetting.maxTextureSize = maxSize;
                    textureImporter.SetTextureSettings(importSetting);
                    AssetDatabase.ImportAsset(path);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void FixGamma(TextureImporter textureImporter, string path, bool on)
        {
            if (textureImporter.sRGBTexture != on)
            {
                GUILayout.BeginHorizontal("box",GUILayout.Width(320));
                if (textureImporter.sRGBTexture == false)
                {
                    GUILayout.Label("该图片为线性空间图片，应该为gamma空间图片");
                }
                else
                {
                    GUILayout.Label("该图片为gamma空间图片，应当为线性空间图片");
                }

                if (GUILayout.Button("fix"))
                {
                    TextureImporterSettings importSetting = new TextureImporterSettings();
                    textureImporter.ReadTextureSettings(importSetting);
                    importSetting.sRGBTexture = on;
                    textureImporter.SetTextureSettings(importSetting);
                    AssetDatabase.ImportAsset(path);
                }

                GUILayout.EndHorizontal();
            }

        }

        private Texture LoadTextureWithField(FieldInfo field)
        {
            string path = "Assets/Resourses/Textures/" + SceneInfoHandler.GetSceneName() + "_" + field.Name + ".asset";
            TextureList textureList = AssetDatabase.LoadAssetAtPath<TextureList>(path);
            Texture2D thisTex = null;

            if (textureList == null)
            {
                
                textureList = ScriptableObject.CreateInstance<TextureList>();
                AssetDatabase.CreateAsset(textureList, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                thisTex = textureList.GetTextureAt(objectId);
            }
            else
            {
                thisTex = textureList.GetTextureAt(objectId);
            }



            SerializedObject so = new SerializedObject(textureList);
            textureWarehouseSO.Add(so);
            SerializedProperty sp = null;
            SerializedProperty tempSP = so.FindProperty("textures");
            if (objectId < tempSP.arraySize)
            {
                sp = tempSP.GetArrayElementAtIndex(objectId);
            }
            else
            {
                tempSP.InsertArrayElementAtIndex(objectId);
                sp = tempSP.GetArrayElementAtIndex(objectId);
            }


            texturesSP.Add(sp);
            textureWarehouse.Add(textureList);
            materialTextures.Add(thisTex);

            return thisTex;

        }
        



        private void OnDisable()
        {
           
            
        }

        private void OnEnable()
        {
            if (target != null)
            {
                materialPropertySP = serializedObject.FindProperty("materialProperty");
                MaterialPropertyData data = (MaterialPropertyData)target;
                objectId = data.renderObjectId;
            }

            materialTextures = new List<Texture>();
            textureWarehouse = new List<TextureList>();
            textureWarehouseSO = new List<SerializedObject>();
            texturesSP = new List<SerializedProperty>();

        }
        
    }
    
}
