using UnityEngine;
using System.Collections;

public class ThirdWheelGuyScript : MonoBehaviour {
    private Transform thisTransform;
    private Rigidbody thisRigidbody;
   
    enum State { Still, Slow, Normal, Fast, Pushed};
    enum Direction { Left, Right};

    private State currState = State.Still;
    private Direction currDirection = Direction.Left;

    public float basicForceSpeed = 100.0f;
    public float basicMaxSpeed = 5.0f;
    private float currMaxSpeed;

	// Use this for initialization
	void Start ()
    {
        thisTransform = this.transform;
        thisRigidbody = thisTransform.GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Move();
    }

    public void Move()
    {
        if (currState != State.Pushed && IsGrounded() == true) //man ska inte kunna poke:a honom medans han flyger
        {
            if (currDirection == Direction.Left)
            {
                thisRigidbody.AddForce(Vector3.left * (int)currState * basicForceSpeed * Time.deltaTime);
            }
            else if (currDirection == Direction.Right)
            {
                thisRigidbody.AddForce(Vector3.right * (int)currState * basicForceSpeed * Time.deltaTime);
            }        
            currMaxSpeed = basicMaxSpeed * (int)currState;
            if (thisRigidbody.velocity.magnitude > currMaxSpeed)
            {
                thisRigidbody.velocity = thisRigidbody.velocity.normalized * currMaxSpeed;
            }
        }
    }

    public void Poke(int knockedDir) //1 = från vänster, 2 = från höger
    {
        if (currState != State.Pushed) //man ska inte kunna poke:a honom medans han flyger
        {
            if (currState == State.Still) //sätter igång honom
            {
                if (knockedDir == 1) //knuffar ÅT höger
                {
                    currDirection = Direction.Right;
                }
                else //knuffar ÅT vänster
                {
                    currDirection = Direction.Left;
                }

                currState = State.Slow;
                return;
            }

            if (knockedDir == 1) //knuffar ÅT höger
            {
                if (currDirection == Direction.Left)
                {

                    currState--;

                }
                else
                {

                    currState++;

                }
            }
            else //knuffar ÅT vänster
            {
                if (currDirection == Direction.Left)
                {

                    currState++;

                }
                else
                {

                    currState--;

                }
            }

            if (currState > State.Fast)
            {
                currState = State.Fast;
            }
        }
    }

    void OnCollisionEnter(Collision collidingUnit)
    {
        if(collidingUnit.transform.position.x > thisTransform.position.x) //du träffa mig från höger
        {
            //Debug.Log("Träffar mig från höger");
            Push(2, 10);
        }
        else if (collidingUnit.transform.position.x < thisTransform.position.x) //du träffa mig från vänster shiiatt
        {
            //Debug.Log("Träffar mig från vänster");
            Push(1, 10);
        }
        else
        {
            Push(2, 1);
        }
    }

    public void Push(int pushDir, float pushForce) //hård 
    {
        if(currState != State.Pushed)
            StartCoroutine(PushCo(pushDir, pushForce));
    }

    IEnumerator PushCo(int pushDir, float pushForce) //hård 
    {
        currState = State.Pushed;
        if (pushDir == 1)
        {
            Vector3 pushVector = (Vector3.right + Vector3.up).normalized;
            thisRigidbody.AddForce(pushVector * pushForce, ForceMode.Impulse);
        }
        else
        {
            Vector3 pushVector = (Vector3.left + Vector3.up).normalized;
            thisRigidbody.AddForce(pushVector * pushForce, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(0.8f); //bara så att den inte blir grounded dirr
        while (!IsGrounded())
        {
            yield return new WaitForSeconds(0.2f);
        }

        currState = State.Still;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(thisTransform.position, Vector3.down, 1);
    }
}
