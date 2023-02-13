using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInstantiator : MonoBehaviour
{
    [SerializeField] private float spawnInterval=2f;
    [SerializeField] private GameObject spawnedObject;
    [SerializeField] private int maxSpawns = 1;
    private Queue<GameObject> _spawnedObjectQueue= new Queue<GameObject>();
    [SerializeField, Range(-1.0f, 1.0f)]
    private float x = 0f;
    [SerializeField, Range(-1.0f, 1.0f)]
    private float y = 1f;
    public Vector2 ForceDirection{
        get{return new Vector2(x,y);}
    }
    [SerializeField, Range(0.0f, 20.0f)]
    private float forceOnSpawn = 10;
    // Start is called before the first frame update

    void Start()
    {
        SpawnPrefab();
        StartCoroutine(SpawnPrefabCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnPrefabCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            SpawnPrefab();
            if (_spawnedObjectQueue.Count>maxSpawns){
                Destroy(_spawnedObjectQueue.Dequeue());
            }
            
        }
    }

    private void SpawnPrefab(){
        GameObject instance = Instantiate(spawnedObject, transform.position, Quaternion.identity);
        _spawnedObjectQueue.Enqueue(instance);
        Rigidbody2D _rb = instance.GetComponent<Rigidbody2D>();
        if(_rb!=null){
            _rb.AddForce(this.ForceDirection.normalized * forceOnSpawn * _rb.mass, ForceMode2D.Impulse);
        }
    }
}
