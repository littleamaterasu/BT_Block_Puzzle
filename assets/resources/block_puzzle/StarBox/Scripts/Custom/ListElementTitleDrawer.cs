#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ListElementTitleAttribute))]
public class ListElementTitleDrawer : PropertyDrawer
{
    protected virtual ListElementTitleAttribute Atribute
    {
        get { return (ListElementTitleAttribute)attribute; }
    }


    public override void OnGUI(Rect position,
                              SerializedProperty property,
                              GUIContent label)
    {

        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);

            switch (Atribute.Type)
            {
                case ListElementTitleAttribute.TypeSupport.INTEGER:
                    property.intValue = EditorGUI.IntField(position, new GUIContent(Atribute.ElementName[pos]), property.intValue);
                    break;
                case ListElementTitleAttribute.TypeSupport.FLOAT:
                    property.floatValue = EditorGUI.FloatField(position, new GUIContent(Atribute.ElementName[pos]), property.floatValue);
                    break;
                case ListElementTitleAttribute.TypeSupport.STRING:
                    property.stringValue = EditorGUI.TextField(position, new GUIContent(Atribute.ElementName[pos]), property.stringValue);
                    break;
                case ListElementTitleAttribute.TypeSupport.VECTOR2:
                    property.vector2Value = EditorGUI.Vector2Field(position, new GUIContent(Atribute.ElementName[pos]), property.vector2Value);
                    break;
                case ListElementTitleAttribute.TypeSupport.VECTOR3:
                    property.vector3Value = EditorGUI.Vector3Field(position, new GUIContent(Atribute.ElementName[pos]), property.vector3Value);
                    break;
                case ListElementTitleAttribute.TypeSupport.VECTOR4:
                    property.vector4Value = EditorGUI.Vector3Field(position, new GUIContent(Atribute.ElementName[pos]), property.vector4Value);
                    break;
                case ListElementTitleAttribute.TypeSupport.OBJECT:
                    EditorGUI.ObjectField(position, property, new GUIContent(Atribute.ElementName[pos]));
                    break;
                default:
                    break;
            }
            
        }
        catch
        {
            switch (Atribute.Type)
            {
                case ListElementTitleAttribute.TypeSupport.INTEGER:
                    property.intValue = EditorGUI.IntField(position, label, property.intValue);
                    break;
                case ListElementTitleAttribute.TypeSupport.FLOAT:
                    property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                    break;
                case ListElementTitleAttribute.TypeSupport.STRING:
                    property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                    break;
                case ListElementTitleAttribute.TypeSupport.VECTOR2:
                    property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case ListElementTitleAttribute.TypeSupport.VECTOR3:
                    property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
                    break;
                case ListElementTitleAttribute.TypeSupport.VECTOR4:
                    property.vector4Value = EditorGUI.Vector3Field(position, label, property.vector4Value);
                    break;
                case ListElementTitleAttribute.TypeSupport.OBJECT:
                    EditorGUI.ObjectField(position, property, label);
                    break;
                default:
                    break;
            }
        }
    }
}

#endif