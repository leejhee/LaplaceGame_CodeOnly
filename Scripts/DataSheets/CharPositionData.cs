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
    public partial class CharPositionData : SheetData
    {
public long Index; // Index
		
		public SystemEnum.eScene mapScene; // 맵 인덱스
		public int xPos; // X 위치 (만배율)
		public int yPos; // Y 위치 (만배율)
		public int zPos; // Z 위치 (만배율)
		public string charPrefab; // 캐릭터 프리팹 이름
		

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

                    CharPositionData data = new CharPositionData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[4] == "")
					    data.mapScene = default;
					else
					    data.mapScene = (SystemEnum.eScene)Enum.Parse(typeof(SystemEnum.eScene), values[4]);
					
					if(values[5] == "")
					    data.xPos = default;
					else
					    data.xPos = Convert.ToInt32(values[5]);
					
					if(values[6] == "")
					    data.yPos = default;
					else
					    data.yPos = Convert.ToInt32(values[6]);
					
					if(values[7] == "")
					    data.zPos = default;
					else
					    data.zPos = Convert.ToInt32(values[7]);
					
					if(values[8] == "")
					    data.charPrefab = default;
					else
					    data.charPrefab = Convert.ToString(values[8]);
					

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