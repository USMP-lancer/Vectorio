using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class StatsPanel : MonoBehaviour
{
    // Active or not
    public static bool isOpen = false;

    // Additional settings
    [System.Serializable]
    public class AdditionalSetting
    {
        public Entity entity;
        public bool useButton;
        public bool useSelector;
        public HorizontalSelector[] selectors;
    }

    // Entityvariables
    public Entity entity;
    public BaseEntity selectedEntity;

    // Stat variables
    public Image buildingIcon;
    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI buildingDesc;
    public ProgressBar buildingHealth;
    public GameObject button;
    public GameObject selector;
    public GameObject noSettings;
    public AdditionalSetting[] additionalSettings;
    public static CanvasGroup canvasGroup;

    // Runtime created
    public Dictionary<Entity, AdditionalSetting> settings = new Dictionary<Entity, AdditionalSetting>();

    // Setup runtime 
    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Events.active.onEntityClicked += SetPanel;

        foreach(AdditionalSetting additionalSetting in additionalSettings)
            settings.Add(additionalSetting.entity, additionalSetting);

        foreach (HorizontalSelector selector in settings[entity].selectors)
        {
            selector.index = 0;
            selector.UpdateUI();
        }
    }

    // Keep entity updated
    public void Update()
    {
        if (selectedEntity != null)
        {
            buildingHealth.currentPercent = selectedEntity.health / selectedEntity.maxHealth * 100;
            buildingHealth.UpdateUI();
        }
        else CloseMenu();
    }

    // Set panel
    public void SetPanel(BaseEntity baseEntity)
    {
        if (BuildingController.entitySelected || Inventory.isOpen || ResearchUI.isOpen || isOpen || InputController.shiftHeld) return;

        if (baseEntity == null)
        {
            Debug.Log("A building with a null value was passed to the panel class");
            return;
        }

        if (ScriptableLoader.allLoadedEntities.ContainsKey(baseEntity.name))
        {
            entity = ScriptableLoader.allLoadedEntities[baseEntity.name];
            selectedEntity = baseEntity;

            buildingIcon.sprite = Sprites.GetSprite(entity.name);
            buildingName.text = baseEntity.name;
            buildingDesc.text = entity.description;
            buildingHealth.currentPercent = baseEntity.health / baseEntity.maxHealth * 100;
            buildingHealth.UpdateUI();

            if (settings.ContainsKey(entity))
            {
                Debug.Log("Settings found on "+ entity.name + ", applying");
                selector.SetActive(settings[entity].useSelector);
                button.SetActive(settings[entity].useButton);
                noSettings.SetActive(false);
                foreach (HorizontalSelector selector in settings[entity].selectors)
                {
                    selector.index = baseEntity.metadata;
                    selector.UpdateUI();
                }
            }
            else
            {
                Debug.Log("No settings were found for " + entity.name);
                selector.SetActive(false);
                button.SetActive(false);
                noSettings.SetActive(true);
            }

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            OpenMenu();
        }
        else Debug.Log("A building with no registered entity counterpart was clicked.\nCannot display data!");
    }

    // Value changes
    public void ChangeSettings(int index)
    {
        if (entity == null)
        {
            Debug.Log("No selected entity");
            return;
        }

        if (selectedEntity != null && settings.ContainsKey(entity))
        {
            AdditionalSetting setting = settings[entity];
            if (index < setting.selectors.Length)
                Communicator.active.SyncMetadata(selectedEntity.runtimeID, setting.selectors[index].index);
            else Debug.Log("Index passed was outside of available selectors");
        }
        else Debug.Log("No settings available for " + entity.name);
    }

    // Button clicked
    public void ButtonClicked()
    {
        if (entity == null)
        {
            Debug.Log("No selected entity");
            return;
        }

        if (selectedEntity != null && settings.ContainsKey(entity)) selectedEntity.ButtonClicked();
    }

    // Open menu
    public static void OpenMenu()
    {
        isOpen = true;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    // Close menu
    public static void CloseMenu()
    {
        isOpen = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
