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
    public partial class TierColorData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eTier tier; // 등급
		public string hexColorForItemDes; // 아이템창 설명용 색상
		

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

                    TierColorData data = new TierColorData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.tier = default;
					else
					    data.tier = (SystemEnum.eTier)Enum.Parse(typeof(SystemEnum.eTier), values[1]);
					
					if(values[2] == "")
					    data.hexColorForItemDes = default;
					else
					    data.hexColorForItemDes = Convert.ToString(values[2]);
					

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