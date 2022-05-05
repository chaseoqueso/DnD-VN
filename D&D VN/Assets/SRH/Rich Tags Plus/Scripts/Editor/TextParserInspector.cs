using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SRH
{
    [CustomEditor(typeof(TextParser))]
    public class TextParserInspector : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
