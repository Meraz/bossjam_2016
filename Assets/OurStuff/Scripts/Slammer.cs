using UnityEngine;
using System.Collections;

public class Slammer : MonoBehaviour {
    public Transform platform;
    private Rigidbody platformRigidbody;

    private bool goingToEnd;
    private Vector3 startPos;
    public Transform endPos;

    private Vector3 wantedPos;

    private float startTime;
    public float speedMultiplier = 0.05f;
    // Use this for initialization
    void Start () {
        startTime = Time.time;
        startPos = platform.position;
        goingToEnd = true;

        wantedPos = endPos.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float fracComplete = (Time.time - startTime) * speedMultiplier/100 * Mathf.Pow(startTime - Time.time, 16);
        platform.position = Vector3.Lerp(platform.position, wantedPos, fracComplete);

        if(Vector3.Distance(platform.position, wantedPos) < 1.0f)
        {
            //kolla vilken den ska byta till
            if(goingToEnd == true)
            {
                startTime = Time.time;
                goingToEnd = false;
                wantedPos = startPos;
            }
            else
            {
                startTime = Time.time;
                goingToEnd = true;
                wantedPos = endPos.position;
            }
        }
    }

}
