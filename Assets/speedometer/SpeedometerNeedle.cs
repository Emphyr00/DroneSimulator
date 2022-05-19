using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedometerNeedle : MonoBehaviour
{
    public Rigidbody target;
    public Vector3 velocity;
    public float speed;
    public RectTransform arrow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
   
        velocity = target.velocity;
        speed = velocity.magnitude;
        arrow.localEulerAngles = new Vector3(0, 0, 45 - (speed * 60));
    }
}
