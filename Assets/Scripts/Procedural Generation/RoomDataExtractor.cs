using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomDataExtractor : MonoBehaviour
{
    private MapData _mapData;
    private TilemapVisualizer _tilemapVisualizer;

    [SerializeField] private Tilemap gizmoMap;

    [SerializeField] private TileBase innerTile,
        upTile,
        downTile,
        rightTile,
        leftTile,
        cornerTile;
    
    
    public void ProcessRooms(MapData mapData, TilemapVisualizer tilemapVisualizer)
    {
        _mapData = mapData;
        _tilemapVisualizer = tilemapVisualizer;
        gizmoMap.ClearAllTiles();
        if (_mapData == null)
            return;

        foreach (Room room in _mapData.Rooms)
        {
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
    }
    
    
    private void PaintGizmo()
    {
        if (_mapData == null)
            return;
        foreach (Room room in _mapData.Rooms)
        {
            foreach (Vector2Int floorPosition in room.InnerTiles)
            {
                if (_mapData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, innerTile, floorPosition);
            }
            
            foreach (Vector2Int floorPosition in room.NearWallTilesUp)
            {
                if (_mapData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, upTile, floorPosition);
            }
            
            foreach (Vector2Int floorPosition in room.NearWallTilesDown)
            {
                if (_mapData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, downTile, floorPosition);
            }
            
            foreach (Vector2Int floorPosition in room.NearWallTilesRight)
            {
                if (_mapData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, rightTile, floorPosition);
            }
            
            foreach (Vector2Int floorPosition in room.NearWallTilesLeft)
            {
                if (_mapData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, leftTile, floorPosition);
            }
            
            foreach (Vector2Int floorPosition in room.CornerTiles)
            {
                if (_mapData.Path.Contains(floorPosition))
                    continue;
                _tilemapVisualizer.PaintSingleTile(gizmoMap, cornerTile, floorPosition);
            }
        }
    }
}