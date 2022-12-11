using UnityEngine;
using UnityEditor;

namespace GlitchedPolygons.Identification
{
    /// <summary>
    /// Custom inspector for the <see cref="ID"/> component; automatically checks if you have duplicate IDs and reassigns accordingly.
    /// Implements the <see cref="UnityEditor.Editor" />
    /// </summary>
    /// <seealso cref="UnityEditor.Editor" />
    [CustomEditor(typeof(ID))]
    public class IDEditor : Editor
    {
        /// <summary>
        /// The inspected <see cref="ID"/> instance.
        /// </summary>
        private ID id;

        private SerializedProperty idInt;

        private void OnEnable()
        {
            id = (ID)target;
            idInt = serializedObject.FindProperty("id");

            if (id.GetComponents<ID>().Length > 1)
            {
                EditorUtility.DisplayDialog("ERROR!", "There is already an ID component attached to this GameObject. There may only be one per GameObject!", "K, sorry :(");
                DestroyImmediate(id);
                return;
            }

            serializedObject.Update();

            var ids = FindObjectsOfType<ID>();
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id) continue;
                if (idInt.intValue == ids[i].GetID())
                {
                    idInt.intValue++;
                    i = 0;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.color = Color.green;
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("ID:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(idInt.intValue.ToString());
            }
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;

            serializedObject.ApplyModifiedProperties();
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com
