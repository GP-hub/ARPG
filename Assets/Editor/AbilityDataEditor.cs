using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(AbilityData))]
public class AbilityDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        AbilityData abilityData = (AbilityData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Enemy Function", EditorStyles.boldLabel);

        // === ATTACK METHOD SELECTION ===
        Type enemyType = typeof(Enemy);
        MethodInfo[] attackMethods = enemyType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<AttackMethodAttribute>() != null)
            .ToArray();

        string[] attackMethodNames = attackMethods.Select(m => m.Name).ToArray();

        if (attackMethodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No attack methods found in Enemy class!", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (string.IsNullOrEmpty(abilityData.selectedFunctionName) || !attackMethodNames.Contains(abilityData.selectedFunctionName))
        {
            abilityData.selectedFunctionName = attackMethodNames[0];
        }

        int attackIndex = Array.IndexOf(attackMethodNames, abilityData.selectedFunctionName);
        attackIndex = EditorGUILayout.Popup("Function to Call", attackIndex, attackMethodNames);
        if (attackIndex >= 0)
            abilityData.selectedFunctionName = attackMethodNames[attackIndex];

        // === PREVIEW METHOD SELECTION ===
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Preview Function", EditorStyles.boldLabel);

        MethodInfo[] previewMethods = enemyType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<AbilityPreviewMethodAttribute>() != null)
            .ToArray();

        // Add "None" as the first option in the dropdown
        string[] previewMethodNames = new string[] { "None" }
            .Concat(previewMethods.Select(m => m.Name))
            .ToArray();

        if (previewMethodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No preview methods found in Enemy class!", MessageType.Info);
        }
        else
        {
            if (string.IsNullOrEmpty(abilityData.selectedPreviewFunctionName) || !previewMethodNames.Contains(abilityData.selectedPreviewFunctionName))
            {
                abilityData.selectedPreviewFunctionName = previewMethodNames[0];
            }

            int previewIndex = Array.IndexOf(previewMethodNames, abilityData.selectedPreviewFunctionName);
            previewIndex = EditorGUILayout.Popup("Preview Function", previewIndex, previewMethodNames);

            if (previewIndex == 0)
            {
                // "None" selected — clear the preview function name
                abilityData.selectedPreviewFunctionName = null;
            }
            else if (previewIndex > 0)
            {
                abilityData.selectedPreviewFunctionName = previewMethodNames[previewIndex];
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

}
