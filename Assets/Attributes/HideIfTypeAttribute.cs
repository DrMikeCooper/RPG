using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfTypeAttribute : PropertyAttribute {

    public string varName;
    public System.Type hideType;
    
    public HideIfTypeAttribute(System.Type _type)
    {
        hideType = _type;
    }
}
