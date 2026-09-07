using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
namespace Client
{
    /// <summary> 전투 시 타일의 책임에 대한 클래스 </summary>
    public class TileCombatBehaviour : MonoBehaviour
    {
        private HashSet<CharBase> steppingOnChar = new();
        public bool IsEmpty => steppingOnChar.Count == 0;
        
        /// <summary> 타일 간 해시셋의 합집합을 구함. 타일 기반 범위기 처리 </summary>
        /// <param name="charList"> union할 해시셋 </param>
        /// <returns> 현재 타일과 union된 캐릭터 해시셋 </returns>
        public HashSet<CharBase> GetTotalChar(HashSet<CharBase> charList)
        {
            // 복사본으로 원본 오염 방지
            HashSet<CharBase> result = new(steppingOnChar);
            result.UnionWith(charList);
            return result;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out CharBase client))
            {
                if(!steppingOnChar.Contains(client)) 
                    steppingOnChar.Add(client);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharBase client))
            {
                if (steppingOnChar.Contains(client))
                    steppingOnChar.Remove(client);
            }
        }
    }
}