using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
{
    [NonSerialized] public Dictionary<string, Room> dungeonBuilderRoomDictionary = new();

    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool wasDungeonBuildSuccessful;

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();

        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        LoadRoomTemplatesIntoDictionary();

        wasDungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!wasDungeonBuildSuccessful && dungeonBuildAttempts < Settings.MAX_DUNGEON_BUILD_ATTEMPTS)
        {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            wasDungeonBuildSuccessful = false;

            while (!wasDungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.MAX_DUNGEON_REBUILD_ATTEMPTS_FOR_ROOM_GRAPH)
            {
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                wasDungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (wasDungeonBuildSuccessful)
                InstantiateRoomGameObjects();
        }

        return wasDungeonBuildSuccessful;
    }

    #region Helper Methods
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID) => roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate) ? roomTemplate : null;
    public Room GetRoomByRoomID(string roomID) => dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room) ? room : null;
    #endregion

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new()
        {
            templateID = roomTemplate.guid,
            id = roomNode.id,
            prefab = roomTemplate.prefab,
            roomNodeType = roomTemplate.roomNodeType,
            lowerBounds = roomTemplate.lowerBounds,
            upperBounds = roomTemplate.upperBounds,
            spawnPositionArray = roomTemplate.spawnPositionArray,
            templateLowerBounds = roomTemplate.lowerBounds,
            templateUpperBounds = roomTemplate.upperBounds,
            childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList),
            doorwayList = CopyDoorwayList(roomTemplate.doorwayList)
        };

        // ENTRANCE
        if (roomNode.parentRoomNodeIDList.Count == 0)
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
        }
        else
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];

        return room;
    }

    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.North:
                case Orientation.South:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;
                case Orientation.East:
                case Orientation.West:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                case Orientation.None:
                    break;
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {
        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.East && doorwayToCheck.orientation == Orientation.West)
                return doorwayToCheck;
            else if (parentDoorway.orientation == Orientation.West && doorwayToCheck.orientation == Orientation.East)
                return doorwayToCheck;
            else if (parentDoorway.orientation == Orientation.North && doorwayToCheck.orientation == Orientation.South)
                return doorwayToCheck;
            else if (parentDoorway.orientation == Orientation.South && doorwayToCheck.orientation == Orientation.North)
                return doorwayToCheck;
        }

        return null;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new()
            {
                position = doorway.position,
                orientation = doorway.orientation,
                doorPrefab = doorway.doorPrefab,
                isConnected = doorway.isConnected,
                isUnavailable = doorway.isUnavailable,
                doorwayStartCopyPosition = doorway.doorwayStartCopyPosition,
                doorwayCopyTileHeight = doorway.doorwayCopyTileHeight,
                doorwayCopyTileWidth = doorway.doorwayCopyTileWidth
            };

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new();

        foreach (string stringValue in oldStringList)
            newStringList.Add(stringValue);

        return newStringList;
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        else
        {
            Debug.Log("No room node graphs in the list!");
            return null;
        }
    }

    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
            if (roomTemplate.roomNodeType == roomNodeType)
                matchingRoomTemplateList.Add(roomTemplate);

        if (matchingRoomTemplateList.Count == 0)
            return null;

        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            if (IsOverlappingRoom(roomToTest, room))
                return room;
        }

        return null;
    }

    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> doorwayList)
    {
        foreach (Doorway doorway in doorwayList)
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
    }

    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
                openRoomNodeQueue.Enqueue(childRoomNode);

            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway parentDoorway, Room room)
    {
        Doorway doorway = GetOppositeDoorway(parentDoorway, room.doorwayList);

        if (doorway == null)
        {
            parentDoorway.isUnavailable = true;

            return false;
        }

        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + parentDoorway.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west, we need
        // to add (1, 0) to the east parent doorway
        switch (doorway.orientation)
        {
            case Orientation.North:
                adjustment = new(0, -1);
                break;
            case Orientation.East:
                adjustment = new(-1, 0);
                break;
            case Orientation.South:
                adjustment = new(0, 1);
                break;
            case Orientation.West:
                adjustment = new(1, 0);
                break;
            case Orientation.None:
                break;
            default:
                break;
        }

        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        switch (overlappingRoom)
        {
            case null:
                parentDoorway.isConnected = true;
                parentDoorway.isUnavailable = true;

                doorway.isConnected = true;
                doorway.isUnavailable = true;

                return true;
            default:
                parentDoorway.isUnavailable = true;
                return false;
        }
    }

    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            List<Doorway> unconnectedAvailableParentDoorwaysList = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if (unconnectedAvailableParentDoorwaysList.Count == 0)
                return false;

            Doorway parentDoorway = unconnectedAvailableParentDoorwaysList[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorwaysList.Count)];
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, parentDoorway);
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            if (PlaceTheRoom(parentRoom, parentDoorway, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
                roomOverlaps = true;
        }

        return true;
    }

    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
            return true;

        return false;
    }

    private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
            return true;
        else
            return false;
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new();

        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));
        if (entranceNode != null)
            openRoomNodeQueue.Enqueue(entranceNode);
        else
        {
            Debug.Log("No Entrance Node!");
            return false;
        }

        bool noRoomOverlaps = true;

        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
            return true;

        return false;
    }

    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            else
                Debug.Log($"Duplicate Room Template Key in {roomTemplateList}!");

        }
    }

    private void InstantiateRoomGameObjects()
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            Vector3 roomPosition = new(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();
            instantiatedRoom.room = room;
            instantiatedRoom.Init(roomGameObject);

            room.instantiatedRoom = instantiatedRoom;
        }
    }

    private void LoadRoomNodeTypeList() => roomNodeTypeList = GameResources.Instance.roomNodeTypeList;

    private void ClearDungeon()
    {
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if (room.instantiatedRoom != null)
                    Destroy(room.instantiatedRoom.gameObject);
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }
}