using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInstantiator : MonoBehaviour
{
    [SerializeField] private float spawnInterval=2f;
    [SerializeField] private GameObject spawnedObject;
    // Start is called before the first frame update
    void Start()
    {
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
            Instantiate(spawnedObject, transform.position, Quaternion.identity);
        }
    }
}
