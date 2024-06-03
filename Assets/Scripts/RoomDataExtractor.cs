using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class RoomDataExtractor : MonoBehaviour
{
    private DungeonData _dungeonData;
    private TilemapVisualizer _tilemapVisualizer;

    [SerializeField] private Tilemap gizmoMap;

    [SerializeField] private TileBase innerTile,
        upTile,
        downTile,
        rightTile,
        leftTile,
        cornerTile;
    
    // CHANGED
    // [SerializeField]
    private bool showGizmo = true;

    //CHANGED
    //public UnityEvent OnFinishedRoomProcessing;

    // CHANGED
    // private void Awake()
    // {
    //     dungeonData = FindObjectOfType<DungeonData>();
    // }
    public void ProcessRooms(DungeonData dungeonData, TilemapVisualizer tilemapVisualizer)
    {
        _dungeonData = dungeonData;
        _tilemapVisualizer = tilemapVisualizer;
        gizmoMap.ClearAllTiles();
        if (_dungeonData == null)
            return;

        foreach (Room room in _dungeonData.Rooms)
        {
            //find corener, near wall and inner tiles
            foreach (Vector2Int tilePosition in room.FloorTiles)
            {
                int neighboursCount = 4;

                if(room.FloorTiles.Contains(tilePosition+Vector2Int.up) == false)
                {
                    room.NearWallTilesUp.Add(tilePosition);
                    neighboursCount--;
                }
                if (room.FloorTiles.Contains(tilePosition + Vector2Int.down) == false)
                {
                    room.NearWallTilesDown.Add(tilePosition);
                    neighboursCount--;
                }
                if (room.FloorTiles.Contains(tilePosition + Vector2Int.right) == false)
                {
                    room.NearWallTilesRight.Add(tilePosition);
                    neighboursCount--;
                }
                if (room.FloorTiles.Contains(tilePosition + Vector2Int.left) == false)
                {
                    room.NearWallTilesLeft.Add(tilePosition);
                    neighboursCount--;
                }

                //find corners
                if (neighboursCount <= 2)
                    room.CornerTiles.Add(tilePosition);

                if (neighboursCount == 4)
                    room.InnerTiles.Add(tilePosition);
            }

            room.NearWallTilesUp.ExceptWith(room.CornerTiles);
            room.NearWallTilesDown.ExceptWith(room.CornerTiles);
            room.NearWallTilesLeft.ExceptWith(room.CornerTiles);
            room.NearWallTilesRight.ExceptWith(room.CornerTiles);          
        }
        
        PaintGizmo();

        //OnFinishedRoomProcessing?.Invoke();
        
        // CHANGED
        // Invoke("RunEvent", 1);
    }

    //CHANGED
    // public void RunEvent()
    // {
    //     OnFinishedRoomProcessing?.Invoke();
    // }

    private void PaintGizmo()
    {
        if (_dungeonData == null || showGizmo == false)
            return;
        foreach (Room room in _dungeonData.Rooms)
        {
            //Draw inner tiles
            //Gizmos.color = Color.yellow;
            foreach (Vector2Int floorPosition in room.InnerTiles)
            {
                if (_dungeonData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, innerTile, floorPosition);
                //Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles UP
            //Gizmos.color = Color.blue;
            foreach (Vector2Int floorPosition in room.NearWallTilesUp)
            {
                if (_dungeonData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, upTile, floorPosition);
                //Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles DOWN
            //Gizmos.color = Color.green;
            foreach (Vector2Int floorPosition in room.NearWallTilesDown)
            {
                if (_dungeonData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, downTile, floorPosition);
                //Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles RIGHT
            //Gizmos.color = Color.white;
            foreach (Vector2Int floorPosition in room.NearWallTilesRight)
            {
                if (_dungeonData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, rightTile, floorPosition);
                //Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles LEFT
            //Gizmos.color = Color.cyan;
            foreach (Vector2Int floorPosition in room.NearWallTilesLeft)
            {
                if (_dungeonData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, leftTile, floorPosition);
                //Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
            //Draw near wall tiles CORNERS
            //Gizmos.color = Color.magenta;
            foreach (Vector2Int floorPosition in room.CornerTiles)
            {
                if (_dungeonData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, cornerTile, floorPosition);
                //Gizmos.DrawCube(floorPosition + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}