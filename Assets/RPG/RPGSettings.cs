using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG
{
    [ExecuteInEditMode]
    public class RPGSettings : MonoBehaviour
    {
        public float baseAccuracy = 50.0f;

        [Tooltip("Standard HUD to display over the head of every character in the scene")]
        public GameObject overheadHUD;
        [Tooltip("Targetting reticle prefab")]
        public GameObject reticle;

        [HideInInspector]
        public static RPGSettings instance; // singleton accessor

        public ObjectPool numbersPool;
        public ObjectPool beamPool;

        Dictionary<Prop, OverheadHUD> healthBars = new Dictionary<Prop, OverheadHUD>();
        
        public enum DamageType
        {
            Crushing = 1,
            Piercing = 2,
            Toxic = 4,
            Fire = 8,
            Cold = 16,
            Electric = 32,
            Energy = 64,
            Negative = 128,
            Psionic = 256,
            Magic = 512,
            Healing = 1024,
            Physical = Crushing | Piercing | Toxic,
            Elemental = Fire | Cold | Energy | Electric,
            Paranormal = Negative | Psionic | Magic,
            All = Physical | Elemental | Paranormal
        };
        // count of the number of non-compound entries above
        public const int BasicDamageTypesCount = 10; 

        public enum StatName
        {
            Res,
            Dam,
            Def, // this is the last composite one, check Buff.Apply to see how its used
            EnergyRegen,
            HealthRegen,
            Speed,
            Jump,
            Recharge,
            Health,
            Energy,
            Charge,
            Root, 
            Hold,
            Stun,
            Accuracy,
            Enrage,
            Confusion,
        }

        public static bool IsMez(StatName stat)
        {
            return stat == StatName.Root || stat == StatName.Stun || stat == StatName.Hold || stat == StatName.Enrage || stat == StatName.Confusion;
        }

        public enum ColorCode
        {
            Health,
            Mana,
            Crushing, 
            Piercing,
            Toxic,
            Fire,
            Cold,
            Electric,
            Energy,
            Negative,
            Psionic,
            Magic,
            Physical,
            Elemental,
            Paranormal,
            Custom
        };

        [Serializable]
        public struct Tint
        {
            public ColorCode code;
            public Color customColor;

            public Color GetColor()
            {
                if (code == ColorCode.Custom)
                    return customColor;
                else
                    return RPGSettings.instance ? RPGSettings.instance.colors[(int)code] : Color.white;
            }
        }

        public GameObject projectile;
        public GameObject beam;

        [HideInInspector] // these get drawn in the Inspector with some custom code
        public Color[] colors = new Color[(int)ColorCode.Custom];

        public static Color GetColor(ColorCode cc)
        {
            return instance.colors[(int)cc];
        }
    
        // Use this for initialization
        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            Prop.UpdateProps();
        }

        public void SetupCharacter(Prop prop)
        {
            if (healthBars.ContainsKey(prop))
                return;

            // copy the HUD template and make it this
            GameObject hud = Instantiate(overheadHUD);
            hud.name = "HUD_" + prop.name;
            hud.transform.SetParent(transform);
            hud.GetComponent<CharacterHUD>().character = prop;
            healthBars[prop] = hud.GetComponent<OverheadHUD>();
        }

        public void RemoveCharacter(Prop p)
        {
            if (healthBars.ContainsKey(p))
            {
                if (healthBars.ContainsKey(p) && healthBars[p].gameObject)
                {
                    healthBars[p].gameObject.SetActive(false);
                }
            }
            Power.ClearCharacterList();
        }

        public OverheadHUD GetHUD(Character ch)
        {
            return healthBars[ch];
        }

        // Utility functions for damage types
        public static bool IsComposite(DamageType dt)
        {
            return (dt == DamageType.All) || (dt == DamageType.Physical) || (dt == DamageType.Paranormal) || (dt == DamageType.Elemental);
        }

        // if an attack gets set a composite type, return an appropriate damage type for it.
        static DamageType GetDamageType(DamageType dt)
        {
            if (dt == DamageType.All || dt == DamageType.Physical)
                return DamageType.Crushing;
            if (dt == DamageType.Elemental)
                return DamageType.Fire;
            if (dt == DamageType.Paranormal)
                return DamageType.Magic;
            return dt;
        }

        public NumberFloater GetNumberFloater()
        {
            GameObject go = numbersPool.GetObject();
            if (go)
                return go.GetComponent<NumberFloater>();
            return null;
        }


        public static string GetResistanceStat(DamageType dt) { return dt.ToString() + StatName.Res.ToString(); }
        public static string GetDefenceStat(DamageType dt) { return dt.ToString() + StatName.Def.ToString(); }
        public static string GetDamageStat(DamageType dt) { return dt.ToString() + StatName.Dam.ToString(); }
    }
}