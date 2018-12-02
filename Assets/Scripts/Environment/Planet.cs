using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float maxDustRate;

    private MeshCollider _collider;
    private ParticleSystem.EmissionModule dustStorm;

    private float radius;

    
	public void Init()
    {
        _collider = GetComponentInChildren<MeshCollider>();
        dustStorm = GetComponent<ParticleSystem>().emission;

        radius = _collider.bounds.extents.x;
    }
	
	void Update ()
    {
		if (GameManager.instance.IsReady())
        {
            dustStorm.rateOverTime = Mathf.Lerp(0, maxDustRate, 1f - GameManager.instance.GetAsteroidDistanceToPlanetNormalized());
        }
	}

    public float GetRadius()
    {
        return radius;
    }
}
