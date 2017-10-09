using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfTypeAttribute : PropertyAttribute {

    public string varName;
    public System.Type hideType;
    public bool reverse;

    public HideIfTypeAttribute(System.Type _type, bool rev = false)
    {
        hideType = _type;
        reverse = rev;
    }
}
