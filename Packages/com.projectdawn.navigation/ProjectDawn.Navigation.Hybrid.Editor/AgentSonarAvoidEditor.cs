using UnityEngine;
using UnityEditor;

namespace ProjectDawn.Navigation.Hybrid.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AgentAvoidAuthoring))]
    class AgentSonarAvoidEditor : UnityEditor.Editor
    {
        static class Styles
        {
            public static readonly GUIContent Radius = EditorGUIUtility.TrTextContent("Radius", "The maximum distance at which agent will attempt to avoid nearby agents.");
            public static readonly GUIContent Angle = EditorGUIUtility.TrTextContent("Angle", "The maximum angle at which agent will attempt to nearby agents.");
            public static readonly GUIContent Mode = EditorGUIUtility.TrTextContent("Mode", "Mode that modifies avoidance behaviour.");
            public static readonly GUIContent BlockedStop = EditorGUIUtility.TrTextContent("Blocked Stop", "Whenever agent should stop, if all directions are blocked.");
            public static readonly GUIContent UseWalls = EditorGUIUtility.TrTextContent("Use Walls (Experimental)", "Should avoidance account for static obstacles. Having this option enable will cost more performance.");
        }

        SerializedProperty m_Radius;
        SerializedProperty m_Angle;
        SerializedProperty m_Mode;
        SerializedProperty m_BlockedStop;
        SerializedProperty m_UseWalls;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Radius, Styles.Radius);
            EditorGUILayout.PropertyField(m_Angle, Styles.Angle);
            EditorGUILayout.PropertyField(m_Mode, Styles.Mode);
            EditorGUILayout.PropertyField(m_BlockedStop, Styles.BlockedStop);
            if (!serializedObject.isEditingMultipleObjects)
            {
                var authoring = target as AgentAvoidAuthoring;
                bool hasNavMesh = authoring.GetComponent<AgentNavMeshAuthoring>();
                using (new EditorGUI.DisabledScope(!hasNavMesh))
                    EditorGUILayout.PropertyField(m_UseWalls, Styles.UseWalls);
                if (!hasNavMesh)
                    EditorGUILayout.HelpBox("Use Walls property requires AgentNavMeshAuthoring component!", MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                // Update entities
                foreach (var target in targets)
                {
                    var authoring = target as AgentAvoidAuthoring;
                    if (authoring.HasEntityAvoid)
                        authoring.EntityAvoid = authoring.DefaultAvoid;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void OnEnable()
        {
            m_Radius = serializedObject.FindProperty("Radius");
            m_Angle = serializedObject.FindProperty("Angle");
            m_Mode = serializedObject.FindProperty("Mode");
            m_BlockedStop = serializedObject.FindProperty("BlockedStop");
            m_UseWalls = serializedObject.FindProperty("UseWalls");
        }
    }
}
