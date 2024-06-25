using System;
using System.Collections;
using UnityEngine;
using Lucky.UI.TMP_Text.TextEffect;

namespace Lucky.UI.TMP_Text
{
    [RequireComponent(typeof(TextEffectController))]
    public class DialogueText : MonoBehaviour
    {
        private TextEffectController textEffectController;
        public KeyCode nextKey = KeyCode.Return;

        public float dialogueSpeed = 10;
        private bool isSkipping;
        public bool isTalking;

        public Action onDialogueOver;
        public bool isFadeText;

        private void Awake()
        {
            textEffectController = GetComponent<TextEffectController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(nextKey))
            {
                isSkipping = true;
            }
        }

        public void ShowDialogues(params Dialogue_SO[] dialogue_SOList)
        {
            StartCoroutine(_ShowDialogues(dialogue_SOList));
        }

        private IEnumerator _ShowDialogues(params Dialogue_SO[] dialogue_SOList)
        {
            isTalking = true;
            foreach (var dialogueSO in dialogue_SOList)
            {
                yield return StartCoroutine(ShowDialogue(dialogueSO));
            }

            isTalking = false;
            onDialogueOver?.Invoke();
        }

        private IEnumerator ShowDialogue(Dialogue_SO dialogue_SO)
        {
            foreach (var content in dialogue_SO.contents)
            {
                yield return ShowCharOneByOne(content);
                yield return new WaitUntil(() => Input.GetKeyDown(nextKey));
            }
        }

        private IEnumerator ShowCharOneByOne(string content)
        {
            var dialogueSpeed = this.dialogueSpeed;
            textEffectController.showCharNum = 0;
            textEffectController.RawContent = content;
            // yield return new WaitForSeconds(0.1f);
            // yield return new WaitForEndOfFrame();
            // yield return new WaitForEndOfFrame();

            isSkipping = false;
            for (float i = 0; i <= textEffectController.ParsedString.Length; i += isFadeText ? 0.5f : 1)
            {
                textEffectController.showCharNum = i;
                if (textEffectController.charPosToEventInfo.ContainsKey((int)i))
                {
                    var info = textEffectController.charPosToEventInfo[(int)i];
                    if (info.ContainsKey("speed"))
                    {
                        dialogueSpeed = float.Parse(info["speed"]);
                    }

                    if (info.ContainsKey("delay"))
                    {
                        yield return new WaitForSeconds(float.Parse(info["delay"]));
                    }
                }

                if (isSkipping)
                    break;
                if (isFadeText)
                {
                    yield return new WaitForSeconds(1 / dialogueSpeed / 2);
                    i += 0.5f;
                    textEffectController.showCharNum = i;
                    yield return new WaitForSeconds(1 / dialogueSpeed / 2);
                }
                else
                {
                    yield return new WaitForSeconds(1 / dialogueSpeed);
                }
            }

            textEffectController.showCharNum = textEffectController.ParsedString.Length;
        }
    }
}