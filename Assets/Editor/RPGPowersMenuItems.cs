using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RPGPowersMenuItems : MonoBehaviour
{
    [MenuItem("RPG/Make Melee Power")]
    static void MakeMeleePower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerDirect>(s.name + "Melee");
        power.range = 1;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Crushing);
        power.targetType = RPG.Power.TargetType.Enemies;
        power.mode = RPG.Power.Mode.Instant;
        power.tint.code = RPG.RPGSettings.ColorCode.Crushing;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.icon = // TODO prefs
    }

    [MenuItem("RPG/Make Projectile Power")]
    static void MakeProjectilePower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerProjectile power = CreateAsset<RPG.PowerProjectile>(s.name + "Projectile");
        power.range = 20;
        power.speed = 1;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Piercing);
        power.targetType = RPG.Power.TargetType.Enemies;
        power.mode = RPG.Power.Mode.Instant;
        power.tint.code = RPG.RPGSettings.ColorCode.Piercing;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.icon = // TODO prefs
        // power.projectileFX = // TODO prefs
    }

    [MenuItem("RPG/Make Beam Power")]
    static void MakeBeamPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerDirect>(s.name + "Beam");
        power.range = 1;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Energy);
        power.targetType = RPG.Power.TargetType.Enemies;
        power.mode = RPG.Power.Mode.Instant;
        power.tint.code = RPG.RPGSettings.ColorCode.Energy;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.beamMaterial = // TODO prefs
        // power.icon = // TODO prefs
    }

    [MenuItem("RPG/Make Direct Power")]
    static void MakeDirectPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerDirect>(s.name + "Direct");
        power.range = 10;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Magic);
        power.targetType = RPG.Power.TargetType.Enemies;
        power.mode = RPG.Power.Mode.Instant;
        power.tint.code = RPG.RPGSettings.ColorCode.Magic;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.icon = // TODO prefs
    }

    [MenuItem("RPG/Make Area Power")]
    static void MakeAreaPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerArea power = CreateAsset<RPG.PowerArea>(s.name + "Area");
        power.range = 0;
        power.radius = 5;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Fire);
        power.targetType = RPG.Power.TargetType.Enemies;
        power.mode = RPG.Power.Mode.Instant;
        power.tint.code = RPG.RPGSettings.ColorCode.Fire;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.icon = // TODO prefs
    }

    [MenuItem("RPG/Make Block Power")]
    static void MakeBlockPower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerDirect power = CreateAsset<RPG.PowerDirect>(s.name + "Block");
        power.range = 0;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Crushing);
        power.targetType = RPG.Power.TargetType.SelfOnly;
        power.mode = RPG.Power.Mode.Block;
        power.tint.code = RPG.RPGSettings.ColorCode.Crushing;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.icon = // TODO prefs
    }

    [MenuItem("RPG/Make Toggle Power")]
    static void MakeTogglePower()
    {
        RPG.Status s = Selection.activeObject as RPG.Status;
        RPG.PowerToggle power = CreateAsset<RPG.PowerToggle>(s.name + "Toggle");
        power.range = 0;
        power.type = GetDamageType(s, RPG.RPGSettings.DamageType.Magic);
        power.targetType = RPG.Power.TargetType.SelfOnly;
        power.mode = RPG.Power.Mode.Instant;
        power.tint.code = RPG.RPGSettings.ColorCode.Magic;
        power.effects = new RPG.Status[1];
        power.effects[0] = s;
        // power.icon = // TODO prefs
    }

    // validations
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

}
