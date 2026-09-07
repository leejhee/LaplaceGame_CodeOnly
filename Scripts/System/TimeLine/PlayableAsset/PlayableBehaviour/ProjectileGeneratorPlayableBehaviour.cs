using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using Unity.VisualScripting;

namespace Client
{
    /// <summary>
    /// 스킬용 플레이어블 행동
    /// </summary>
    public class ProjectileGeneratorPlayableBehaviour : SkillTimeLinePlayableBehaviour
    {
        public GameObject ProjectilePrefab { get; set; }       
        public  float Speed { get; set; }
        public float Range {  get; set; }

        public GameObject ProjectileInstance;
        public float elasped = 0f;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (ProjectilePrefab == null) 
                return;
            ProjectileInstance = ObjectManager.Instance.Instantiate(ProjectilePrefab, charBase.transform);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            if (ProjectileInstance == null)
            {
                Debug.LogError("ProjectileInstance is null!");
                return;
            }

            if (elasped < Range)
            {
                float deltaDist = Time.deltaTime * Speed;
                ProjectileInstance.transform.position += Vector3.forward * deltaDist;
                elasped += deltaDist;
            }
            else
            {
                Object.Destroy(ProjectileInstance);
                playable.GetGraph().Stop();
                elasped = 0f;
                Debug.Log("사거리까지 projectile 도달. 소멸");
            }
        }
    }
}