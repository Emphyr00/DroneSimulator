using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drone : MonoBehaviour
{
    const float MAX_FORCE = 100;
    const float MAX_TILT = 15;
    const float STEER_FORCE = .3f;
    const float MAX_SPIN = .5f;
    Vector3 frontLeft, frontRight, rearLeft, rearRight;

    public Rigidbody body;
    public Vector3 velocity;
    public float speed;

    public 
    
    public Transform backHomeTarget;
    Transform mTransform;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        mTransform = GetComponent<Transform>();
        frontLeft = new Vector3(-mTransform.localScale.x, 0, mTransform.localScale.x);
        frontRight = new Vector3(mTransform.localScale.x, 0, mTransform.localScale.x);
        rearLeft = new Vector3(-mTransform.localScale.x, 0, -mTransform.localScale.x);
        rearRight = new Vector3(mTransform.localScale.x, 0, -mTransform.localScale.x);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OVRInput.FixedUpdate();
        float forward = 0;
        float up = 0;
        float right = 0;
        float spin = 0;

        forward = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
        right = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        spin = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        up = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        
        Vector3 relativePos = backHomeTarget.position - transform.position;
        targetRotation = Quaternion.LookRotation(relativePos, Vector3.up);

        if (OVRInput.Get(OVRInput.Button.One) || Input.GetKey(KeyCode.H) {
            while(target.rotationY != targetRotation) {
                right = 1;
            }
        }

        if (Input.GetKey(KeyCode.W) || OVRInput.Get(OVRInput.Button.One))
        {
            forward = 1;
        }
        up = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        if (Input.GetKey(KeyCode.Space))
        {
            up = 1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            up = -1;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            up = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            right = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            right = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forward = -1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            spin = -1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            spin = 1;
        }
        Vector3 orientation = mTransform.localRotation.eulerAngles;
        orientation.y = 0;
        FixRanges(ref orientation);

        Vector3 localangularvelocity = mTransform.InverseTransformDirection(body.angularVelocity);

        float velY = body.velocity.y;

        float desiredForward = forward * MAX_TILT - (orientation.x + localangularvelocity.x * 15);
        float desiredRight = -right * MAX_TILT - (orientation.z + localangularvelocity.z * 15);
        float desiredSpin = spin - localangularvelocity.y;

        ApplyForces(desiredForward / MAX_TILT, desiredRight / MAX_TILT, up - velY, desiredSpin);
    }

    void ApplyForces(float forward, float right, float up, float spin)
    {
        //need to maintain this level of upwards thrust to gain/lose altitude at the desired rate
        float totalY = Mathf.Min((up * 10) + 9.81f, MAX_FORCE);

        if (totalY < 0) totalY = 0;

        //distribute according to forward/right (which are indices based on max tilt)
        //front left
        body.AddForceAtPosition(mTransform.up * (totalY * .25f - forward * STEER_FORCE - right * STEER_FORCE), mTransform.position + mTransform.TransformDirection(frontLeft));

        //front right
        body.AddForceAtPosition(mTransform.up * (totalY * .25f - forward * STEER_FORCE + right * STEER_FORCE), mTransform.position + mTransform.TransformDirection(frontRight));

        //rear left
        body.AddForceAtPosition(mTransform.up * (totalY * .25f + forward * STEER_FORCE - right * STEER_FORCE), mTransform.position + mTransform.TransformDirection(rearLeft));

        //rear right
        body.AddForceAtPosition(mTransform.up * (totalY * .25f + forward * STEER_FORCE + right * STEER_FORCE), mTransform.position + mTransform.TransformDirection(rearRight));

        spin = Mathf.Min(MAX_SPIN, spin);

        //Front
        body.AddForceAtPosition(mTransform.right * spin, mTransform.position + mTransform.forward);
        //Rear
        body.AddForceAtPosition(-mTransform.right * spin, mTransform.position - mTransform.forward);
    }

    void FixRanges(ref Vector3 euler)
    {
        if (euler.x < -180)
            euler.x += 360;
        else if (euler.x > 180)
            euler.x -= 360;

        if (euler.y < -180)
            euler.y += 360;
        else if (euler.y > 180)
            euler.y -= 360;

        if (euler.z < -180)
            euler.z += 360;
        else if (euler.z > 180)
            euler.z -= 360;
    }
}
