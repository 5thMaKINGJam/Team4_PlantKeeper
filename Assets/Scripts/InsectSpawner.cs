using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectSpawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public GameObject prefab;
    public GameObject insects;
    List<GameObject> pools;
    float nextSpawnTime;
    float timer;

    private void Awake()
    {
        pools= new List<GameObject>();
        spawnPoint = GetComponentsInChildren<Transform>();
        timer = 0;
        nextSpawnTime = Random.Range(2.0f, 4.0f);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > nextSpawnTime)
        {
            Spawn();
            timer= 0;
            nextSpawnTime = Random.Range(2.0f, 4.0f);
        }
    }
    private void Spawn()
    {
        GameObject insect=Get();
        insect.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
    }
    public GameObject Get()
    {
        GameObject select = null;
        foreach (GameObject item in pools)
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select)
        {
            select = Instantiate(prefab, insects.transform);
            pools.Add(select);
        }
        return select;
    }
}
