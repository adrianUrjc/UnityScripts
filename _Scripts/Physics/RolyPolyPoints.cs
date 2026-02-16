using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RolyPolyPoints : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    Quaternion initialRot = new Quaternion();
    Rigidbody _rb;
    [SerializeField]
    protected float _uprightJointSpringStrength = 1;
    [SerializeField]
    protected float _uprightJointSpringDamper = 1;
    void Start()
    {
        initialRot =Quaternion.LookRotation(endPoint.position- startPoint.position);
        
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUprightForce();
    }
    public void UpdateUprightForce()
    {
        Quaternion characterCurrent = startPoint.rotation;
        Quaternion toGoal = ShortestRotation(initialRot, characterCurrent);
        Vector3 rotAxis;
        float rotDegrees;
        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        _rb.AddTorque((rotAxis * (rotRadians * _uprightJointSpringStrength)) - (_rb.angularVelocity * _uprightJointSpringDamper));

    }
    protected Quaternion ShortestRotation(Quaternion to, Quaternion from)
    {
        // Invertimos el Quaternion "from" para obtener la rotación relativa
        Quaternion inverseFrom = Quaternion.Inverse(from);

        // Multiplicamos "to" por el inverso de "from" para obtener la rotación que lo lleva a "to"
        Quaternion deltaRotation = to * inverseFrom;

        // Aseguramos que tomamos el camino más corto
        if (Quaternion.Dot(deltaRotation, Quaternion.identity) < 0f)
        {
            deltaRotation = new Quaternion(-deltaRotation.x, -deltaRotation.y, -deltaRotation.z, -deltaRotation.w);
        }

        return deltaRotation;
    }
}
