using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RPGPowersMenuItems : MonoBehaviour
{
    [MenuItem("RPG/Make Copy")]
    static void MakeCopy()
    {
        ScriptableObject obj = Selection.activeObject as ScriptableObject;
        ScriptableObject power = CreateClone<ScriptableObject>(obj);
    }

    [MenuItem("RPG/Make Melee Power")]
    static void MakeMeleePower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerMelee>(s.name + "Melee");
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        power.icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerMelee Icon.png");
    }

    [MenuItem("RPG/Make Projectile Power")]
    static void MakeProjectilePower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerProjectile power = CreateAsset<RPG.PowerProjectile>(s.name + "Projectile");
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Piercing);
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
    }

    [MenuItem("RPG/Make Beam Power")]
    static void MakeBeamPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerBeam>(s.name + "Beam");
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
    }

    [MenuItem("RPG/Make Direct Power")]
    static void MakeDirectPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerDirect>(s.name + "Direct");
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
    }

    [MenuItem("RPG/Make Area Power")]
    static void MakeAreaPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerArea power = CreateAsset<RPG.PowerArea>(s.name + "Area");
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Fire);
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
    }

    [MenuItem("RPG/Make MoveTo Power")]
    static void MakeMoveToPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerMoveTo>(s.name + "MoveTo");
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        power.icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Gizmos/PowerMoveTo Icon.png");
    }

    [MenuItem("RPG/Make Block Power")]
    static void MakeBlockPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerBlock>(s.name + "Block");
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Crushing);
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
    }

    [MenuItem("RPG/Make Toggle Power")]
    static void MakeTogglePower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerToggle power = CreateAsset<RPG.PowerToggle>(s.name + "Toggle");
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Magic);
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
    }

    // validations
    [MenuItem("RPG/Make Copy", true)]
    static bool ValidateMakeCopy()
    {
        return (Selection.activeObject as ScriptableObject) != null;
    }

    [MenuItem("RPG/Make Melee Power", true)]
    static bool ValidateMakeMeleePower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make Projectile Power", true)]
    static bool ValidateMakeProjectilePower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make Beam Power", true)]
    static bool ValidateMakeBeamPower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make Direct Power", true)]
    static bool ValidateMakeDirectPower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make Area Power", true)]
    static bool ValidateMakeAreaPower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make MoveTo Power", true)]
    static bool ValidateMakeMoveToPower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make Block Power", true)]
    static bool ValidateMakeBlockPower()
    {
        return IsStatus();
    }

    [MenuItem("RPG/Make Toggle Power", true)]
    static bool ValidateMakeTogglePower()
    {
        return IsStatus();
    }


    // utility functions
    static bool IsStatus()
    {
        return (Selection.activeObject as RPG.Status) != null;
    }

    static RPG.RPGSettings.DamageType GetDamageType(RPG.Status s, RPG.RPGSettings.DamageType def)
    {
        RPG.DamageOverTime dot = s as RPG.DamageOverTime;
        if (dot)
            return dot.damageType;
        return def;
    }

    public static T CreateAsset<T>(string assetName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + assetName + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

    public static T CreateClone<T>(T original) where T : ScriptableObject
    {
        T asset = Instantiate(original);

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + original.name + "Copy.asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

}
