using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Client
{
    // GameScene 안에 들어갈 UI 총체.
    // [TODO] : UI 묶어줄 개체에 대한 타입 정의 생각할 것.(캔버스 공통으로 가짐. setcanvas 써도 된다.)
    // [TODO] : 해당 타입에 대해 해상도 대응 생각할 것.
    public class UI_GameScene : MonoBehaviour
    {
        // GameScene 안에 들어가는 UI 프리팹 전부 넣어줄것.
        [SerializeField] List<GameObject> Prefabs = new List<GameObject>();

        private void Awake()
        {
            foreach(GameObject prefab in Prefabs)
            {
                ObjectManager.Instance.Instantiate(prefab, transform);
            }
        }

    }
}