using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    /// <summary>
    /// 시너지 관리 모듈
    /// </summary>
    public class SynergyManager : Singleton<SynergyManager>
    {
        private readonly Dictionary<eCharType, Dictionary<eSynergy, SynergyContainer>> _synergyMembers = new();
        private readonly Dictionary<eCharType, Dictionary<eSynergy, SynergyAnchor>> _anchors = new();
        private readonly Dictionary<eCharType, IReadOnlyList<SynergyDisplayInfo>> _teamSynergies = new();

        private SynergyRouter _router;
        private bool _rebuilding;
        
        #region 생성자
        private SynergyManager() { }
        #endregion

        public override void Init()
        {
            base.Init();
            _router ??= new SynergyRouter(this);          //라우터(분배기) 초기화
            GameManager.Instance.AddOnUpdate(Update);   //시너지 관련 업데이트 필요
        }

        public void Reset()
        {
            _router?.Clear();
            foreach (var bySynergy in _anchors.Values)
                foreach (var anchor in bySynergy.Values) anchor.Dispose();
            _synergyMembers.Clear();
            _anchors.Clear();
            _teamSynergies.Clear();
            NotifyTeamSynergies();
        }

        public IReadOnlyList<SynergyDisplayInfo> GetTeamSynergies(eCharType side)
        {
            return _teamSynergies.TryGetValue(side, out var synergies)
                ? synergies : Array.Empty<SynergyDisplayInfo>();
        }

        private void NotifyTeamSynergies()
        {
            foreach (var side in new[] { eCharType.ALLY, eCharType.ENEMY })
                MessageManager.SendMessage(new OnSynergyChange(side, GetTeamSynergies(side)));
        }
        
        public ReadOnlyCollection<CharLightWeightInfo> GetInfo(eCharType side, eSynergy synergy)
        {
            if(_synergyMembers.ContainsKey(side))
                if(_synergyMembers[side].ContainsKey(synergy))
                    return _synergyMembers[side][synergy].SynergyMembers;
            return null;
        }
        
        /// <summary> 캐릭터 단위 시너지 등록 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
        public void RegisterCharSynergy(CharLightWeightInfo registrar)
        {
            var side = registrar.Side;

            if (!_synergyMembers.ContainsKey(side)) 
                _synergyMembers.Add(side, new Dictionary<eSynergy, SynergyContainer>());
            
            foreach (var synergy in registrar.SynergyList)
            {
                RegisterSynergy(registrar, synergy);
            }
            
            var others = _synergyMembers[side].Keys.Except(registrar.SynergyList).ToList();
            foreach (var other in others)
            {
                _synergyMembers[side][other].GuestRegister(registrar, _rebuilding);
            }
            
        }
        
        public void DeleteCharSynergy(CharLightWeightInfo leaver)
        {
            var side = leaver.Side;

            foreach (eSynergy synergy in leaver.SynergyList)
            {
                DeleteSynergy(leaver, synergy);
            }

            if (_synergyMembers[side] == null) return;
            var others = _synergyMembers[side].Keys.Except(leaver.SynergyList).ToList();
            foreach (var other in others)
            {
                _synergyMembers[side][other].GuestDelete(leaver);
            }

        }


        /// <summary> 시너지 등록 단위 </summary>
        /// <param name="registrar"> 가입자 정보 </param>
        /// <param name="synergy"> 가입할 시너지 </param>
        public void RegisterSynergy(CharLightWeightInfo registrar, eSynergy synergy)
        {
            if (synergy == eSynergy.None) return;
            var side = registrar.Side;
            
            
            if (!_synergyMembers.ContainsKey(side))
                _synergyMembers.Add(side, new Dictionary<eSynergy, SynergyContainer>());
            
            var synergyActivator = _synergyMembers[side];
            //시너지 원래 없었어서 새로 만들어지면
            if (!synergyActivator.TryGetValue(synergy, out var ct))
            {
                ct = new SynergyContainer(synergy, side);
                synergyActivator.TryAdd(synergy, ct);
                //_router.Wire(ct);

                var fams = CharManager.Instance.GetOneSide(side);
                if (fams != null)
                {
                    foreach (var cb in fams)
                    {
                        var info = cb.GetCharSynergyInfo();
                        if(!info.SynergyList.Contains(synergy))
                            ct.GuestRegister(info, _rebuilding);
                    }
                }
            }
            
            //if (!synergyActivator.ContainsKey(synergy))
            //{
            //    _synergyMembers[side].Add(synergy, new SynergyContainer(synergy, side));
            //    var allChars = CharManager.Instance.GetOneSide(side);
            //    foreach (var charBase in allChars)
            //    {
            //        CharLightWeightInfo info = charBase.GetCharSynergyInfo(); // 있으면 생성자, 없으면 직접 구성
            //        if (!info.SynergyList.Contains(synergy))
            //            synergyActivator[synergy].GuestRegister(info);
            //    }
            //}
            //_synergyMembers[side][synergy].Register(registrar);
            ct.Register(registrar);
        }

        public void DeleteSynergy(CharLightWeightInfo leaver, eSynergy synergy)
        {           
            if (synergy == eSynergy.None) return;
            var side = leaver.Side;
            
            if (_synergyMembers.TryGetValue(side, out var bySynergy) && bySynergy.TryGetValue(synergy, out var ct))
            {
                ct.Delete(leaver);
                if (ct.MemberCount == 0) bySynergy.Remove(synergy);
            }
        }
        
        public SynergyAnchor GetOrCreateAnchor(eCharType side, eSynergy syn)
        {
            if (!_anchors.TryGetValue(side, out var bySyn))
                bySyn = _anchors[side] = new Dictionary<eSynergy, SynergyAnchor>();

            if (!bySyn.TryGetValue(syn, out var a))
                a = bySyn[syn] = new SynergyAnchor(side, syn);

            return a;
        }
        
        public void FlushPendingFunctions()
        {
            foreach (var bySynergy in _anchors.Values)
                foreach (var anchor in bySynergy.Values) anchor.FunctionInfo.FlushPendingFunctions();
            foreach (var character in CharManager.Instance.GetCurrentCharacters())
                character.FunctionInfo.FlushPendingFunctions();
        }

        private void Update()
        {
            foreach (Dictionary<eSynergy, SynergyAnchor> bySyn in _anchors.Values)
                foreach (SynergyAnchor a in bySyn.Values)
                    a.Tick();
        }
        
        /// <summary>
        /// 스테이지 초기화시 캐릭터들에 시너지 분배
        /// </summary>
        public void RebuildFromFieldAndDistribute()
        {
            Reset();
            _rebuilding = true;
            
            Debug.Log("시너지 시스템 초기화 완료");
            
            foreach (var side in new[] { eCharType.ALLY, eCharType.ENEMY })
            {
                var list = CharManager.Instance.GetOneSide(side);
                if (list == null) continue;

                foreach (var cb in list)
                {
                    var info = cb.GetCharSynergyInfo();
                    RegisterCharSynergy(info);
                }
            }
            Debug.Log("시너지 등록 완료");
            _rebuilding = false;

            foreach (var bySyn in _synergyMembers.Values)
                foreach (var cont in bySyn.Values)
                    _router.ApplySystem(cont);
            foreach (var bySyn in _synergyMembers.Values)
                foreach (var cont in bySyn.Values)
                    _router.ApplyAll(cont);
            // 실행 순서에 의존하는 한 프레임 대기 대신 조건 감시자를 먼저 설치한다.
            foreach (var anchors in _anchors.Values)
                foreach (var anchor in anchors.Values) anchor.FunctionInfo.FlushPendingFunctions();
            foreach (var character in CharManager.Instance.GetCurrentCharacters())
                character.FunctionInfo.FlushPendingFunctions();
            foreach (var bySynergy in _synergyMembers.Values)
                foreach (var container in bySynergy.Values) container.NotifyBuffDistribution();
            foreach (var character in CharManager.Instance.GetCurrentCharacters())
                character.FunctionInfo.FlushPendingFunctions();
            Debug.Log("시너지 분배 완료");

            // 모든 캐릭터의 등록과 분배가 끝난 후 팀별 목록을 한 번 확정한다.
            _teamSynergies.Clear();
            foreach (var side in _synergyMembers)
            {
                _teamSynergies[side.Key] = side.Value.Values
                    .Where(container => container.MemberCount > 0)
                    .Select(container => new SynergyDisplayInfo(container))
                    .OrderByDescending(info => info.IsActive)
                    .ThenBy(info => info.Data.Index)
                    .ToList().AsReadOnly();
            }
            NotifyTeamSynergies();
            
        }
        
        #region Test_Method
        // 씬상 테스트만 하는 용도
        public void ShowCurrentSynergies()
        {
            StringBuilder view = new("현재 시너지\n");
            foreach (var value in _synergyMembers)
            {
                view.AppendLine($"{value.Key} 사이드 시너지 :");
                foreach (var syn in value.Value.Values)
                {
                    view.AppendLine(syn.ToString());
                }
            }
            Debug.Log(view.ToString());
        }
        #endregion

    }

}
