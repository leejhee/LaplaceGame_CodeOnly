using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
namespace Client
{
    public class DataTool
    {
        [MenuItem("Data/데이터검증")]
        public static void DataVerification()
        {
            DataManager.Instance.ClearCache();
            DataManager.Instance.DataLoad();

            Debug.Log("데이터 검증 끝.");
        }

        //[MenuItem("Data/Verify Dialougue")]
        //public static void DialogueDataVerification()
        //{
        //    //dialougedata의 모든 레코드 기본적으로 순회하기
        //    DataManager.Instance.DataLoad();
        //    var DialogueDict = DataManager.Instance.GetDictionary("DialogueData");

        //    foreach(var kvp in DialogueDict )
        //    {
        //        //Debug.Log(kvp.Key);
        //        //Debug.Log(DataManager.Instance.GetData<StringData>(kvp.Key).stringKor);

        //    }            

        //}
    }
}
#endif
