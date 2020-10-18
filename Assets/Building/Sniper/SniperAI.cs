﻿using UnityEngine;

public class SniperAI : TurretDefense
{
    // Turret AI variables 
    public Transform Point;
    public Rigidbody2D Gun;
    public GameObject Bullet;

    // On start, assign weapon variables
    void Start()
    {
        fireRate = 3f;
        bulletForce = 180f;
        bulletSpread = 0f;
        range = 5000;
    }

    // Targetting system
    void Update()
    {
        // Find closest enemy 
        var target = EnemyPool.FindClosestEnemy(transform.position, range);

        // If a target exists, shoot at it
        if (target != null)
        {
            // Rotate turret towards target
            Vector2 TargetPosition = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.y);
            Vector2 lookDirection = (TargetPosition - Gun.position);
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;

            // Smooth rotation when targetting enemies
            if (Gun.rotation >= angle && !((Gun.rotation - angle) <= 0.3 && (Gun.rotation - angle) >= -0.3))
            {
                Gun.rotation -= 0.3f;
            }
            else if (Gun.rotation <= angle && !((Gun.rotation - angle) <= 0.3 && (Gun.rotation - angle) >= -0.3))
            {
                Gun.rotation += 0.3f;
            }

            if ((Gun.rotation - angle) <= 5 && (Gun.rotation - angle) >= -5)
            {
                // Shoot bullet
                if (nextFire > 0)
                {
                    nextFire -= Time.deltaTime;
                    return;
                }

                // Call shoot function
                Shoot(Bullet, Point);
            }
        }

        // Update cooldown
        nextFire = fireRate;
    }
}
