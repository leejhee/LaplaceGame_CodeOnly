using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;
using UnityEngine.AI;

namespace Client
{
    /// <summary>
    /// 캐릭터 베이스 class
    /// </summary>
    public abstract class CharBase : MonoBehaviour
    {
        #region Serialized Fields
        // SerializeField 
        [SerializeField] private    long            _index;         // CharData 테이블의 인덱스 
        [SerializeField] private    Collider        _FightCollider; // 전투 콜라이더
        [SerializeField] private    Collider        _MoveCollider;  // 이동 콜라이더
        [SerializeField] private    GameObject      _SkillRoot;     // 스킬 루트
        [SerializeField] protected  NavMeshAgent    _NavMeshAgent;  // 네비 메쉬 에이전트
        [SerializeField] protected  GameObject      _CharCamaraPos; // 캐릭터 애니메이션 리스트
        [SerializeField] protected  Animator        _Animator;       // 애니메이터\
        [SerializeField] private    Transform       _UnitRoot;
        #endregion
        
        #region Fields
        protected FunctionInfo _functionInfo = null; // 기능 정보
        protected EffectInfo _effectInfo = null;

        private CharSKillInfo   _charSKillInfo;       // 캐릭터 스킬
        private CharStat        _charStat = null;      // Stat 정보
        private CharAnim        _charAnim = null;      // 캐릭터 애니메이션 리스트
        private CharAction      _charAction = null;     // 캐릭터 동작 명령 클래스
        protected CharData      _charData = null;      // 캐릭터 데이터
        protected CharAI        _charAI = null;
    
        private Transform  _CharTransform = null; // 캐릭터 트렌스폼
        private Transform  _CharUnitRoot = null;  // 캐릭터 유닛 루트 트렌스폼


        private PlayerState _currentState; // 현재 상태
        private bool _isAction = false;    // 행동중인가? 판별
        private Dictionary<PlayerState, int> _indexPair = new(); // 
        
        protected long _uid;  // 캐릭터 고유 ID

        private Vector3 _rightRotation = new Vector3(0, 180,0); // 오른쪽 로테이션
        private Vector2 _lefRotation = Vector3.zero;        // 왼쪽 로테이션

        private Vector3 _rightPos = new Vector3(1, 0, 0); // 오른쪽 방향
        private Vector2 _leftPos = new Vector3(-1, 0, 0); // 왼쪽 방향

        private Coroutine _coroutine; // UpdateAI용,,
        public Vector3 LookAtPos { get; private set; } = Vector3.right; // 현재 방향성

        private CharLightWeightInfo _lightWeightInfo;
        private List<eSynergy> _charSynergies = null;

        private Camera _mainCamera;
        
        protected virtual SystemEnum.eCharType CharType => SystemEnum.eCharType.None; // 캐릭터 타입
        #endregion
        
        #region Properties
        public long Index => _index;
        public Collider FightCollider => _FightCollider; // 
        public Collider MoveCollider  => _MoveCollider;
        public FunctionInfo FunctionInfo => _functionInfo;  // 기능 정보
        public CharSKillInfo CharSKillInfo => _charSKillInfo; // 캐릭터 스킬
        public EffectInfo EffectInfo => _effectInfo;
        public Transform CharTransform => _CharTransform;
        private Transform CharUnitRoot => _CharUnitRoot; // 캐릭터 유닛 루트 트렌스폼
        public GameObject CharCamaraPos => _CharCamaraPos; // 카메라 위치
        public CharAnim CharAnim => _charAnim; // 카메라 위치
        public NavMeshAgent Nav => _NavMeshAgent;
        public CharStat CharStat => _charStat;
        public CharAction CharAction => _charAction;
        public CharAI CharAI => _charAI;
        public CharData CharData => _charData;
        public PlayerState PlayerState => _currentState;

        public int TileIndex { get; set; } = default;
        #endregion
        private bool _combatFrozen;
        private bool _dead;
        private bool _destroyRequested;
        protected CharBase() { }

        public void FreezeCombat()
        {
            _combatFrozen = true;
            AISwitch(false);
            StopAllCoroutines();
            if (Nav && Nav.enabled && Nav.isOnNavMesh)
            {
                Nav.ResetPath();
                Nav.isStopped = true;
            }
            if (_charSKillInfo != null)
                foreach (var skill in _charSKillInfo.DicSkill.Values)
                    if (skill && skill.Director) skill.Director.Pause();
        }

