using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject prefab;
    public int size;
    public bool isSpawned;

    Queue<GameObject> objectPool = new Queue<GameObject>();
    public Box[] basicBoxes;

    public static ObjectPooler instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        foreach (var b in objectPool)
        {
            b.transform.position = Vector3.zero;
            b.SetActive(false);
        }

        foreach (var b in basicBoxes)
        {
            b.Reset();

            if (b.isBasicBox)
                b.ResetBasicBox();
        }
    }

    private void Start()
    {
        if (isSpawned)
            return;

        basicBoxes = GetComponentsInChildren<Box>();

        isSpawned = true;

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public GameObject SpawnFromPool(Vector3 pos, Quaternion rotation)
    {
        GameObject object2Spawn = objectPool.Dequeue();

        object2Spawn.SetActive(true);
        object2Spawn.transform.position = pos;
        object2Spawn.transform.rotation = rotation;

        var pooledoObj = object2Spawn.GetComponent<IPooledObject>();
        if (pooledoObj != null)
            pooledoObj.OnObjectSpawn();

        objectPool.Enqueue(object2Spawn);

        return object2Spawn;
    }
}
