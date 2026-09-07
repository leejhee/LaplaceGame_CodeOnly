using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public static class SkillCreator
    {
        public static SkillBase CreateSkill(string skillName)
        {
            SkillBase skillBase = null;
            skillBase = ObjectManager.Instance.Instantiate<SkillBase>($"Skill/{skillName}");
            return skillBase;
        }
        public static SkillBase CreateSkill(long skillIndex)
        {
            SkillBase skillBase = null;
            var _skillData = DataManager.Instance.GetData<SkillData>(skillIndex);

            if (_skillData == null)
            {
                Debug.LogWarning($"CreateSkill : {skillIndex} 스킬 생성 실패.");
                return null;
            }

            skillBase = ObjectManager.Instance.Instantiate<SkillBase>($"Skill/{_skillData.animPath}");
            skillBase.Init(_skillData);

            if (_skillData == null)
            {
                Debug.LogWarning($"CreateSkill : {skillIndex} 스킬 생성 실패.");
            }
            return skillBase;
        }
    }
}