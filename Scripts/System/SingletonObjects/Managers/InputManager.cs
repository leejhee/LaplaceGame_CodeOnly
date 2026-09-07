using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Client
{

    public class InputManager : Singleton<InputManager>
    {
        public readonly int SKILL_NUM = (int)eInputSystem.MaxValue; //None을 고려

        #region ENUM
        /// <summary>
        /// Input system에서 입력과 바인딩할 스킬의 종류들을 나타냅니다.
        /// Skill_Num을 사용할 경우 무조건 1부터 인덱싱하기.(None 때문에)
        /// </summary>
        public enum eInputSystem
        {
            None,
            Skill1,
            Skill2,
            Skill3,
            Skill4,
            MaxValue
        }

        enum eMiddleLevel
        {
            None,
            midLevel1,
            midLevel2,
            midLevel3,
            midLevel4,
            MaxValue
        }
        #endregion
        #region Singleton
        private InputManager()
        { }
        #endregion
        #region Containers 
        // SkillBindDict만 추후 스킬들 가지고 있는 스크립트에서 할당 필요할 때 사용 가능
        public Dictionary<eInputSystem, Action<SkillParameter>> SkillBindDict { get; set; } = new Dictionary<eInputSystem, Action<SkillParameter>>();

        private Dictionary<eMiddleLevel, eInputSystem> MidKeyBind;
        private Dictionary<KeyCode, eMiddleLevel> WinKeyBind;

        private Dictionary<int, eInputSystem> AndBtnBind;

        #endregion

        //KeySetPrefab 구독용 액션.
        public Action BindAction { get; set; }
        // 키코드 인식할 함수들을 가짐. Update 기반으로 돌아감.
        public Action InputAction;

        public override void Init()
        {
            base.Init();
            GameManager.Instance.AddOnUpdate(OnUpdate);

            #region 인풋 딕셔너리 관련 하드코딩(초깃값을 데이터에서 갖고 오게 된다면 반드시 삭제할 것.)            
            {
                AndBtnBind = new Dictionary<int, eInputSystem>()
                {
                    {0, eInputSystem.Skill1},
                    {1, eInputSystem.Skill2},
                    {2, eInputSystem.Skill3},
                    {3, eInputSystem.Skill4},
                };
                WinKeyBind = new Dictionary<KeyCode, eMiddleLevel>()
                {
                    {KeyCode.Q, eMiddleLevel.midLevel1},
                    {KeyCode.W, eMiddleLevel.midLevel2},
                    {KeyCode.E, eMiddleLevel.midLevel3},
                    {KeyCode.R, eMiddleLevel.midLevel4},
                };
                MidKeyBind = new Dictionary<eMiddleLevel, eInputSystem>()
                {
                    {eMiddleLevel.None, eInputSystem.None },
                    {eMiddleLevel.midLevel1, eInputSystem.Skill1},
                    {eMiddleLevel.midLevel2, eInputSystem.Skill2},
                    {eMiddleLevel.midLevel3, eInputSystem.Skill3},
                    {eMiddleLevel.midLevel4, eInputSystem.Skill4},
                };

                SkillBindDict = new Dictionary<eInputSystem, Action<SkillParameter>>()
                {
                    {eInputSystem.Skill1 , null},
                    {eInputSystem.Skill2 , null},
                    {eInputSystem.Skill3 , null},
                    {eInputSystem.Skill4 , null}
                };
                SkillBindDict[eInputSystem.Skill1] += Skill1;
                SkillBindDict[eInputSystem.Skill2] += Skill2;
                SkillBindDict[eInputSystem.Skill3] += Skill3;
                SkillBindDict[eInputSystem.Skill4] += Skill4;
            }
            #endregion

            KeyBind();
        }

        #region 스킬 디버깅용으로 하드코딩
        void Skill1(SkillParameter param)
        {
            Debug.Log("스킬1번 발사.");
        }

        void Skill2(SkillParameter param)
        {
            Debug.Log("스킬2번 발사.");
        }

        void Skill3(SkillParameter param)
        {
            Debug.Log("스킬3번 발사.");
        }
        void Skill4(SkillParameter param)
        {
            Debug.Log("스킬4번 발사.");
        }
        #endregion


        void KeyBind()
        {
            foreach (var keycode in WinKeyBind.Keys)
            {
                InputAction -= () => ThrowSkill(keycode);
                InputAction += () => ThrowSkill(keycode);
            }
        }

        public void InputDeactivate()
        {
            GameManager.Instance.DeleteOnUpdate(OnUpdate);
        }

        public void InputActivate()
        {
            GameManager.Instance.DeleteOnUpdate(OnUpdate);
            GameManager.Instance.AddOnUpdate(OnUpdate);
        }

        /// <summary>
        /// Update 기반으로, 키 또는 버튼의 입력을 감지하여 액션 실행
        /// </summary>
        public void OnUpdate()
        {
            if(InputAction != null)
                InputAction.Invoke();
        }

        #region 키 설정 관련 메서드

        /// <summary>
        /// 설정창에서 키 바인딩에 대한 정보를 띄우기 위해서만 사용합니다.
        /// </summary>
        /// <returns></returns>
        public Dictionary<KeyCode, eInputSystem> GetAllKeyBinds()
        {
            var DirectKeyDict = new Dictionary<KeyCode, eInputSystem>();
            foreach (var keycode in WinKeyBind.Keys)
                DirectKeyDict.Add(keycode, MidKeyBind[WinKeyBind[keycode]]);
            return DirectKeyDict;
        }

        /// <summary>
        /// 설정창에서 키 세팅을 할 때 키코드를 받아서 딕셔너리를 편집
        /// </summary>
        /// <param name="setKey"></param>
        public void SetKeyBinds(KeyCode originKey, KeyCode setKey, eInputSystem targetInput)
        {
            if (!WinKeyBind.ContainsKey(setKey))
            {
                //새 키가 유효하지 않을 때
                Debug.Log($"{setKey}는 반영되어있지 않은 키의 종류입니다. 다른 키를 입력해주세요.");
            }
            else if (originKey == setKey) { }
            else
            {
                // 새 키가 유효할 때
                eMiddleLevel newMidKey = WinKeyBind[setKey];
                eMiddleLevel originMidKey = WinKeyBind.ContainsKey(originKey) ? WinKeyBind[originKey] : eMiddleLevel.None;

                // 새 키가 바인딩되어있지 않았다면
                if (!MidKeyBind.ContainsKey(newMidKey))
                {
                    // 일단 추가를 하고, 타겟 행동이 바인딩되어있었다면 그것만 끊어준다.
                    MidKeyBind.Add(newMidKey, targetInput);
                    if (!(originMidKey == eMiddleLevel.None))
                        MidKeyBind.Remove(originMidKey);
                }
                else
                {
                    // 새 키가 이미 다른 스킬에 바인딩되어 있었다면
                    //원래 키의 바인딩을 없애고 새 바인딩을 넣어준다. 만약 필요한 수만큼 세팅이 안됐다면 마저 하고 닫을 수 있도록 조건 세팅이 필요하다.
                    Debug.Log($"그 키({setKey})는 이미 다른 기능에 바인딩되어있습니다. " +
                        $"원래 바인딩을 해제하고 새 바인딩으로 추가합니다. 원래 기능의 바인딩을 완료해주세요.");

                    MidKeyBind.Remove(newMidKey);
                    if (originMidKey == eMiddleLevel.None)
                        MidKeyBind.Add(newMidKey, targetInput);
                    else
                    {
                        MidKeyBind.Add(newMidKey, MidKeyBind[originMidKey]);
                        MidKeyBind.Remove(originMidKey);
                    }
                }

                BindAction.Invoke();
            }
        }
        
        /// <summary>
        /// 키코드에 연결된 미들키가 바인딩되어있는지 여부 점검
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckKeyValidity(KeyCode e)
        {
            if(!WinKeyBind.ContainsKey(e)) return false;
            return MidKeyBind.ContainsKey(WinKeyBind[e]);
        }

        public bool CheckPrefabPairValidity(KeyCode k, eInputSystem e)
        {
            return MidKeyBind[WinKeyBind[k]] == e;
        }

        public KeyCode SearchNewKeyCode(eInputSystem e)
        {
            eMiddleLevel eMid = MidKeyBind.FirstOrDefault(x => x.Value == e).Key;
            if(eMid == eMiddleLevel.None) return KeyCode.None;
            return WinKeyBind.FirstOrDefault(x => x.Value == eMid).Key;
        }

        public bool CheckBindingIntegrity()
        {
            if (MidKeyBind.Count < SKILL_NUM) 
            {
                Debug.Log("스킬 바인딩을 마무리해주세요.");
                return false;
            }
            else return true;
        }

        #endregion

        #region Throwing Skills
        /// <summary>
        /// 버튼을 눌렀을 때 Input Manager에서 딕셔너리 통해 아~ 이거 쓰려는구나! 하고 쓰게 해준다.
        /// InputParameter은 어떻게 받아야하는걸까...?
        /// </summary>
        /// <param name="skillIndex">
        /// 버튼 ID를 뜻함 
        /// </param>
        public void ThrowSkill(int skillIndex)
        {
            if (AndBtnBind.ContainsKey(skillIndex))
            {
                Action<SkillParameter> targetAction = SkillBindDict[AndBtnBind[skillIndex]];
                if (targetAction == null)
                {
                    Debug.Log($"으잉 스킬 {AndBtnBind[skillIndex]} 바인딩된거 없는뎁쇼");
                    return;
                }

                targetAction.Invoke(new SkillParameter());
                //일단 아무것도 없으니까 저렇게 넣는다.
                Debug.Log($"옛다 {targetAction} 스킬이나 먹어라~");
            }
        }

        /// <summary>
        /// 키보드 키를 눌렀을 때 Input Manager에서 딕셔너리 통해 아~ 이거 쓰려는구나! 하고 쓰게 해준다.
        /// </summary>
        /// <param name="keyCode">
        /// 입력한 키코드를 뜻함
        /// </param>
        public void ThrowSkill(KeyCode keyCode)
        {
            if (Input.GetKey(keyCode))
            {
                if (WinKeyBind.ContainsKey(keyCode))
                {
                    eMiddleLevel midByKey =  WinKeyBind[keyCode];
                    eInputSystem inputByKey = MidKeyBind[midByKey];
                    Action<SkillParameter> targetAction = SkillBindDict[inputByKey];
                    if (targetAction == null)
                    {
                        Debug.Log($"으잉 스킬 {keyCode}에 바인딩된거 없는뎁쇼");
                        return;
                    }

                    /*
                     * 현재는 마땅히 쓸 게 없어서 new InputParameter로 했는데,
                     * 추후 InputParamGenerator같은 메서드를 만들어서, 
                     * 각 장비/캐릭터에 붙은 스킬들에 대해 정보(InputParameter)들을 인스턴스화해서
                     * 나중엔 InputParameter param = InputParamGenererator(어쩌구);로 한 param을 인자에 넣으면 될것이다.
                     */
                    targetAction.Invoke(new SkillParameter());
                    Debug.Log($"옛다 {targetAction} 스킬이나 먹어라~");
                }
            }            
        }
        #endregion
    }
}