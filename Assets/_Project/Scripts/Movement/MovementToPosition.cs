using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rb;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionEventArgs movementToPositionEventArgs)
    {
        MoveRigidbody(movementToPositionEventArgs.movePosition, movementToPositionEventArgs.currentPosition, movementToPositionEventArgs.moveSpeed);
    }

    private void MoveRigidbody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        rb.MovePosition(rb.position + (moveSpeed * Time.fixedDeltaTime * unitVector));
    }
}