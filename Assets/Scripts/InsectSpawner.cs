using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class InsectSpawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public GameObject prefab;
    public GameObject insects;
    List<GameObject> pools;
    public bool[] isSpawn;
    float nextSpawnTime;
    float timer;

    private void Awake()
    {
        pools= new List<GameObject>();
        spawnPoint = GetComponentsInChildren<Transform>();
        timer = 0;
        nextSpawnTime = Random.Range(2.0f, 4.0f);
        isSpawn = new bool[spawnPoint.Length];
        for(int i=0;i<isSpawn.Length;i++)
        {
            isSpawn[i] = false;
        }
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
        int index = Random.Range(1, spawnPoint.Length);
        int count = 0;
        while (isSpawn[index]&&count<10) {
            index = Random.Range(1, spawnPoint.Length);
            count++;
        }
        insect.transform.position = spawnPoint[index].position;
        isSpawn[index] = true;
        insect.GetComponent<Insect>().index = index;
        // insect.GetComponent<Insect>().insectSpawner = gameObject;
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
