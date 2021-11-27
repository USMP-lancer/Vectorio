﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DefaultEnemy : BaseEntity
{
    // Class variables
    public Enemy enemy;
    public float moveSpeed;
    [HideInInspector] public bool isMenu;
    [HideInInspector] public Variant variant;
    [HideInInspector] public BaseTile target;
    public Transform rotator;

    // Sprite info
    public SpriteRenderer[] border;
    public SpriteRenderer[] fill;
    public TrailRenderer[] trail;

    public override void Setup()
    {
        // Check applied variant
        if (variant == null)
        {
            Debug.Log("Enemy was instantiated with a null variant!");
            Destroy(gameObject);
            return;
        }

        foreach (SpriteRenderer a in border)
            a.material = variant.border;

        foreach (SpriteRenderer a in fill)
            a.material = variant.fill;

        foreach (TrailRenderer a in trail)
            a.material = variant.trail;

        moveSpeed = enemy.moveSpeed * variant.speedModifier;
        health = enemy.health * variant.healthModifier;
        maxHealth = health;
    }

    public virtual void GiveDamage(BaseTile building)
    {
        building.DamageEntity(enemy.damage);
    }

    // Damages the entity (IDamageable interface method)
    public override void DamageEntity(float dmg)
    {
        health -= dmg;
        if (health <= 0) DestroyEntity();
        else Events.active.EnemyHurt(this);
    }

    public override void DestroyEntity()
    {
        // Create particle and set material / trail material
        ParticleSystemRenderer holder = Instantiate(variant.particle, transform.position,
            Quaternion.identity).GetComponent<ParticleSystemRenderer>();
        holder.material = variant.border;
        holder.trailMaterial = variant.border;

        // Invoke enemy death event
        Events.active.EnemyDestroyed(this);

        // Update unlockables
        Buildables.UpdateEntityUnlockables(Unlockable.UnlockType.DestroyEnemyAmount, enemy, 1);

        // Destroy game object
        Destroy(gameObject);
    }

    public virtual void MoveTowards(Transform obj, Transform target)
    {
        float step = moveSpeed * Time.deltaTime;
        obj.position = Vector2.MoveTowards(obj.position, target.position, step);
    }

    // If a collision is detected, destroy the other entity and apply damage to self
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isMenu)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            if (other is BoxCollider2D)
            {
                BaseTile building = other.GetComponent<BaseTile>();

                if (building != null)
                {
                    GiveDamage(building);
                    if (building != null)
                        Destroy(gameObject);
                }
            }
            else
            {
                DefaultTurret turret = other.GetComponent<DefaultTurret>();

                if (turret != null)
                    turret.AddTarget(this);
            }
        }
    }

    // If entity leaves defense range, remove self from target list
    public virtual void OnTriggerExit2D(Collider2D other)
    {
        DefaultTurret turret = other.GetComponent<DefaultTurret>();

        if (turret != null)
            turret.RemoveTarget(this);
    }

    // Get material
    public override Material GetMaterial()
    {
        return variant.border;
    }
}
