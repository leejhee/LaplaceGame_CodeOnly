using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    /// <summary> 스킬용 플레이어블 에셋 </summary>
    /// <remarks> </remarks>
    public abstract class SkillTimeLinePlayableAsset : PlayableAsset
    {
        protected CharBase charBase = null;
        protected SkillBase skillBase = null;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            //skillBase = owner.GetComponent<SkillBase>();
            //charBase = skillBase.CharPlayer;

            return new Playable();
        }

    }
}
