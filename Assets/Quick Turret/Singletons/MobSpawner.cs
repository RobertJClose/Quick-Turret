using DG.Tweening;
using QuickTurret.Waypoints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public Transform Targets;

    public Waypoints Waypoints;

    public WaypointsNavigator Prefab_Containozoid;
    public WaypointsNavigator Prefab_Cubazoid;
    public WaypointsNavigator Prefab_Monkezoid;
    public WaypointsNavigator Prefab_Spikazoid;

    [Min(0.5f)]
    public float ContainozoidAverageSpawnPeriod = 20f;
    public float CubazoidAverageSpawnPeriod = 150f;
    public float MonkezoidAverageSpawnPeriod = 20f;
    public float SpikazoidAverageSpawnPeriod = 5f;

    public Transform SpawnRectCornerOne;
    public Transform SpawnRectCornerTwo;

    private void Update()
    {
        CheckMobSpawn(Prefab_Containozoid, ContainozoidAverageSpawnPeriod);
        //CheckMobSpawn(Prefab_Cubazoid, CubazoidAverageSpawnPeriod);
        CheckMobSpawn(Prefab_Monkezoid, MonkezoidAverageSpawnPeriod);
        CheckMobSpawn(Prefab_Spikazoid, SpikazoidAverageSpawnPeriod);
    }

    private void CheckMobSpawn(WaypointsNavigator mob, float averagePeriod)
    {
        // We use the Poisson distribution to spawn our mobs. Their average will
        // be the specified period argument, but the exact time to spawn will be random.
        float averageRate = 1f / averagePeriod;
        float dTime = Time.deltaTime;
        float probOfZeroSpawning = Mathf.Exp(-averageRate * dTime);
        float roll = Random.Range(0f, 1f);

        if (roll > probOfZeroSpawning)
        {
            Vector3 spawnPoint = GetSpawnPoint();
            WaypointsNavigator newMob = Instantiate(mob, spawnPoint, Quaternion.identity, Targets);
            newMob.SetDestination(Waypoints.points[0]);
        }
    }

    private Vector3 GetSpawnPoint()
    {
        float x = Random.Range(SpawnRectCornerOne.position.x, SpawnRectCornerTwo.position.x);
        float y = Random.Range(SpawnRectCornerOne.position.y, SpawnRectCornerTwo.position.y);
        float z = Random.Range(SpawnRectCornerOne.position.z, SpawnRectCornerTwo.position.z);
        Vector3 output = new Vector3(x, y, z);

        return output;
    }
}
