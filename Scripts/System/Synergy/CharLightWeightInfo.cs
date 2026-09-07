using System;
using System.Collections.Generic;

namespace Client
{
    public struct CharLightWeightInfo : IEquatable<CharLightWeightInfo>
    {
        public long Index;
        public long Uid;
        public SystemEnum.eCharType Side;
        public List<SystemEnum.eSynergy> SynergyList;

        public readonly CharBase SpecifyCharBase()
        {
            return CharManager.Instance.GetFieldChar(Uid);
        }

        public bool Equals(CharLightWeightInfo other)
        {
            return Index == other.Index && Uid == other.Uid && Side == other.Side && Equals(SynergyList, other.SynergyList);
        }

        public override bool Equals(object obj)
        {
            return obj is CharLightWeightInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, Uid, (int)Side, SynergyList);
        }
    }
}