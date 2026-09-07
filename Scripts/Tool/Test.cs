using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;


#if UNITY_EDITOR
using UnityEditor;

public class Test : Editor
{
    [MenuItem("Test/Test1")]
    public static void Test1()
    {
        GameSceneMessageParam tmp = new();
        tmp.message = "public class Test:Editor 에서 브로드캐스팅한 메시지입니다.";
        MessageManager.SendMessage<GameSceneMessageParam>(tmp);
    }

    [MenuItem("Test/Test2")]
    public static void Test2()
    {

    }

    [MenuItem("Test/Test3")]
    public static void Test3()
    {

    }
}
#endif