        private void Awake()
        {
            _charData = DataManager.Instance.GetData<CharData>(_index);
            if (_charData != null)
            {
                StatsData charStat = DataManager.Instance.GetData<StatsData>(_charData.statsIndex);
                if (charStat == null)
                {
                    Debug.LogError($"캐릭터 ID : {_index} 데이터 Get 성공 charStat {_charData.statsIndex} 데이터 Get 실패");
                }
                _charStat = new CharStat(charStat, this);
            }
            else
            {
                Debug.LogError($"캐릭터 ID : {_index} Data 데이터 Get 실패");
            }
            
            _CharTransform = transform;
            _CharUnitRoot = Util.FindChild<Transform>(gameObject,"UnitRoot");
            _uid = CharManager.Instance.GetNextID();
            _functionInfo = new FunctionInfo();
            _effectInfo = new EffectInfo();
            _functionInfo.Init();
            _NavMeshAgent = GetComponent<NavMeshAgent>();
            _charAnim =     new();
            _charAction =   new(this);
            _charAI =       new(this);
            _mainCamera =   Camera.main;
            
            CharInit();
        }

        protected virtual void Start()
        {
            if (_charAnim != null)
            {
                //_charAnim.Initialized(_Animator);
                _charAnim.Initialized(GetComponentInChildren<Animator>());
            }
            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                _indexPair[state] = 0;
            }
            //CharInit();
        }

        void Update()
        {
            if (!_combatFrozen)
            {
                _functionInfo?.UpdateFunctionDic();
                _effectInfo?.UpdateEffect();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
                {
                    // 클릭된 오브젝트가 나 자신인지 확인
                    if (hitInfo.transform == transform)
                    {
                        Debug.Log("<color=green>우클릭</color> - " + name);
                        CharInfo charInfoMsg = new CharInfo();
                        charInfoMsg.charBase = this;
                        MessageManager.SendMessage(charInfoMsg);
                    }
                }
            }

        }

        protected virtual void SetChar(CharBase character)
        {
            #if UNITY_EDITOR
            MessageManager.SendMessage(new OnSetChar());
            #endif
        }
        
        // Char의 Start시점에 불림
        protected virtual void CharInit()
        {
            SetChar(this);
            
            #region 스킬
            _charSKillInfo = new CharSKillInfo(this);
            if (_charSKillInfo != null)
            {               
                _charSKillInfo.Init(_charData.skill1, _charData.skill2);
            }
            #endregion

            #region 초기 패시브
            foreach (long functionIndex in _charData.func)
            {
                var functiondata = DataManager.Instance.GetData<FunctionData>(functionIndex);
                _functionInfo.AddFunction(new BuffParameter()
                {
                    FunctionIndex = functiondata.Index,
                    CastChar = this,
                    TargetChar = this,
                    eFunctionType = functiondata.function
                });
            }
            #endregion
            
            #region 시너지 등록

            _lightWeightInfo = GetCharSynergyInfo();
            //SynergyManager.Instance.RegisterCharSynergy(_lightWeightInfo);
            
            #endregion
            
            #region 사망 구독
            _charStat.OnDeath += () =>
            {
                Debug.Log($"캐릭터 사망 : uid {_uid}, 이름 {_charData.charKorName}");
                CharDead();
            };
            #endregion
            
            
        }

        public CharLightWeightInfo GetCharSynergyInfo()
        {
            _charSynergies = new List<eSynergy>()
            {
                _charData.synergy1,
                _charData.synergy2,
                _charData.synergy3
            };

            return new CharLightWeightInfo()
            {
                Index = Index,
                Uid = _uid,
                SynergyList = _charSynergies,
                Side = CharType
            };
        }
        
        public bool IsAlive => CharStat.GetStat(eStats.NHP) > 0;
        
        public virtual void CharDead()
        {
            if (_dead) return;
            _dead = true;
            _functionInfo?.Dispose();
            _effectInfo?.Clear();
            gameObject.SetActive(false);
            Type myType = this.GetType();
            CharManager.Instance.Clear(myType, _uid);
            
            #if UNITY_EDITOR
            MessageManager.SendMessage(new OnDeadChar());
            #endif
        }

        public long GetID() => _uid;

        public SystemEnum.eCharType GetCharType()
        {
            return CharType;
        }

        //public void CharMove(Vector2 vector)
        //{
        //    if (vector == Vector2.zero)
        //        return;
        //    if (_charStat == null)
        //    {
        //        Debug.LogWarning($"{transform.name} 의 Stat가 없음");
        //        return;
        //    }
        //    float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
            
