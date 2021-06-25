﻿using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using TMPro;

public class Research : MonoBehaviour
{

    // Research variables
    public static int research_damage = 0;
    public static int research_burning = 0;
    public static int research_freezing = 0;
    public static int research_poisoning = 0;
    public static int research_shield = 0;
    public static int research_health = 0;
    public static int research_wall_health = 0;
    public static int research_pierce = 0;
    public static float research_range = 0;
    public static float research_firerate = 0;
    public static float research_bulletspeed = 1;
    public static float research_gold_time = 5f;
    public static int research_gold_yield = 50;
    public static int research_gold_storage = 2500;
    public static float research_essence_time = 8f;
    public static int research_essence_yield = 25;
    public static int research_essence_storage = 500;
    public static float research_iridium_time = 12f;
    public static int research_iridium_yield = 10;
    public static int research_iridium_storage = 100;
    public static float research_construction_speed = 35f;
    public static int research_construction_placements = 1;
    public static float research_resource_speed = 25f;
    public static int research_resource_collections = 5;
    public static int research_resource_amount = 3;
    public static float research_resource_range = 50f;
    public static float research_combat_speed = 20f;
    public static int research_combat_targets = 10;
    public static bool research_explosive_storages = false;
    public static int research_research_speed = 1;

    // Research UI stuff
    public Researchable Researching;
    public GameObject ResearchBackground;
    public ProgressBar ResearchBar;
    public static int LabsAvailable = 0;
    public static bool ResearchUnlocked = false;
    public static bool ResearchActive = false;
    public int resourcesNeeded = 0;
    public int resourcesTracked = 0;
    public int goldNeeded = 0;
    public int goldTracked = 0;
    public int essenceNeeded = 0;
    public int essenceTracked = 0;
    public int iridiumNeeded = 0;
    public int iridiumTracked = 0;
    public float researchSpeed = 1f;

    // Movement stuff
    protected Vector2 movement;
    public float moveSpeed = 60000f;
    public ScrollRect overlay;

    // Data source scripts
    public Survival SurvivalCS;

    // Research array class
    [System.Serializable]
    public class Researchable
    {
        public string Research; // Name of the research type
        public string ResearchType;
        public int[] RequiredResearch; // Array of indexs of the required research nodes
        public int GoldRequired; // The cost of the research
        public int EssenceRequired; // The cost of the research
        public int IridiumRequired; // The cost of the research
        public bool needsGold; // Bool that tracks if gold is required
        public bool needsEssence; // Bool that tracks if essence is required
        public bool needsIridium; // Bool that tracks if iridium is required
        public Button ResearchButton; // The button object associated with this research
        public bool IsResearched; // If this researchable is researched
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI essenceText;
        public TextMeshProUGUI iridiumText;
    }

    // Research tracking
    public Researchable[] Researchables;

    // On start
    public void Start()
    {
        UpdateAllButtons();
        UpdateAllPrices();
        ResearchBackground.SetActive(false);
    }

    // Movement trackingoverlay.normalizedPosition = new Vector2(0, 1);
    public void Update()
    {
        // Check if research open
        if (!SurvivalCS.UI.ResearchOpen) return;

        // Check if space pressed
        if (Input.GetKeyDown(KeyCode.Space)) ScreenSet();

        // Get directional movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift))
            moveSpeed = 400000f;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            moveSpeed = 60000f;
        else if (Input.GetKeyDown(KeyCode.LeftControl))
            moveSpeed = 30000f;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            moveSpeed = 60000f;

        overlay.velocity = -movement * moveSpeed * Time.fixedDeltaTime;
    }

    public void ScreenSet()
    {
        overlay.normalizedPosition = new Vector2(0, 1);
    }

    public void UpdateAvailable()
    {
        CancelInvoke("UpdateResearch");
        InvokeRepeating("UpdateResearch", 1f, researchSpeed);
    }

    public void SetResearchUI(Transform research, bool status)
    {
        if (status)
        {
            ResearchBar.currentPercent = (float)goldTracked / (float)goldNeeded * 100;
            ResearchBar.UpdateUI();
            ResearchBackground.transform.SetParent(research);
            ResearchBackground.transform.position = research.position;
            ResearchBackground.SetActive(true);
        }
        else ResearchBackground.SetActive(false);
    }

