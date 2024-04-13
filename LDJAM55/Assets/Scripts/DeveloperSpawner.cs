using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataTypes;

public class DeveloperSpawner : MonoBehaviour
{
    [SerializeField] LayerMask blockingLayers;
    [SerializeField] Transform spawnPointsParent;
    [SerializeField] GameObject developerInHell;

    public GameObject TrySpawnDeveloper(Developer developer)
    {
        int r = Random.Range(0, spawnPointsParent.childCount);
        Vector3 spawnPos = spawnPointsParent.GetChild(r).position;

        Collider[] cols = Physics.OverlapSphere(spawnPos, 0.2f, blockingLayers);

        if (cols.Length <= 0)
        {
            Debug.Log("Developer spawned!");
            GameObject dev = Instantiate(developerInHell, spawnPos, Quaternion.identity);
            dev.GetComponent<DeveloperInHell>().developer = developer;
            return dev;
        }
        else
        {
            Debug.Log("Developer failed to spawn!", cols[0].gameObject);
            Debug.DrawLine(spawnPos, spawnPos + Vector3.down * 0.2f, Color.green, 10.0f);
            Debug.DrawLine(Vector3.zero, spawnPos, Color.white, 10.0f);
        }

        return null;
    }
}
