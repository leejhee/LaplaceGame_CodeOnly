using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Client
{
    [Serializable]
    public partial class EnemyData : SheetData
    {
public long Index; // enemyID
		public string enemyName; // 몬스터 이름
		public long CharID; // CharID
		
		public SystemEnum.eMonsterType MonsterType; // MonsterType
		public long dropTableGroupID; // 드랍테이블그룹아이디
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                var lines = Regex.Split(csvContent, @"\r?\n");
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = CSVParser.Parse(lines[i].Trim());

                    line = i;

                    EnemyData data = new EnemyData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.enemyName = default;
					else
					    data.enemyName = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.CharID = default;
					else
					    data.CharID = Convert.ToInt64(values[2]);
					
					if(values[3] == "")
					    data.MonsterType = default;
					else
					    data.MonsterType = (SystemEnum.eMonsterType)Enum.Parse(typeof(SystemEnum.eMonsterType), values[3]);
					
					if(values[4] == "")
					    data.dropTableGroupID = default;
					else
					    data.dropTableGroupID = Convert.ToInt64(values[4]);
					

                    dataList[data.Index] = data;
                }

                return dataList;
            }
			catch (Exception e)
			{
				Debug.LogError($"{this.GetType().Name}의 {line}전후로 데이터 문제 발생");
				return new Dictionary<long, SheetData>();
			}
        }
    }
}