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
    public partial class SkillData : SheetData
    {
public long Index; // ID
		public string nameStringCode; // 이름 스트링 코드
		public string desStringCode; // 설명 스트링 코드
		public int skillRange; // 스킬 사거리
		
		public SystemEnum.eSkillTargetType skillTarget; // 스킬 대상
		public string animPath; // 애니메이션 경로
		public string fxPath; // 이펙트 경로
		public string sfxPath; // SFX 경로
		

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

                    SkillData data = new SkillData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[1] == "")
					    data.nameStringCode = default;
					else
					    data.nameStringCode = Convert.ToString(values[1]);
					
					if(values[2] == "")
					    data.desStringCode = default;
					else
					    data.desStringCode = Convert.ToString(values[2]);
					
					if(values[4] == "")
					    data.skillRange = default;
					else
					    data.skillRange = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.skillTarget = default;
					else
					    data.skillTarget = (SystemEnum.eSkillTargetType)Enum.Parse(typeof(SystemEnum.eSkillTargetType), values[5]);
					
					if(values[7] == "")
					    data.animPath = default;
					else
					    data.animPath = Convert.ToString(values[7]);
					
					if(values[8] == "")
					    data.fxPath = default;
					else
					    data.fxPath = Convert.ToString(values[8]);
					
					if(values[9] == "")
					    data.sfxPath = default;
					else
					    data.sfxPath = Convert.ToString(values[9]);
					

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