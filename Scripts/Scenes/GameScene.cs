using System.Collections;
using UnityEngine;
using static Client.SystemEnum;


namespace Client
{
    /// <summary>
    /// Scene 초기화 class
    /// </summary>
    public class GameScene : MonoBehaviour
    {
        [SerializeField] GameObject GameSceneUIPrefab;

        [Header("Test(shootindex의 경우 편하게 하려고 임의로 이렇게 함)")]
        [SerializeField] private CharBase TestChar;
        [SerializeField] private long testPlayerIndex;
        [SerializeField] private int testPlayerTile;
        
        [SerializeField] private CharBase TestEnemy;
        [SerializeField] private long enemyPlayerIndex;
        [SerializeField] private int testEnemyTile;
        [SerializeField] private long projectileShootIndex;

        private void Awake()
        {
            GameManager instance = GameManager.Instance;
            StageManager.Instance.Init();
        }

        private IEnumerator Start()
        {
            UIManager.Instance.ShowUI(GameSceneUIPrefab);
            // TileObj.Start가 타일을 등록한 뒤 첫 라운드 편성을 배치한다.
            yield return null;
            StageManager.Instance.StartNewRun();
            TestChar = CharManager.Instance.GetOneSide(eCharType.ALLY)?.Find(c => c);
            TestEnemy = CharManager.Instance.GetOneSide(eCharType.ENEMY)?.Find(c => c);
            MessageManager.SubscribeMessage<GameSceneMessageParam>(this, TestMessageSystem);
        }

        private void OnDestroy() => MessageManager.RemoveMessageAll(this);

        [ContextMenu("투사체를 날려보아요")]
        private void TestProjectileShoot()
        {
            TestChar.CharAction.CharAttackAction
                (new CharAttackParameter(TestEnemy, projectileShootIndex, CharAI.eAttackMode.None));
        }
        
        [ContextMenu("전투를 시작해보아요")]
        public void TestAIInitialize()
        {
            StageManager.Instance.StartCombat();
        }

        [ContextMenu("시너지 조회")]
        public void TestShowSynergy()
        {
            SynergyManager.Instance.ShowCurrentSynergies();
        }
        
        [ContextMenu("스킬 연속사용 테스트")]
        public void TestSkillChain()
        {
            //엠마의 스킬이 연쇄된 상태에서 사용해본다.
            TestChar.CharAI.TestSkillAction();
        }

        public void TestMessageSystem(GameSceneMessageParam param)
        {
            Debug.Log($"GameScene 에서 수신 완료 메시지 내용은 - {param.message} - ");
        }
    }
}
