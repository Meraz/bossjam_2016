using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    private Transform thisTransform;
    private Rigidbody thisRigidbody;
    private Transform[] players;

    public Transform meshObject;

    enum Direction { Left, Right}

    Direction currDirection = Direction.Left;

    public float activationDistance = 50.0f;

    public float moveForce = 900.0f;

    public GameObject deathExplosion;
	// Use this for initialization
	void Start () {
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Move();
        CheckIfTurn();

        if(!IsGrounded())
        {
            meshObject.eulerAngles = new Vector3(0, -90, 0);
        }
	}

    void Move()
    {
        if(currDirection == Direction.Left)
        {
            meshObject.eulerAngles = new Vector3(0, -90, 0);
            thisRigidbody.AddForce(Vector3.left * moveForce * Time.deltaTime);
        }
        else
        {
            meshObject.eulerAngles = new Vector3(0, 90, 0);
            thisRigidbody.AddForce(Vector3.right * moveForce * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collidingUnit)
    {
        if(collidingUnit.transform.tag == "SmartMan" || collidingUnit.transform.tag == "DeadlyAll")
        {
            Die();
        }

        if (collidingUnit.transform.position.x > thisTransform.position.x) //du träffa mig från höger
        {
            if (Physics.Raycast(thisTransform.position, Vector3.right, 4.0f))
            {
                //Debug.Log("Träffar mig från höger");
                currDirection = Direction.Left;
            }
        }
        else //du träffa mig från vänster shiiatt
        {
            if (Physics.Raycast(thisTransform.position, Vector3.left, 4.0f))
            {
                //Debug.Log("Träffar mig från vänster");
                currDirection = Direction.Right;
            }
        }
    }

    void OnTriggerEnter(Collider collidingUnit)
    {
        if (collidingUnit.transform.tag == "SmartMan" || collidingUnit.transform.tag == "DeadlyAll")
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject temp = Instantiate(deathExplosion.gameObject, thisTransform.position, thisTransform.rotation) as GameObject;
        Destroy(temp, 4.0f);
        Destroy(thisTransform.gameObject);
    }

    void CheckIfTurn()
    {
        if(currDirection == Direction.Right)
        {
            if(!Physics.Raycast(thisTransform.position + new Vector3(3, 0, 0), Vector3.down, 5.0f))
            {
                currDirection = Direction.Left;
            }
        }
        else
        {
            if (!Physics.Raycast(thisTransform.position + new Vector3(-3, 0, 0), Vector3.down, 5.0f))
            {
                currDirection = Direction.Right;
            }
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        bool result = Physics.Raycast(thisTransform.position, Vector3.down, out hit, 1.8f);

        return result;
    }


}
