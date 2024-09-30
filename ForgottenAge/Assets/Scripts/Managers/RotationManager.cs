using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationManager : MonoBehaviour
{
    void Update()
    {
        // Find all objects with the "RepairTroop" tag
        GameObject[] repairTroops = GameObject.FindGameObjectsWithTag("RepairTroop");

        // Loop through each RepairTroop and correct their rotation
        foreach (GameObject repairTroop in repairTroops)
        {
            repairTroop.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
