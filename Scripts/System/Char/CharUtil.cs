using System;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    public class CharComparer : IEqualityComparer<CharBase>
    {
        public bool Equals(CharBase x, CharBase y)
        {
            if (x == null || y == null) return false;
            return x.Index == y.Index;
        }

        public int GetHashCode(CharBase obj)
        {
            return obj.Index.GetHashCode();
        }
    }

    public class CharUtil
    {
        /// <summary>
        /// 캐릭터 리스트에서 특정 거리 순위에 있는 캐릭터를 반환한다.
        /// </summary>
        /// <param name="clientChar">기준 캐릭터</param>
        /// <param name="targetList">후보군 캐릭터 리스트</param>
        /// <param name="nTH">몇 번째?</param>
        /// <param name="inverse">true면 멀리서부터.</param>
        public static CharBase GetNearestInList(CharBase clientChar, List<CharBase> targetList, int nTH = 0, bool inverse = false)
        {
            if(clientChar == false)
            {
                Debug.LogError("Client is null.");
                return null;
            }
            Vector3 clientPosition = clientChar.CharTransform.position;
            return GetNearestInList(clientPosition, targetList, nTH, inverse);
        }

        public static CharBase GetNearestInList(Vector3 abstractPos, eCharType type, int nTH = 0, bool inverse = false)
        {
            List<CharBase> list = CharManager.Instance.GetOneSide(type);
            return GetNearestInList(abstractPos, list, nTH, inverse);
        }

        private static CharBase GetNearestInList(Vector3 abstractPos, List<CharBase> targetList, int nTH = 0, bool inverse = false)
        {
            if(nTH >= targetList.Count || nTH < 0)
            {
                Debug.LogError("Wrong Input");
                return null;
            }
            
            var distances = new List<(CharBase target, float dist)>(targetList.Count);

            for (int i = 0; i< targetList.Count; i++)
            {
                if (!targetList[i]) continue;
                float distSqr = (abstractPos - targetList[i].CharTransform.position).sqrMagnitude;
                distances.Add((targetList[i], distSqr));
            }

            if (inverse)
                distances.Sort((a, b) => b.dist.CompareTo(a.dist));
            else
                distances.Sort((a, b) => a.dist.CompareTo(b.dist));

            CharBase result;
            try
            {
                result = distances[nTH].target;
            }
            catch (ArgumentOutOfRangeException ae)
            {
                result = null;
                Debug.LogError(ae.Message + $"\nSize of Collection : {targetList.Count} / Tried to Access : {nTH}");
            }
            return result;
        }
        
        
        public static eCharType GetEnemyType(eCharType clientType)
        {
            return clientType switch
            {
                eCharType.ALLY or eCharType.NEUTRAL => eCharType.ENEMY,
                eCharType.ENEMY => eCharType.ALLY,
                _ => eCharType.None
            };
        }
    }

}