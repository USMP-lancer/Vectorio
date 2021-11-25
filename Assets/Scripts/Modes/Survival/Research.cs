﻿using System.Collections.Generic;
using UnityEngine;

// Still needs to be refactored

public class Research : MonoBehaviour
{
    // Turret variables
    public static float damageBoost = 1;
    public static float healthBoost = 1;
    public static float wallBoost = 1;
    public static int pierceBoost = 0;
    public static int bulletBoost = 0;
    public static float firerateBoost = 1;

    // Resource boosts
    public class ResourceBoost
    {
        public float extractionRate;
        public int extractionYield;
        public int storageAmount;
    }
    public static Dictionary<Resource.CurrencyType, ResourceBoost> resource;

    // Drone research variables
    public static int drone_tile_coverage = 5;
    public static float drone_deployment_speed = 3f;
    public static float droneMoveSpeed = 25f;

    // Currency get variables (I hate this, and will redo it)
    public static ResourceBoost GenerateBoost(Resource.CurrencyType type, float defaultRate, int defaultYield, int defaultStorage)
    {
        ResourceBoost newResource = new ResourceBoost();
        newResource.extractionRate = defaultRate;
        newResource.extractionYield = defaultYield;
        newResource.storageAmount = defaultStorage;
        resource.Add(type, newResource);
        return newResource;
    }

    // Apply research
    // THIS IS GONNA BE REDONE
    public static void ApplyResearch(ResearchType type, float amount, Resource.CurrencyType currency = Resource.CurrencyType.Power)
    {
        switch(type)
        {
            case ResearchType.DamageBoost:
                damageBoost += amount;
                break;
            case ResearchType.HealthBoost:
                healthBoost += amount;
                break;
            case ResearchType.WallBoost:
                wallBoost += amount;
                break;
            case ResearchType.PierceBoost:
                pierceBoost += (int)amount;
                break;
            case ResearchType.BulletBoost:
                bulletBoost += (int)amount;
                break;
            case ResearchType.FirerateBoost:
                firerateBoost += amount;
                break;
            case ResearchType.DroneSpeed:
                droneMoveSpeed += amount;
                break;
            case ResearchType.ExtractionRate:
                resource[currency].extractionRate += amount;
                break;
            case ResearchType.ExtractionYield:
                resource[currency].extractionRate += amount;
                break;
            case ResearchType.StorageAmount:
                resource[currency].extractionRate += amount;
                break;
        }
    }
}
