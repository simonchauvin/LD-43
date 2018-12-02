﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObject : MonoBehaviour
{
    public void Align()
    {
        transform.up = (transform.position - FindObjectOfType<Planet>().transform.position).normalized;
        transform.position = FindObjectOfType<Planet>().transform.position + (transform.position - FindObjectOfType<Planet>().transform.position).normalized * FindObjectOfType<Planet>().GetComponent<Collider>().bounds.extents.x;
    }
}
