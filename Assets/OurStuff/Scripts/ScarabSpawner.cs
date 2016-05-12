using UnityEngine;
using System.Collections;

public class ScarabSpawner : MonoBehaviour {

    public int maxScarabs = 10;
    public float spawnCooldown = 1.0f;
    public Rigidbody scarab;
    private bool currentlySpawning = false;
    private int spawnedScarabs = 0;

	// Use this for initialization
	void Start () {
	
	}

    IEnumerator SpawnScarab()
    {
        Rigidbody scarabInstance = (Rigidbody)Instantiate(scarab, transform.position, transform.rotation);
        spawnedScarabs += 1;
        yield return new WaitForSeconds(spawnCooldown);
        currentlySpawning = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(spawnedScarabs > maxScarabs)
        {
            return;
        }
        if(!currentlySpawning)
        {
            currentlySpawning = true;
            StartCoroutine(SpawnScarab());
        }
    }
}
