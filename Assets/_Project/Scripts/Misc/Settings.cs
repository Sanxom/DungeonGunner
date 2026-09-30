using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region Dungeon Build Settings
    public const int MAX_DUNGEON_REBUILD_ATTEMPTS_FOR_ROOM_GRAPH = 1000;
    public const int MAX_DUNGEON_BUILD_ATTEMPTS = 10;
    #endregion

    #region Room Settings
    public const int MAX_CHILD_CORRIDORS = 3;
    #endregion
}