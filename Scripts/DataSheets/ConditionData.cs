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
    public partial class ConditionData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eCondition conditionType; // 기능 함수 조건
		public long value1; // 값1
		public long value2; // 값2
		public long value3; // 값3
		public long value4; // 값4
		

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

                    ConditionData data = new ConditionData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.conditionType = default;
					else
					    data.conditionType = (SystemEnum.eCondition)Enum.Parse(typeof(SystemEnum.eCondition), values[2]);
					
					if(values[3] == "")
					    data.value1 = default;
					else
					    data.value1 = Convert.ToInt64(values[3]);
					
					if(values[4] == "")
					    data.value2 = default;
					else
					    data.value2 = Convert.ToInt64(values[4]);
					
					if(values[5] == "")
					    data.value3 = default;
					else
					    data.value3 = Convert.ToInt64(values[5]);
					
					if(values[6] == "")
					    data.value4 = default;
					else
					    data.value4 = Convert.ToInt64(values[6]);
					

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