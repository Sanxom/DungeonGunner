using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The MovementDetails Scriptable Object containing movement details such as moveSpeed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;
    #region Tooltip
    [Tooltip("The Player WeaponShootPosition GameObject in the Hierarchy")]
    #endregion
    [SerializeField] private Transform weaponShootPosition;

    private GameInput gameInput;
    private InputAction moveAction;
    private InputAction dodgeRollAction;
    private Player player;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private float moveSpeed;
    private float playerRollCooldownTimer = 0f;
    private bool isPlayerRolling = false;

    private void Awake()
    {
        gameInput = new();
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void OnEnable()
    {
        moveAction = gameInput.Gameplay.Move;
        dodgeRollAction = gameInput.Gameplay.DodgeRoll;
        gameInput.Enable();
    }

    private void Start()
    {
        waitForFixedUpdate = new();
    }

    private void Update()
    {
        if (isPlayerRolling) return;

        MovementInput();
        WeaponInput();
        PlayerRollCooldownTimer();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopPlayerRollCoroutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerRollCoroutine();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValues(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        Vector3 playerDirection = mouseWorldPosition - transform.position;

        weaponDirection = mouseWorldPosition - weaponShootPosition.position;

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private IEnumerator PlayerRollCoroutine(Vector3 direction)
    {
        float minDistance = 0.2f;
        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;
        playerRollCooldownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;
    }

    private void StopPlayerRollCoroutine()
    {
        if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f)
            playerRollCooldownTimer -= Time.deltaTime;
    }

    private void WeaponInput()
    {
        AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection);
    }

    private void MovementInput()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction != Vector2.zero)
        {
            if (!dodgeRollAction.inProgress)
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            else if (dodgeRollAction.inProgress && playerRollCooldownTimer <= 0f)
                PlayerRoll((Vector3)direction);
        }
        else
            player.idleEvent.CallIdleEvent();
    }

    private void PlayerRoll(Vector3 direction) 
    {
        playerRollCoroutine = StartCoroutine(PlayerRollCoroutine(direction));
    }
}