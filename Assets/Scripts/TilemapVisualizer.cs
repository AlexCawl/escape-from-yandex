using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTileMap;
    [SerializeField] private TileBase floorTile, wallTopTileFirst, wallTopTileSecond , wallSideRight, wallSideLeft, wallBottom, 
        wallFull, wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight,
        wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        Clear();
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.WallTop.Contains(typeAsInt))
            tile = wallTopTileSecond;
        else if (WallTypesHelper.WallSideRight.Contains(typeAsInt))
            tile = wallSideRight;
        else if (WallTypesHelper.WallSideLeft.Contains(typeAsInt))
            tile = wallSideLeft;
        else if (WallTypesHelper.WallBottom.Contains(typeAsInt))
            tile = wallBottom;
        else if (WallTypesHelper.WallFull.Contains(typeAsInt))
            tile = wallFull;
        if (tile != null)
            PaintSingleTile(wallTileMap, tile, position);
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

    public void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.WallInnerCornerDownLeft.Contains(typeAsInt))
            tile = wallInnerCornerDownLeft;
        else if (WallTypesHelper.WallInnerCornerDownRight.Contains(typeAsInt))
            tile = wallInnerCornerDownRight;
        else if (WallTypesHelper.WallDiagonalCornerDownLeft.Contains(typeAsInt))
            tile = wallDiagonalCornerDownLeft;
        else if (WallTypesHelper.WallDiagonalCornerDownRight.Contains(typeAsInt))
            tile = wallDiagonalCornerDownRight;
        else if (WallTypesHelper.WallDiagonalCornerUpRight.Contains(typeAsInt))
            tile = wallDiagonalCornerUpRight;
        else if (WallTypesHelper.WallDiagonalCornerUpLeft.Contains(typeAsInt))
            tile = wallDiagonalCornerUpLeft;
        else if (WallTypesHelper.WallFullEightDirections.Contains(typeAsInt))
            tile = wallFull;
        else if (WallTypesHelper.WallBottomEightDirections.Contains(typeAsInt))
            tile = wallBottom;
        
        if (tile != null)
            PaintSingleTile(wallTileMap, tile, position);
    }

    public void PaintSpecialWalls(Vector2Int position)
    {
        TileBase tile = wallTopTileFirst;
        PaintSingleTile(wallTileMap, tile, position);
    }
}