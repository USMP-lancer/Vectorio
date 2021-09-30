using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuildingSystem : MonoBehaviour
{
    // Grid variable
    public GridSystem tileGrid;

    // Building variables
    public static BuildingSystem active;
    private Vector2 offset;
    public float border = 742.5f;

    [HideInInspector] public Entity selected;
    [HideInInspector] public bool canDelete = true;

    // Enemy variables
    public Variant variant;
    public LayerMask enemyLayer;

    // Sprite values
    private SpriteRenderer spriteRenderer;
    private float alphaAdjust = 0.005f;
    private float alphaHolder;

    // Start method grabs tilemap
    public void Start()
    {
        // Grabs active component if it exists
        if (this != null) active = this;
        else active = null;

        // Sets static variables on start
        tileGrid = new GridSystem();
        tileGrid.cells = new Dictionary<Vector2Int, Cell>();
        selected = null;
        offset = new Vector2(0, 0);

        // Sets static anim variables
        spriteRenderer = GetComponent<SpriteRenderer>();
        alphaHolder = alphaAdjust;

        // Setup events
        Events.active.onRightMousePressed += DeleteTile;
        Events.active.onRightMouseReleased += Deselect;
    }

    // Update is called once per frame
    public void Update()
    {
        // Check if active is null
        if (active == null) return;

        // Round to grid
        OffsetBuilding();
        AdjustTransparency();
    }
   
    private void OffsetBuilding()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (selected != null)
        {
            if (selected.snap) transform.position = new Vector2(5 * Mathf.Round(mousePos.x / 5) + offset.x, 5 * Mathf.Round(mousePos.y / 5) + offset.y);
            else transform.position = new Vector2(mousePos.x + offset.x, mousePos.y + offset.y);
        }
        else
        {
            transform.position = new Vector2(5 * Mathf.Round(mousePos.x / 5), 5 * Mathf.Round(mousePos.y / 5));
        }
    }

    // Adjusts the alpha transparency of the SR component 
    private void AdjustTransparency()
    {
        // Switches
        if (spriteRenderer.color.a >= 1f)
            alphaHolder = -alphaAdjust;
        else if (spriteRenderer.color.a <= 0f)
            alphaHolder = alphaAdjust;

        // Set alpha
        spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a + alphaHolder);
    }

    // Sets the selected building
    public void SetBuilding(Entity entity)
    {
        selected = entity;
        if (entity != null)
        {
            canDelete = false;

            spriteRenderer.sprite = Sprites.GetSprite(entity.name);
            transform.localScale = new Vector2(entity.size, entity.size);
            offset = entity.tile.offset;
        }
        else
        {
            canDelete = true;

            spriteRenderer.sprite = Sprites.GetSprite("Transparent");
        }
    }

    // Sets the tiles for a specified entity
    public void SetTiles(Entity entity, GameObject obj)
    {
        // Attempt to get the default building script
        BaseTile building = obj.GetComponent<BaseTile>();

        // Check to see if the building contains a DB script
        if (building == null)
        {
            Debug.LogError("Entity has cells but does not contain a DefaultBuilding script!\nRemoving from scene.");
            Destroy(obj);
            return;
        }

        // Set the tiles on the grid class
        Vector2Int coords;
        if (entity.tile.cells.Length > 0)
        {
            foreach (Tile.Cell cell in entity.tile.cells)
            {
                coords = Vector2Int.RoundToInt(new Vector2(obj.transform.position.x + cell.x, obj.transform.position.y + cell.y));
                tileGrid.SetCell(coords, true, entity.tile, building);
                building.cells.Add(coords);
            }
        }
    }

    // Checks to make sure tile(s) isn't occupied
    public bool CheckTiles()
    {
        float xCoord, yCoord;

        if (selected.tile.cells.Length > 0)
        {
            foreach (Tile.Cell cell in selected.tile.cells)
            {
                xCoord = transform.position.x + cell.x;
                yCoord = transform.position.y + cell.y;

                if (tileGrid.RetrieveCell(Vector2Int.RoundToInt(new Vector2(xCoord, yCoord))) != null) return false;
                else if (xCoord < -border || xCoord > border || yCoord < -border || yCoord > border) return false;
            }
        }
        return true;
    }

    // Creates a building
    public void CmdCreateBuilding()
    {
        // Check if active is null
        if (selected == null || selected.obj == null) return;

        // Check if snap is enabled
        if (!selected.snap)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
            foreach(RaycastHit2D hit in hits) 
                if (hit.collider != null && (hit.collider.GetComponent<DefaultEnemy>() != null ||
                    hit.collider.GetComponent<DefaultGuardian>() != null)) return;
        }
        else if (!CheckTiles()) return;

        // Instantiate the object like usual
        if (Instantiator.active != null)
        {
            GameObject last = Instantiator.active.CreateEntity(selected, transform);
            if (last != null && selected.tile.cells.Length > 0) SetTiles(selected, last);
        }
        else Debug.LogError("Scene is missing an instantiator!");
    }

    public BaseTile GetClosestBuilding(Vector2Int position)
    {
        BaseTile nearest = null;
        float distance = float.PositiveInfinity;

        foreach (KeyValuePair<Vector2Int, Cell> cell in tileGrid.cells)
        {
            float holder = Vector2Int.Distance(position, cell.Key);
            if (holder < distance)
            {
                distance = holder;
                nearest = cell.Value.building;
            }
        }

        return nearest;
    }

    private void Deselect()
    {
        if (selected != null)
            SetBuilding(null);
    }

    public void DeleteTile()
    {
        if (canDelete)
        {
            tileGrid.DestroyCell(Vector2Int.RoundToInt(transform.position));
        }
    }


    public void ClearBuildings()
    {
        tileGrid.DestroyAllCells();
    }
}
