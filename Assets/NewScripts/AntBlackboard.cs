using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBlackboard : MonoBehaviour
{
    public float pointReachedRadius = 2f;

    private GameObject[] wanderPoints;
    private GameObject[] exitPoints;

    void Start()
    {
        wanderPoints = GameObject.FindGameObjectsWithTag("WANDERPOINT");
        exitPoints = GameObject.FindGameObjectsWithTag("EXITPOINT");
    }


    public GameObject GetRandomWanderPoint()
    {
        return wanderPoints[Random.Range(0, wanderPoints.Length)];
    }

    public GameObject GetRandomExitPoint()
    {
        return exitPoints[Random.Range(0, exitPoints.Length)];
    }
}
