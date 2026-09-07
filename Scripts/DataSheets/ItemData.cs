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
    public partial class ItemData : SheetData
    {
public long Index; // itemID
		public string nameStringCode; // 이름 스트링 코드
		
		public SystemEnum.eItemType itemType; // 아이템타입
		public string iconImagePath; // 아이템 아이콘
		
		public SystemEnum.eTier itemTier; // 아이템 티어
		
		public SystemEnum.eStats mainStats; // 메인스탯이름
		public int mainStatsIncrease; // 메인 스탯증가량(만분율)
		public int subStatsCount; // 서브스탯수
		
		public SystemEnum.eFunction func; // 특수효과
		public string descID; // 설명ID
		public string mainOpStringCode; // 메인스탯 스트링코드
		

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

                    ItemData data = new ItemData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.nameStringCode = default;
					else
					    data.nameStringCode = Convert.ToString(values[1]);
					
					if(values[3] == "")
					    data.itemType = default;
					else
					    data.itemType = (SystemEnum.eItemType)Enum.Parse(typeof(SystemEnum.eItemType), values[3]);
					
					if(values[4] == "")
					    data.iconImagePath = default;
					else
					    data.iconImagePath = Convert.ToString(values[4]);
					
					if(values[5] == "")
					    data.itemTier = default;
					else
					    data.itemTier = (SystemEnum.eTier)Enum.Parse(typeof(SystemEnum.eTier), values[5]);
					
					if(values[6] == "")
					    data.mainStats = default;
					else
					    data.mainStats = (SystemEnum.eStats)Enum.Parse(typeof(SystemEnum.eStats), values[6]);
					
					if(values[7] == "")
					    data.mainStatsIncrease = default;
					else
					    data.mainStatsIncrease = Convert.ToInt32(values[7]);
					
					if(values[8] == "")
					    data.subStatsCount = default;
					else
					    data.subStatsCount = Convert.ToInt32(values[8]);
					
					if(values[9] == "")
					    data.func = default;
					else
					    data.func = (SystemEnum.eFunction)Enum.Parse(typeof(SystemEnum.eFunction), values[9]);
					
					if(values[10] == "")
					    data.descID = default;
					else
					    data.descID = Convert.ToString(values[10]);
					
					if(values[11] == "")
					    data.mainOpStringCode = default;
					else
					    data.mainOpStringCode = Convert.ToString(values[11]);
					

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