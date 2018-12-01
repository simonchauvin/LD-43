﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed;

    private Vector3 direction;


    public void Init(Vector3 planetPosition)
    {
        direction = (planetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
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
}
