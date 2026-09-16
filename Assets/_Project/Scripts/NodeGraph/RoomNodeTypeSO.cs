using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("Only flag RoomNodeTypes that should be visible in the Editor")]
    #endregion
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("One type should be a Corridor")]
    #endregion
    public bool isCorridor;
    #region Header
    [Header("One type should be a CorridorNS")]
    #endregion
    public bool isCorridorNS;
    #region Header
    [Header("One type should be a CorridorEW")]
    #endregion
    public bool isCorridorEW;
    #region Header
    [Header("One type should be an Entrance")]
    #endregion
    public bool isEntrance;
    #region Header
    [Header("One type should be a Boss Room")]
    #endregion
    public bool isBossRoom;
    #region Header
    [Header("One type should be None (Unassigned)")]
    #endregion
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
