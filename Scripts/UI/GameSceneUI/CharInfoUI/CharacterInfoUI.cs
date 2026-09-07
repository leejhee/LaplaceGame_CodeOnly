using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class CharacterInfoUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI TMP_Name;
        [SerializeField] Image IMG_NameTag;
        [SerializeField] Image IMG_CharPortrait;
        [SerializeField] Button BTN_SkillIcon;
        [SerializeField] SkillInfoUI skillInfoUI;

        [SerializeField] TextMeshProUGUI TMP_HP;
        [SerializeField] TextMeshProUGUI TMP_MANA;
        [SerializeField] TextMeshProUGUI TMP_Synergy;

        [SerializeField] List<TextMeshProUGUI> TMP_CharStatsList = new();
        
        SkillData currentSkill;

        readonly SystemEnum.eStats[][] statGroups = new SystemEnum.eStats[][]
        {
            new [] { eStats.NAD, eStats.NAP },
            new [] { eStats.NARMOR, eStats.NMAGIC_RESIST },
            new [] { eStats.NCRIT_CHANCE, eStats.NCRIT_DAMAGE },
            new [] { eStats.NAS, eStats.NRANGE },
        };
        readonly string[][] statLabels = new string[][]
        {
            new [] { "공격력", "주문력" },
            new [] { "방어력", "마법저항" },
            new [] { "치명타 확률", "치명타 피해" },
            new [] { "공격속도", "사거리" },
        };

        /// <summary> 캐릭터 기본 정보 띄우기 </summary>
        public void DisplayCharInfo(CharInfo charInfo)
        {
            skillInfoUI.gameObject.SetActive(false);
            DisplayCharData(charInfo.charBase.CharData);
            DisplayCharStat(charInfo.charBase.CharStat);
        }


        /// <summary> 전투 중에 계속 변경되는 스탯들 업데이트 </summary>
        void DisplayCharStat(CharStat charStat)
        {
            for (int i = 0; i < TMP_CharStatsList.Count; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < statGroups[i].Length; j++)
                {
                    float value = charStat.GetStat(statGroups[i][j]);
                    string label = statLabels[i][j];

                    if (statGroups[i][j] is eStats.NCRIT_CHANCE or eStats.NCRIT_DAMAGE)
                        value *= 100f;
                    // 임시 >> 소수점 뒤는 선택적으로 값이 있으면 출력
                    sb.AppendLine($"{label} {value:0.##}" +
                        (statGroups[i][j] is eStats.NCRIT_CHANCE or eStats.NCRIT_DAMAGE ? "%" : ""));
                }
                TMP_CharStatsList[i].text = sb.ToString();
            }
        }

        // 스탯 바뀔 때마다 업데이트 호출 어떻게 할지 아직 안 정함
        void DisplayCharStat1(CharStat charStat)
        {
            StringBuilder sb1 = new();
            StringBuilder sb2 = new();
            StringBuilder sb3 = new();
            StringBuilder sb4 = new();

            sb1.AppendLine($"공격력 {charStat.GetStat(SystemEnum.eStats.NAD)} ");
            sb1.AppendLine($"주문력 {charStat.GetStat(SystemEnum.eStats.NAP)}");

            sb2.AppendLine($"방어력 {charStat.GetStat(SystemEnum.eStats.NARMOR)}");
            sb2.AppendLine($"마법저항 {charStat.GetStat(SystemEnum.eStats.NMAGIC_RESIST)}");

            sb3.AppendLine($"치명타 확률 {charStat.GetStat(SystemEnum.eStats.NCRIT_CHANCE) * 100}%");
            sb3.AppendLine($"치명타 피해 {charStat.GetStat(SystemEnum.eStats.NCRIT_DAMAGE) * 100}%"); // percentage 처리

            sb4.AppendLine($"공격속도 {charStat.GetStat(SystemEnum.eStats.NAS)}");
            sb4.AppendLine($"사거리 {charStat.GetStat(SystemEnum.eStats.NRANGE)}");

        }

        void DisplayCharData(CharData charData)
        {
            // 이름 초상화 시너지 스킬 정보

            TMP_Name.text = charData.charKorName;

            // TODO : 캐릭터 초상화 설정
            //IMG_CharPortrait.sprite = ObjectManager.Instance.LoadSprite(charData.imagePath);

            // TODO : 스킬 이미지 설정
            currentSkill = DataManager.Instance.GetData<SkillData>(charData.skill2[0]);
            //BTN_SkillIcon.image.sprite = ObjectManager.Instance.LoadSprite(currentSkill.imagePath);
            SetupSkillButton();


            // 티어 확인 -> TierColorData -> hexColorForItemDes
            SystemEnum.eTier tier = charData.charTier;
            string tierColorCode;
            DataManager.Instance.TierColorDataMap.TryGetValue(tier, out tierColorCode);

            // 이름표 색상 티어별로 설정
            IMG_NameTag.color = Util.GetHexColor(tierColorCode);

            TMP_Synergy.text = $"{charData.synergy1} {charData.synergy2} {charData.synergy3}";
        }

        void SetupSkillButton()
        {
            EventTrigger trigger = BTN_SkillIcon.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = BTN_SkillIcon.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry rightClick = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            rightClick.callback.AddListener((data) =>
            {
                if (((PointerEventData)data).button == PointerEventData.InputButton.Right)
                {
                    Debug.Log("스킬 아이콘 우클릭 누름");
                    skillInfoUI.gameObject.SetActive(true);
                    skillInfoUI.DisplaySkillInfoUI(currentSkill);
                }
            });

            trigger.triggers.Add(rightClick);
        }
    }
}