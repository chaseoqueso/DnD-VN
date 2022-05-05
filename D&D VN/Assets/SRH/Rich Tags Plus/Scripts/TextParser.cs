using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace SRH
{
    [AddComponentMenu("SRH/Rich Tags Plus/Text Parser")]
    public class TextParser : MonoBehaviour
    {
        private Text textComponent;
        private TextMeshProUGUI tmpComponent;

        [SerializeField] private bool startOnAwake = true;
        [SerializeField] private bool overwriteTags = true;

        [HideInInspector] public RichTagsText tags;
        private string text;
        private string currentWord;
        private bool logWord;

        private string newText;
        private bool tagDetected;

        void Awake()
        {
            tagDetected = false;

            if (tags == null)
            {
                Debug.LogError("Rich Tags Plus: Could not load Rich Tags.asset! Open the rich tags window at SRH/Rich Tags and apply it to all text parsers there!", gameObject);
            }

            TryGetComponent<Text>(out textComponent);
            TryGetComponent<TextMeshProUGUI>(out tmpComponent);

            if (startOnAwake)
            {
                GetTextFromComponent(overwriteTags, true);
            }
        }

        private void GetTextFromComponent(bool overwrite, bool awake)
        {
            if (textComponent != null)
            {
                if (awake)
                    text = textComponent.text;
                else if (overwrite)
                        text = textComponent.text;
            }
            else if (tmpComponent != null)
            {
                if (awake)
                    text = tmpComponent.text;
                else if (overwrite)
                    text = tmpComponent.text;
            }
            else
            {
                Debug.LogError("Rich Tags Plus: Could not find any text component on this Game Object!", gameObject);
            }

            ReadText();
        }

        private void ReadText()
        {
            tagDetected = false;

            foreach (char c in text)
            {
                if (logWord)
                {
                    currentWord += c;
                }

                if (c == '<')
                {
                    logWord = true;
                }
                else if (c == '>')
                {
                    logWord = false;

                    currentWord = currentWord.Remove(currentWord.Length - 1);

                    string newWord = tags.GetValue(currentWord);

                    newText += newWord;
                    currentWord = "";

                    if (newWord.Contains("<"))
                    {
                        tagDetected = true;
                    }
                }

                if (!logWord && c != '>')
                {
                    newText += c;
                }
            }

            ApplyText();
        }

        private void ApplyText()
        {
            if (textComponent != null)
            {
                textComponent.text = newText;
                newText = "";
            }
            else if (tmpComponent != null)
            {
                tmpComponent.text = newText;
                newText = "";
            }

            if (tagDetected)
            {
                ReadText();
            }
        }

        public void ParseRichTags()
        {
            GetTextFromComponent(overwriteTags, false);
        }
    }
}
