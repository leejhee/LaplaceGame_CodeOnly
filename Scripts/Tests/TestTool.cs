using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;

#if UNITY_EDITOR
using UnityEditor;
// 테스트 코드 머지 하지말고 로컬 테스트용
 public class TestTool : Editor
{
    [MenuItem("TestTool/Test1")]
    public static void Test1()
    {


    }

    [MenuItem("TestTool/Test2")]
    public static void Test2()
    {
        System.GC.Collect();
    }

    [MenuItem("TestTool/Test3")]
    public static void Test3()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
#endif