        //    // Y축을 기준으로 회전 (2D 평면에서 바라보는 방향)
        //    Vector3 deltaRotation = new Vector3(0, angle, 0);
        //    transform.eulerAngles += deltaRotation * Time.deltaTime;
            
        //    Vector3 deltaMove = transform.forward * _charStat.GetStat(eState.NSpeed);
            
        //    deltaMove = deltaMove * Time.deltaTime;
        //    _NavMeshAgent.Move(deltaMove);
        //}

        ///// <summary>
        ///// 호출 시점 해당 위치로 이동
        ///// </summary>
        ///// <param name="vector"></param>
        //public void CharMoveTo(Vector2 vector)
        //{
        //    if (_NavMeshAgent == false)
        //        return;

        //    _NavMeshAgent.SetDestination(vector);
        //}

        ///// <summary>
        ///// 호출 시점 캐릭터의 위치로 이동
        ///// </summary>
        ///// <param name="targetChar"></param>
        //public void CharMoveTo(CharBase targetChar)
        //{
        //    if (targetChar == false || _NavMeshAgent == false)
        //        return;

        //    _NavMeshAgent.SetDestination(targetChar.transform.position);
        //}

        ///// <summary>
        ///// 호출 시점 캐릭터의 위치로 이동
        ///// </summary>
        ///// <param name="targetChar"></param>
        //public void CharMoveTo(long targetCharUID)
        //{
        //    CharBase charBase = CharManager.Instance.GetFieldChar(targetCharUID);
        //    if (charBase == false)
        //        return;
        //    CharMoveTo(charBase);
        //}
        
        /// <summary>
        /// 캐릭터 AI on & off 기능
        /// </summary>
        public void AISwitch(bool turnOn = true)
        {
            if (CharAI == null) return;
            // CharBase에 함수를 선언하여 CharManager에서(호출하는놈) _cache 내 모든 CharBase를 대상으로 
            // 해당 함수를 호출하게 하면 AI를 키고 끌 수 있다.
            CharAI.isAIRun = turnOn;
            if (turnOn)
            {                
                _coroutine = StartCoroutine(CharAI.UpdateAI(_currentState));
            }
            else if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }

        public void SetNavMeshAgent(bool active)
        {
            if (_NavMeshAgent == false)
                return;
            _NavMeshAgent.enabled = active;
        }

        public void SetStateAnimationIndex(PlayerState state, int index = 0)
        {
            _indexPair[state] = index;
        }
        public void PlayStateAnimation(PlayerState state)
        {
            _charAnim.PlayAnimation(state);
        }

        public void Move(bool move)
        {
            _charAnim.MoveState(move);
        }
        
        public Action OnRealDead;
        public virtual void Dead()
        {
            if (_destroyRequested) return;
            _destroyRequested = true;
            //SynergyManager.Instance.DeleteCharSynergy(_lightWeightInfo);
            CharDead();
            OnRealDead?.Invoke();
            Destroy(gameObject);
        }
        
        protected virtual void OnDestroy()
        {
            _functionInfo?.Dispose();
            _effectInfo?.Clear();
        }

        #region On Editor
        #if UNITY_EDITOR
        void OnMouseDown()
        {
            // 캐릭터 AI 플래이 중 조작 금지
            if (_charAI?.isAIRun ?? true)
                return;
            
            if (_mainCamera == null) return;
            SetNavMeshAgent(false);
            //// 마우스 클릭 위치와 캐릭터 위치의 차이 계산
            //offset = transform.position - GetMouseWorldPosition();
        }
        private void OnMouseDrag()
        {
            // 캐릭터 AI 플래이 중 조작 금지
            if (_charAI?.isAIRun ?? true)
                return;
            if (_mainCamera == null) return;

            // 마우스 위치에 오프셋을 더해서 캐릭터 이동
            transform.position = GetMouseWorldPosition(); //+ offset;

        }
        private void OnMouseUp()
        {
            PlayerMove msg = new();
            msg.beforeTileIndex = TileIndex;
            msg.moveChar = this;
            MessageManager.SendMessage<PlayerMove>(msg);
            SetNavMeshAgent(true);

        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = _mainCamera.WorldToScreenPoint(transform.position).z; // 현재 Z 값 유지
            return _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }
        #endif
        #endregion
    }
}
