using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierSpawner : MonoBehaviour
{

    public ArmyCollection team;
    public ArmyCollection enemyTeam;
    public GameObject soldier;
    public uint soldierCount = 100;
    public uint areaRange = 3;
    public int soldiersSpawnedPerFrame = 20;
    public float minHeight = 0.9f;
    public float maxHeight = 1.1f;
    public float minWidth = 0.6f;
    public float maxWidth = 1.4f;
    public float minSpeedMultiplier = 0.6f;
    public float maxSpeedMultiplier = 1.4f;
    public bool respawn = false;

    int overallSoldierCount = 0;

    bool firstSpawn = true;

    // Use this for initialization
    void Start()
    {
        // Spawn one soldier at the very beginning, so that the team counts as "alive"
        if (soldierCount > 0)
        {
            SpawnSoldier();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((firstSpawn || respawn) && transform.childCount < soldierCount)
        {
            int missingSoldiers = (int)soldierCount - transform.childCount;
            missingSoldiers = missingSoldiers < soldiersSpawnedPerFrame ? missingSoldiers : soldiersSpawnedPerFrame;

            for (int i = 0; i < missingSoldiers; i++)
            {
                // Spawn one soldier per frame, in order to maintain the soldier count
                SpawnSoldier();
            }
        }
        else if (firstSpawn)
        {
            // Initial spawn has finished
            firstSpawn = false;
        }
    }

    // Spawns a soldier
    GameObject SpawnSoldier()
    {
        GameObject newSoldier = Instantiate(soldier, transform);
        Vector2 randomPosition = areaRange * Random.insideUnitCircle;
        Vector3 newPosition = transform.position + new Vector3(randomPosition.x, 0, randomPosition.y);
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(newPosition, out closestHit, 500, 1))
        {
            newSoldier.transform.position = closestHit.position;
            NavMeshAgent agent = newSoldier.AddComponent<NavMeshAgent>();
            agent.angularSpeed = 240;
            agent.stoppingDistance = 1.3f;
            agent.radius = 2.0f;
            agent.height = 10.0f;
        }
        // newSoldier.transform.localPosition = new Vector3(newPosition.x, 0, newPosition.y);


        newSoldier.GetComponent<SoldierController>().respawn = respawn;
        newSoldier.GetComponent<SoldierController>().myTeam = team;
        newSoldier.GetComponent<SoldierController>().teamMaterial = team.TeamOutfit;
        newSoldier.GetComponent<SoldierAI>().aggressiveness = Random.Range(0.3f, 1.0f);
        newSoldier.GetComponent<SoldierAI>().cowardice = Random.Range(0.0f, 1.0f);
        newSoldier.GetComponent<SoldierAI>().enemy = enemyTeam;
        newSoldier.GetComponent<SoldierAI>().offset = (overallSoldierCount % 3) + 1;
        newSoldier.transform.localScale = Vector3.Scale(newSoldier.transform.localScale, new Vector3(1, Random.Range(minHeight, maxHeight), Random.Range(minWidth, maxWidth)));
        newSoldier.GetComponent<NavMeshAgent>().speed = Random.Range(minSpeedMultiplier, maxSpeedMultiplier) * newSoldier.GetComponent<NavMeshAgent>().speed;

        overallSoldierCount++;
        return newSoldier;
    }
}
