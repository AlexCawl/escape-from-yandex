using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable All

public class PropPlacementManager : MonoBehaviour
{
    private MapData _mapData;

    [SerializeField] private GameObject propContainer;

    [SerializeField] private List<Prop> propsToPlace;

    [SerializeField, Range(0, 1)] private float cornerPropPlacementChance = 0.7f;

    [SerializeField] private GameObject propPrefab;
    
    public void ProcessRooms(MapData mapData)
    {
        _mapData = mapData;
        if (_mapData == null)
            return;
        
        
        foreach (Room room in _mapData.Rooms)
        {
            // добавление направления пути, чтобы на нем не стояло мебели
            room.PropPositions.UnionWith(_mapData.Path);
            
            // Размещение в углах
            List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, room.CornerTiles, cornerProps);
        
            // Размещение у левой стены
            List<Prop> leftWallProps = propsToPlace
            .Where(x => x.NearWallLeft)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, leftWallProps, room.NearWallTilesLeft);
            
            // Размещение у правой стены
            List<Prop> rightWallProps = propsToPlace
            .Where(x => x.NearWallRight)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, rightWallProps, room.NearWallTilesRight);
            
            // Размещение у верхней стены
            List<Prop> topWallProps = propsToPlace
            .Where(x => x.NearWallUP)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, topWallProps, room.NearWallTilesUp);
            
            // Размещение у нижней стены
            List<Prop> downWallProps = propsToPlace
            .Where(x => x.NearWallDown)
            .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
            .ToList();
            
            PlaceProps(room, downWallProps, room.NearWallTilesDown);
            
            // Размещение внутри комнаты
            List<Prop> innerProps = propsToPlace
                .Where(x => x.Inner)
                .OrderByDescending(x => x.PropSize.x * x.PropSize.y)
                .ToList();
            PlaceProps(room, innerProps, room.InnerTiles);
        }

        List<Prop> techoProp = propsToPlace.Where(x => x.TechnoRoom).ToList();
        PlaceProps(_mapData.techRoom, techoProp, _mapData.techRoom.NearWallTilesUp);

        List<Prop> endRoomProps = propsToPlace.Where(x => x.EndRoom).ToList();
        Prop doorProp = endRoomProps[0];
        PlaceDoor(_mapData.endRoom, doorProp);
    }
    
    // Функция размещения мебели
    private void PlaceProps(
        Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles)
    {
        // Удаляем путь из возможных позиций чтобы путь был чистым 
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        tempPositons.ExceptWith(_mapData.Path);

        // Перебираем все элементы  
        foreach (Prop propToPlace in wallProps)
        {
            // Выбираем количество определенного элемента
            int quantity
                = UnityEngine.Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                // убираем уже занятые позиции
                tempPositons.ExceptWith(room.PropPositions);
                // перемешиваем позиции для рандома
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                // Пытаемся разместить элемент на карте
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions) == false)
                {
                    break;
                }
                    
            }
        }
    }
    
    
    private bool TryPlacingPropBruteForce(
        Room room, Prop propToPlace, List<Vector2Int> availablePositions)
    {
        // Перебираем позиции для размещения 
        for (int i = 0; i < availablePositions.Count; i++)
        {
            // Проверяем свободна ли позиция 
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;
            
            // Проверяем близлежащие тайлы
            bool freePositionsAround
                = TryToFitProp(room, propToPlace, availablePositions, position);

            // Размещаем объект если место свободно
            if (freePositionsAround)
            {
                PlacePropGameObjectAt(room, position, propToPlace);
                return true;
            }
        }
        return false;
    }
    
    
    private bool TryToFitProp(
        Room room,
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition
        )
    {
        List<Vector2Int> freePositions = new();
        
        // Вычисляем границы проверяемой зоны
        int leftBorderValue = originPosition.x - Convert.ToInt32(prop.PropSize.x / 2);
        int rightBorderValue = originPosition.x + Convert.ToInt32(prop.PropSize.x / 2);
        int downBorderValue = originPosition.y - Convert.ToInt32(prop.PropSize.y / 2);
        int upBorderValue = originPosition.y + Convert.ToInt32(prop.PropSize.y / 2);
        
        // Досканально проверяем каждый тайл
        for (int x = leftBorderValue; x <= rightBorderValue; x++)
        {
            for (int y = downBorderValue; y <= upBorderValue; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (room.PropPositions.Contains(position))
                    return false;
            }
        }
        
        return true;
    }

   
    private void PlaceCornerProps(Room room, HashSet<Vector2Int> cornerTiles, List<Prop> cornerProps)
    {
        float tempChance = cornerPropPlacementChance;
        HashSet<Vector2Int> tempPositions = new HashSet<Vector2Int>(cornerTiles);
        tempPositions.ExceptWith(_mapData.Path);
        

        foreach (Vector2Int cornerTile in tempPositions)
        {
            if (UnityEngine.Random.value < tempChance)
            {
                Prop propToPlace
                    = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];

                PlacePropGameObjectAt(room, cornerTile, propToPlace);
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }
    
   
    private void PlacePropGameObjectAt(Room room, Vector2Int placementPosition, Prop propToPlace)
    {
        // НАПОМИНАНИЕ: ПОНАСТАВИЛ ТУТ ЛОГОВ ЧЕРТ НОГУ СЛОМИТ
        //Debug.Log($"Potential Placement position: {placementPosition}");
        
        // Создаем объект-мебель
        GameObject prop = Instantiate(propPrefab);
        prop.transform.SetParent(propContainer.transform);

        // НАПОМИНАНИЕ: СОТРИ ТЫ УЖЕ ЭТИ ЛОГИ ВСЕ У ТЕБЯ НОРМАЛЬНО РАБОТАЕТ ХОРОШ ССАТЬ
        // if (prop == null)
        // {
        //     Debug.LogError("Failed to instantiate propPrefab.");
        //     return null;
        // }

        // Настраиваем рендер спрайтов для отрисовки
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        // if (propSpriteRenderer == null)
        // {
        //     Debug.LogError("SpriteRenderer component not found in propPrefab.");
        //     return null;
        // }

        // Устанавливаем спрайт
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        // Добавляем коллайдер
        PolygonCollider2D collider = propSpriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
        

        // Устанавливаем позицию объекта в глобальных координатах
        prop.transform.position = (Vector2)placementPosition + new Vector2(0.5f, 1);
        
        // НАПОМИНАНИЕ: СТЕРЕТЬ ЛОГИ
        //Debug.Log($"Prop global coordinates after setting position: {prop.transform.position}");
        propSpriteRenderer.transform.localPosition = (Vector2)placementPosition + new Vector2(0.5f, 1);
        
        // НАПОМИНАНИЕ: СТЕРЕТЬ ЛОГИ
        //Debug.Log($"Prop sprite local coordinates after adjustment: {propSpriteRenderer.transform.localPosition}");
        
        // Проверка слоев и порядка отрисовки
        propSpriteRenderer.sortingLayerName = "Ground"; // Убедитесь, что слой существует
        propSpriteRenderer.sortingOrder = 1;
        

        // Записываем позиции объекта в сет занятых тайлов
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
    }

    private void PlaceDoor(Room room, Prop propToPlace)
    {
        // Создаем объект-мебель
        GameObject prop = Instantiate(propPrefab);
        prop.transform.SetParent(propContainer.transform);
        
        // Настраиваем рендер спрайтов для отрисовки
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();
        

        // Устанавливаем спрайт
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        
        int xPosition = Convert.ToInt32(room.RoomCenterPos.x);
        int yPosition = Convert.ToInt32(room.NearWallTilesUp.ToList()[0].y);
        Vector2 position = new Vector2(xPosition, yPosition + 2);
        

        // Устанавливаем позицию объекта в глобальных координатах
        prop.transform.position = position;
        
        // НАПОМИНАНИЕ: СТЕРЕТЬ ЛОГИ
        //Debug.Log($"Prop global coordinates after setting position: {prop.transform.position}");
        propSpriteRenderer.transform.localPosition = position;
        
        // НАПОМИНАНИЕ: СТЕРЕТЬ ЛОГИ
        //Debug.Log($"Prop sprite local coordinates after adjustment: {propSpriteRenderer.transform.localPosition}");
        
        // Проверка слоев и порядка отрисовки
        propSpriteRenderer.sortingLayerName = "Ground"; // Убедитесь, что слой существует
        propSpriteRenderer.sortingOrder = 1;
    }
}