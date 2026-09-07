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
    public partial class DropTableGroup : SheetData
    {
public long Index; // ID
		public List<long> fixDrop; // 확정드랍
		public List<long> ranDrop; // 변동드랍
		public int noItem; // 아이템0개나올확률
		public int oneItem; // 아이템1개나올확률
		public int twoItem; // 아이템2개나올확률
		public int threeItem; // 아이템3개나올확률
		

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

                    DropTableGroup data = new DropTableGroup();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					ListStr = values[1].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var fixDropData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.fixDrop = fixDropData;
					
					ListStr = values[2].Replace('[',' ');
					ListStr = ListStr.Replace(']', ' ');
					var ranDropData = ListStr.ToString().Split('.').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
					data.ranDrop = ranDropData;
					
					if(values[3] == "")
					    data.noItem = default;
					else
					    data.noItem = Convert.ToInt32(values[3]);
					
					if(values[4] == "")
					    data.oneItem = default;
					else
					    data.oneItem = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.twoItem = default;
					else
					    data.twoItem = Convert.ToInt32(values[5]);
					
					if(values[6] == "")
					    data.threeItem = default;
					else
					    data.threeItem = Convert.ToInt32(values[6]);
					

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