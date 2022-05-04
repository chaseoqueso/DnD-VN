using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SRH
{
    [CustomEditor(typeof(RichTagsText))]
    public class RichTagsInspector : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Do not inlcude < or > in the tag! Don't worry about capitalization either.", MessageType.Info);

            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
