using UnityEngine;
using UnityEngine.Playables;
using System;

namespace Client
{
    /// <summary>
    /// 스킬 객체
    /// </summary>
    [RequireComponent(typeof(SkillMarkerReceiver))]
    public class SkillBase : MonoBehaviour
    {
        private PlayableDirector _PlayableDirector;
        private CharBase _caster;

        private SkillData _skillData;
        
        private int _nSkillRange;
        private SystemEnum.eSkillTargetType _targetType;

        private int _skillPlayCount;
        private SkillParameter _inputParameter;
        public CharBase CharPlayer => _caster;
        public PlayableDirector Director => _PlayableDirector;
        public SkillParameter InputParameter
        {
            get => _inputParameter;
            private set
            {
                _inputParameter = value;
                _inputParameter.SkillUseCount = _skillPlayCount;
            }
        }

        public SkillAIInfo GetAIInfo() => new(){ TargetType = _targetType, Range = _nSkillRange };
        
        public void SetCharBase(CharBase caster)
        {
            _caster = caster;
        }

        // 추후 데이터 포함할 방법이 있다면 반드시 이 코드 교체할 것
        public void Init(SkillData data)
        {
            _skillData = data;
            _nSkillRange = data.skillRange;
            _targetType = data.skillTarget;
            _skillPlayCount = 0;

            _PlayableDirector.played += _ => _caster.CharAI.IsSkillPlaying = true;
            _PlayableDirector.stopped += _ =>  _caster.CharAI.IsSkillPlaying = false;
            
            _PlayableDirector.stopped += OnSkillTimelineEnd;
        }

        public event Action OnSkillEnd;

        private void OnSkillTimelineEnd(PlayableDirector director)
        {
            OnSkillEnd?.Invoke();
        }
        
        public void ResetSkill()
        {
            Init(_skillData);
        }
        
        public void SetRange(int value, bool delta=false)
        {
            if (_nSkillRange == 0) 
                return;

            if (delta)
                _nSkillRange += value;
            else
                _nSkillRange = value;
        }

        private void Awake()
        {
            _PlayableDirector = GetComponent<PlayableDirector>();
        }
    
        public void PlaySkill(SkillParameter parameter)
        {
            if (!_PlayableDirector)
                return;
            
            InputParameter = parameter;
            _PlayableDirector.Play();
            _skillPlayCount++;
        }
    }
}