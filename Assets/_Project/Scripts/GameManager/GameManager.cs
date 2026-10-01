using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion
    #region Tooltip
    [Tooltip("Populate the dungeon level ScriptableObjects")]
    #endregion
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]
    #endregion
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private PlayerDetailsSO playerDetails;

    [HideInInspector] public GameState gameState;

    public Player Player { get; private set; }
    public Room CurrentRoom { get; private set; }
    public Room PreviousRoom { get; private set; }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion

    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    private void Start()
    {
        gameState = GameState.GameStarted;
    }

    private void Update()
    {
        HandleGameState();

        if (Keyboard.current.rKey.wasPressedThisFrame)
            gameState = GameState.GameStarted;
    }

    public void SetCurrentRoom(Room room)
    {
        PreviousRoom = CurrentRoom;
        CurrentRoom = room;
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.GameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.PlayingLevel;
                break;
            case GameState.PlayingLevel:
                break;
            case GameState.EngagingEnemies:
                break;
            case GameState.BossStage:
                break;
            case GameState.EngagingBoss:
                break;
            case GameState.LevelCompleted:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            case GameState.GamePaused:
                break;
            case GameState.DungeonOverviewMap:
                break;
            case GameState.RestartGame:
                break;
            default:
                break;
        }
    }

    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        Player = playerGameObject.GetComponent<Player>();
        Player.Init(playerDetails);
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs!");

        Player.gameObject.transform.position = new Vector3((CurrentRoom.lowerBounds.x + CurrentRoom.upperBounds.x) * 0.5f,
            (CurrentRoom.lowerBounds.y + CurrentRoom.upperBounds.y) * 0.5f, 0f);

        Player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(Player.gameObject.transform.position);
    }
}