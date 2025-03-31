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

        // Get all attack methods from Enemy class that have the [AttackMethod] attribute
        Type enemyType = typeof(Enemy);
        MethodInfo[] methods = enemyType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<AttackMethodAttribute>() != null) // Filter only attack methods
            .ToArray();

        string[] methodNames = methods.Select(m => m.Name).ToArray();

        // Ensure there is at least one valid function
        if (methodNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No attack methods found in Enemy class!", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // If no function is assigned, set the first method as default
        if (string.IsNullOrEmpty(abilityData.selectedFunctionName) || !methodNames.Contains(abilityData.selectedFunctionName))
        {
            abilityData.selectedFunctionName = methodNames[0]; // Assign default method
        }

        // Find the index of the current function in the list
        int selectedIndex = Array.IndexOf(methodNames, abilityData.selectedFunctionName);
        selectedIndex = EditorGUILayout.Popup("Function to Call", selectedIndex, methodNames);

        // Update the selected function name
        if (selectedIndex >= 0)
        {
            abilityData.selectedFunctionName = methodNames[selectedIndex];
        }

        serializedObject.ApplyModifiedProperties();
    }
}
