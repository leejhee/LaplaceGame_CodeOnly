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
    public partial class FunctionData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eFunction function; // 기능
		
		public SystemEnum.eStats statsType; // 스테이트 타입
		
		public SystemEnum.eDamageType damageType; // 데미지 타입
		
		public SystemEnum.eCCType CCType; // CC 타입
		public long time; // 시간(MS)천분율
		public long input1; // input1
		public long input2; // input2
		public long input3; // input3
		public long input4; // input4
		public long input5; // input5
		public long ConditionCheck; // 조건
		public List<long> ConditionFuncList; // 조건부Func
		

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

                    FunctionData data = new FunctionData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[3] == "")
					    data.function = default;
					else
					    data.function = (SystemEnum.eFunction)Enum.Parse(typeof(SystemEnum.eFunction), values[3]);
					
					if(values[4] == "")
					    data.statsType = default;
					else
					    data.statsType = (SystemEnum.eStats)Enum.Parse(typeof(SystemEnum.eStats), values[4]);
					
					if(values[5] == "")
					    data.damageType = default;
					else
					    data.damageType = (SystemEnum.eDamageType)Enum.Parse(typeof(SystemEnum.eDamageType), values[5]);
					
					if(values[6] == "")
					    data.CCType = default;
					else
					    data.CCType = (SystemEnum.eCCType)Enum.Parse(typeof(SystemEnum.eCCType), values[6]);
					
					if(values[7] == "")
					    data.time = default;
					else
					    data.time = Convert.ToInt64(values[7]);
					
					if(values[8] == "")
					    data.input1 = default;
					else
					    data.input1 = Convert.ToInt64(values[8]);
					
					if(values[9] == "")
					    data.input2 = default;
					else
					    data.input2 = Convert.ToInt64(values[9]);
					
					if(values[10] == "")
					    data.input3 = default;
					else
					    data.input3 = Convert.ToInt64(values[10]);
					
					if(values[11] == "")
					    data.input4 = default;
					else
					    data.input4 = Convert.ToInt64(values[11]);
					
					if(values[12] == "")
					    data.input5 = default;
					else
					    data.input5 = Convert.ToInt64(values[12]);
					
					if(values[13] == "")
					    data.ConditionCheck = default;
					else
					    data.ConditionCheck = Convert.ToInt64(values[13]);
					
					ListStr = values[14].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var ConditionFuncListData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.ConditionFuncList = ConditionFuncListData;
					

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