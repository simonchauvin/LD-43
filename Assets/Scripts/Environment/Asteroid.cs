using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed;

    private Vector3 direction;
    private Vector3 planetPosition;
    private float startDistance;


    public void Init(Vector3 planetPosition)
    {
        this.planetPosition = planetPosition;
        direction = (planetPosition - transform.position).normalized;
        startDistance = (planetPosition - transform.position).magnitude;
    }

    void Update ()
    {
        if (GameManager.instance.IsReady())
        {
            transform.position += direction * speed * Time.deltaTime;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.GameOver();
    }

    public float GetDistanceToPlanetNormalized()
    {
        return (planetPosition - transform.position).magnitude / startDistance;
    }
}
