namespace Script.Tools
{
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}

public class GameObjectPool
{
    private readonly GameObject _prefab;
    private readonly Transform _container;
    private readonly Queue<GameObject> _pool = new();
    private readonly HashSet<GameObject> _active = new();
    private readonly int _maxCapacity;
    private int _spawnedCount;

    public int CountInactive => _pool.Count;
    public int CountActive => _active.Count;
    public int CountAll => _spawnedCount;

    public GameObjectPool(GameObject prefab, int initialCapacity = 10, int maxCapacity = 100, Transform container = null)
    {
        _prefab = prefab;
        _maxCapacity = maxCapacity;
        _container = container ?? new GameObject($"Pool_{prefab.name}").transform;
        
        PreWarm(initialCapacity);
    }

    public void PreWarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = CreateInstance();
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        GameObject obj;
        
        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        else if (_spawnedCount < _maxCapacity)
        {
            obj = CreateInstance();
        }
        else
        {
            Debug.LogWarning($"Pool '{_prefab.name}' reached max capacity ({_maxCapacity})");
            return null;
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(parent, true);
        obj.SetActive(true);
        _active.Add(obj);

        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnSpawn();

        return obj;
    }

    public T Get<T>(Vector3 position = default, Quaternion rotation = default, Transform parent = null) where T : Component
    {
        var obj = Get(position, rotation, parent);
        return obj != null ? obj.GetComponent<T>() : null;
    }

    public void Return(GameObject obj)
    {
        if (obj == null) return;
        if (!_active.Contains(obj)) return;

        _active.Remove(obj);
        
        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnDespawn();

        ResetObject(obj);
        
        if (_pool.Count + _active.Count < _maxCapacity)
        {
            _pool.Enqueue(obj);
        }
        else
        {
            _spawnedCount--;
            UnityEngine.Object.Destroy(obj);
        }
    }

    public void ReturnAll()
    {
        var activeSnapshot = new List<GameObject>(_active);
        foreach (var obj in activeSnapshot)
        {
            Return(obj);
        }
    }

    private GameObject CreateInstance()
    {
        var obj = UnityEngine.Object.Instantiate(_prefab, _container);
        obj.name = $"{_prefab.name}_{_spawnedCount}";
        _spawnedCount++;
        return obj;
    }

    private void ResetObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(_container);
        obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Clear()
    {
        foreach (var obj in _pool)
        {
            if (obj != null) UnityEngine.Object.Destroy(obj);
        }
        _pool.Clear();
        
        foreach (var obj in _active)
        {
            if (obj != null) UnityEngine.Object.Destroy(obj);
        }
        _active.Clear();
        
        _spawnedCount = 0;
    }
}
}