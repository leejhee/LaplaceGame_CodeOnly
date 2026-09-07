using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Client
{
    /// <summary>
    /// Popup UI를 위한 데이터 타입
    /// </summary>
    public abstract class UI_Popup : UI_Base
    {
        /// <summary> 정렬 설정 </summary>
        public override void Init()
        {
            base.Init();
            UIManager.Instance.SetCanvas(gameObject, true);
        }

        /// <summary> pop up 다시 열 때마다 실행</summary>
        public virtual void ReOpenPopupUI() { }

        public virtual void SetParameter(UIParameter param) { }
    }
}