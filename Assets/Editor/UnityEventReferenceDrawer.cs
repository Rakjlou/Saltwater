using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(UnityEventReference))]
public class UnityEventReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        // Get the serialized properties
        var targetComponentProp = property.FindPropertyRelative("targetComponent");
        var eventFieldNameProp = property.FindPropertyRelative("eventFieldName");
        
        // Calculate rects
        var totalRect = EditorGUI.PrefixLabel(position, label);
        var componentRect = new Rect(totalRect.x, totalRect.y, totalRect.width, EditorGUIUtility.singleLineHeight);
        var dropdownRect = new Rect(totalRect.x, totalRect.y + EditorGUIUtility.singleLineHeight + 2, totalRect.width, EditorGUIUtility.singleLineHeight);
        
        // Draw component field
        EditorGUI.PropertyField(componentRect, targetComponentProp, GUIContent.none);
        
        // Draw dropdown for UnityEvent fields
        if (targetComponentProp.objectReferenceValue != null)
        {
            var component = targetComponentProp.objectReferenceValue as Component;
            var unityEventFields = GetUnityEventFields(component);
            
            if (unityEventFields.Length > 0)
            {
                var fieldNames = unityEventFields.Select(f => f.Name).ToArray();
                var currentIndex = System.Array.IndexOf(fieldNames, eventFieldNameProp.stringValue);
                if (currentIndex == -1) currentIndex = 0;
                
                var newIndex = EditorGUI.Popup(dropdownRect, "Event Field", currentIndex, fieldNames);
                if (newIndex >= 0 && newIndex < fieldNames.Length)
                {
                    eventFieldNameProp.stringValue = fieldNames[newIndex];
                }
            }
            else
            {
                EditorGUI.LabelField(dropdownRect, "Event Field", "No UnityEvents found");
            }
        }
        else
        {
            EditorGUI.LabelField(dropdownRect, "Event Field", "Select a component first");
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 2; // Two lines with small gap
    }
    
    private FieldInfo[] GetUnityEventFields(Component component)
    {
        if (component == null) return new FieldInfo[0];
        
        return component.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => typeof(UnityEventBase).IsAssignableFrom(f.FieldType))
            .ToArray();
    }
}