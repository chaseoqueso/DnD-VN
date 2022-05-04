using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRH
{
    public class ToggleRichTags : MonoBehaviour
    {
        [SerializeField] private GameObject textParsers;
        [SerializeField] private GameObject nonTextParsers;
        [SerializeField] private TMPro.TextMeshProUGUI toggleText;
        [SerializeField] private TextParser timerParser;
        [SerializeField] private RichTagsText tags;

        private bool _enabled = true;
        private float _timer = 0f;

        private void Start() 
        {
            _timer = float.Parse(tags.GetValue("current_time"));
            textParsers.SetActive(false);
            nonTextParsers.SetActive(false);

            Toggle();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            tags.SetValue("current_time", _timer.ToString("N2"));
            timerParser.ParseRichTags();
        }

        public void Toggle()
        {
            textParsers.SetActive(_enabled);
            _enabled = !_enabled;
            nonTextParsers.SetActive(_enabled);

            if (!_enabled)
                toggleText.text = "Toggle Rich Text Tags Off";
            else
                toggleText.text = "Toggle Rich Text Tags On";
        }
    }
}
