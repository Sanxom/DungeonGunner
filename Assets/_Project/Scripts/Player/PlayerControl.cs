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
    private Player player;
    private float moveSpeed;

    private void Awake()
    {
        gameInput = new();
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void OnEnable()
    {
        moveAction = gameInput.Gameplay.Move;
        gameInput.Enable();
    }

    private void Update()
    {
        MovementInput();
        WeaponInput();
    }

    private void OnDisable()
    {
        gameInput.Disable();
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

    private void WeaponInput()
    {
        AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection);
    }

    private void MovementInput()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction != Vector2.zero)
            player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
        else
            player.idleEvent.CallIdleEvent();
    }
}