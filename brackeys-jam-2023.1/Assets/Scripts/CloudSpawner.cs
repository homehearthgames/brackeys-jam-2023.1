using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour {

    public GameObject[] cloudPrefabs;
    public float minSpawnDelay;
    public float maxSpawnDelay;
    public bool spawnOnTop;
    public bool moveRight;

    public float spawnHeight = 15f;

    IEnumerator Start () {
        while (true) {
            float randomY = Random.Range(-spawnHeight, spawnHeight);
            if (spawnOnTop) {
                randomY = Random.Range(0f, 15);
            } else {
                randomY = Random.Range(-spawnHeight, 0f);
            }
            int randomIndex = Random.Range(0, cloudPrefabs.Length);
            GameObject cloudPrefab = cloudPrefabs[randomIndex];
            Vector3 spawnPos;
            if (moveRight) {
                spawnPos = new Vector3(-spawnHeight, randomY, 0f);
            } else {
                spawnPos = new Vector3(spawnHeight, randomY, 0f);
            }
            GameObject newCloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
            CloudMovement cloudMovement = newCloud.GetComponent<CloudMovement>();
            if (cloudMovement) {
                cloudMovement.moveRight = moveRight;
            }
            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }
}




