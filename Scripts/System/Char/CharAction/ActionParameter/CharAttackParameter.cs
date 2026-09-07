using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CharAttackParameter : CharActionParameter
    {
        public List<CharBase> targetChar;
        public long skillIndex;
        public CharAI.eAttackMode mode;

        public CharAttackParameter
        (
            List<CharBase> targetCharList,
            CharAI.eAttackMode mode,
            long skillIndex = 0
        )
        {
            this.targetChar = targetCharList;
            this.skillIndex = skillIndex;
            this.mode = mode;
        }

        // 하나만 할 수도 있으니까. 혹시 몰라서...
        public CharAttackParameter
            (
                CharBase targetChar,
                long skillIndex,
                CharAI.eAttackMode mode
            )
        {
            this.targetChar = new List<CharBase>(){ targetChar };
            this.skillIndex = skillIndex;
            this.mode = mode;
        }



    }
}
