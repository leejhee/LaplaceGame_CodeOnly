using System;
using static Client.SystemEnum;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections;

namespace Client
{
    public class SynergyContainer
    {
        #region Fields
        private readonly eSynergy _mySynergy;
        private readonly eCharType _myCharType;

        private readonly List<CharLightWeightInfo> _synergyMembers;
        private readonly HashSet<CharLightWeightInfo> _guestMemberSet;
        #endregion
        
        #region Properties
        public eSynergy Synergy => _mySynergy;
        public eCharType Side => _myCharType;
        public ReadOnlyCollection<CharLightWeightInfo> SynergyMembers => _synergyMembers.AsReadOnly();
        public int MemberCount => _synergyMembers.Count;
        #endregion
        
        #region Container Events
        public event Action<SynergyContainer> OnLevelChanged;
        public event Action<CharLightWeightInfo> OnMemberJoined;
        public event Action<CharLightWeightInfo> OnMemberLeft;
        
        public event Action<CharLightWeightInfo> OnGuestJoined;
        public event Action<CharLightWeightInfo> OnGuestLeft;
        #endregion
        
        #region 생성자
        public SynergyContainer(eSynergy synergy, eCharType myCharType)
        {
            _synergyMembers = new List<CharLightWeightInfo>();
            _guestMemberSet = new HashSet<CharLightWeightInfo>();
            _mySynergy = synergy;
            _myCharType = myCharType;
        }
        #endregion
        public override string ToString()
        {
            return $"{_mySynergy} 시너지, Members : {_synergyMembers.Count}, DistinctMembers : {DistinctMembers}";
        }
        
        #region Synergy Member Control

        // 해당 시너지에 포함되는 멤버가 등록될 때 호출된다.
        public void Register(CharLightWeightInfo registrar)
        {
            _guestMemberSet.Remove(registrar);
            if (!_synergyMembers.Contains(registrar))
            {
                _synergyMembers.Add(registrar);
                OnMemberJoined?.Invoke(registrar);
            }
            
            if (CheckSynergyChange())
                OnLevelChanged?.Invoke(this);
        }

        //public void GuestRegister(CharLightWeightInfo guest)
        //{
        //    _guestMemberSet.Add(guest);
        //    GetCurrentSynergyBuff(guest);
        //}
        
        // 해당 시너지에 포함되는 멤버가 탈퇴할 때 호출된다.
        public void Delete(CharLightWeightInfo leaver)
        {
            if (_synergyMembers.Contains(leaver))
                _synergyMembers.Remove(leaver);
    
            if (_synergyMembers.Count == 0)
            {
                ClearSynergyBuff();
                return;           
            }
            
            KillLeaverBuff(leaver);

            if (CheckSynergyChange())
                SetCurrentSynergy();

        }

        //public void GuestDelete(CharLightWeightInfo guestLeaver)
        //{
        //    _guestMemberSet.Remove(guestLeaver);
        //    KillLeaverBuff(guestLeaver);
        //}
        public void GuestRegister(CharLightWeightInfo guest, bool silent = false)
        {
            if (_guestMemberSet.Add(guest) && !silent)
                OnGuestJoined?.Invoke(guest);
        }

        public void GuestDelete(CharLightWeightInfo guestLeaver, bool silent = false)
        {
            if (_guestMemberSet.Remove(guestLeaver) && !silent)
                OnGuestLeft?.Invoke(guestLeaver);
        }

        
        #endregion

        #region Synergy Data Getter
        private List<SynergyData> _currentSynergyBuff = new();
        public int DistinctMembers
        {
            get
            {
                var distinctList = _synergyMembers
                    .GroupBy(member => member.Index)
                    .Select(g => g.First())
                    .ToList();
                return distinctList.Count;
            }
        }

        public List<SynergyData> GetSynergyByLevel()
        {
            var synergyInfo = DataManager.Instance.SynergyDataMap[_mySynergy];
            var sortedKeys = new List<int>(synergyInfo.Keys);
            sortedKeys.Sort();

            int nowDistinct = DistinctMembers;

            // 문턱이 1인 시너지는 시너지 종류별로 반드시 존재해야 함.
            int levelThreshold = 0;
            foreach (var threshold in sortedKeys)
            {
                if (nowDistinct < threshold)
                {
                    break;
                }
                levelThreshold = threshold;
            }
            
            return synergyInfo.TryGetValue(levelThreshold, out var list) ? list : new List<SynergyData>();
        }
        #endregion

        #region Synergy Buff Managing(Will be deprecated)

        private Queue<SynergyBuffRecord> synergyBuffRecords = new();
        
        // 시너지가 갱신되어야 하는지에 대한 체크 함수
        private bool CheckSynergyChange()
        {
            List<SynergyData> newSynergy = GetSynergyByLevel();
            if (ReferenceEquals(_currentSynergyBuff, newSynergy))
            {
                return false;
            }
            
            _currentSynergyBuff = newSynergy;
            return true;
        }

        // 현재 적용되는 글로벌 시너지 버프를 얻는 함수
        private void GetCurrentSynergyBuff(CharLightWeightInfo receiver)
        {
            var caster = receiver.SpecifyCharBase();
            if (caster == false) return;

            foreach(var synergyData in _currentSynergyBuff)
            {
                if(synergyData.casterType == eSynergyRange.GLOBAL_ALLY)
                {
                    GetBuff(caster, synergyData);
                }
            }
            
            OnNewBuffDistributed(caster);
        }

