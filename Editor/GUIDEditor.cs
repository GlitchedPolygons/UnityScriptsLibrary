using UnityEngine;
using UnityEditor;

namespace GlitchedPolygons.Identification
{
    /// <summary>
    /// <see cref="GUID"/> component custom inspector.<para> </para>
    /// Automatically checks for duplicates when selected and focussed in the inspector.<para> </para>
    /// If duplicates are found, <see cref="GUID.ChangeGUID()"/> is called.
    /// </summary>
    [CustomEditor(typeof(GUID))]
    public class GUIDEditor : Editor
    {
        /// <summary>
        /// The inspected <see cref="GUID"/> instance.
        /// </summary>
        private GUID guid;

        private SerializedProperty guidString;

        private void OnEnable()
        {
            guid = (GUID)target;
            guidString = serializedObject.FindProperty("guid");

            if (guid.GetComponents<GUID>().Length > 1)
            {
                EditorUtility.DisplayDialog("ERROR!", "There is already a GUID component attached to this GameObject. There may only be one per GameObject!", "K, sorry :(");
                DestroyImmediate(guid);
                return;
            }

            foreach (var _guid in FindObjectsOfType<GUID>())
            {
                if (_guid.GetInstanceID() == guid.GetInstanceID())
                {
                    continue;
                }

                if (string.CompareOrdinal(guid.GetGUID(), _guid.GetGUID()) == 0)
                {
                    Debug.LogWarning("GUID collision: two identical GUIDs detected... reassigning a new one");
                    guid.ChangeGUID();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(guidString.stringValue))
            {
                ((GUID)target)?.ChangeGUID();
            }

            GUI.color = Color.green;
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("GUID:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(guidString.stringValue);
            }
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;

            serializedObject.ApplyModifiedProperties();
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
