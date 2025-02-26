using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

//프리펩 마다 풀을 가져감
class Pool
{
    GameObject _prefab;
    IObjectPool<GameObject> _pool;

    Transform _root;
    Transform Root
    {
        get
        {
            if (_root == null)
            {
                GameObject go = new GameObject() { name = $"{_prefab.name}Root" };
                _root = go.transform;
            }
            return _root;
        }

    }
    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public GameObject Pop()
    {
        return _pool.Get();

    }

    public void Push(GameObject go)
    {
        _pool.Release(go);
    }

    #region Funcs

    GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab); go.transform.parent = Root;
        go.name = _prefab.name;
        return go;
    }

    void OnGet(GameObject go)
    {
        go.SetActive(true);
    }
    void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }
    void OnDestroy(GameObject go)
    {

        GameObject.Destroy(go);
    }



    #endregion
}
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();



    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //여기 둘을 주로 씀
    //프리펩 생성 혹은 재활용
    public GameObject Pop(GameObject prefab)
    {
        if (_pools.ContainsKey(prefab.name) == false)
        {
            CreatePool(prefab);
        }

        return _pools[prefab.name].Pop();
    }

    //프리펩 반납
    public bool Push(GameObject go)
    {
        if (_pools.ContainsKey(go.name) == false)
        {
            return false;
        }

        _pools[go.name].Push(go);
        return true;

    }
    //재활용 할 프리펩이 없을 경우
    void CreatePool(GameObject prefab)
    {
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }
}