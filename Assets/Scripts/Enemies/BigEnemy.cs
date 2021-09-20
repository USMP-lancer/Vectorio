using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : DefaultEnemy
{
    // If a collision is detected, destroy the other entity and apply damage to self
    void OnCollisionEnter2D(Collision2D other)
    {
        DefaultBuilding building = other.collider.GetComponent<DefaultBuilding>();

        if (building != null)
        {
            BuildingSystem.active.tileGrid.DestroyCell
                (Vector2Int.RoundToInt(building.transform.position));
            DamageEntity(building.health);
        }
    }
}