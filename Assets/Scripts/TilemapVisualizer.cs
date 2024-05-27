using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTileMap;
    [SerializeField] private TileBase floorTile, wallTopTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        Clear();
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    public void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTileMap, wallTopTile, position);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
}