    public void UpdateResource(int type)
    {
        switch (type)
        {
            case 1:
                SurvivalCS.RemoveGold(research_research_speed);
                goldTracked += research_research_speed;
                resourcesTracked += research_research_speed;
                if (goldTracked >= goldNeeded)
                {
                    Researching.needsGold = false;
                    if (Researching.goldText != null) Researching.goldText.text = "x0";
                }
                break;
            case 2:
                SurvivalCS.RemoveEssence(research_research_speed);
                essenceTracked += research_research_speed;
                resourcesTracked += research_research_speed;
                if (essenceTracked >= essenceNeeded)
                {
                    Researching.needsEssence = false;
                    if (Researching.essenceText != null) Researching.essenceText.text = "x0";
                }
                break;
            case 3:
                SurvivalCS.RemoveIridium(research_research_speed);
                iridiumTracked += research_research_speed;
                resourcesTracked += research_research_speed;
                if (iridiumTracked >= iridiumNeeded)
                {
                    Researching.needsIridium = false;
                    if (Researching.iridiumText != null) Researching.iridiumText.text = "x0";
                }
                break;
        }
    }

    public void UpdateResearch()
    {
        // Update all resources
        if (Researching.needsGold && SurvivalCS.gold >= research_research_speed) UpdateResource(1);
        if (Researching.needsEssence && SurvivalCS.essence >= research_research_speed) UpdateResource(2);
        if (Researching.needsIridium && SurvivalCS.iridium >= research_research_speed) UpdateResource(3);

        // Update the UI elements
        ResearchBar.currentPercent = (float)resourcesTracked / (float)resourcesNeeded * 100;
        ResearchBar.UpdateUI();

        // Check if resources still left
        if (!Researching.needsGold && !Researching.needsEssence && !Researching.needsIridium)
        {
            SetResearchUI(Researchables[0].ResearchButton.transform, false);
            ApplyResearch(Researching);
            CancelInvoke("UpdateResearch");
        }
    }

    // On button click
    public void ResearchTreeButton(int number)
    {
        // Get research data ands tore in temp var
        Researchable research = Researchables[number];

        // Check if at least one research lab available
        if (LabsAvailable < 1) return;

        // Check if being research
        if (research == Researching) return;

        // Check if already researched
        if (research.IsResearched) return;

        // Returns false if one or more required research is not researched
        foreach (int j in research.RequiredResearch)
            if (!Researchables[j].IsResearched) return;

        // Set resource tracking
        resourcesNeeded = research.GoldRequired + research.EssenceRequired + research.IridiumRequired;
        resourcesTracked = 0;

        // Set gold tracking
        if (research.GoldRequired > 0)
        {
            research.goldText = research.ResearchButton.transform.Find("Gold Amount").GetComponent<TextMeshProUGUI>();
            research.needsGold = true;
        }
        else research.needsGold = false;
        goldNeeded = research.GoldRequired;
        goldTracked = 0;

        // Set essence tracking
        if (research.GoldRequired > 0)
        {
            research.essenceText = research.ResearchButton.transform.Find("Essence Amount").GetComponent<TextMeshProUGUI>();
            research.needsEssence = true;
        }
        else research.needsEssence = false;
        essenceNeeded = research.EssenceRequired;
        essenceTracked = 0;

        // Set iridium tracking
        if (research.GoldRequired > 0)
        {
            research.iridiumText = research.ResearchButton.transform.Find("Iridium Amount").GetComponent<TextMeshProUGUI>();
            research.needsIridium = true;
        }
        else research.needsIridium = false;
        iridiumNeeded = research.IridiumRequired;
        iridiumTracked = 0;

        // If all checks passed, apply research
        Researching = research;
        SetResearchUI(research.ResearchButton.transform, true);
        ResearchActive = true;

        // Start research updating
        researchSpeed = 1f / LabsAvailable;
        CancelInvoke("UpdateResearch");
        InvokeRepeating("UpdateResearch", 1f, researchSpeed);
    }

