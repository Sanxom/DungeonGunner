using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector, NonSerialized] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    private const string GROUND_TILEMAP_NAME = "groundTilemap";
    private const string DECORATION_1_TILEMAP_NAME = "decoration1Tilemap";
    private const string DECORATION_2_TILEMAP_NAME = "decoration2Tilemap";
    private const string FRONT_TILEMAP_NAME = "frontTilemap";
    private const string COLLISION_TILEMAP_NAME = "collisionTilemap";
    private const string MINIMAP_TILEMAP_NAME = "minimapTilemap";

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider2D.bounds;
    }

    public void Init(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        DisableCollisionTilemapRenderer();
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameObject)
    {
        grid = roomGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag(GROUND_TILEMAP_NAME))
                groundTilemap = tilemap;
            else if (tilemap.CompareTag(DECORATION_1_TILEMAP_NAME))
                decoration1Tilemap = tilemap;
            else if (tilemap.CompareTag(DECORATION_2_TILEMAP_NAME))
                decoration2Tilemap = tilemap;
            else if (tilemap.CompareTag(FRONT_TILEMAP_NAME))
                frontTilemap = tilemap;
            else if (tilemap.CompareTag(COLLISION_TILEMAP_NAME))
                collisionTilemap = tilemap;
            else if (tilemap.CompareTag(MINIMAP_TILEMAP_NAME))
                minimapTilemap = tilemap;
        }
    }

    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}