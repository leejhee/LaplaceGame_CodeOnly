using UnityEditor;
using UnityEngine;

public class AnimationExtractor : Editor
{
    [MenuItem("Tools/Extract Animations From FBX")]
    public static void ExtractAnimations()
    {
        // 선택된 FBX 파일 가져오기
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (!assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning($"{obj.name} is not an FBX file.");
                continue;
            }

            // FBX 파일 내부의 에셋 불러오기
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            string folderPath = System.IO.Path.GetDirectoryName(assetPath);
            string fbxName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            foreach (Object asset in assets)
            {
                if (asset is AnimationClip animationClip)
                {
                    // 애니메이션 클립 추출
                    string clipPath = $"{folderPath}/{fbxName}_{animationClip.name}.anim";
                    AnimationClip newClip = Object.Instantiate(animationClip);
                    AssetDatabase.CreateAsset(newClip, clipPath);
                    Debug.Log($"Extracted {animationClip.name} to {clipPath}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Animation extraction completed.");
    }
}
