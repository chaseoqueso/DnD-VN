using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SRH
{
    [CreateAssetMenu(fileName = "Rich Tags", menuName = "SRH/New Rich Tags")]
    public class RichTagsText : ScriptableObject
    {
        [System.Serializable]
        private class Tag
        {
            public string tag;
            public string replace;
        }

        [SerializeField] private Tag[] tags;

        [SerializeField] private bool capitializationMatters = false;

        public string GetValue(string tag)
        {
            if (!capitializationMatters)
                tag = tag.ToUpper();

            string word = "";

            for (int i = 0; i < tags.Length; i++)
            {
                if (tag == tags[i].tag.ToUpper())
                {
                    word = tags[i].replace;
                }
            }

            return word;
        }

        public void SetValue(string tag, string value)
        {
            if (!capitializationMatters)
                tag = tag.ToUpper();

            for (int i = 0; i < tags.Length; i++)
            {
                if (tag == tags[i].tag.ToUpper())
                {
                    tags[i].replace = value;
                }
            }
        }
    }
}
