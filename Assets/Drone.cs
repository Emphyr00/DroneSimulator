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
    public Quaternion targetRotation;
    public float yRotation;
    public float dronePostionZ;
    public float homePostionZ;
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

        Vector3 relativePos = backHomeTarget.position - mTransform.position;
        targetRotation = Quaternion.LookRotation(relativePos, Vector3.up);
        yRotation = mTransform.rotation.eulerAngles.y - targetRotation.eulerAngles.y;
        dronePostionZ = mTransform.position.z;
        homePostionZ = backHomeTarget.position.z;
        if (OVRInput.Get(OVRInput.Button.One) || Input.GetKey(KeyCode.H)) 
        {
            BackHome(ref spin, ref forward, ref up);
        }

        if (Input.GetKey(KeyCode.W))
        {
            forward = 1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            up = 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            up = -2;
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
        CorrectAngles(ref orientation);

        Vector3 localangularvelocity = mTransform.InverseTransformDirection(body.angularVelocity);

        float velY = body.velocity.y;

        float correctedForward = forward * MAX_TILT - (orientation.x + localangularvelocity.x * 15);
        float correctedRight = -right * MAX_TILT - (orientation.z + localangularvelocity.z * 15);
        float correctedSpin = spin - localangularvelocity.y;

        ApplyForces(correctedForward / MAX_TILT, correctedRight / MAX_TILT, up - velY, correctedSpin);
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

    void CorrectAngles(ref Vector3 euler)
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

    void BackHome (ref float spin, ref float forward, ref float up)
    {
        if ((mTransform.position.z - backHomeTarget.position.z > 0.4 || mTransform.position.z - backHomeTarget.position.z < -0.4) && (mTransform.position.x - backHomeTarget.position.x > 0.4 || mTransform.position.x - backHomeTarget.position.x < -0.4))
        {
            if (mTransform.rotation.eulerAngles.y - targetRotation.eulerAngles.y > 3)
            {
                spin = -1;
            }
            else if (mTransform.rotation.eulerAngles.y - targetRotation.eulerAngles.y < -3)
            {
                spin = 1;
            }
            else
            {
                forward = 1;
            }
        }
        else
        {
            forward = 0;
            up = -1;
        }
    }
}
