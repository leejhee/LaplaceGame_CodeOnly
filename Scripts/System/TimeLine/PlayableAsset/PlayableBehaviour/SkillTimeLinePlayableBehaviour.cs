using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    /// <summary>
    /// 스킬용 플레이어블 행동
    /// </summary>
    public abstract class SkillTimeLinePlayableBehaviour : PlayableBehaviour
    {
        public CharBase charBase;
        public SkillBase skillBase;

        // 생성자를 쓸수는 없고 초기화는 하는게 좋겠고...싶어서 초기화를 일단 virtual로 만들어둠...
        // 또한 디버깅 시 두 요소를 아는 것은 큰 이득도 된다.
        public virtual void InitBehaviour(CharBase charBase, SkillBase skillBase)
        {
            this.charBase = charBase;
            this.skillBase = skillBase;
        }
    }
}