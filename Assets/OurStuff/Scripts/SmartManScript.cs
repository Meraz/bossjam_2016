using UnityEngine;
using System.Collections;

public class SmartManScript : MonoBehaviour {
    private Transform thisTransform;
    private Rigidbody thisRigidbody;

    private LayerMask layerMaskNonStatic;

    public float basicForceSpeed = 100.0f;
    private float currForceSpeed;

    public float basicMaxSpeed = 5.0f;
    private float currMaxSpeed;

    public float jumpForce = 25;

    public float pushCooldown = 3.0f;
    private float pushTimer = 0.0f;

    // Use this for initialization
    void Awake () {
        layerMaskNonStatic = ~(1 << LayerMask.NameToLayer("Player") | (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("MovingPlatform"))); // ignore collisions with statics


        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();

        currForceSpeed = basicForceSpeed;
    }

    // Update is called once per frame
    void FixedUpdate () {
        Move();

        if(Obstacle())
        {
            Jump();
        }
	}

    void Move()
    {
        if (IsGrounded())
        {
            thisRigidbody.AddForce(Vector3.right * currForceSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (IsGrounded())
        {
            Vector3 jumpVector = (Vector3.right + Vector3.up).normalized;
            thisRigidbody.AddForce(jumpVector * jumpForce, ForceMode.Impulse);
        }
    }

    public void Push(float pushForce, Transform pusher)
    {
        if (pushTimer < Time.time)
        {
            pushTimer = Time.time + pushCooldown;
            if (Vector3.Distance(pusher.position, thisTransform.position) < 5.0f) //avståndet till den som knuffar ska inte vara för stor
                if (pusher.position.x < thisTransform.position.x - 0.5f) //bara en offset så det inte blir en gräns i mitten
                {
                    //kolla y oxå kanske
                    Vector3 pushVector = (Vector3.right + Vector3.up).normalized;
                    thisRigidbody.AddForce(pushVector * pushForce, ForceMode.Impulse);
                    //Debug.Log("Pushed");
                }
        }
    }

    void OnTriggerEnter(Collider collidingUnit)
    {
        if (collidingUnit.tag == "DeadlyAll")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Lost();
        }

        else if(collidingUnit.tag == "Goal")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Won();
        }
    }

    void OnCollisionEnter(Collision collidingUnit)
    {
        //Debug.Log(collidingUnit.gameObject.name)
        if (collidingUnit.gameObject.tag == "DeadlyAll")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Lost();
        }

        else if (collidingUnit.gameObject.tag == "Goal")
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Won();
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        bool result = Physics.Raycast(thisTransform.position, Vector3.down, out hit, 1.8f);

        if (result)
        {
            if (hit.transform.tag == "DeadlyAll")
            {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().Lost();
            }
        }

        return result;
    }

    bool Obstacle()
    {
        if(thisRigidbody.velocity.magnitude < 2.0f)
        {
            return true; //dum lösning
        }

        if(Physics.Raycast(thisTransform.position + new Vector3(0, 0.9f, 0), Vector3.right, 2, layerMaskNonStatic))
        {
            return true;
        }
        else if(Physics.Raycast(thisTransform.position + new Vector3(0, -0.9f, 0), Vector3.right, 2, layerMaskNonStatic))
        {
            return true;
        }

        return false;
    }
}
