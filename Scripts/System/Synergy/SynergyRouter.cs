using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// 시너지 버프 분배 책임을 가지는 클래스(SynergyContainer에서 분리)
    /// </summary>
    public sealed class SynergyRouter
    {
        private readonly SynergyManager _mgr;
        private readonly Dictionary<(eCharType, eSynergy), List<SynergyBuffRecord>> _records = new();
        private readonly Dictionary<(eCharType, eSynergy), Dictionary<CharBase, Dictionary<long, SynergyBuffRecord>>>
            _auraRecords = new(); // 전체적용은 Aura라 합시다
        
        public SynergyRouter(SynergyManager mgr) => _mgr = mgr;
        
        
        public void Clear()
        {
            foreach (var records in _records.Values)
                foreach (var record in records) record.KillSynergyBuff();
            foreach (var recipients in _auraRecords.Values)
                foreach (var records in recipients.Values)
                    foreach (var record in records.Values) record.KillSynergyBuff();
            _records.Clear();
            _auraRecords.Clear();
        }

        public void Wire(SynergyContainer ct)
        {
            ct.OnLevelChanged += ApplyAll;
            ct.OnMemberJoined += _ => ApplyAll(ct);
            ct.OnMemberLeft += _ => ApplyAll(ct);
            
            //guest는?
        }
        
        public void ApplySystem(SynergyContainer ct)
        {
            var anchor = _mgr.GetOrCreateAnchor(ct.Side, ct.Synergy);
            anchor.BindMembers(ct.SynergyMembers);
            foreach (var effect in ct.GetSynergyByLevel())
                if (effect.functionIndex != 0 && effect.casterType == eSynergyRange.SYSTEM)
                    ApplyToAnchor(anchor, effect);
            anchor.FunctionInfo.FlushPendingFunctions();
        }

        public void ApplyAll(SynergyContainer ct)
        {
            (eCharType Side, eSynergy Synergy) key = (ct.Side, ct.Synergy);
            if (_records.TryGetValue(key, out List<SynergyBuffRecord> memRecords))
            {
                foreach(var r in memRecords) r?.KillSynergyBuff();
                memRecords.Clear();
            }
            else _records[key] = new List<SynergyBuffRecord>();

            if (_auraRecords.TryGetValue(key, out var charAuraRecords))
            {
                foreach(Dictionary<long, SynergyBuffRecord> dict in charAuraRecords.Values)
                    foreach(var rec in dict.Values) rec?.KillSynergyBuff();
                charAuraRecords.Clear();
            }
            else _auraRecords[key] = new Dictionary<CharBase, Dictionary<long, SynergyBuffRecord>>();
            
            var anchor = _mgr.GetOrCreateAnchor(ct.Side, ct.Synergy);
            anchor.BindMembers(ct.SynergyMembers);
            
            var effects = ct.GetSynergyByLevel();
            
            //가짓수가 적어서 switch 안쓰고 이거로 한다.
            foreach (var e in effects)
            {
                // 엑셀의 0번 기능은 비활성 단계이며 FunctionData에 존재하지 않는다.
                if (e.functionIndex == 0) continue;

                if (e.skillTarget == eSkillTargetType.EVERY_ENEMY || e.skillTarget == eSkillTargetType.EVERY_ALLY)
                {
                    // 타이머의 전체 적 기절 등은 팀별로 각 수혜자에게 한 번만 등록한다.
                    var side = e.skillTarget == eSkillTargetType.EVERY_ENEMY ? CharUtil.GetEnemyType(ct.Side) : ct.Side;
                    foreach (var target in CharManager.Instance.GetOneSide(side) ?? Enumerable.Empty<CharBase>())
                        ApplyAuraOne(key, ct, e, target);
                    continue;
                }

                if (e.casterType == eSynergyRange.SYSTEM) continue;

                if (e.casterType == eSynergyRange.GLOBAL_ALLY || e.casterType == eSynergyRange.GLOBAL_ENEMY)
                {
                    ApplyAuraForAll(ct, e);
                    continue;
                }
                
                ApplyToMembers(ct, e);
            }
            
            // 조건 평가는 전체 팀의 BORN 효과를 설치한 후 SynergyManager에서 수행한다.
            Debug.Log($"{ct.Side}의 {ct.Synergy}가 {ct.DistinctMembers}명 구성으로 {ct.MemberCount}에 적용");
        }
        
        private void ApplyAuraForAll(SynergyContainer ct, SynergyData data)
        {
            var key = (ct.Side, ct.Synergy);
            var recipients = GetAuraRecipients(ct, data.casterType); // 아군/적 전체
            foreach (var target in recipients)
                ApplyAuraOne(key, ct, data, target);
        }

        private void ApplyAuraForGuest(SynergyContainer ct, CharLightWeightInfo guest)
        {
            var key = (ct.Side, ct.Synergy);
            if (!_auraRecords.TryGetValue(key, out var charRecords)) return;

            var cb = guest.SpecifyCharBase();
            if (!cb) return;

            if (charRecords.TryGetValue(cb, out var funcDict))
            {
                foreach(var rec in funcDict.Values) rec?.KillSynergyBuff();
                charRecords.Remove(cb);
            }
        }

        private void ApplyAuraOne(
            (eCharType, eSynergy) key, 
            SynergyContainer ct, 
            SynergyData data, 
            CharBase target)
        {
            if (!target) return;

            var anchor = _mgr.GetOrCreateAnchor(ct.Side, ct.Synergy);
            var caster = anchor.Representative ?? target;
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            
            if(!_auraRecords.TryGetValue(key, out var charRecords))
                _auraRecords[key] = charRecords = new Dictionary<CharBase, Dictionary<long, SynergyBuffRecord>>();
            if (!charRecords.TryGetValue(target, out var rec))
            {
                rec = new Dictionary<long, SynergyBuffRecord>();
                charRecords[target] = rec;
            }
                
            if (rec.ContainsKey(funcData.Index)) return; // 중복 허용 X

            var sf = target.FunctionInfo.AddFunction(
                new BuffParameter()
                {
                    CastChar = caster,
                    TargetChar = target,
                    eFunctionType = funcData.function,
                    FunctionIndex = funcData.Index
                }, data.buffTriggerTime);
            
            if (sf == null) {
                Debug.LogWarning($"[SynergyRouter] AddFunction returned null: fn={funcData.function}, idx={funcData.Index}, target={target.name}");
                return;
            }
            
            rec[funcData.Index] = new SynergyBuffRecord(caster, sf);
        }
        
        private void ApplyToMembers(SynergyContainer ct, SynergyData data)
        {
            var key = (ct.Side, ct.Synergy);
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);

            foreach (var lw in ct.SynergyMembers)
            {
                var caster = lw.SpecifyCharBase();
                if (!caster) continue;
                var fb = caster.FunctionInfo.AddFunction(
                    new BuffParameter
                    {
                        CastChar = caster,
                        TargetChar = caster,
                        eFunctionType = funcData.function,
                        FunctionIndex = data.functionIndex
                    }, data.buffTriggerTime); // BORN / COMBAT
                _records[key].Add(new SynergyBuffRecord(caster, fb));
            }
        }

        private void ApplyToAnchor(SynergyAnchor anchor, SynergyData data)
        {
            var caster = anchor.Representative;
            if (!caster) return;
            var funcData = DataManager.Instance.GetData<FunctionData>(data.functionIndex);
            
            var fp = new BuffParameter
            {
                CastChar = caster,
                TargetChar = caster,
                eFunctionType = funcData.function,
                FunctionIndex = data.functionIndex
            };
            var fb = FunctionFactory.FunctionGenerate(fp);
            anchor.FunctionInfo.AddFunction(fb, data.buffTriggerTime);
            anchor.EnsureSingle($"{fp.eFunctionType}:{fp.FunctionIndex}", new SynergyBuffRecord(caster, fb));
        }
        
        private IEnumerable<CharBase> GetAuraRecipients(SynergyContainer cont, eSynergyRange casterType)
        {
            if (casterType == eSynergyRange.GLOBAL_ALLY)
                return CharManager.Instance.GetOneSide(cont.Side) ?? Enumerable.Empty<CharBase>();
            if (casterType == eSynergyRange.GLOBAL_ENEMY)
                return CharManager.Instance.GetOneSide(CharUtil.GetEnemyType(cont.Side)) ?? Enumerable.Empty<CharBase>();
            return Enumerable.Empty<CharBase>();
        }
        
        
    }

    
}
