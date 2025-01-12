using System;
using System.Collections.Generic;
using UnityEngine;

// List of all buildables
public class Buildable
{
    // Constructor
    public Buildable(Building building)
    {
        this.building = building;
        obj = building.obj;
        discount = 1f;

        showButtons = new List<MenuButton>();
        unlockable = building.unlockable;

        resources = new Cost[building.resources.Length];
        for (int i = 0; i < resources.Length; i++)
        {
            resources[i] = new Cost();
            resources[i].type = building.resources[i].type;
            resources[i].storage = building.resources[i].storage;
            resources[i].amount = building.resources[i].amount;
        }

        isUnlocked = building.unlockable.unlocked || Gamemode.active.unlockEverything;
        blueprintSlots = new CollectedBlueprint[building.engineeringSlots];

        isCollector = obj.GetComponent<DefaultCollector>() != null;
        isStorage = obj.GetComponent<DefaultStorage>() != null;
        isDefense = obj.GetComponent<DefaultTurret>() != null;
        isDroneport = obj.GetComponent<Droneport>() != null;
        
        Debug.Log("Registered " + building.name + " buildable and linked to " + building.InternalID);
    }

    // Buildable variables
    public Building building;
    public MenuButton button;
    public GameObject obj;
    public List<MenuButton> showButtons;
    public Unlockable unlockable;
    public int tracked;
    public bool isUnlocked;

    // Resource variables (THIS NEEDS TO BE CHANGED)
    public bool isCollector;
    public bool isStorage;
    public bool isDefense;
    public bool isDroneport;

    // Building variables
    public float discount;
    public Cost[] resources;

    // Blueprints applied to this buildable
    public CollectedBlueprint[] blueprintSlots;
    
    // Update stats
    public void GetModifier(string name)
    {
        
    }

    // Applies a blueprint
    public bool ApplyBlueprint(CollectedBlueprint blueprint)
    {
        for (int i = 0; i < blueprintSlots.Length; i++)
        {
            if (blueprintSlots[i] == null)
            {
                blueprintSlots[i] = blueprint;
                Panel.active.SetPanel(building);
                return true;
            }
        }
        return false;
    }

    // Get resource
    public virtual int GetResource(Resource.CurrencyType type)
    {
        foreach (Cost cost in resources)
            if (cost.type == type)
                return cost.amount;
        return 0;
    }
}