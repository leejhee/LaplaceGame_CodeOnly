using System;
using UnityEngine;

namespace Client
{
    public class BattleTestScene : MonoBehaviour
    {
        private void Start()
        {
            GameManager instance = GameManager.Instance;
            StageManager.Instance.Init();
        }

        [ContextMenu("베팅 없이 전투 시작")]
        public void CombatTest()
        {
            CharManager.Instance.WakeAllCharAI();
        }
    }
}