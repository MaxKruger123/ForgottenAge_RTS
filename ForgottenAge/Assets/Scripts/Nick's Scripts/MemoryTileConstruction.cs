using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTileConstruction : MonoBehaviour
{
    public Concentration concentration;
    public GameObject constructionMenu;
    public GameObject buildingPrefab;
    public int numBuildings;

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

        if (Input.GetMouseButtonDown(1) && numBuildings < 1)// add if statement for if the tile is captured
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            constructionMenu.gameObject.SetActive(true);
            constructionMenu.transform.position = mouseScreenPosition;
        }
    }

    public void ConstructBuilding()
    {
        Instantiate(buildingPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z-0.5f),  Quaternion.identity);
        concentration.SubtractConcentration(20);
        numBuildings++;
    }

    // make it so that when a buidling is on a tile it stays captured
    // make it so that tiles give extra concentration
}
