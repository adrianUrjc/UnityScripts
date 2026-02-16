using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPointsForce : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] Vector3 height = Vector3.zero;
    [SerializeField] float SpringStrength = 2.0f;
    [SerializeField] float SpringDamper = 1.5f;
    [SerializeField] float MaxForce = 20f;
    Rigidbody rb;

    void Start()
    {
        if (height.magnitude <= 0.01f)
        {
            height = endPoint.position - startPoint.position;
            height -= Vector3.one*height.magnitude / 2;
        }
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //Vector3 springDir = endPoint.position - startPoint.position;
        //Vector3 currentDisplacement = springDir;
        //Vector3 restDisplacement = height;
        //Vector3 displacementFromRest = currentDisplacement - restDisplacement;

        //// Velocidad relativa en la dirección del resorte
        //Vector3 relativeVelocity = rb.velocity;
        //Vector3 springDirNormalized = springDir.normalized;
        //float dampingForceMag = Vector3.Dot(relativeVelocity, springDirNormalized) * SpringDamper;

        //// Fuerza del resorte
        //Vector3 springForce = (-displacementFromRest * SpringStrength) - (dampingForceMag * springDirNormalized);

        Vector3 springDir = endPoint.position - startPoint.position;
        float relVel=Vector3.Dot(springDir, rb.velocity);
        Vector3 dampingForce = -SpringDamper * relVel * springDir;
        Vector3 springForce =SpringStrength*(springDir-height);
        Vector3 forceToAdd = springForce + dampingForce;
        forceToAdd = Vector3.ClampMagnitude(forceToAdd, MaxForce);
        rb.AddForce(forceToAdd);
    }

}
