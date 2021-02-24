using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

public class Health : MonoBehaviour
{
    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }
    private void Update()
    {

    }
}

