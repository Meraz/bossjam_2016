using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    
    private Rigidbody m_rigidBody;
    private GameManager m_gameObject;

    public Transform meshObject;
    private Animation animator;

    private SmartManScript smartManScript;

    public AnimationClip run;
    public AnimationClip idle;

    private string runAnim;
    private string idleAnim;

    // Movement
    private Vector3 m_appliedForce;
    private float m_movementXPower = 80.0f;
    private float m_maxSpeed = 15.0f;
    private float m_groundDrag = 0.0f;

    bool m_belowRayCast = false;

    // Air movement
    private float m_extraGravity = 35.0f;
    private float m_airDrag = 0.0f;
    private float m_jumpPower = 25.0f;

    private float m_inverseJumpPower = -20.0f;
    private bool m_inverseJumpAvailable = false;
    private float m_airRetardnessFactor = 0.45f;
    private float m_airRetardnessThreshold = 0.25f;
    private float m_airControlFactor = 0.75f; // 0-1, 0-100 %
    private bool m_jumpActivated = false;

    // Double jump
    private float m_secondJumpPower = 5.0f;
    private float m_doubleJumpTimer = 0.0f;
    private const float m_doubleJumpCooldown = 0.2f;
    private bool m_doubleJumpAvailable = false;
    private bool m_inAir = false;

    // Collision
    private float m_rayCastRange = 1.2f;

    // Controller mappings
    string m_buttonA = "A";
    string m_buttonB = "B";
    string m_buttonX = "X";
    string m_buttonY = "Y";
    string m_leftJoyStickX = "LeftJoystickX";
    string m_leftJoyStickY = "LeftJoystickY";

    string m_KeyBoardX = "Horizontal";
    string m_KeyBoardY = "Vertical";

    string m_KeyBoardJump = "Jump";
    string m_KeyBoardInvJump = "InvJump";

    string m_KeyBoardPush = "Push";

    int m_playerNumber = 0;

    enum Control { Handcontrol, Keyboard}

    private static Control control = Control.Keyboard;
    // Respawn logic
    Vector3 m_respawnPosition;

    // Audio
    private AudioSource m_audioSource;
    public float volumeMultiplier = 5.0f;
    public AudioClip m_jumpSound;

    private void DefinePlayerNumber()
    {
        if (transform.tag == "Player1")
        {
            m_playerNumber = 1;
        }
        else if (transform.tag == "Player2")
        {
            m_playerNumber = 2;
        }
        else
        {
            Debug.Assert(m_playerNumber != 0);
        }
        m_leftJoyStickX += m_playerNumber;
        m_leftJoyStickY += m_playerNumber;

        m_buttonA += m_playerNumber;
        m_buttonB += m_playerNumber;
        m_buttonX += m_playerNumber;
        m_buttonY += m_playerNumber;

        m_KeyBoardX += m_playerNumber;
        m_KeyBoardY += m_playerNumber;

        m_KeyBoardJump += m_playerNumber;
        m_KeyBoardInvJump += m_playerNumber;

        m_KeyBoardPush += m_playerNumber;

    }

    // Use this for initialization
    void Awake ()
    {
        DefinePlayerNumber();
        m_gameObject = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        smartManScript = GameObject.FindGameObjectWithTag("SmartMan").GetComponent<SmartManScript>();
        m_appliedForce.y = 0;
        m_appliedForce.z = 0;
        m_rigidBody = transform.GetComponent<Rigidbody>();
        m_respawnPosition = transform.position;

        animator = meshObject.GetComponent<Animation>();

        runAnim = run.name;
        idleAnim = idle.name;

        m_audioSource = GetComponent<AudioSource>();
    }

    private bool CheckForControllerJump()
    {
        if (control == Control.Handcontrol)
        {
            if (Input.GetButtonDown(m_buttonA))
            {
                return true;
            }
        }
        else if(control == Control.Keyboard)
        {
            if(Input.GetButtonDown(m_KeyBoardJump))
            {
                return true;
            }
        }
        return false;
    }
    
    bool CheckForJoystickJump()
    {
        float deltaY = Input.GetAxis(m_leftJoyStickY);
        if (deltaY < 0.0 && deltaY < -0.5f)
        {
            return true;
        }
        return false;
    }

    private bool CheckForControllerInverseJump()
    {
        if (control == Control.Handcontrol)
        {
            if (Input.GetButtonDown(m_buttonY))
            {
                return true;
            }
        }
        else if (control == Control.Keyboard)
        {
            if (Input.GetButtonDown(m_KeyBoardInvJump))
            {
                return true;
            }
        }
        return false;
    }

    bool CheckForJoystickInverseJump()
    {
        float deltaY = Input.GetAxis(m_leftJoyStickY);
        if (deltaY > 0.5)
        {
            return true;
        }
        return false;
    }

    private bool CheckForJumpInput()
    {
        if (CheckForControllerJump())
        {
            return true;
        }
        return false;
    }

    private bool CheckForInverseJumpInput()
    {
        if (CheckForControllerInverseJump() || CheckForJoystickInverseJump())
        {
            return true;
        }
        return false;
    }

    void OnPlayerDeath()
    {
        transform.parent = null;
        m_gameObject.RespawnPlayer(m_playerNumber);
    }

    private void ComputeXMovement()
    {
        float deltaX;
        if (control == Control.Handcontrol)
        {
            deltaX = Input.GetAxis(m_leftJoyStickX);
        }
        else if(control == Control.Keyboard)
        {
            deltaX = Input.GetAxis(m_KeyBoardX);
        }
        else
        {
            deltaX = Input.GetAxis(m_KeyBoardX);
        }

        if (deltaX > 0)
        {
            /// Going right
        }
        else if (deltaX < 0)
        {
            /// Going left
        }
        else
        {
            /// No command, desired state is still
        }

        if (deltaX == 0 && m_belowRayCast)
        {
            m_rigidBody.velocity = new Vector3(0, m_rigidBody.velocity.y, 0);
        }
        else if(!m_belowRayCast)
        {
            deltaX *= m_airControlFactor;
            float velocityX = m_rigidBody.velocity.x;
            if (velocityX < m_airRetardnessThreshold) // If going right higher than a specific threshold
            {
                // Keep going right
                deltaX += m_airRetardnessFactor;
            }
            else if(velocityX > -m_airRetardnessThreshold) // If going left higher than a specific threshold
            {
                deltaX -= m_airRetardnessFactor;
            }
        }

        m_appliedForce.x = m_movementXPower * deltaX;
    }

    private void ComputeDoubleJump()
    {
        if (CheckForJumpInput() && m_doubleJumpAvailable)
        {
            m_doubleJumpAvailable = false;

            float power = m_secondJumpPower;
            if (m_rigidBody.velocity.y < 0.0f)
            {
                power *= 4.5f;
            }

            AddImpulseToJump(power);
        }
    }

    private void ComputeInverseJump()
    {
        if (CheckForInverseJumpInput() && m_inverseJumpAvailable)
        {
            m_inverseJumpAvailable = false;
            AddImpulseToJump(m_inverseJumpPower);
        }
    }

    private void ComputeYMovement()
    {
        // Check if there's an object below
        if (m_belowRayCast)
        {
            m_inAir = false;
            /// On ground
            if (CheckForJumpInput())
            {
                AddImpulseToJump(m_jumpPower);
                m_inAir = true;
                m_doubleJumpAvailable = true;
                m_inverseJumpAvailable = true;
            }
            m_rigidBody.drag = m_groundDrag;
        }
        else
        {
            /// In air
            m_rigidBody.drag = m_airDrag;
            m_appliedForce.y = -m_extraGravity;
            ComputeDoubleJump();
            ComputeInverseJump();
        }
    }

    private void LimitMovement()
    {
        // Limit maxspeed
        Vector3 velocity = m_rigidBody.velocity;
        if (Mathf.Abs(velocity.x) > m_maxSpeed)
        {
            Vector3 newVelocity = m_rigidBody.velocity.normalized * m_maxSpeed;
            newVelocity.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = newVelocity;
        }
    }

    private void Animate()
    {
        if(m_rigidBody.velocity.magnitude > 1.0f) //run
        {
            animator.CrossFade(runAnim);
        }
        else //idle
        {
            animator.CrossFade(idleAnim);
        }

        if (m_rigidBody.velocity.x >= -2.0f) //så den inte ska lagga
        {
            meshObject.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            meshObject.eulerAngles = new Vector3(0, -90, 0);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "DeadlyAll" || other. gameObject.tag == "DeadlyPlayer")
        {
            OnPlayerDeath();
        }
        else if (other.transform.tag == "Enemy") //kör denna check sist helst
        {
            if (other.transform.position.y < transform.position.y)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    if (hit.transform == other.transform)
                    {
                        other.transform.GetComponent<Enemy>().Die();
                        return;
                    }
                }
            }

            OnPlayerDeath();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DeadlyAll" || other.tag == "DeadlyPlayer")
        {
            OnPlayerDeath();
        }
        else if(other.tag == "Enemy") //kör denna check sist hellst
        {
            if(other.transform.position.y < transform.position.y)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    if(hit.transform == other.transform)
                    {
                        other.GetComponent<Enemy>().Die();
                        return;
                    }
                }
            }

            OnPlayerDeath();
        }
    }

    private void AddImpulseToJump(float power)
    {
        if (m_jumpSound != null)
        {
            m_audioSource.PlayOneShot(m_jumpSound);
        }

        m_jumpActivated = true;
        if(transform.parent != null)
        {
            Rigidbody parentRigidBody = transform.parent.GetComponent<Rigidbody>();
            Vector3 up = parentRigidBody.velocity.normalized;
            m_rigidBody.AddForce(new Vector3(up.x, 1.0f, 0.0f) * power * 1.2f, ForceMode.Impulse);
        }
        else
        {
            m_rigidBody.AddForce(Vector3.up * power, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        m_appliedForce = Vector3.zero;
        m_rigidBody.isKinematic = false;
        m_jumpActivated = false;
        RaycastHit belowHit;
        m_belowRayCast = Physics.Raycast(transform.position, Vector3.down, out belowHit, m_rayCastRange);

        if (m_belowRayCast && belowHit.transform.tag == "Platform")
        {
            if(transform.parent != belowHit.transform)
            {
                transform.parent = belowHit.transform;
            }
        }
        
        ComputeXMovement();
        ComputeYMovement();
        m_rigidBody.AddForce(m_appliedForce);
        LimitMovement();

        if (transform.parent && transform.parent.tag == "Platform" && m_appliedForce.magnitude ==  0.0f && !m_jumpActivated)
        {
            m_rigidBody.isKinematic = true;
        }
        else
        {
            m_rigidBody.isKinematic = false;
            transform.parent = null;
        }

        if (control == Control.Handcontrol)
        {
            if (Input.GetButtonDown(m_buttonB))
            {
                smartManScript.Push(45, this.transform);
            }
        }
        else if(control == Control.Keyboard)
        {
            if (Input.GetButtonDown(m_KeyBoardPush))
            {
                smartManScript.Push(45, this.transform);
            }
        }

        Animate();
    }
    
}
