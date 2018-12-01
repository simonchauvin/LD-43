using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private MeshCollider _collider;

    private float radius;

    
	public void Init()
    {
        _collider = GetComponent<MeshCollider>();

        radius = _collider.bounds.extents.x;
    }
	
	void Update ()
    {
		
	}

    public float GetRadius()
    {
        return radius;
    }
}
