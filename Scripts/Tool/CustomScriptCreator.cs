using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class CustomScriptCreator
{
    // Project View에서 우클릭 메뉴 추가
    [MenuItem("Assets/Create/Custom C# Script with Namespace", false, 80)]
    public static void CreateCustomCSharpScript()
    {
        string path = GetSelectedPathOrFallback();
        string scriptName = "NewCustomScript.cs";
        string fullPath = Path.Combine(path, scriptName);

        // 스크립트 내용 생성
        string scriptContent =
@"using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class NewCustomScript
    {
        
    }
}";

        // 파일 생성
        File.WriteAllText(fullPath, scriptContent);
        AssetDatabase.Refresh();

        // 새로 만든 스크립트 선택
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
        Selection.activeObject = obj;
    }

    // 선택된 폴더의 경로를 가져오거나, 선택되지 않았다면 기본 경로를 반환
    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        if (Selection.activeObject != null)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
        }

        return path;
    }
}
#endif