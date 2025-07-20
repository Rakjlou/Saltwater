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

        // Draw dropdown for UnityEvent fields with component selection
        if (targetComponentProp.objectReferenceValue != null)
        {
            var component = targetComponentProp.objectReferenceValue as Component;
            var gameObject = component.gameObject;

            // Get all components with UnityEvents from the GameObject
            var componentEventPairs = GetAllComponentEventPairs(gameObject);

            if (componentEventPairs.Count > 0)
            {
                // Create display names: "ComponentName.EventName"
                var displayNames = componentEventPairs.Select(pair =>
                    $"{pair.Component.GetType().Name}.{pair.EventField.Name}").ToArray();

                // Find current selection
                var currentSelection = $"{component.GetType().Name}.{eventFieldNameProp.stringValue}";
                var currentIndex = System.Array.IndexOf(displayNames, currentSelection);
                if (currentIndex == -1) currentIndex = 0;

                var newIndex = EditorGUI.Popup(dropdownRect, currentIndex, displayNames);
                if (newIndex >= 0 && newIndex < componentEventPairs.Count)
                {
                    var selectedPair = componentEventPairs[newIndex];
                    targetComponentProp.objectReferenceValue = selectedPair.Component;
                    eventFieldNameProp.stringValue = selectedPair.EventField.Name;
                }
            }
            else
            {
                EditorGUI.LabelField(dropdownRect, "No UnityEvents found on GameObject");
            }
        }
        else
        {
            EditorGUI.LabelField(dropdownRect, "Select a component first");
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

    private System.Collections.Generic.List<ComponentEventPair> GetAllComponentEventPairs(GameObject gameObject)
    {
        var pairs = new System.Collections.Generic.List<ComponentEventPair>();

        if (gameObject == null) return pairs;

        var components = gameObject.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component == null) continue;

            var eventFields = GetUnityEventFields(component);
            foreach (var eventField in eventFields)
            {
                pairs.Add(new ComponentEventPair { Component = component, EventField = eventField });
            }
        }

        return pairs;
    }

    private struct ComponentEventPair
    {
        public Component Component;
        public FieldInfo EventField;
    }
}
