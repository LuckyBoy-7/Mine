using System.Collections;
// using DG.Tweening;
using Lucky.Managers;
using UnityEngine;

namespace Lucky.UI.TMP_Text
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public Dialogue_SO testDialogue;
        public float openPanelDuration;
        public CanvasGroup canvasGroup;
        public GameObject dialoguePanel;
        public DialogueText dialogueText;

        private void Start()
        {
            dialogueText.ShowDialogues(testDialogue);
        }

        private IEnumerator OpenPanel()
        {
            dialoguePanel.SetActive(true);
            // 伸缩
            // var scale = transform.localScale;
            // transform.localScale = new Vector3(scale.x, 0, scale.z);
            // transform.DOScaleY(1, openPanelDuration);
            //
            // // 淡入
            // canvasGroup.alpha = 0;
            // canvasGroup.DOFade(1, openPanelDuration);
            // yield return new WaitForSeconds(openPanelDuration);

            yield break;
        }

        private IEnumerator ClosePanel()
        {
            dialoguePanel.SetActive(false);
            // 伸缩
            // var scale = transform.localScale;
            // transform.localScale = new Vector3(scale.x, 1, scale.z);
            // transform.DOScaleY(0, openPanelDuration);
            //
            // // 淡出
            // canvasGroup.alpha = 1;
            // canvasGroup.DOFade(0, openPanelDuration).OnComplete(() =>
            //     {
            //         textEffect.RawContent = "";
            //         dialoguePanel.SetActive(false);
            //     }
            // );
            // yield return new WaitForSeconds(openPanelDuration);
            yield break;
        }
    }
}