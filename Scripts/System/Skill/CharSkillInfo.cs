using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 캐릭터 스킬 정보 (캐릭터 Start시점에 생성)
    /// </summary>
    public class CharSKillInfo
    {
        private Dictionary<long, SkillBase> _dicSkill = new(); // 스킬 리스트
        private CharBase _charBase; // 스킬 시전자
        private Transform _SkillRoot; // 스킬 루트 
        
        private long _currentAutoAttack;
        private long _currentSkill;

        private readonly long _defaultAutoAttack;
        private readonly long _defaultSkill;
        
        public Dictionary<long, SkillBase> DicSkill => _dicSkill; // 스킬 리스트

        public long AutoAttack => _currentAutoAttack;
        public long Skill => _currentSkill;
        
        public CharSKillInfo(CharBase charBase)
        {
            _charBase = charBase;
            _currentAutoAttack = _defaultAutoAttack = charBase.CharData.skill1;
            if (charBase.CharData.skill2.Count == 0 || charBase.CharData.skill2 == null)
                return;
            _currentSkill = _defaultSkill = charBase.CharData.skill2[0];
        }

        /// <summary>
        /// Char Start 시점
        /// </summary>
        /// <param name="skillArray"></param>
        public void Init(long aaSkill, List<long> skillArray)
        {
            // 스킬 루트 오브젝트 제작
            string SkillRoot = "SkillRoot";
            GameObject skillRoot = Util.FindChild(_charBase.gameObject, SkillRoot, false);
            if (!skillRoot)
            {
                skillRoot = new GameObject(SkillRoot);
                skillRoot.transform.SetParent(_charBase.transform, false);
            }
            _SkillRoot = skillRoot.transform;
            
            AddSkill(aaSkill);
            // 스킬 추가
            for (int i = 0; i < skillArray.Count; i++)
            {
                AddSkill(skillArray[i]);
            }
        }

        public void ChangeSkill(CharAI.eAttackMode changingMode, long changingIndex)
        {
            AddSkill(changingIndex);
            if(changingMode == CharAI.eAttackMode.Auto)
                _currentAutoAttack = changingIndex;
            else if(changingMode == CharAI.eAttackMode.Skill)
                _currentSkill = changingIndex;
        }

        public void ResetSkill()
        {
            _currentSkill = _defaultSkill;
            _currentAutoAttack = _defaultAutoAttack;
        }
        
        private void AddSkill(long skillIndex)
        {
            if (_dicSkill == null || skillIndex == SystemConst.NO_CONTENT)
                return;
            
            if (!_dicSkill.ContainsKey(skillIndex))
            {
                SkillBase skillBase = SkillCreator.CreateSkill(skillIndex);
                if (!skillBase)
                    return;
                skillBase.SetCharBase(_charBase);
                _dicSkill.Add(skillIndex, skillBase);
                skillBase.transform.parent = _SkillRoot;
            }
        }
        
        public void PlaySkill(long skillIndex, SkillParameter parameter)
        {
            if (_dicSkill == null)
                return;

            if (_dicSkill.ContainsKey(skillIndex))
            {
                _dicSkill[skillIndex].PlaySkill(parameter);
            }
        }

        public void PlayByMode(CharAI.eAttackMode mode, SkillParameter parameter)
        {
            if (mode == CharAI.eAttackMode.Auto)
            {
                PlaySkill(_currentAutoAttack, parameter);
            }
            else if (mode == CharAI.eAttackMode.Skill)
            {
                PlaySkill(_currentSkill, parameter);
                Debug.Log($"{_currentSkill} playing skill");
            }
        }
        
        // 시전자 의존성 없앨 거면 여기 참고할 것
        public void PlayByModeOnly(CharAI.eAttackMode mode)
        {
            long playing = mode switch
            {
                CharAI.eAttackMode.Skill => _currentSkill,
                CharAI.eAttackMode.Auto => _currentAutoAttack,
                _ => 0
            };
            if (!_dicSkill.ContainsKey(playing)) return;
            var info = _dicSkill[playing].GetAIInfo();
            var targets = TargetStrategyFactory.CreateTargetStrategy(new TargettingStrategyParameter()
            {
                Caster = _charBase, type = info.TargetType
            }).GetTargets();
            Debug.Log($"{targets.Count} targets remaining");
            PlaySkill(playing, new SkillParameter(targets, _charBase));
        }
        
        public SkillAIInfo GetInfoByMode(CharAI.eAttackMode mode)
        {
            long playing = mode switch
            {
                CharAI.eAttackMode.Skill => _currentSkill,
                CharAI.eAttackMode.Auto => _currentAutoAttack,
                _ => 0
            };
            return playing == 0 ? default : _dicSkill[playing].GetAIInfo();
        }

		public void SubscribeSkillEnd(CharAI.eAttackMode attackMode, Action onSkillEnd) 
            => _dicSkill[_currentAutoAttack].OnSkillEnd += onSkillEnd;

        public void UnsubscribeSkillEnd(CharAI.eAttackMode attackMode, Action onSkillEnd)
            => _dicSkill[_currentAutoAttack].OnSkillEnd -= onSkillEnd;
        
        
        #if UNITY_EDITOR
        public SkillAIInfo GetInfoByIndex(int index)
        {
            if (!_dicSkill.ContainsKey(index))
            {
                Debug.LogError($"해당 캐릭터 : {_charBase.GetID()}번 캐릭터 {_charBase.name}에 {index}번 스킬은 없습니다.");
                return default;
            }
            return _dicSkill[index].GetAIInfo();
        }
        #endif
        
        
        
    }

    public struct SkillAIInfo
    {
        public SystemEnum.eSkillTargetType TargetType;
        public int Range;
    }
}
