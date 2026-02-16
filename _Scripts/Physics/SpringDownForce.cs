using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringDownForce : MonoBehaviour
{


    [Header("SPRING PROPERTIES")]
    [SerializeField] Vector3 rayVector = Vector3.down;
    [SerializeField] float height = 1.0f;
    [SerializeField] float SpringStrength = 2.0f;
    [SerializeField] float SpringDamper = 1.5f;
    [SerializeField] LayerMask IgnoreMask;
    RaycastHit hit;
    Rigidbody rb;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {

        if (Physics.Raycast(transform.position, rayVector, out hit, height,~IgnoreMask))
        {
            Vector3 vel=rb.velocity;
            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if(hitBody != null)
            {
                otherVel = hitBody.velocity;
            }
            float rayDirVel = Vector3.Dot(rayVector, vel);
            float otherDirVel=Vector3.Dot(rayVector, otherVel);

            float relVel=rayDirVel-otherDirVel;
            float x = hit.distance - height;
            float springForce = (x * SpringStrength) - (relVel * SpringDamper);
            rb.AddForce(rayVector * springForce);
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position;
        Vector3 direction = rayVector;
        float length = hit.distance;
        Vector3 endPoint = origin + direction * length;
        Gizmos.DrawRay(origin, direction * length);
        Gizmos.DrawSphere(endPoint, 0.1f);

    }
#endif

}
