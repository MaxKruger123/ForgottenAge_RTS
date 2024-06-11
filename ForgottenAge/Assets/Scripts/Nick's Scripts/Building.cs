using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Building : MonoBehaviour
{
    public GameObject allyTroopPrefab;
    public GameObject rangedAllyTroopPrefab;
    public GameObject recruitmentMenu;
    public MenuManager menuManager;
    public Concentration concentration;
    public float spawnRadius = 4f;


    // Start is called before the first frame update
    void Start()
    {
        concentration = FindAnyObjectByType<Concentration>();
        menuManager = concentration.gameObject.GetComponent<MenuManager>();
        recruitmentMenu = menuManager.GetMenuObject("RecruitmentMenu");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            recruitmentMenu.gameObject.SetActive(true);
            recruitmentMenu.transform.position = mouseScreenPosition;
            recruitmentMenu.GetComponent<RecruitmentMenu>().SetButton(gameObject.GetComponent<Building>());
        }
    }



    public void SpawnTroop()
    {
        if(concentration.GetConcentration() >= 5)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            Instantiate(allyTroopPrefab, spawnPosition, Quaternion.identity);
            concentration.SubtractConcentration(5);
        }
        
    }

    public void SpawnRangedTroop()
    {
        if (concentration.GetConcentration() >= 10)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);

            Instantiate(rangedAllyTroopPrefab, spawnPosition, Quaternion.identity);
            concentration.SubtractConcentration(10);
        }
           
    }
}
