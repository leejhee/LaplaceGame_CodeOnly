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
    public partial class ProjectileData : SheetData
    {
public long Index; // ID
		
		public SystemEnum.eProjectilePathType path; // 날아가는 방식
		
		public SystemEnum.eProjectileRangeType rangeType; // 투사체 데미지 범위 타입
		public int penetrationCount; // 관통 수
		public int splashRange; // 폭발범위
		public string FXPath; // 이펙트 경로
		public string monsterHitPath; // 피격이펙트경로
		

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

                    ProjectileData data = new ProjectileData();

                    
					if(values[0] == "")
					    data.Index = default;
					else
					    data.Index = Convert.ToInt64(values[0]);
					
					if(values[2] == "")
					    data.path = default;
					else
					    data.path = (SystemEnum.eProjectilePathType)Enum.Parse(typeof(SystemEnum.eProjectilePathType), values[2]);
					
					if(values[3] == "")
					    data.rangeType = default;
					else
					    data.rangeType = (SystemEnum.eProjectileRangeType)Enum.Parse(typeof(SystemEnum.eProjectileRangeType), values[3]);
					
					if(values[4] == "")
					    data.penetrationCount = default;
					else
					    data.penetrationCount = Convert.ToInt32(values[4]);
					
					if(values[5] == "")
					    data.splashRange = default;
					else
					    data.splashRange = Convert.ToInt32(values[5]);
					
					if(values[6] == "")
					    data.FXPath = default;
					else
					    data.FXPath = Convert.ToString(values[6]);
					
					if(values[7] == "")
					    data.monsterHitPath = default;
					else
					    data.monsterHitPath = Convert.ToString(values[7]);
					

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