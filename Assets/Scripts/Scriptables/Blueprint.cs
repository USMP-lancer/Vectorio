using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The entire blueprint system is currently a WIP,
// it will take a few iterations to get it exactly
// how I want, so please keep that in mind!

[CreateAssetMenu(fileName = "New Blueprint", menuName = "Building/Blueprint")]
public class Blueprint : IdentifiableScriptableObject
{
    public enum RarityType
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum Type
    {
        Building,
        Resource,
        Defense,
        All
    }

    [System.Serializable]
    public class EffectType
    {
        [TableColumnWidth(110)]
        public Effect effect;
        [TableColumnWidth(10)]
        public bool negative;
    }

    [System.Serializable]
    public class Rarity
    {
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
        [Title("Rarity Modifier")]
        public RarityType rarity;
        [ColorPalette("Rarity Colors")]
        public Color color;
        public float[] modifier;
        [Range(0f, 0.01f)]
        public float dropChance;
        public Cost applicationCost;
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 20)]
        public Cost removalCost;
    }

    [FoldoutGroup("Blueprint Info")]
    public new string name;
    [FoldoutGroup("Blueprint Info")]
    [TextArea] public string description;
    [FoldoutGroup("Blueprint Info")]
    public Type type;
    [FoldoutGroup("Blueprint Info")]
    public Sprite icon;
    [FoldoutGroup("Blueprint Info")]
    public List<Blueprint> blacklist;

    [FoldoutGroup("Blueprint Effect")]
    public List<EffectType> effects;
    
    [FoldoutGroup("Blueprint Rarities")]
    public List<Rarity> rarities;
}
//