using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingStats : MonoBehaviour
{
    public MemoryTileConstruction numBuildings;
    public float maxHealth = 10;
    public float currentHealth;

    public Image healthBar;

    public void Start()
    {
        currentHealth = maxHealth;
        FindNearestMemoryTile();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            if (numBuildings != null)
            {
                numBuildings.numBuildings--;
            }
            Destroy(gameObject);
        }
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void TakeHeals(float amount)
    {
        if (currentHealth <= 0)
        {
            if (numBuildings != null)
            {
                numBuildings.numBuildings--;
            }
            Destroy(gameObject);
        }
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void FindNearestMemoryTile()
    {
        GameObject[] memoryTiles = GameObject.FindGameObjectsWithTag("MemoryTile");
        float minDistance = Mathf.Infinity;
        GameObject nearestTile = null;

        foreach (GameObject tile in memoryTiles)
        {
            float distance = Vector3.Distance(transform.position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTile = tile;
            }
        }

        if (nearestTile != null)
        {
            numBuildings = nearestTile.GetComponent<MemoryTileConstruction>();
        }
    }
}
