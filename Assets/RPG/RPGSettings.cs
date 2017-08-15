using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class RPGSettings : MonoBehaviour
    {
        [Tooltip("Standard HUD to display over the head of every character in the scene")]
        public GameObject overheadHUD;

        [HideInInspector]
        public static RPGSettings instance; // singleton accessor

        public ObjectPool numbersPool;

        Dictionary<Prop, OverheadHUD> healthBars = new Dictionary<Prop, OverheadHUD>();

        public enum DamageType
        {
            Crushing = 1,
            Piercing = 2,
            Fire = 4,
            Cold = 8,
            Energy = 16,
            Negative = 32,
            Psionic = 64,
            Magic = 128,
            Toxic = 256,
            Healing = 512,
            Physical = Crushing | Piercing | Toxic,
            Elemental = Fire | Cold | Energy,
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
        }


        public enum ColorCode
        {
            Health,
            Mana,
            Crushing, 
            Piercing,
            Fire,
            Cold,
            Energy,
            Negative,
            Psionic,
            Magic,
            Physical,
            Elemental,
            Paranormal,
            Custom
        };

        public GameObject projectile;
        public GameObject beam;

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

        public void SetupCharacter(Character character)
        {
            // copy the HUD template and make it this
            GameObject hud = Instantiate(overheadHUD);
            hud.name = "HUD_" + character.name;
            hud.transform.SetParent(transform);
            hud.GetComponent<CharacterHUD>().character = character;
            healthBars[character] = hud.GetComponent<OverheadHUD>();
        }

        public void RemoveCharacter(Prop p)
        {
            if (healthBars.ContainsKey(p))
                Destroy(healthBars[p].gameObject);
            Power.ClearCharacterList();
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