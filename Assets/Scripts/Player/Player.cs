using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float gravityForce;
    public float speed;
    public float maxSpeed;
    public float rotationSpeed;
    public float maxDistanceToPlanet;

    private Planet planet;

    private Rigidbody _rigidbody;

    private Vector3 toPlanetVector;
    private float planetRadius;
    private bool movingForward;
    private bool rotateLeft;
    private bool rotateRight;


    public void Init(Planet planet)
    {
        this.planet = planet;

        _rigidbody = GetComponent<Rigidbody>();

        toPlanetVector = planet.transform.position - _rigidbody.position;
        planetRadius = planet.GetRadius();
        movingForward = false;
        rotateLeft = false;
        rotateRight = false;
    }
	
	void Update ()
    {
        float translation = Input.GetAxis("Vertical"),
            rotation = Input.GetAxis("Horizontal");

        movingForward = translation != 0;
        rotateLeft = rotation < 0;
        rotateRight = rotation > 0;
    }

    void FixedUpdate()
    {
        // Gravity
        toPlanetVector = planet.transform.position - _rigidbody.position;
        _rigidbody.AddForce(toPlanetVector.normalized * Mathf.Clamp01(maxDistanceToPlanet / (toPlanetVector.magnitude - planetRadius)) * gravityForce);
        _rigidbody.rotation = Quaternion.FromToRotation(transform.up, -toPlanetVector.normalized) * _rigidbody.rotation;

        // Movement
        if (Vector3.Dot(transform.forward, _rigidbody.velocity) < maxSpeed)
        {
            if (movingForward)
            {
                _rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
            }
        }
        
        if (rotateLeft)
        {
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0));
        }

        if (rotateRight)
        {
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0));
        }
    }
}
