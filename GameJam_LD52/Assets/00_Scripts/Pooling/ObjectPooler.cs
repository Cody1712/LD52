using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : Manager<ObjectPooler>
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int count;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            GameObject objContainer = new GameObject();
            objContainer.name = pool.tag + " Container";
            objContainer.transform.parent = this.transform;

            for (int i = 0; i < pool.count; i++)
            {
                GameObject obj = Instantiate(pool.prefab, objContainer.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

    }

    public GameObject SpawnFromPool(string _tag, Vector3 _pos, Transform _parent, Quaternion _rot)
    {
        if (!poolDictionary.ContainsKey(_tag))
        {
            Debug.LogWarning("Pool Tag " + _tag + " was not found");
            return null;
        }

        GameObject _objectToSpawn = poolDictionary[_tag].Dequeue();

        _objectToSpawn.transform.position = _pos;
        _objectToSpawn.transform.rotation = _rot;

        if (_parent != null)
            _objectToSpawn.transform.parent = _parent;

        _objectToSpawn.SetActive(true);

        IPooledObject _pooledObject = _objectToSpawn.GetComponent<IPooledObject>();
        if (_pooledObject != null)
        {
            _pooledObject.OnObjectSpawn();
        }

        poolDictionary[_tag].Enqueue(_objectToSpawn);

        return _objectToSpawn;
    }
}
