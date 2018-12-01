using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed;

    private Vector3 direction;


    public void Init(Vector3 planetPosition)
    {
        direction = (planetPosition - transform.position).normalized;
    }

    void Update ()
    {
        transform.position += direction * speed * Time.deltaTime;
	}

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.GameOver();
    }
}
