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
    public partial class CharacterSpawnData : SheetData
    {
public long Index; // 
		public long StageID; // StageID
		public long SceneID; // SceneID
		public long CharacterID; // CharacterID
		public int PositionIndex; // PositionIndex
		

        public override Dictionary<long, SheetData> LoadData()
        {
            var dataList = new Dictionary<long, SheetData>();

            string ListStr = null;
			int line = 0;
            TextAsset csvFile = Resources.Load<TextAsset>($"CSV/{this.GetType().Name}");
            try
			{            
                string csvContent = csvFile.text;
                var lines = DSV.SplitRecords(csvContent);
                for (int i = 3; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = DSV.ParseCsv(lines[i]);

                    line = i;

                    CharacterSpawnData data = new CharacterSpawnData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.StageID = default;
					else
					    data.StageID = Convert.ToInt64(values[2]);
					
					if(values[3] == "")
					    data.SceneID = default;
					else
					    data.SceneID = Convert.ToInt64(values[3]);
					
					if(values[4] == "")
					    data.CharacterID = default;
					else
					    data.CharacterID = Convert.ToInt64(values[4]);
					
					if(values[6] == "")
					    data.PositionIndex = default;
					else
					    data.PositionIndex = Convert.ToInt32(values[6]);
					

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