using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListElementTitleAttribute : PropertyAttribute
{

    public string[] ElementName;
    public TypeSupport Type;

    public ListElementTitleAttribute(string[] name, TypeSupport type)
    {
        ElementName = name;
        Type = type;
    }

    public enum TypeSupport
    {
        INTEGER,
        FLOAT,
        STRING,
        VECTOR2,
        VECTOR3,
        VECTOR4,
        OBJECT
    }
}
