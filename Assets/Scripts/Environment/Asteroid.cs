using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float acceleration;
    public float maxScale;

    private Vector3 direction;
    private Vector3 planetPosition;
    private float speed;
    private float startDistance;


    public void Init(Vector3 planetPosition)
    {
        this.planetPosition = planetPosition;
        direction = (planetPosition - transform.position).normalized;
        speed = 0;
        startDistance = (planetPosition - transform.position).magnitude;
    }

    void Update ()
    {
        if (GameManager.instance.IsReady())
        {
            speed += acceleration * Time.deltaTime;
            transform.position += direction * speed * Time.deltaTime;

            transform.localScale = Vector3.one * Mathf.Lerp(1, maxScale, 1f - GetDistanceToPlanetNormalized());
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
