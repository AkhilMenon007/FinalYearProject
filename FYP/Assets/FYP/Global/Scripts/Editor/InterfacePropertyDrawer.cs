using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(InterfaceTypeAttribute))]
public class InterfaceTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InterfaceTypeAttribute att = attribute as InterfaceTypeAttribute;

        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, label.text, "InterfaceType Attribute can only be used with Serializable Objects!");
            return;
        }

        // Pick a specific component
        UnityEngine.Object oldComp = property.objectReferenceValue as UnityEngine.Object;

        //GameObject temp = null;
        string oldName = "";

        if (Event.current.type == EventType.Repaint)
        {
            //if (oldComp == null)
            //{
            //	temp = new GameObject("None [" + att.type.Name + "]");
            //	oldComp = temp.AddComponent<DummyScript>();
            //}
            //else
            if (oldComp != null)
            {
                oldName = oldComp.name;
                oldComp.name = oldName + " [" + att.type.Name + "]";
            }
        }

        UnityEngine.Object comp = EditorGUI.ObjectField(position, label, oldComp, typeof(UnityEngine.Object), true) as UnityEngine.Object;

        if (Event.current.type == EventType.Repaint)
        {
            //if (temp != null)
            //	GameObject.DestroyImmediate(temp);
            //else
            if (oldComp != null)
                oldComp.name = oldName;
        }

        // Make sure something changed.
        if (oldComp == comp) return;

        // If a component is assigned, make sure it is the interface we are looking for.
        if (comp != null)
        {
            // Make sure component is of the right interface
            if (comp.GetType() != att.type)
                // Component failed. Check game object.
                if (comp.GetType() == typeof(GameObject))
                {
                    GameObject c = comp as GameObject;
                    comp = c.GetComponent(att.type);
                }


            // Item failed test. Do not override old component
            if (comp == null) return;
        }

        property.objectReferenceValue = comp;
        property.serializedObject.ApplyModifiedProperties();
    }
}
