using UnityEngine;
using System.Collections;

public class SpinningObstacle : MonoBehaviour {

    public Transform spinner;
    public Rigidbody spinnerRigidBody;
    public float rotationSpeed = 10;
    private float rotation = 0;

    // Use this for initialization
    void Start () {
        spinnerRigidBody = spinner.transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        rotation += rotationSpeed;
        if (rotation > 360)
        {
            rotation = 0;
        }
        else if(rotation < 0)
        {
            rotation = 360;
        }
        spinnerRigidBody.MoveRotation(Quaternion.Euler(0.0f, 0.0f, rotation));
    }
}