        private void KillLeaverBuff(CharLightWeightInfo releaser)
        {
            var caster = releaser.SpecifyCharBase();
            if (caster == false) return;

            // 안전하게 하자.
            int count = synergyBuffRecords.Count;
            while (count-- > 0)
            {
                var record = synergyBuffRecords.Dequeue();
                if (record.Caster != caster)
                    synergyBuffRecords.Enqueue(record);               
            }
        }

        // 시너지 버프 얻는 함수
        private void GetBuff(CharBase caster, SynergyData data)
        {
            #region GETTING PARAMETERS
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            if (funcData == null)
            {
                Debug.LogError("잘못 됐다잖냐 이녀석아 데이터 확인 다시해봐라.");
                return;
            }
            
            var intendedTargets = TargetStrategyFactory.CreateTargetStrategy
                (new TargettingStrategyParameter { Caster = caster, type = data.skillTarget })?.GetTargets();

            CharBase target = null;
            if (intendedTargets == null)
            {
                Debug.LogWarning($"현재 버프 {data.functionIndex}가 타겟이 null입니다. 시스템 제공 대상인지 확인하세요.");
            }
            else if (intendedTargets.Count == 0)
                Debug.Log($"현재 타겟이 아직 타게팅 타입 {data.skillTarget}에 따라 정해지지 않습니다.");
            else
                target = intendedTargets[0];
            #endregion

            #region ADD BUFF AND RECORD
            //////////Add Buff///////////
            var synergyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
            {
                CastChar = caster,
                TargetChar = target,
                eFunctionType = funcData.function,
                FunctionIndex = funcData.Index
            });

            caster.FunctionInfo.AddFunction(synergyFunction, data.buffTriggerTime);
            Debug.Log($"{caster.GetID()}번 캐릭터 {caster.name}에 synergy Function {funcData.Index}번 function 주입. " +
                                    $"기능 : {funcData.function}");
            //////////Add Record/////////
            synergyBuffRecords.Enqueue(new SynergyBuffRecord(caster, synergyFunction));
            #endregion

        }
        
        //TODO : 반드시 이 겹치는 생성부를 리팩토링 할 것.
        private void GetSynergySystemBuff(SynergyData data)
        {
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            var synergyFunction = FunctionFactory.FunctionGenerate(new BuffParameter()
            {
                CastChar = null,
                TargetChar = null,
                eFunctionType = funcData.function,
                FunctionIndex = funcData.Index
            });
            
            synergyBuffRecords.Enqueue(new SynergyBuffRecord(null, synergyFunction));
            synergyFunction.RunFunction(); // TODO : 단일로 그냥 돌아가게 하는 거긴 한데, 전역 클래스에서 관리하게 할지 고민할 것
        }

        private void ClearSynergyBuff()
        {
            while(synergyBuffRecords.Count > 0)
            {
                SynergyBuffRecord record = synergyBuffRecords.Dequeue();
                record?.KillSynergyBuff();
            }
        }
        
        // 시너지 갱신 함수
        private void SetCurrentSynergy()
        {
            ClearSynergyBuff();

            List<CharBase> casters = new();            
            foreach(var synergyData in _currentSynergyBuff)
            {
                // eSynergyRange에 따라서, Caster로 지정할 아군의 범위를 지정한다.
                switch (synergyData.casterType)
                {
                    case eSynergyRange.SYSTEM:
                        {
                            GetSynergySystemBuff(synergyData);
                            return;
                        }
                    case eSynergyRange.SELF:
                        foreach (var info in _synergyMembers)
                        {
                            casters.Add(info.SpecifyCharBase());
                        }
                        break;
                    case eSynergyRange.GLOBAL_ALLY:
                        casters = CharManager.Instance.GetAllySide(_myCharType);
                        break;
                    case eSynergyRange.GLOBAL_ENEMY:
                        casters = CharManager.Instance.GetEnemySide(_myCharType);
                        break;
                    default: break;
                }

                Debug.Log($"시너지 변경이 감지되었습니다. " +
                            $"Synergy {_mySynergy}에서 {DistinctMembers}명 달성하여 {synergyData.Index}번 버프 " +
                            $"casters {casters.Count}에게 들어감");

                // 캐스터 순회하여 각각 caster로 설정한 function을 만들어 add한다.
                foreach (var caster in casters)
                {
                    if (caster == false) continue;
                    GetBuff(caster, synergyData); 
                }
            }

            //ScanAllMembers();
        }
        
        public void NotifyBuffDistribution()
        {
            // 소환수도 전체 아군 효과를 받지만 시너지 인원에는 포함하지 않는다.
            var recipients = CharManager.Instance.GetOneSide(_myCharType);
            if (recipients == null) return;
            foreach (var character in recipients) OnNewBuffDistributed(character);
        }
        
        private void OnNewBuffDistributed(CharBase specifiedMember)
        {
            if (specifiedMember == false) return;
            specifiedMember.FunctionInfo.EvaluateCondition(new SynergyConditionInput
            {
                ChangedSynergy = _mySynergy,
                CharTypeContext = _myCharType,
                RegistrarIndex = specifiedMember.Index
            });
        }
        #endregion
        
        
    }

}
