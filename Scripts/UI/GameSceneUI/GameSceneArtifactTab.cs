using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Client
{
    public class GameSceneArtifactTab : MonoBehaviour
    {
        [SerializeField] Transform ArtifactPanel;

        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }

        private void Start()
        {
            SetArtifact();
        }

        void SetArtifact()
        {
            // 아티팩트 리스트에 변화 생길 시 호출,
            // UI 요소 업데이트.
        }
    }
}