using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Loads scriptables at runtime which can then be accesed from anywhere.

public static class ScriptableLoader
{
    // Resource paths
    public static string BuildingPath = "Scriptables/Buildings";
    public static string EnemyPath = "Scriptables/Enemies";
    public static string GuardianPath = "Scriptables/Guardians";
    public static string VariantPath = "Scriptables/Variants";
    public static string StagePath = "Scriptables/Stages";
    public static string ResearchPath = "Scriptables/Research";

    public static Dictionary<string, Building> buildings;
    public static Dictionary<string, Enemy> enemies;
    public static Dictionary<string, Guardian> guardians;
    public static Dictionary<string, Variant> variants;
    public static Dictionary<string, Stage> stages;
    public static Dictionary<string, ResearchTech> researchTechs;

    public static Dictionary<string, Entity> allLoadedEntities = 
              new Dictionary<string, Entity>();

    // Generate all scriptables
    public static void GenerateAllScriptables()
    {
        allLoadedEntities = new Dictionary<string, Entity>();
        Buildables.active = new Dictionary<Entity, Buildable>();

        Building hub = Resources.Load<Building>("Scriptables/Hub");
        if (hub != null)
        {
            Buildables.Register(hub);
            allLoadedEntities.Add(hub.name, hub);
        }
        else Debug.Log("The hub scriptable could not be parsed");

        GenerateBuildings();
        GenerateEnemies();
        GenerateGuardians();
        GenerateVariants();
        GenerateStages();
        GenerateResearch();
    }

    // Generates buildings on run
    public static void GenerateBuildings()
    {
        buildings = new Dictionary<string, Building>();
        List<Building> loaded = Resources.LoadAll(BuildingPath, typeof(Building)).Cast<Building>().ToList();

        Debug.Log("Loading " + loaded.Count + " buildings from " + BuildingPath + "...");
        foreach (Building building in loaded)
        {
            BaseEntity baseEntity = building.obj.GetComponent<BaseEntity>();
            if (baseEntity == null) Debug.Log("Entity " + building.name + "'s object has no BaseEntity script!\nBecause of this, it will not load properly.");
            allLoadedEntities.Add(building.name, building);
            buildings.Add(building.InternalID, building);
            
            Debug.Log("Loaded " + building.name + " with UUID " + building.InternalID);
            if (Gamemode.active.initBuildings)
                Buildables.Register(building);
        }

        if (Gamemode.active.initBuildings && Inventory.active != null)
            Inventory.active.GenerateBuildings(loaded.ToArray());
        
        // Set requirements
        if (!Gamemode.active.unlockEverything)
            Buildables.HideUnmetRequirements();
    }

    // Generates enemies on run
    public static void GenerateEnemies()
    {
        enemies = new Dictionary<string, Enemy>();
        List<Enemy> loaded = Resources.LoadAll(EnemyPath, typeof(Enemy)).Cast<Enemy>().ToList();

        Debug.Log("Loaded " + loaded.Count + " enemies from " + EnemyPath);
        foreach (Enemy enemy in loaded)
        {
            BaseEntity baseEntity = enemy.obj.GetComponent<BaseEntity>();
            if (baseEntity == null) Debug.Log("Entity " + enemy.name + "'s object has no BaseEntity script!\nBecause of this, it will not load properly.");
            allLoadedEntities.Add(enemy.name, enemy);
            enemies.Add(enemy.InternalID, enemy);
            Debug.Log("Loaded " + enemy.name + " with UUID " + enemy.InternalID);
        }
        if (Gamemode.active.initEnemies)
            Inventory.active.GenerateEntities(loaded.ToArray());
    }

    // Generates guardians on run
    public static void GenerateGuardians()
    {
        guardians = new Dictionary<string, Guardian>();
        List<Guardian> loaded = Resources.LoadAll(GuardianPath, typeof(Guardian)).Cast<Guardian>().ToList();

        Debug.Log("Loaded " + loaded.Count + " guardians from " + GuardianPath);
        foreach (Guardian guardian in loaded)
        {
            BaseEntity baseEntity = guardian.obj.GetComponent<BaseEntity>();
            if (baseEntity == null) Debug.Log("Entity " + guardian.name + "'s object has no BaseEntity script!\nBecause of this, it will not load properly.");
            allLoadedEntities.Add(guardian.name, guardian);
            guardians.Add(guardian.InternalID, guardian);
            Debug.Log("Loaded " + guardian.name + " with UUID " + guardian.InternalID);
        }
        if (Gamemode.active.initGuardians)
            Inventory.active.GenerateEntities(loaded.ToArray());
    }

    // Generates guardians on run
    public static void GenerateVariants()
    {
        variants = new Dictionary<string, Variant>();
        List<Variant> loaded = Resources.LoadAll(VariantPath, typeof(Variant)).Cast<Variant>().ToList();
        Debug.Log("Loaded " + loaded.Count + " variants from " + VariantPath);

        foreach (Variant variant in loaded)
        {
            variants.Add(variant.InternalID, variant);
            Debug.Log("Loaded " + variant.name + " with UUID " + variant.InternalID);
        }
    }

    // Generates guardians on run
    public static void GenerateStages()
    {
        stages = new Dictionary<string, Stage>();
        List<Stage> loaded = Resources.LoadAll(StagePath, typeof(Stage)).Cast<Stage>().ToList();
        Debug.Log("Loaded " + loaded.Count + " stages from " + StagePath);

        foreach (Stage stage in loaded)
        {
            stages.Add(stage.InternalID, stage);
            Debug.Log("Loaded " + stage.name + " with UUID " + stage.InternalID);
        }
    }

    // Generates guardians on run
    public static void GenerateResearch()
    {
        researchTechs = new Dictionary<string, ResearchTech>();
        List<ResearchTech> loaded = Resources.LoadAll(ResearchPath, typeof(ResearchTech)).Cast<ResearchTech>().ToList();
        Debug.Log("Loaded " + loaded.Count + " research techs from " + ResearchPath);

        foreach (ResearchTech tech in loaded)
        {
            researchTechs.Add(tech.InternalID, tech);
            Debug.Log("Loaded " + tech.name + " with UUID " + tech.InternalID);
        }
    }
}