using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Prop : ScriptableObject
{
    [Header("Prop data:")]
    public Sprite propSprite;
    /// <summary>
    /// Affects the collider size of the prop
    /// </summary>
    public Vector2Int PropSize = Vector2Int.one;

    [Space, Header("Placement type:")]
    public bool Corner = true;
    public bool NearWallUP = true;
    public bool NearWallDown = true;
    public bool NearWallRight = true;
    public bool NearWallLeft = true;
    public bool Inner = true;
    
    [Min(0)]
    public int PlacementQuantityMin = 1;
    [Min(0)]
    public int PlacementQuantityMax = 1;
}