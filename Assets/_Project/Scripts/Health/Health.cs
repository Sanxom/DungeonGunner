using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    public int StartingHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    public void SetStartingHealth(int startingHealth)
    {
        StartingHealth = startingHealth;
        CurrentHealth = startingHealth;
    }
}