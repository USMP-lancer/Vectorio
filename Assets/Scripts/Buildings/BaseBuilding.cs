﻿using UnityEngine;
using Mirror;

[HideInInspector]
public class BaseBuilding : NetworkBehaviour, IDamageable
{
    // Building scriptable
    public Building building;

    // IDamageable interface variables
    public int health { get; set; }
    public int maxHealth { get; set; }
    public ParticleSystem deathParticle { get; set; }

    // Building default stat variables
    protected int cost;
    protected int power;
    protected int heat;

    public void SetBuildingStats()
    {
        if (building == null)
        {
            Debug.LogError(transform.name + " does not have a scriptable attached to it!");
        }
        else
        {
            health = building.health;
            maxHealth = building.health;
            cost = building.cost;
            power = building.power;
            heat = building.heat;
            deathParticle = building.deathParticle;
        }
    }

    // Damages the entity (IDamageable interface method)
    public void DamageEntity(int dmg)
    {
        health -= dmg;
        if (health <= 0) DestroyEntity();
    }

    // Destroys the entity (IDamageable interface method)
    public void DestroyEntity()
    {
        Destroy(gameObject);
        // Do other stuff
    }

    // Heals the entity (IDamageable interface method)
    public void HealEntity(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    // Death effect for the entity (IDamageable interface method)
    public void PlayDeathEffect()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity); 
    }

    public int GetHealth() { return building.health; }
    public int GetMaxHealth() { return building.maxHealth; }
    public int GetCost() { return building.cost; }
    public int GetPower() { return building.power; }
    public int GetHeat() { return building.heat; }
    public string GetDescription() { return building.description; }
}
