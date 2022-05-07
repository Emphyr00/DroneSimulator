using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeler : MonoBehaviour
{
    public float thrust;
    public Rigidbody propeler;
    public float rotationY;
    public Transform drone;

    void Start()
    {
        propeler = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            propeler.AddRelativeForce(Vector3.up * thrust);
        }
        // rotationY = drone.eulerAngles.y;
        // if (propeler.name == "Mesh2") // LEFT FRONT
        // {
            
        // }

        // if (propeler.name == "RF") // RIGHT FRONT
        // {
        //     if (Input.GetKey(KeyCode.Space))
        //     {
        //         propeler.AddForceAtPosition(Vector3.up * thrust, propeler.transform.position);
        //     }
        // }

        // if (propeler.name == "LB") // LEFT BACK
        // {

        // }

        // if (propeler.name == "RB") // RIGHT BACK
        // {

        // }
    }
}
