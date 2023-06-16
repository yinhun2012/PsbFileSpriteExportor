using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class EditorPsbFileProcessor : EditorWindow
{
    [MenuItem("UnityTools/PSB文件处理", false, 24)]
    private static void ShowWindow()
    {
        EditorPsbFileProcessor win = GetWindow<EditorPsbFileProcessor>("PSB文件处理");
        win.Show();
    }

    private GameObject psbGo;

    private Object psbObj;
    private string spriteDir = "Assets/Sprite";

    private void OnGUI()
    {
        EditorGUILayout.LabelField("选择Canvas下的Psb GameObject");
        psbGo = (GameObject)EditorGUILayout.ObjectField(psbGo, typeof(GameObject), true);
        if (psbGo != null)
        {
            if (GUILayout.Button("转换成Image组件"))
            {
                RectTransform rectroot = psbGo.AddComponent<RectTransform>();

                for (int i = 0; i < rectroot.childCount; i++)
                {
                    RectTransform rect = rectroot.GetChild(i).gameObject.AddComponent<RectTransform>();

                    SpriteRenderer spriteRenderer = rect.GetComponent<SpriteRenderer>();

                    rect.gameObject.AddComponent<Image>().sprite = spriteRenderer.sprite;

                    rect.sizeDelta *= 100;

                    rect.anchoredPosition *= 100;

                    SpriteRenderer.DestroyImmediate(spriteRenderer);
                }
            }
        }
        EditorGUILayout.LabelField("选择Psb文件夹下的Psb File，Read/Write=Enable");
        psbObj = EditorGUILayout.ObjectField(psbObj, typeof(Object), false);
        if (psbObj != null)
        {
            EditorGUILayout.LabelField("保存Sprites的文件夹，设置TextureType=Sprite");
            spriteDir = EditorGUILayout.TextField(spriteDir);
            if (GUILayout.Button("提取Psb.Sprites"))
            {
                Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(psbObj));

                foreach (Object obj in objs)
                {
                    if (obj is Sprite)
                    {
                        Sprite sprite = obj as Sprite;

                        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.ARGB32, false);
                        Color[] spritePixels = sprite.texture.GetPixels((int)sprite.rect.x,
                                                                        (int)sprite.rect.y,
                                                                        (int)sprite.rect.width,
                                                                        (int)sprite.rect.height);
                        texture.SetPixels(spritePixels);
                        texture.Apply();

                        byte[] bytes = texture.EncodeToPNG();

                        File.WriteAllBytes(string.Format("{0}/{1}.png", spriteDir, sprite.name), bytes);
                    }
                }
                AssetDatabase.Refresh();
            }
        }
    }
}