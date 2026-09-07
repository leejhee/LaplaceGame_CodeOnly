using System;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] 
        private long _index;
        
        private Collider        _projectileCollider;
        private Transform       _projectileTransform;
        private CharBase        _caster;
        private CharBase        _target;
        private eCharType       _targetType;
        
        private ProjectileData _projectileData;
        private float _projectileDamageInput;
        private List<FunctionData> _functionDataList;
        private int _projectileLife; // 투사체의 관통 허용 수치
        
        private GameObject _effectPrefab;


        private bool _shouldSpawnEffect = false;
        private bool destroyFlag = false;
        public void SetDestroyFlag(bool flag) => destroyFlag = flag;

        private IPathStrategy       pathStrategy;       // 경로 이동 방식
        private IRangeStrategy      rangeStrategy;      // 위치 도달 후 범위 처리

        public Collider Collider => _projectileCollider;
        public Transform ProjectileTransform => _projectileTransform;
        public CharBase Caster => _caster;
        public CharBase Target {
            get => _target;
            set
            {
                _target = value;
                if(_target)
                    _targetType = _target.GetCharType();
            }
        }
        public IPathStrategy PathGuide => pathStrategy;
        public IRangeStrategy RangeGuide => rangeStrategy;

        private void Awake()
        {
            _projectileTransform = transform;
            _projectileCollider = GetComponent<Collider>();
            _projectileData = DataManager.Instance.GetData<ProjectileData>(_index);
        }

        #region Initiallize Projectile(Call from Outside)
        public void InitProjectile(StatPackedSkillParameter param)
        {
            _caster = param.skillCaster;
            _target = param.skillTargets[param.TargetIndex];
            _targetType = _target.GetCharType();
            if(_projectileData == null)
            {
                Debug.LogError("이 데이터 없는거라는데요? 인덱스 제대로 확인바람");
                return;
            }
            _projectileDamageInput =
                (param.Percent / SystemConst.PER_CENT) * _caster.CharStat.GetStat(param.StatOperand);

            pathStrategy = PathStrategyFactory.CreatePathStrategy
                (new PathStrategyParameter()
                {
                    target = _target,
                    type = _projectileData.path,
                    Speed = 2f, // 이거어떻게 결정하지
                });

            rangeStrategy = RangeStrategyFactory.CreateRangeStrategy
                (new RangeParameter() { RangeType = _projectileData.rangeType });
            
            _projectileLife = _projectileData.penetrationCount;
        }

        public void InjectProjectileFunction(List<long> indices)
        {
            _functionDataList = new();
            foreach (long index in indices)
            {
                _functionDataList.Add(DataManager.Instance.GetData<FunctionData>(index));
            }
        }

        public void InjectProjectileEffect(GameObject effect)
        {
            _effectPrefab = effect;
        }
        #endregion
        
        private void FixedUpdate()
        {
            if (_caster == false || destroyFlag)
            {
                Destroy(gameObject);
                return;
            }
            if (!_target || !_target.gameObject.activeSelf)
            {
                Target = CharUtil.GetNearestInList(transform.position, _targetType);
                if (!_target)
                {
                    destroyFlag = true;
                    return;
                }
                pathStrategy = PathStrategyFactory.CreatePathStrategy
                (new PathStrategyParameter()
                {
                    target = _target,
                    type = _projectileData.path,
                    Speed = 2f,
                });
            }
            
            pathStrategy.UpdatePosition(this);
        }


        // 효과 적용 판정이 된 대상에게 대미지 및 function을 주입
        public void ApplyEffect(CharBase target)
        {
            var targets = rangeStrategy.GetTargetsInRange(target);
            foreach (var t in targets)
            {
                // 대미지 파트
                t.CharStat.ReceiveDamage(_caster.CharStat.SendDamage(_projectileDamageInput));
                Debug.Log(t.CharStat.GetStat(SystemEnum.eStats.NHP));
            
                // 효과 파트
                foreach(var data in _functionDataList)
                {
                    t.FunctionInfo.AddFunction(new BuffParameter()
                    {
                        eFunctionType = data.function,
                        CastChar = Caster,
                        TargetChar = t,
                        FunctionIndex = data.Index
                    });
                }

                // 경우에 따라 수치 바뀔수도...? 2저지 3저지 그런거 있을수도 있고
                if(--_projectileLife < 0)
                {
                    _shouldSpawnEffect = true;
                    SetDestroyFlag(true);
                }
            }

            if (_shouldSpawnEffect)
            {
                GameObject eff = Instantiate(_effectPrefab, transform.position, Quaternion.Euler(-90f, 90f, 0f));
                eff.AddComponent<EffectBehaviour>();
            }
            
        }

        // path type의 종류로 인해 선택함
        private void OnTriggerEnter(Collider other)
        {
            if (pathStrategy.ManageCollision(other, CharUtil.GetEnemyType(Caster.GetCharType())))
                ApplyEffect(other.GetComponent<CharBase>());           
        }

    }


}