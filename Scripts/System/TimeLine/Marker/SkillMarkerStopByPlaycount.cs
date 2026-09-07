using UnityEngine;

namespace Client
{
    public class SkillMarkerStopByPlayCount : SkillTimeLineMarker
    {
        [SerializeField] private int passCount = 0;
        public override void MarkerAction()
        {
            if (SkillParam.SkillUseCount < passCount)
            {
                Debug.Log($"게임 내 사용 횟수 {SkillParam.SkillUseCount}번이 {passCount}보다 낮으므로 중단");
                Director.Stop();
                return;
            }

            Debug.Log($"{SkillParam.SkillUseCount}번 재생 기록으로 속행함");
        }
    }
}