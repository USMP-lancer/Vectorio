using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintHandler : MonoBehaviour
{
    // Get blueprint object 
    public List<BlueprintObj> _blueprintObjs;
    public Dictionary<Blueprint.RarityType, BlueprintObj> blueprintObjs;

    // Register events and setup dictionary
    public void Start()
    {
        foreach (BlueprintObj obj in _blueprintObjs)
            blueprintObjs.Add(obj.rarity, obj);
        Events.active.onEnemyDestroyed += TrySpawnBlueprint;
    }

    // Iterate through blueprint table and try to spawn 
    public void TrySpawnBlueprint(DefaultEnemy enemy)
    {
        // Grab list of drops
        List<Blueprint> blueprints = enemy.enemy.drops;
        
        // Loop variables
        float random;

        // Iterate through blueprints
        foreach(Blueprint blueprint in blueprints)
        {
            // Generate random value
            random = Random.value;

            // Iterate through blueprints (highest to lowest rarity)
            foreach (Blueprint.Rarity rarity in blueprint.rarities)
            {
                // If value lower then drop chance, spawn and return
                if (random < rarity.dropChance)
                {
                    BlueprintObj newBlueprint = Instantiate(blueprintObjs[rarity.rarity], enemy.transform.position, Quaternion.identity).GetComponent<BlueprintObj>();
                    newBlueprint.Setup(blueprint);
                    return;
                }
            }
        }
    }
}