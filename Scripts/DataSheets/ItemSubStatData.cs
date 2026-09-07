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
    public partial class ItemSubStatData : SheetData
    {
public long Index; // itemID
		public int proWeight; // 확률가중치
		
		public SystemEnum.eStats subStats; // 서브스탯이름
		public int min; // 최소값
		public int max; // 최댓값
		public string stringCode; // 설명텍스트
		

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

                    ItemSubStatData data = new ItemSubStatData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.proWeight = default;
					else
					    data.proWeight = Convert.ToInt32(values[1]);
					
					if(values[2] == "")
					    data.subStats = default;
					else
					    data.subStats = (SystemEnum.eStats)Enum.Parse(typeof(SystemEnum.eStats), values[2]);
					
					if(values[3] == "")
					    data.min = default;
					else
					    data.min = Convert.ToInt32(values[3]);
					
					if(values[4] == "")
					    data.max = default;
					else
					    data.max = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.stringCode = default;
					else
					    data.stringCode = Convert.ToString(values[5]);
					

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