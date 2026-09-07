using UnityEngine;

namespace Client
{
    // 롤체 레퍼상 마나 얻는 타이밍이 따로 있어서 마커로 처리함
    public class SkillMarkerManaExchanger : SkillTimeLineMarker
    {
        private enum choice { Earn, Pay }

        [SerializeField] 
        choice Choice;

        [SerializeField, Header("Pay일 경우, 그냥 두셔도 됩니다. 알아서 계산됨")] 
        int amount = 5;

        public override void MarkerAction()
        {
            base.MarkerAction();
           
            var stat = SkillParam.skillCaster.CharStat;
            if (Choice == choice.Earn)
            {
                stat.GainMana(amount, true);
                Debug.Log(@$"{SkillParam.skillCaster.GetID()}번 캐릭터에서 평타로 {amount}마나 획득
현재 마나 {stat.GetStat(SystemEnum.eStats.N_MANA)}");
            }
            else if(Choice == choice.Pay)
            {
                amount = (int)stat.GetStat(SystemEnum.eStats.MAX_MANA);
                stat.GainMana(amount, false);
                Debug.Log(@$"{SkillParam.skillCaster.GetID()}번 캐릭터에서 스킬로 {amount}마나 소비
현재 마나 {stat.GetStat(SystemEnum.eStats.N_MANA)}");
            }

        }

    }
}