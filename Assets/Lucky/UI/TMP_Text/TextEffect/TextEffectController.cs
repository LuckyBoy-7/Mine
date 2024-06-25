using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lucky.Collections;
using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.TMP_Text.TextEffect
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class TextEffectController : MonoBehaviour
    {
        private TMPro.TMP_Text tmpText;
        public HyperLink hyperLink;
        private TMP_TextInfo TextInfo => tmpText.textInfo;

        public float simulationSpeed = 20;

        // [Title("Common")] public float simulationSpeed = 20;
        public float showCharNum;


        // 有odin调试的时候可以开
        // [ShowInInspector] public string ParsedString { get; set; }
        public string ParsedString { get; set; } = "";

        // 方便调试
        [Multiline, SerializeField] private string rawContent = "";

        public string RawContent
        {
            get => rawContent;
            set
            {
                rawContent = value;
                charPosToEventInfo.Clear();
                parsedRes.Clear();
                ParseString(); // 设置的时候就要parse，不然update执行顺序会滞后，逻辑错误
                AdjustCharactersVisibility();
            }
        }

        private string preRawContent = ""; // 只是方便调试


        /// 对应字符位置拿到单个标签的信息如<speed=11>, <delay=0.5>
        /// 搭配dialogueManager食用
        public DefaultDict<int, Dictionary<string, string>> charPosToEventInfo = new(() => new());

        private List<ParsedInfo> parsedRes = new();
        private List<TextEffectBase> textEffects = new();
        private Dictionary<TextEffectType, TextEffectBase> textEffectTypeToTextEffect = new();

        void Awake()
        {
            tmpText = gameObject.GetComponent<TMPro.TMP_Text>();
            foreach (TextEffectBase textEffect in new List<TextEffectBase>
                     {
                         new EventEffect(),
                         new ShakeEffect(),
                         new FloatEffect(),
                         new JumpEffect(),
                         new JitterEffect(),
                         new NoneEffect()
                     })
            {
                textEffects.Add(textEffect);
                textEffectTypeToTextEffect[textEffect.textEffectType] = textEffect;
                textEffect.tmpText = tmpText;
            }
        }

        private void Start()
        {
            StartCoroutine(StartEffect());
        }

        private void SetText(string content)
        {
            tmpText.text = "";
            tmpText.text = content;
            tmpText.ForceMeshUpdate(); // 不这么做AdjustCharactersVisibility拿不到正确网格了
        }

        private IEnumerator StartEffect()
        {
            while (true)
            {
                if (RawContent != preRawContent) // 方便测试
                    RawContent = rawContent;
                tmpText.ForceMeshUpdate();
                foreach (var parsedInfo in parsedRes)
                    textEffectTypeToTextEffect[parsedInfo.textEffectType].TakeEffect(parsedInfo);
                AdjustCharactersVisibility();

                // Update the mesh vertex data
                tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

                yield return new WaitForSeconds(1 / simulationSpeed);
            }
        }

        private void AdjustCharactersVisibility()
        {
            // 不知道为什么空格就是有bug
            // 管理字符显隐
            int j = ParsedString.LastIndexOf(' ');
            for (int i = 0; i < ParsedString.Length; i++)
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[i]; // 拿到单个字符信息
                int vertexIndex = charInfo.vertexIndex;
                Color32 color = TextInfo.meshInfo[0].colors32[vertexIndex + 0];
                // color.a = i < Mathf.Max(0, showCharNum) || i == ParsedString.Length - 1 ? Convert.ToByte(255) : Convert.ToByte(0);
                if (i == showCharNum - 0.5f)
                {
                    print(123);
                    TextInfo.meshInfo[0].colors32[vertexIndex + 0] = color;
                    TextInfo.meshInfo[0].colors32[vertexIndex + 1] = color;
                    color.a = 0;
                    TextInfo.meshInfo[0].colors32[vertexIndex + 2] = color;
                    TextInfo.meshInfo[0].colors32[vertexIndex + 3] = color;
                }
                else
                {
                    color.a = i < Mathf.Max(0, showCharNum) || i == j ? Convert.ToByte(255) : Convert.ToByte(0);
                    SetColor(vertexIndex, color);
                }
            }

            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void SetColor(int idx, Color32 color)
        {
            TextInfo.meshInfo[0].colors32[idx + 0] = color;
            TextInfo.meshInfo[0].colors32[idx + 1] = color;
            TextInfo.meshInfo[0].colors32[idx + 2] = color;
            TextInfo.meshInfo[0].colors32[idx + 3] = color;
        }


        private void ParseString()
        {
            StringBuilder s = new StringBuilder(rawContent);
            List<ParsedInfo> ranges = new(); // 首位位置，字符串长度，额外信息

            foreach (var textEffect in textEffects)
                textEffect.ParseAndCover(s, ranges);
            // 到此为止s里面只剩下占位符了
            ranges.Sort((a, b) => a.start - b.start);
            int idx = 0; // 最后要赋值给tmp对应的idx
            int realIdx = 0; // 去除所有标签单纯时文本对应的idx
            foreach (ParsedInfo parsedInfo in ranges)
            {
                if (parsedInfo.textEffectType == TextEffectType.Event)
                {
                    charPosToEventInfo[realIdx].Update(parsedInfo.args); // merge一下
                    continue;
                }

                SetText(rawContent.Substring(parsedInfo.start, parsedInfo.length));
                int realLength = tmpText.GetParsedText().Length;
                parsedRes.Add(new(parsedInfo.textEffectType, realIdx, realLength, parsedInfo.args));

                realIdx += realLength;
                for (int i = 0; i < parsedInfo.length; i++)
                    s[idx++] = rawContent[parsedInfo.start + i];
            }

            SetText(s.ToString().Substring(0, idx));
            preRawContent = rawContent;
            ParsedString = tmpText.GetParsedText();
            hyperLink?.UpdateLink();
        }
    }
}