using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Client.SystemEnum;

namespace Client
{
    // 기본 버프 생성 데이터 
    public struct BuffParameter
    {
        public eFunction eFunctionType;
        public CharBase TargetChar;
        public CharBase CastChar;
        public long FunctionIndex;
    }
}