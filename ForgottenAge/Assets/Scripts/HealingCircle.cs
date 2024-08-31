using System.Collections.Generic;
using UnityEngine;

public class HealingCircle : MonoBehaviour
{
    private List<AllyTroopStats> troopsInRange = new List<AllyTroopStats>();
    private bool isHealingActive = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        AllyTroopStats troop = other.GetComponent<AllyTroopStats>();
        if (troop != null && !troopsInRange.Contains(troop))
        {
            troopsInRange.Add(troop);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        AllyTroopStats troop = other.GetComponent<AllyTroopStats>();
        if (troop != null && troopsInRange.Contains(troop))
        {
            troopsInRange.Remove(troop);
        }
    }

    private void Update()
    {
        if (isHealingActive)
        {
            foreach (AllyTroopStats troop in troopsInRange)
            {
                troop.TakeHeals(0.2f * Time.deltaTime);
            }
        }
    }

    public void SetHealingActive(bool active)
    {
        isHealingActive = active;

        // Clear the list when healing is inactive to avoid healing troops that might have left the range
        if (!isHealingActive)
        {
            troopsInRange.Clear();
        }
    }
}
