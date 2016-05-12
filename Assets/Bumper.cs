using UnityEngine;
using System.Collections;

public class Bumper : MonoBehaviour {

    public float startSize = 0.9f;
    public float growSize = 1.0f;
    public float force = 15.0f;

    // Use this for initialization
    void Start () {
        transform.localScale = new Vector3(growSize, growSize, growSize);
    }

    // Update is called once per frame
    void Update () {
	
	}

    IEnumerator ShrinkAgain()
    {
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(startSize, startSize, startSize);
    }

    void OnCollisionEnter(Collision collision)
    {
        transform.localScale = new Vector3(growSize, growSize, growSize);
        Vector3 otherPosition = collision.transform.position;
        Vector3 awayVector = otherPosition - transform.position;
        Rigidbody otherRigidBody = collision.transform.GetComponent<Rigidbody>();

        if(otherRigidBody != null)
        {
            otherRigidBody.AddForce(awayVector.normalized * force, ForceMode.Impulse);
        }

        StartCoroutine(ShrinkAgain());
    }
}
