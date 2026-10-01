using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public static class HelperUtilities
{
    public static Camera mainCamera;

    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    /// <summary>
    /// Get AimDirection enum value from the parameter
    /// </summary>
    /// <param name="angleDegrees"></param>
    /// <returns></returns>
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection = angleDegrees switch
        {
            >= 22f and <= 67f => AimDirection.UpRight,
            > 67f and <= 112f => AimDirection.Up,
            > 112f and <= 158f => AimDirection.UpLeft,
            <= 180f and > 158f or > -180f and <= -135f => AimDirection.Left,
            > -135f and <= -45f => AimDirection.Down,
            > -45f and <= 0f or > 0f and < 22f => AimDirection.Right,
            _ => AimDirection.Right,
        };

        //if (angleDegrees >= 22f && angleDegrees <= 67f)
        //    aimDirection = AimDirection.UpRight;
        //else if (angleDegrees > 67f && angleDegrees <= 112f)
        //    aimDirection = AimDirection.Up;
        //else if (angleDegrees > 112f && angleDegrees <= 158f)
        //    aimDirection = AimDirection.UpLeft;
        //else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180f && angleDegrees <= -135f))
        //    aimDirection = AimDirection.Left;
        //else if (angleDegrees > -135f && angleDegrees <= -45f)
        //    aimDirection = AimDirection.Down;
        //else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0f && angleDegrees < 22f))
        //    aimDirection = AimDirection.Right;
        //else
        //    aimDirection = AimDirection.Right;

        return aimDirection;
    }

    /// <summary>
    /// Get the angle in degrees from a direction Vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float degrees = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        return degrees;
    }

    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.CurrentRoom;

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new(10000f, 10000f, 0f);

        foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
                nearestSpawnPosition = spawnPositionWorld;
        }

        return nearestSpawnPosition;
    }

    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log($"{fieldName} is empty and must contain a value in object {thisObject.name}");
            return true;
        }
        return false;
    }

    public static bool ValidateCheckNullValues(Object thisObject, string fieldName, Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log($"{fieldName} is null and must contain a value in object {thisObject.name}");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if List is empty or contains a null value.
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="enumerableObjectToCheck"></param>
    /// <returns></returns>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log($"{fieldName} is null in object {thisObject.name}.");
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log($"{fieldName} has null values in object {thisObject.name}");
                error = true;
            }
            else
                count++;
        }

        if (count == 0)
        {
            Debug.Log($"{fieldName} has no values in object {thisObject.name}");
            error = true;
        }

        return error;
    }

    /// <summary>
    /// Int version
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log($"{fieldName} must contain a positive value or zero in object {thisObject.name}");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log($"{fieldName} must contain a positive value in object {thisObject.name}");
                error = true;
            }
        }

        return error;
    }

    /// <summary>
    /// Float version
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0f)
            {
                Debug.Log($"{fieldName} must contain a positive value or zero in object {thisObject.name}");
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0f)
            {
                Debug.Log($"{fieldName} must contain a positive value in object {thisObject.name}");
                error = true;
            }
        }

        return error;
    }

    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMin, float valueToCheckMin, string fieldNameMax, float valueToCheckMax, bool isZeroAllowed)
    {
        bool error = false;

        if (valueToCheckMin > valueToCheckMax)
        {
            Debug.Log($"{fieldNameMin} must be less than or equal to {fieldNameMax} in object {thisObject.name}!");
            error = true;
        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMin, valueToCheckMin, isZeroAllowed))
            error = true;
        if (ValidateCheckPositiveValue(thisObject, fieldNameMax, valueToCheckMax, isZeroAllowed))
            error = true;

        return error;
    }
}