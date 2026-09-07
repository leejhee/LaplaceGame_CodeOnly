using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Client.SystemEnum;

namespace Client
{
    public class SkillInfoUI : MonoBehaviour
    {
        [SerializeField] Image IMG_Skill;
        [SerializeField] TextMeshProUGUI TMP_Skill;
        [SerializeField] TextMeshProUGUI TMP_SkillDesc;

        SkillData skillData;

        public void SetSkillData(SkillData skill)
        {
            skillData = skill;
        }
        public void DisplaySkillInfoUI(SkillData skillData)
        {
            Dictionary<eLocalize, string> nameDic = new();
            Dictionary<eLocalize, string> descDic = new();

            Debug.Log($" 스킬 이름 : {skillData.nameStringCode} 스킬 설명 코드 : {skillData.desStringCode}");
            TMP_Skill.text = DataManager.GetStringCode(skillData.nameStringCode);
            TMP_SkillDesc.text = DataManager.GetStringCode(skillData.desStringCode);

            /*
            if (DataManager.Instance.LocalizeStringCodeMap.TryGetValue(skillData.nameStringCode, out nameDic))
            {
                Debug.Log($"{skillData.nameStringCode} dictionary 존재");
                nameDic.TryGetValue(eLocalize.KOR, out string skillName);
                TMP_Skill.text = skillName;
            }
            else
            {
                Debug.Log($"{skillData.nameStringCode} dictionary 없음");
            }

            if(DataManager.Instance.LocalizeStringCodeMap.TryGetValue(skillData.desStringCode, out descDic))
            {
                Debug.Log($"{skillData.desStringCode} dictionary 존재");
                nameDic.TryGetValue(eLocalize.KOR, out string descName);
                TMP_SkillDesc.text = descName;
            }
            else
            {
                Debug.Log($"{skillData.desStringCode} dictionary 없음");
            }*/

            //IMG_Skill.sprite = ObjectManager.Instance.LoadSprite(skillData.imagePath);

        }
    }
}