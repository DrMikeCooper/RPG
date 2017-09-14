using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute {

    public string varName;
    public float threshold;
    public enum Comparison
    {
        Equals,
        Not,
        Greater,
        Less
    };
    public Comparison comparison;

    public ShowIfAttribute(string vn, float th = 0, ShowIfAttribute.Comparison c = ShowIfAttribute.Comparison.Equals)
    {
        varName = vn;
        threshold = th;
        comparison = c;
    }

}
