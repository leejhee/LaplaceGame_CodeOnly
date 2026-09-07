using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Client
{
    public class CharMonster : CharBase
    {
        protected override SystemEnum.eCharType CharType => SystemEnum.eCharType.ENEMY;
        protected override void SetChar(CharBase character)
        {
            CharManager.Instance.SetChar<CharMonster>(this);
            base.SetChar(character);
        }
        protected override void CharInit()
        {
            base.CharInit();
            CharTransform.localScale = new Vector3(1, 1, -1);
        }

        public void Patrol()
        {
            //Patrol
        }

        public override void CharDead()
        {
            base.CharDead();
            //DrawItems();
        }

        /// <summary>
        /// CharID를 EnemyID로 변경
        /// </summary>
        /// <param name="charID"></param>
        /// <returns></returns>
        long GetEnemyID(long charID)
        {
            var enemyDataList = DataManager.Instance.GetDataList<EnemyData>();
            foreach (var _enemyData in enemyDataList)
            {
                var enemyData = _enemyData as EnemyData;
                if (enemyData.CharID == charID) return enemyData.Index;
            }
            return -1;
        }

        //void DrawItems()
        //{
        //    List<long> dropList = ItemManager.Instance.SetDropTableList(GetEnemyID(Index));
        //    #region 테스트용 디버그
        //    StringBuilder sb = new();
        //    foreach (long id in dropList)
        //    {
        //        sb.Append($"{id} ");
        //    }
        //    Debug.Log($"{Index}가 드랍할 건 {dropList.Count}개, {sb.ToString()}");
        //    #endregion
        //    ItemManager.Instance.DropItemBeads(this.CharTransform.position, dropList);
        //}

    }
}