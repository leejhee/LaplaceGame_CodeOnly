using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Client
{
    public class ToastMessageUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txt;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] CanvasGroup panel;
        [SerializeField] float fadeInOutTime = 0.5f;

        private void OnEnable() => ResetVisuals();
        private void OnDisable() => ResetVisuals();
        private void ResetVisuals()
        {
            if (txt) txt.enabled = false;
            if (canvasGroup) { canvasGroup.alpha = 0; canvasGroup.blocksRaycasts = false; }
            if (panel) { panel.alpha = 0; panel.blocksRaycasts = false; }
        }

        private IEnumerator Fade(CanvasGroup target, bool show)
        {
            if (!target) yield break;
            float from = target.alpha;
            float to = show ? 1 : 0;
            float duration = Mathf.Max(0, fadeInOutTime);
            for (float time = 0; time < duration; time += Time.deltaTime)
            {
                target.alpha = Mathf.Lerp(from, to, time / duration);
                yield return null;
            }
            target.alpha = to;
        }

        public IEnumerator ShowMessageCoroutine(string message, Action onMidpoint = null)
        {
            if (!txt) yield break;
            yield return Fade(panel, true);
            txt.text = message;
            txt.enabled = true;
            yield return Fade(canvasGroup, true);
            onMidpoint?.Invoke();
            yield return new WaitForSeconds(Mathf.Max(0, fadeInOutTime));
            yield return Fade(canvasGroup, false);
            txt.enabled = false;
            yield return Fade(panel, false);
        }
    }
}
