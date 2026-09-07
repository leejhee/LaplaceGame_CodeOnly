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
    public partial class SynergyData : SheetData
    {
public long Index; // synergyID
		public long functionIndex; // 기능
		
		public SystemEnum.eBuffTriggerTime buffTriggerTime; // 시전 시작 시간
		
		public SystemEnum.eSynergyRange casterType; // 시전자
		
		public SystemEnum.eSkillTargetType skillTarget; // 스킬 대상
		
		public SystemEnum.eSynergy synergy; // 시너지
		public int synergyCount; // 인원 수
		public string synergyIcon; // 아이콘
		public string synergyIconColor; // 아이콘 색상
		public string nameStringCode; // 이름
		public string desStringCode; // 설명
		

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

                    SynergyData data = new SynergyData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.functionIndex = default;
					else
					    data.functionIndex = Convert.ToInt64(values[1]);
					
					if(values[2] == "")
					    data.buffTriggerTime = default;
					else
					    data.buffTriggerTime = (SystemEnum.eBuffTriggerTime)Enum.Parse(typeof(SystemEnum.eBuffTriggerTime), values[2]);
					
					if(values[3] == "")
					    data.casterType = default;
					else
					    data.casterType = (SystemEnum.eSynergyRange)Enum.Parse(typeof(SystemEnum.eSynergyRange), values[3]);
					
					if(values[4] == "")
					    data.skillTarget = default;
					else
					    data.skillTarget = (SystemEnum.eSkillTargetType)Enum.Parse(typeof(SystemEnum.eSkillTargetType), values[4]);
					
					if(values[5] == "")
					    data.synergy = default;
					else
					    data.synergy = (SystemEnum.eSynergy)Enum.Parse(typeof(SystemEnum.eSynergy), values[5]);
					
					if(values[6] == "")
					    data.synergyCount = default;
					else
					    data.synergyCount = Convert.ToInt32(values[6]);
					
					if(values[7] == "")
					    data.synergyIcon = default;
					else
					    data.synergyIcon = Convert.ToString(values[7]);
					
					if(values[8] == "")
					    data.synergyIconColor = default;
					else
					    data.synergyIconColor = Convert.ToString(values[8]);
					
					if(values[9] == "")
					    data.nameStringCode = default;
					else
					    data.nameStringCode = Convert.ToString(values[9]);
					
					if(values[10] == "")
					    data.desStringCode = default;
					else
					    data.desStringCode = Convert.ToString(values[10]);
					

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