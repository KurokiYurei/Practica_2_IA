using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AntSpawner : MonoBehaviour
{

    float timer;
    float chance;
    public float timeToSpawn;
    public GameObject seedAnt;
    public GameObject eggAnt;

    void Start()
    {
        // get the prefabs		
        timeToSpawn = Random.Range(5, 26);
    }


    void Update()
    {
        if (timer >= timeToSpawn)
        {
            chance = Random.Range(0f, 1f);
            if (chance > 0.8)
            {
                Instantiate(eggAnt, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(seedAnt, transform.position, Quaternion.identity);
            }
            timer = 0;
            timeToSpawn = Random.Range(5, 26);
        }
        timer += Time.deltaTime;
    }
}
