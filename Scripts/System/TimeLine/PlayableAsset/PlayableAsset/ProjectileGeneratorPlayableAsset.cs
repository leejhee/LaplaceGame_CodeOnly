using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Client
{
    public class ProjectileGeneratorPlayableAsset : SkillTimeLinePlayableAsset
    {
        [SerializeField] GameObject ProjectilePrefab;
        [SerializeField] float Range;
        [SerializeField] float Speed;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            base.CreatePlayable(graph, owner);
            ProjectileGeneratorPlayableBehaviour playableBehaviour = new()
            {
                charBase = charBase,
                skillBase = skillBase,
                ProjectilePrefab = ProjectilePrefab,
                Range = Range,
                Speed = Speed
            };
            var scriptPlayable = ScriptPlayable<ProjectileGeneratorPlayableBehaviour>.Create(graph, playableBehaviour);
            return scriptPlayable;
        }

    }
}