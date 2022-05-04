using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEditor;

namespace SRH
{
    public class RichTagsWindow : EditorWindow
    {
        public RichTagsText source;

        private const string odinRootFolder = "Assets/SRH/Rich Tags Plus/Integration/";
        private const string rootFolder = "Assets/SRH/Rich Tags Plus/Scripts/";

        [MenuItem("SRH/Rich Tags")]
        static void Init()
        {
            RichTagsWindow window = GetWindow<RichTagsWindow>("Rich Tags Plus");

            window.minSize = new Vector2(550, 85);
            window.maxSize = window.minSize;
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("This will automatically apply the rich text tags to all text parsers in this current scene. Read the ReadMe.txt for more information in the SRH/Rich Tags Plus folder.", MessageType.Info);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rich Tags: ");
            source = (RichTagsText)EditorGUILayout.ObjectField(source, typeof(RichTagsText), true);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Apply Rich Tags to all Text Parsers in this scene"))
            {
                TextParser[] textParsers = Resources.FindObjectsOfTypeAll<TextParser>();

                foreach (TextParser parser in textParsers)
                {
                    if (parser.tags != source)
                        parser.tags = source;
                }
            }
        }
    }
}
