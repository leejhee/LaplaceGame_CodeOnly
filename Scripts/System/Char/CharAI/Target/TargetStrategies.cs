using Client;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// ENUM 구현 : SELF
    /// </summary>
    public class SelfTargetStrategy : TargettingStrategyBase
    {
        public SelfTargetStrategy(TargettingStrategyParameter param) : base(param){ }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>() { Caster };
        }
    }

    public class NearEnemyTargetStrategy : TargettingStrategyBase
    {
        public NearEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>() { CharManager.Instance.GetNearestEnemy(Caster)};
        }
    }

    public class NearEnemy2TargetStrategy : TargettingStrategyBase
    {
        public NearEnemy2TargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster),
                CharManager.Instance.GetNearestEnemy(Caster, 1)
            };
        }
    }

    public class NearEnemy3TargetStrategy : TargettingStrategyBase
    {
        public NearEnemy3TargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster),
                CharManager.Instance.GetNearestEnemy(Caster, 1),
                CharManager.Instance.GetNearestEnemy(Caster, 2),
            };
        }
    }

    public class FarthestEnemyTargetStrategy : TargettingStrategyBase
    {
        public FarthestEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster, 0, inverse: true)
            };
        }
    }
    public class FarthestEnemy2TargetStrategy : TargettingStrategyBase
    {
        public FarthestEnemy2TargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestEnemy(Caster, 1, inverse: true),
                CharManager.Instance.GetNearestEnemy(Caster, 0, inverse: true)
            };
        }
    }
    
    public class CurrentEnemyTargetStrategy : TargettingStrategyBase
    {
        public CurrentEnemyTargetStrategy(TargettingStrategyParameter param) : base(param) { }

        public override List<CharBase> GetTargets()
        {
            if (!Caster.CharAI.isAIRun)
            {
                //Debug.LogWarning("현재는 AI가 작동하지 않으므로 타겟을 반환하지 않습니다.");
                //return new List<CharBase>();
            }

            CharBase current = Caster.CharAI.FinalTarget;
            if (current == false)
                current = CharManager.Instance.GetNearestEnemy(Caster);
            return new List<CharBase>()
            {
                current
            };
        }
    }

    public class CurrentEnemyNear1TargetStrategy : TargettingStrategyBase
    {
        public CurrentEnemyNear1TargetStrategy(TargettingStrategyParameter param) : base(param) {}

        public override List<CharBase> GetTargets()
        {
            var current = Caster.CharAI.FinalTarget;
            if (!current)
            {
                current = CharManager.Instance.GetNearestEnemy(Caster);
            }
            var targets = CharManager.Instance.GetBunches(current, SystemConst.TILE_UNIT_LENGTH);

            targets.Add(current);
            return targets;
        }
    }

    public class Near1UnitEnemyTargetStrategy : TargettingStrategyBase
    {
        public Near1UnitEnemyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            var bunches = CharManager.Instance.GetBunches(Caster, SystemConst.TILE_UNIT_LENGTH, false);
            return bunches;
        }
    }

    public class CurrentCloseEnemy2TargetStrategy : TargettingStrategyBase
    {
        public CurrentCloseEnemy2TargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }
        
        public override List<CharBase> GetTargets()
        {
            CharBase current = Caster.CharAI.FinalTarget;
            if (current == false)
                current = CharManager.Instance.GetNearestEnemy(Caster);
            List<CharBase> targets = new()
            {
                CharManager.Instance.GetNearestAlly(current, 1),
                CharManager.Instance.GetNearestAlly(current, 2),
                current
            };
            return targets;
        }
    }
    
    public class NearAllyTargetStrategy : TargettingStrategyBase
    {
        public NearAllyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {}

        public override List<CharBase> GetTargets()
        {
            CharBase nearAlly = CharManager.Instance.GetNearestAlly(Caster, 1);
            if (!nearAlly) return new();
            return new() { nearAlly };
        }
    }
    
    public class NearAlly2TargetStrategy : TargettingStrategyBase
    {
        public NearAlly2TargetStrategy(TargettingStrategyParameter param) : base(param)
        {}

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestAlly(Caster, 1), 
                CharManager.Instance.GetNearestAlly(Caster, 2)
            };
        }
    }
    
    public class NearAlly3TargetStrategy : TargettingStrategyBase
    {
        public NearAlly3TargetStrategy(TargettingStrategyParameter param) : base(param)
        {}

        public override List<CharBase> GetTargets()
        {
            return new List<CharBase>()
            {
                CharManager.Instance.GetNearestAlly(Caster, 1), 
                CharManager.Instance.GetNearestAlly(Caster, 2),
                CharManager.Instance.GetNearestAlly(Caster, 3)
            };
        }
    }

    public class LowHPEnemyTargetStrategy : TargettingStrategyBase
    {
        public LowHPEnemyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            List<CharBase> result = new();
            var targets = CharManager.Instance.GetEnemySide(Caster.GetCharType());
            targets.Sort((a, b) =>
            {
                float da = a.CharStat.GetStat(SystemEnum.eStats.NHP);
                float db = b.CharStat.GetStat(SystemEnum.eStats.NHP);
                return da.CompareTo(db);
            });
            if(targets.Count >= 1)
                result.Add(targets[0]);
            return result;
        }
    }
    
    public class LowHPAllyTargetStrategy : TargettingStrategyBase
    {
        public LowHPAllyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            List<CharBase> result = new();
            var targets = CharManager.Instance.GetAllySide(Caster.GetCharType());
            targets.Sort((a, b) =>
            {
                float da = a.CharStat.GetStat(SystemEnum.eStats.NHP);
                float db = b.CharStat.GetStat(SystemEnum.eStats.NHP);
                return da.CompareTo(db);
            });
            if(targets.Count >= 2)
                result.Add(targets[1]);
            return result;
        }
    }
    
    public class LowHPAlly2TargetStrategy : TargettingStrategyBase
    {
        public LowHPAlly2TargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            List<CharBase> result = new();
            var targets = CharManager.Instance.GetAllySide(Caster.GetCharType());
            targets.Sort((a, b) =>
            {
                float da = a.CharStat.GetStat(SystemEnum.eStats.NHP);
                float db = b.CharStat.GetStat(SystemEnum.eStats.NHP);
                return da.CompareTo(db);
            });
            if (targets.Count >= 3) result.Add(targets[2]);
            if (targets.Count >= 2) result.Add(targets[1]);
            
            return result;
        }
    }
    
    
    public class EveryAllyTargetStrategy : TargettingStrategyBase
    {
        public EveryAllyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            return CharManager.Instance.GetAllySide(Caster.GetCharType());
        }
    }

    public class EveryEnemyTargetStrategy : TargettingStrategyBase
    {
        public EveryEnemyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            return CharManager.Instance.GetEnemySide(Caster.GetCharType());
        }
    }

    public class RandomEnemy3TargetStrategy : TargettingStrategyBase
    {
        public RandomEnemy3TargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            var bunches = CharManager.Instance.GetEnemySide(Caster.GetCharType());
            List<int> indices = new();
            for(int i = 0; i < bunches.Count; i++) indices.Add(i);
            List<CharBase> result = new();
            for (int trial = 0; trial < 3; trial++)
            {
                if (indices.Count == 0) break;

                int randIdx = Random.Range(0, indices.Count);
                int chosenIndex = indices[randIdx];
                result.Add(bunches[chosenIndex]);

                indices.RemoveAt(randIdx); 
            }
            return result;
        }
    }

    public class Row1AllyTargetStrategy : TargettingStrategyBase
    {
        public Row1AllyTargetStrategy(TargettingStrategyParameter param) : base(param)
        {
        }

        public override List<CharBase> GetTargets()
        {
            return TileManager.Instance.GetColCharacters(Caster, true, 1);
        }
    }
}


