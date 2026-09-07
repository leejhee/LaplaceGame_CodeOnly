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
    public partial class StringCodeData : SheetData
    {
public long Index; // ID
		public string StringCode; // StringCode
		public string KOR; // 한국어
		public string ENG; // 영어
		

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

                    string[] values = DSV.ParseTsv(lines[i]);
                    line = i;

                    StringCodeData data = new StringCodeData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.StringCode = default;
					else
					    data.StringCode = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.KOR = default;
					else
					    data.KOR = Convert.ToString(values[2]);
					
					if(values[3] == "")
					    data.ENG = default;
					else
					    data.ENG = Convert.ToString(values[3]);
					

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