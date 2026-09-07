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
    public partial class DropTable : SheetData
    {
public long Index; // ID
		public long itemID; // 아이템 ID
		public int amount; // 수량
		public int prob; // 확률
		
		public SystemEnum.eItemTier beadTier; // 구슬 등급
		public string beadColor; // 구슬 색상
		

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

                    DropTable data = new DropTable();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.itemID = default;
					else
					    data.itemID = Convert.ToInt64(values[1]);
					
					if(values[4] == "")
					    data.amount = default;
					else
					    data.amount = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.prob = default;
					else
					    data.prob = Convert.ToInt32(values[5]);
					
					if(values[6] == "")
					    data.beadTier = default;
					else
					    data.beadTier = (SystemEnum.eItemTier)Enum.Parse(typeof(SystemEnum.eItemTier), values[6]);
					
					if(values[7] == "")
					    data.beadColor = default;
					else
					    data.beadColor = Convert.ToString(values[7]);
					

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