using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyBlackboard : MonoBehaviour
{
    public float pointReachedRadius = 2f;

    public float closestEggRadius = 50f;
    public float randomEggRadius = 180f;
    public float closestSeedRadius = 80f;
    public float randomSeedRadius = 125f;
    public float eggWhileSeedRadius = 25f;

    private GameObject[] wanderPoints;
    private GameObject[] storingPoints;
    private GameObject[] hatchingPoints;

    void Start()
    {
        wanderPoints = GameObject.FindGameObjectsWithTag("WANDERPOINT");
        storingPoints = GameObject.FindGameObjectsWithTag("STORINGPOINT");
        hatchingPoints = GameObject.FindGameObjectsWithTag("HATCHINGPOINT");
    }


    public GameObject GetRandomWanderPoint()
    {
        return wanderPoints[Random.Range(0, wanderPoints.Length)];
    }

    public GameObject GetRandomStoringPoint()
    {
        return storingPoints[Random.Range(0, storingPoints.Length)];
    }
    public GameObject GetRandomHatchingPoint()
    {
        return hatchingPoints[Random.Range(0, hatchingPoints.Length)];
    }
}
