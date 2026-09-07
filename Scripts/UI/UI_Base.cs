using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Client;

namespace Client
{
    public abstract class UI_Base : MonoBehaviour
    {
        /// <summary>
        /// 관리할 산하 오브젝트들
        /// </summary>
        //protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        float setWidth = 3200; // 사용자 설정 너비 (타겟 해상도 해당 해상도에서 비율로 조정)
        float setHeight = 1440; // 사용자 설정 높이 (타겟 해상도 해당 해상도에서 비율로 조정)
        private CanvasScaler canvasScaler;
        /// <summary>
        /// UI 최초 초기화
        /// </summary>
        public virtual void Init()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            SetResolution();
        }
        private void Awake()
        {
            Init();
        }
        
        /// <summary>
        /// UI 비율 조정
        /// </summary>
        public void SetResolution()
        {
            float deviceWidth = Screen.width; // 기기 너비 저장
            float deviceHeight = Screen.height; // 기기 높이 저장

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(setWidth, setHeight);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            if (setWidth / setHeight < deviceWidth / deviceHeight)
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0f;
            }
        }

    }
}