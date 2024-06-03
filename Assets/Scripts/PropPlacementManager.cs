using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PropPlacementManager : MonoBehaviour
{
    private DungeonData _dungeonData;

    [SerializeField] private GameObject propContainer;

    [SerializeField] private List<Prop> propsToPlace;

    [SerializeField, Range(0, 1)] private float cornerPropPlacementChance = 0.7f;

    [SerializeField] private GameObject propPrefab;

    public UnityEvent OnFinished;

    //CHANGED
    // private void Awake()
    // {
    //     dungeonData = FindObjectOfType<DungeonData>();
    // }

    public void ProcessRooms(DungeonData dungeonData)
    {
        _dungeonData = dungeonData;
        if (_dungeonData == null)
            return;
        foreach (Room room in _dungeonData.Rooms)
        {
            room.PropPositions.UnionWith(_dungeonData.Path);
            //Place props place props in the corners
            List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, room.CornerTiles, cornerProps);

            //Place props near LEFT wall
            List<Prop> leftWallProps = propsToPlace
            .Where(x => x.NearWallLeft)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, leftWallProps, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft);
            
            //Place props near RIGHT wall
            List<Prop> rightWallProps = propsToPlace
            .Where(x => x.NearWallRight)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, rightWallProps, room.NearWallTilesRight, PlacementOriginCorner.TopRight);
            
            //Place props near UP wall
            List<Prop> topWallProps = propsToPlace
            .Where(x => x.NearWallUP)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, topWallProps, room.NearWallTilesUp, PlacementOriginCorner.TopLeft);
            
            //Place props near DOWN wall
            List<Prop> downWallProps = propsToPlace
            .Where(x => x.NearWallDown)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, downWallProps, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft);
            
            //Place inner props
            List<Prop> innerProps = propsToPlace
                .Where(x => x.Inner)
                .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
                .ToList();
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }

        //OnFinished?.Invoke();
        Invoke("RunEvent", 1);
    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    private IEnumerator TutorialCoroutine(Action code)
    {
        yield return new WaitForSeconds(3);
        code();
    }

    /// <summary>
    /// Places props near walls. We need to specify the props anw the placement start point
    /// </summary>
    /// <param name="room"></param>
    /// <param name="wallProps">Props that we should try to place</param>
    /// <param name="availableTiles">Tiles that are near the specific wall</param>
    /// <param name="placement">How to place bigger props. Ex near top wall we want to start placemt from the Top corner and find if there are free spaces below</param>
    private void PlaceProps(
        Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        //Remove path positions from the initial nearWallTiles to ensure the clear path to traverse dungeon
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        tempPositons.ExceptWith(_dungeonData.Path);

        //We will try to place all the props
        foreach (Prop propToPlace in wallProps)
        {
            //We want to place only certain quantity of each prop
            int quantity
                = UnityEngine.Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                //remove taken positions
                tempPositons.ExceptWith(room.PropPositions);
                //shuffel the positions
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                //If placement has failed there is no point in trying to place the same prop again
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)
                    break;
            }
        }
    }

    /// <summary>
    /// Tries to place the Prop using brute force (trying each available tile position)
    /// </summary>
    /// <param name="room"></param>
    /// <param name="propToPlace"></param>
    /// <param name="availablePositions"></param>
    /// <param name="placement"></param>
    /// <returns>False if there is no space. True if placement was successful</returns>
    private bool TryPlacingPropBruteForce(
        Room room, Prop propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //try placing the objects starting from the corner specified by the placement parameter
        for (int i = 0; i < availablePositions.Count; i++)
        {
            //select the specified position (but it can be already taken after placing the corner props as a group)
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //check if there is enough space around to fit the prop
            bool freePositionsAround
                = TryToFitProp(room, propToPlace, availablePositions, position, placement);

            //If we have enough spaces place the prop
            if (freePositionsAround)
            {
                //Place the gameobject
                PlacePropGameObjectAt(room, position, propToPlace);
                //Lock all the positions recquired by the prop (based on its size)
                
                
                // foreach (Vector2Int pos in freePositionsAround)
                // {
                //     //Hashest will ignore duplicate positions
                //     room.PropPositions.Add(pos);
                // }

                //Deal with groups
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, propToPlace, 1);
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the prop will fit (accordig to it size)
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="availablePositions"></param>
    /// <param name="originPosition"></param>
    /// <param name="placement"></param>
    /// <returns></returns>
    private bool TryToFitProp(
        Room room,
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();
        
        int leftBorderValue = originPosition.x - Convert.ToInt32(prop.PropSize.x / 2);
        int rightBorderValue = originPosition.x + Convert.ToInt32(prop.PropSize.x / 2);
        int downBorderValue = originPosition.y - Convert.ToInt32(prop.PropSize.y / 2);
        int upBorderValue = originPosition.y + Convert.ToInt32(prop.PropSize.y / 2);
        for (int x = leftBorderValue; x <= rightBorderValue; x++)
        {
            for (int y = downBorderValue; y <= upBorderValue; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (room.PropPositions.Contains(position))
                    return false;
            }
        }

        //Perform the specific loop depending on the PlacementOriginCorner
        // if (placement == PlacementOriginCorner.BottomLeft)
        // {
        //     for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
        //     {
        //         for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
        //         {
        //             Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
        //             if (availablePositions.Contains(tempPos))
        //                 freePositions.Add(tempPos);
        //         }
        //     }
        // }
        // else if (placement == PlacementOriginCorner.BottomRight)
        // {
        //     for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
        //     {
        //         for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
        //         {
        //             Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
        //             if (availablePositions.Contains(tempPos))
        //                 freePositions.Add(tempPos);
        //         }
        //     }
        // }
        // else if (placement == PlacementOriginCorner.TopLeft)
        // {
        //     for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
        //     {
        //         for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
        //         {
        //             Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
        //             if (availablePositions.Contains(tempPos))
        //                 freePositions.Add(tempPos);
        //         }
        //     }
        // }
        // else
        // {
        //     for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
        //     {
        //         for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
        //         {
        //             Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
        //             if (availablePositions.Contains(tempPos))
        //                 freePositions.Add(tempPos);
        //         }
        //     }
        // }

        return true;
    }

    /// <summary>
    /// Places props in the corners of the room
    /// </summary>
    /// <param name="room"></param>
    /// <param name="cornerTiles"></param>
    /// <param name="cornerProps"></param>
    private void PlaceCornerProps(Room room, HashSet<Vector2Int> cornerTiles, List<Prop> cornerProps)
    {
        float tempChance = cornerPropPlacementChance;
        HashSet<Vector2Int> tempPositions = new HashSet<Vector2Int>(cornerTiles);
        tempPositions.ExceptWith(_dungeonData.Path);
        

        foreach (Vector2Int cornerTile in tempPositions)
        {
            if (UnityEngine.Random.value < tempChance)
            {
                Prop propToPlace
                    = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];

                PlacePropGameObjectAt(room, cornerTile, propToPlace);
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 2);
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    /// <summary>
    /// Helps to find free spaces around the groupOriginPosition to place a prop as a group
    /// </summary>
    /// <param name="room"></param>
    /// <param name="groupOriginPosition"></param>
    /// <param name="propToPlace"></param>
    /// <param name="searchOffset">The search offset ex 1 = we will check all tiles withing the distance of 1 unity away from origin position</param>
    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
    {
        //*Can work poorely when placing bigger props as groups

        //calculate how many elements are in the group -1 that we have placed in the center
        int count = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        //find the available spaces around the center point.
        //we use searchOffset to limit the distance between those points and the center point
        List<Vector2Int> availableSpaces = new List<Vector2Int>();
        for (int xOffset = -searchOffset; xOffset <= searchOffset; xOffset++)
        {
            for (int yOffset = -searchOffset; yOffset <= searchOffset; yOffset++)
            {
                Vector2Int tempPos = groupOriginPosition + new Vector2Int(xOffset, yOffset);
                if (room.FloorTiles.Contains(tempPos) &&
                    !_dungeonData.Path.Contains(tempPos) &&
                    !room.PropPositions.Contains(tempPos))
                {
                    availableSpaces.Add(tempPos);
                }
            }
        }

        //shuffle the list
        availableSpaces.OrderBy(x => Guid.NewGuid());

        //place the props (as many as we want or if there is less space fill all the available spaces)
        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObjectAt(room, availableSpaces[i], propToPlace);
        }
    }

    /// <summary>
    /// Place a prop as a new GameObject at a specified position
    /// </summary>
    /// <param name="room"></param>
    /// <param name="placementPostion"></param>
    /// <param name="propToPlace"></param>
    /// <returns></returns>
    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPosition, Prop propToPlace)
    {
        Debug.Log($"Potential Placement position: {placementPosition}");
        // Instantiate the prop at this position
        GameObject prop = Instantiate(propPrefab);
        prop.transform.SetParent(propContainer.transform);

        if (prop == null)
        {
            Debug.LogError("Failed to instantiate propPrefab.");
            return null;
        }

        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        if (propSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found in propPrefab.");
            return null;
        }

        // Set the sprite
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        // Add a collider
        CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        collider.offset = Vector2.zero;
        if (propToPlace.PropSize.x > propToPlace.PropSize.y)
        {
            collider.direction = CapsuleDirection2D.Horizontal;
        }

        Vector2 size = new Vector2(propToPlace.PropSize.x * 0.8f, propToPlace.PropSize.y * 0.8f);
        collider.size = size;

        // Устанавливаем позицию объекта в глобальных координатах
        float xOffset = propToPlace.PropSize.x / 2;
        float yOffset = propToPlace.PropSize.y / 2;
        prop.transform.position = (Vector2)placementPosition + new Vector2(0.5f, 1);
        Debug.Log($"Prop global coordinates after setting position: {prop.transform.position}");

        // Убедимся, что масштаб и ротация установлены правильно
        prop.transform.localScale = Vector3.one;
        prop.transform.rotation = Quaternion.identity;

        // Проверка слоев и порядка отрисовки
        propSpriteRenderer.sortingLayerName = "Ground"; // Убедитесь, что слой существует
        propSpriteRenderer.sortingOrder = 1;

        // Adjust the position to the sprite
        // Применяем смещение (если необходимо) для корректной локальной позиции спрайта
        
        // propSpriteRenderer.transform.localPosition = Vector2.zero;
        // Debug.Log(
        //     $"Prop sprite local coordinates after setting local position: {propSpriteRenderer.transform.localPosition}");

        // Установка локальной позиции спрайта в (4, 4)
        propSpriteRenderer.transform.localPosition = (Vector2)placementPosition + new Vector2(0.5f, 1);
        Debug.Log($"Prop sprite local coordinates after adjustment: {propSpriteRenderer.transform.localPosition}");

        // Проверка родительского объекта
        if (prop.transform.parent != null)
        {
            Debug.Log($"Parent object: {prop.transform.parent.name}");
        }
        else
        {
            Debug.Log("Parent object: None");
        }

        // Save the prop in the room data (so in the dungeon data)
        int leftBorderValue = placementPosition.x - Convert.ToInt32(propToPlace.PropSize.x / 2);
        int rightBorderValue = placementPosition.x + Convert.ToInt32( propToPlace.PropSize.x / 2);
        int downBorderValue = placementPosition.y - Convert.ToInt32(propToPlace.PropSize.y / 2);
        int upBorderValue = placementPosition.y + Convert.ToInt32(propToPlace.PropSize.y / 2);
        room.PropPositions.Add(placementPosition);
        for (int x = leftBorderValue -1; x <= rightBorderValue + 1; x++)
        {
            for (int y = downBorderValue - 1; y <= upBorderValue + 1; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                room.PropPositions.Add(position);
            }
        }
        room.PropObjectReferences.Add(prop);
        return prop;
    }
}

/// <summary>
/// Where to start placing the prop ex. start at BottomLeft corner and search 
/// if there are free space to the Right and Up in case of placing a biggex prop
/// </summary>
public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}