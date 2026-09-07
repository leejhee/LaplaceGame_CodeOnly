using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class GameSceneMap : MonoBehaviour
    {
        [SerializeField] GameObject MapHUD;

        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }

        // 인게임 로직에 따라 맵 UI 갱신하도록 작성할 것.
        // 아마 카메라 따로 필요할거임.
    }
}