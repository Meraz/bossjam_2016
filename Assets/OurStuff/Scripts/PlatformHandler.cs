using UnityEngine;
using System.Collections;

public class PlatformHandler : MonoBehaviour {
    public Transform[] buttons;
    private Transform currButton; //den som stås på atm
    public float buttonActivationRange = 3.0f;
    private LayerMask layerMaskNonStatic;

    //LJUD
    private bool activated; //för ljud
    //private AudioSource audioSource;
    public float volumeMultiplier = 5.0f;
    public AudioClip activatedSound;
    public AudioClip deactivatedSound;

    public Transform platform;
    private Rigidbody platformRigidbody;

    private Vector3 startPos;
    public Transform endPos;

    private Vector3 wantedPos;

    public float speed = 100.0f;
    public float maxSpeed = 20;
    public float drag = 8.0f;
	// Use this for initialization
	void Start () {
        layerMaskNonStatic = ~(1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Untagged") | 1 << LayerMask.NameToLayer("MovingPlatform")); // ignore collisions with statics

        startPos = platform.position;

        platformRigidbody = platform.GetComponent<Rigidbody>();
        platformRigidbody.drag = drag;
        wantedPos = startPos;

        activated = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(ButtonActivated() == true)
        {
            if(activated == false) //hoppade på knappen
            {
                currButton.GetComponent<AudioSource>().PlayOneShot(activatedSound, volumeMultiplier);
            }
            activated = true;
            wantedPos = endPos.position;
        }
        else
        {
            if(activated == true) //hoppade av knappen
            {
                currButton.GetComponent<AudioSource>().PlayOneShot(deactivatedSound, volumeMultiplier);
            }
            activated = false;
            wantedPos = startPos;
        }

        Vector3 toTarget = (wantedPos - platform.position);
        if(toTarget.magnitude < 1.0f)
        {
         //   platformRigidbody.MovePosition(platformRigidbody.position + toTarget * Time.deltaTime);
        }
        else
        {
            platformRigidbody.MovePosition(platformRigidbody.position + toTarget.normalized * 15.0f * Time.deltaTime);
        }
        
        /*
        if (Vector3.Distance(platform.position, wantedPos) > 0.6f)
        {
            platformRigidbody.AddForce(toTarget * Time.deltaTime * speed);

            if (platformRigidbody.velocity.magnitude > maxSpeed)
            {
                platformRigidbody.velocity = platformRigidbody.velocity.normalized * maxSpeed;
            }
        }*/
    }

    bool ButtonActivated()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Collider[] col = Physics.OverlapSphere(buttons[i].position, buttonActivationRange, layerMaskNonStatic);
            if (col.Length > 0)
            {
                currButton = buttons[i];
                return true; //någon stod vid knappen
            }
        }

        return false;
    }

    //void OnTriggerStay(Collider collidingUnit)
    //{
    //    wantedPos = endPos.position;        
    //}

    //void OnTriggerExit(Collider collidingUnit)
    //{
    //    wantedPos = startPos;
    //}


    //void OnCollisionEnter(Collision collidingUnit)
    //{
    //    wantedPos = endPos.position;

    //}

    //void OnCollisionExit(Collision collidingUnit)
    //{
    //    wantedPos = startPos;

    //}
}
