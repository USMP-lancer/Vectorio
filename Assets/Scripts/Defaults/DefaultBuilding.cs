﻿using UnityEngine;
using Mirror;
using System.Collections.Generic;

[HideInInspector]
public class DefaultBuilding : DefaultEntity
{
    [HideInInspector]
    public List<Vector2Int> cells;

    public override void DestroyEntity()
    {
        if (BuildingSystem.active != null)
        {
            foreach (Vector2Int cell in cells)
                BuildingSystem.active.tileGrid.RemoveCell(cell);
        }

        if (particle != null)
            Instantiate(particle, transform.position, transform.rotation);
        Recycler.AddRecyclable(transform);
    }
}