    // Increases a research variable (automatically gets applied to all buildings)
    public void ApplyResearch(Researchable research)
    {
        // Assign type
        string type = research.ResearchType;

        // Apply the research
        switch (type)
        {
            case "disrupt":
                SurvivalCS.Spawner.maxHeat = 25000;
                SurvivalCS.Spawner.maxHeatAmount.text = 25000 + " MAX";
                SurvivalCS.Spawner.increaseHeat(0);
                break;
            case "damage":
                research_damage += 5;
                break;
            case "health":
                research_health += 10;
                research_wall_health += 50;
                break;
            case "range":
                research_range += 5f;
                break;
            case "firerate":
                research_firerate += 0.1f;
                break;
            case "pierce":
                research_pierce += 1;
                break;
            case "gold yield":
                research_gold_yield += 5;
                break;
            case "gold time":
                research_gold_time -= 1;
                CollectorAI[] allGoldCollectors = FindObjectsOfType<CollectorAI>();
                foreach (CollectorAI collector in allGoldCollectors)
                    collector.OffsetStart();
                break;
            case "gold storage":
                research_gold_storage += 2500;
                break;
            case "essence yield":
                research_essence_yield += 5;
                break;
            case "essence time":
                research_essence_time -= 1f;
                CollectorAI[] allEssenceCollectors = FindObjectsOfType<CollectorAI>();
                foreach (CollectorAI collector in allEssenceCollectors)
                    collector.OffsetStart();
                break;
            case "essence storage":
                research_essence_storage += 500;
                break;
            case "iridium yield":
                research_iridium_yield += 5;
                break;
            case "iridium time":
                research_iridium_time -= 1f;
                CollectorAI[] allIridiumCollectors = FindObjectsOfType<CollectorAI>();
                foreach (CollectorAI collector in allIridiumCollectors)
                    collector.OffsetStart();
                break;
            case "iridium storage":
                research_iridium_storage += 100;
                break;
            case "burning":
                research_burning += 5;
                break;
            case "freezing":
                research_freezing += 10;
                break;
            case "poisoning":
                research_poisoning += 3;
                break;
            case "explosive storages":
                research_explosive_storages = true;
                break;
            case "construction speed":
                research_construction_speed += 5f;
                break;
            case "combat speed":
                research_combat_speed += 5f;
                break;
            default:
                Debug.Log("The research type supplied was invalid");
                break;
        }

        // Set research boolean
        research.IsResearched = true;
        UpdateAllButtons();
    }

    public void SetResearchButtonColor(Researchable research, string color)
    {
        var colors = research.ResearchButton.colors;
        Color colorHolder;
        if (color == "red")
        {
            ColorUtility.TryParseHtmlString("#A6232399", out colorHolder);
            colors.normalColor = colorHolder;
            ColorUtility.TryParseHtmlString("#BC181899", out colorHolder);
            colors.highlightedColor = colorHolder;
            ColorUtility.TryParseHtmlString("#F31D1D99", out colorHolder);
            colors.pressedColor = colorHolder;
        }
        else if (color == "green")
        {
            ColorUtility.TryParseHtmlString("#259F3B99", out colorHolder);
            colors.normalColor = colorHolder;
            ColorUtility.TryParseHtmlString("#24B93F99", out colorHolder);
            colors.highlightedColor = colorHolder;
            ColorUtility.TryParseHtmlString("#1FD13F99", out colorHolder);
            colors.pressedColor = colorHolder;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#22222299", out colorHolder);
            colors.normalColor = colorHolder;
            ColorUtility.TryParseHtmlString("#33323299", out colorHolder);
            colors.highlightedColor = colorHolder;
            ColorUtility.TryParseHtmlString("#9C999999", out colorHolder);
            colors.pressedColor = colorHolder;
        }

        research.ResearchButton.colors = colors;
    }

    public void UpdateAllButtons()
    {
        foreach (Researchable research in Researchables)
        {
            if (research.IsResearched) SetResearchButtonColor(research, "green");
            else
            {
                SetResearchButtonColor(research, "default");
                foreach (int j in research.RequiredResearch)
                    if (!Researchables[j].IsResearched) SetResearchButtonColor(research, "red");
            }
        }
    }

    public void UpdateAllPrices()
    {
        foreach (Researchable research in Researchables)
        {
            research.goldText = research.ResearchButton.transform.Find("Gold Amount").GetComponent<TextMeshProUGUI>();
            research.goldText.text = "x" + research.GoldRequired;
            research.essenceText = research.ResearchButton.transform.Find("Essence Amount").GetComponent<TextMeshProUGUI>();
            research.essenceText.text = "x" + research.EssenceRequired;
            research.iridiumText = research.ResearchButton.transform.Find("Iridium Amount").GetComponent<TextMeshProUGUI>();
            research.iridiumText.text = "x" + research.IridiumRequired;
        }
    }

    // Save research values when saving data
    public bool[] GetResearchData()
    {
        bool[] researched = new bool[Researchables.Length];
        for (int i = 0; i < Researchables.Length; i++)
            researched[i] = Researchables[i].IsResearched;
        return researched; // Return to caller
    }

    // Set research values when loading data
    public void SetResearchData(bool[] data)
    {
        // In the case previous data changed, system will pass that research itteration
        try
        {
            // Sets the stored research value if true
            for (int i = 0; i < data.Length; i++)
                if (data[i]) { ResearchUnlocked = true; ApplyResearch(Researchables[i]); };
        } 
        catch
        {
            // Debugs to console that the research data does not match the current versions research structure
            Debug.Log("Mismatch in save data to research data. Skipping itteration.");
        }
    }
}
