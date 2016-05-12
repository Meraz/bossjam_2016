using UnityEngine;
using System.Collections;

public class HatchHandler : MonoBehaviour {
    public Transform[] buttons;
    public float buttonActivationRange = 3.0f;
    private LayerMask layerMaskNonStatic;

    public Transform hatch;
    public float speedMultiplier = 0.5f;

    //public Transform startPos;
    private Vector3 startPos;
    public Transform endPos;

    private Vector3 wantedPos;

    private float startTime;
	// Use this for initialization
	void Start () {
        layerMaskNonStatic = ~(1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Untagged")); // ignore collisions with statics

        startPos = hatch.position;
        wantedPos = startPos;
	}
	
	// Update is called once per frame
	void Update () {
        if(ButtonActivated() == true)
        {

        }
        //Vector3 setRelCenter = wantedPos - hatch.position;
        float fracComplete = (Time.time - startTime) * speedMultiplier;
        hatch.position = Vector3.Lerp(hatch.position, wantedPos, fracComplete);
    }

    void OnTriggerEnter(Collider collidingUnit)
    {
        wantedPos = endPos.position;
        startTime = Time.time;
    }

    void OnTriggerExit(Collider collidingUnit)
    {
        wantedPos = startPos;
        startTime = Time.time;
    }


    void OnCollisionEnter(Collision collidingUnit)
    {
        wantedPos = endPos.position;
        startTime = Time.time;
    }

    void OnCollisionExit(Collision collidingUnit)
    {
        wantedPos = startPos;
        startTime = Time.time;
    }

    bool ButtonActivated()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            Collider[] col = Physics.OverlapSphere(buttons[i].position, buttonActivationRange, layerMaskNonStatic);
            if(col.Length > 0)
            {
                return true; //någon stod vid knappen
            }
        }

        return false;
    }
}
