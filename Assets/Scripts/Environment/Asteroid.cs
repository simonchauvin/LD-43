using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float acceleration;
    public float maxScale;

    private SphereCollider _collider;
    private AudioSource audioSource;

    private Vector3 direction;
    private Vector3 planetPosition;
    private float speed;
    private float startDistance;
    private float planetRadius;


    public void Init(Planet planet, Vector3 startPosition)
    {
        transform.position = startPosition;
        transform.localScale = Vector3.one;
        planetPosition = planet.transform.position;
        direction = (planetPosition - transform.position).normalized;
        speed = 0;
        _collider = GetComponent<SphereCollider>();
        planetRadius = planet.GetRadius();
        startDistance = (planetPosition - transform.position).magnitude - planetRadius - _collider.bounds.extents.x;

        audioSource = GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", 20f * (1f - GetDistanceToPlanetNormalized()));
    }

    void Update ()
    {
        if (GameManager.instance.IsReady())
        {
            speed += acceleration * Time.deltaTime;
            transform.position += direction * speed * Time.deltaTime;

            transform.localScale = Vector3.one * Mathf.Lerp(1, maxScale, 1f - GetDistanceToPlanetNormalized());

            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", 20f * (1f - GetDistanceToPlanetNormalized()));
        }
	}

    public float GetDistanceToPlanetNormalized()
    {
        return ((planetPosition - transform.position).magnitude - planetRadius - _collider.bounds.extents.x) / startDistance;
    }
}
