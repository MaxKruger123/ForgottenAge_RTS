using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject allyTroop;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        
        if (Input.GetMouseButtonDown(1)) 
        {
            Debug.Log("Clicked");
            SpawnTroop();
        }
    }

    private void SpawnTroop()
    {
        GameObject troop = Instantiate(allyTroop, new Vector3(transform.position.x + 3, transform.position.y, transform.position.z), Quaternion.identity);
        Debug.Log("Before setting rotation: " + troop.transform.rotation.eulerAngles);
        troop.transform.rotation = Quaternion.Euler(0, 0, 0); // Force rotation to be strictly zero
        Debug.Log("After setting rotation: " + troop.transform.rotation.eulerAngles);
    }
}
