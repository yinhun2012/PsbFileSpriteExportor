using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class EditorPsbFileProcessor : EditorWindow
{
    [MenuItem("UnityTools/PSB�ļ�����", false, 24)]
    private static void ShowWindow()
    {
        EditorPsbFileProcessor win = GetWindow<EditorPsbFileProcessor>("PSB�ļ�����");
        win.Show();
    }

    private string appAssetsDir;

    private void OnEnable()
    {
        appAssetsDir = Application.dataPath;
    }

    private GameObject psbGo;

    private Object psbObj;
    private string spriteDir = "Sprite";

    private string psbsDir = "Psbs";
    private string spritesDir = "Sprites";

    private void OnGUI()
    {
        EditorGUILayout.LabelField("ѡ��Canvas�µ�Psb GameObject");
        psbGo = (GameObject)EditorGUILayout.ObjectField(psbGo, typeof(GameObject), true);
        if (psbGo != null)
        {
            if (GUILayout.Button("ת����Image���"))
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
        EditorGUILayout.LabelField("ѡ��Psb�ļ����µ�Psb File��Read/Write=Enable");
        psbObj = EditorGUILayout.ObjectField(psbObj, typeof(Object), false);
        if (psbObj != null)
        {
            EditorGUILayout.LabelField("����Sprites���ļ��У�����TextureType=Sprite");
            spriteDir = EditorGUILayout.TextField(spriteDir);
            if (GUILayout.Button("��ȡPsb.Sprites"))
            {
                string dirpath = string.Format("{0}/{1}", appAssetsDir, spriteDir);

                DeleteDirectory(dirpath, true);

                CreateDirectory(dirpath);

                string assetpath = AssetDatabase.GetAssetPath(psbObj);

                Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetpath);

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

                        try
                        {
                            string filepath = string.Format("{0}/{1}.png", dirpath, sprite.name);
                            File.WriteAllBytes(filepath, bytes);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogErrorFormat("δ����Sprite�ɹ���spr.name = {0} ex = {1}", sprite.name, ex);
                        }
                    }
                }
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.LabelField("��ȡPsbs�ļ���������Psbs.Files");
        EditorGUILayout.LabelField("д��Psbs.Files.Sprites��Sprites�ļ���");
        if (GUILayout.Button("��ȡ����Psb.Files.Sprites"))
        {
            string psbsdir = string.Format("{0}/{1}", appAssetsDir, psbsDir);
            List<string> psbfilelist = GetExtensionFile(psbsdir, psbExtens);
            for (int i = 0; i < psbfilelist.Count; i++)
            {
                string psbfile = psbfilelist[i];

                string dirpath = string.Format("{0}/{1}/{2}", appAssetsDir, spritesDir, Path.GetFileNameWithoutExtension(psbfile));
                DeleteDirectory(dirpath, true);
                CreateDirectory(dirpath);

                string assetpath = "Assets/" + psbfile.Replace(appAssetsDir, string.Empty);
                Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetpath);
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

                        try
                        {
                            string filepath = string.Format("{0}/{1}.png", dirpath, sprite.name);
                            File.WriteAllBytes(filepath, bytes);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogErrorFormat("δ����Sprite�ɹ���spr.name = {0} ex = {1}", sprite.name, ex);
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }



    public bool CreateDirectory(string dirPath)
    {
        try
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return true;
        }
        catch (System.Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogError(ex.Message);
#endif
        }
#if UNITY_EDITOR
        Debug.LogErrorFormat("CreateDirectory failed dirPath = {0}", dirPath);
#endif
        return false;
    }

    public bool DeleteDirectory(string dirpath, bool recursive)
    {
        try
        {
            if (Directory.Exists(dirpath))
            {
                Directory.Delete(dirpath, recursive);
            }
            return true;
        }
        catch (System.Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogErrorFormat("DeleteDirectory error dirpath = {0} ex = {1}", dirpath, ex);
#endif
        }
        return false;
    }

    private string[] psbExtens = new string[] { ".psb" };

    public List<string> GetExtensionFile(string dirPath, string[] exts)
    {
        List<string> list = new List<string>();
        string[] totalFiles = Directory.GetFiles(dirPath);
        for (int i = 0; i < totalFiles.Length; i++)
        {
            for (int k = 0; k < exts.Length; k++)
            {
                if (Path.GetExtension(totalFiles[i]) == exts[k])
                {
                    list.Add(totalFiles[i]);
                    break;
                }
            }
        }
        return list;
    }
}