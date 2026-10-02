using UnityEngine;

public static class Settings
{
    #region DUNGEON BUILD SETTINGS
    public const int MAX_DUNGEON_REBUILD_ATTEMPTS_FOR_ROOM_GRAPH = 1000;
    public const int MAX_DUNGEON_BUILD_ATTEMPTS = 10;
    #endregion

    #region ROOM SETTINGS
    public const int MAX_CHILD_CORRIDORS = 3;
    #endregion

    #region ANIMATOR PARAMETERS PLAYER
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    #endregion
}