using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class GameSceneSynergyTab : MonoBehaviour
    {
        [SerializeField] private SynergyUnitUI synergyUnitPrefab;
        [SerializeField] private Transform teamAGridTransform;
        [SerializeField] private Transform teamBGridTransform;

        private readonly List<SynergyUnitUI> _teamARows = new();
        private readonly List<SynergyUnitUI> _teamBRows = new();

        private void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }

        private void OnEnable()
        {
            MessageManager.SubscribeMessage<OnSynergyChange>(this, SynergyUpdate);
            // 패널이 배치 완료 이후 생성되거나 다시 열려도 현재 스테이지를 표시한다.
            RefreshTeam(eCharType.ALLY, SynergyManager.Instance.GetTeamSynergies(eCharType.ALLY));
            RefreshTeam(eCharType.ENEMY, SynergyManager.Instance.GetTeamSynergies(eCharType.ENEMY));
        }

        private void OnDisable()
        {
            MessageManager.RemoveMessageAll(this);
        }

        private void SynergyUpdate(OnSynergyChange change)
        {
            RefreshTeam(change.Team, change.Synergy);
        }

        private void RefreshTeam(eCharType team, IReadOnlyList<SynergyDisplayInfo> synergies)
        {
            if (team != eCharType.ALLY && team != eCharType.ENEMY) return;

            var parent = team == eCharType.ALLY ? teamAGridTransform : teamBGridTransform;
            var rows = team == eCharType.ALLY ? _teamARows : _teamBRows;
            for (int i = 0; i < synergies.Count; i++)
            {
                if (i == rows.Count)
                    rows.Add(Instantiate(synergyUnitPrefab, parent, false));

                rows[i].SetData(synergies[i]);
                rows[i].gameObject.SetActive(true);
            }

            // 다음 스테이지에서 줄어든 행은 숨기고 이후 배치에서 재사용한다.
            for (int i = synergies.Count; i < rows.Count; i++)
                rows[i].gameObject.SetActive(false);
        }
    }
}
