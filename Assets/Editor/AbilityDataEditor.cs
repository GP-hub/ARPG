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

        Type enemyType = typeof(Enemy);
        MethodInfo[] methods = enemyType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<AttackMethodAttribute>() != null) // Filter only attack methods
            .ToArray();

        string[] methodNames = methods.Select(m => m.Name).ToArray();

        if (methodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No attack methods found in Enemy class!", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (string.IsNullOrEmpty(abilityData.selectedFunctionName) || !methodNames.Contains(abilityData.selectedFunctionName))
        {
            abilityData.selectedFunctionName = methodNames[0]; 
        }

        int selectedIndex = Array.IndexOf(methodNames, abilityData.selectedFunctionName);
        selectedIndex = EditorGUILayout.Popup("Function to Call", selectedIndex, methodNames);

        if (selectedIndex >= 0)
        {
            abilityData.selectedFunctionName = methodNames[selectedIndex];
        }

        serializedObject.ApplyModifiedProperties();
    }
}
