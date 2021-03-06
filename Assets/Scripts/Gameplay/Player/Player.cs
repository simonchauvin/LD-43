﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float gravityForce;
    public float speed;
    public float maxSpeed;
    public float rotationSpeed;
    public float maxDistanceToPlanet;
    public float timeToLerpCamera;
    public float maxHeadHeight;
    public float defaultCameraSize;
    public float interactCameraSize;
    public float maxShakeAmplitude;
    public float maxShakeStopDuration;
    public float timeToMoveHead;
    public Transform knowledgeFollowerPrefab;
    public Transform peopleFollowerPrefab;
    public Transform natureFollowerPrefab;
    public Transform artFollowerPrefab;

    private Planet planet;
    private List<Transform> followers;

    private Rigidbody _rigidbody;
    private Transform head;
    private Camera mainCamera;
    private Transform followersFolder;
    private Entity currentEntity;

    private Vector3 toPlanetVector;
    private float planetRadius;
    private bool movingForward;
    private bool movingBackward;
    private bool rotateLeft;
    private bool rotateRight;
    private bool firstOptionSelected;
    private bool secondOptionSelected;
    private bool bringCameraCloser;
    private bool resetCamera;
    private float shakeStopDuration;
    private float shakeStopDurationTimer;
    private float timerLerpCamera;
    private float headMoveTimer;
    private Vector3 originalHeadPosition;
    private Vector3 originalCameraLocalPosition;
    private float startHeadHeight;
    private float targetHeadHeight;

    private void Awake()
    {
        head = transform.Find("Head");
        originalHeadPosition = head.localPosition;
    }

    public void Init(Planet planet, Transform start)
    {
        this.planet = planet;
        transform.position = start.position;
        transform.rotation = start.rotation;

        if (followers != null)
        {
            for (int i = 0; i < followers.Count; i++)
            {
                Destroy(followers[i].gameObject);
            }
            followers.Clear();
        }
        followers = new List<Transform>();

        _rigidbody = GetComponent<Rigidbody>();
        Stop();
        
        mainCamera = GetComponentInChildren<Camera>();
        mainCamera.orthographicSize = defaultCameraSize;
        followersFolder = transform.parent.Find("Followers");
        currentEntity = null;
        head.localPosition = originalHeadPosition;

        toPlanetVector = planet.transform.position - _rigidbody.position;
        planetRadius = planet.GetRadius();
        movingForward = false;
        movingBackward = false;
        rotateLeft = false;
        rotateRight = false;
        firstOptionSelected = true;
        secondOptionSelected = false;
        bringCameraCloser = false;
        resetCamera = false;
        shakeStopDuration = 0;
        shakeStopDurationTimer = 0;
        timerLerpCamera = 0;
        originalCameraLocalPosition = mainCamera.transform.localPosition;
        headMoveTimer = 0;
        startHeadHeight = head.localPosition.y;
        targetHeadHeight = originalHeadPosition.y + maxHeadHeight;
    }
	
	void Update()
    {
        if (GameManager.instance.IsReady())
        {
            float translation = Input.GetAxis("Vertical"),
                rotation = Input.GetAxis("Horizontal");

            movingForward = translation > 0;
            movingBackward = translation < 0;
            rotateLeft = rotation < 0;
            rotateRight = rotation > 0;

            if (bringCameraCloser)
            {
                if (timerLerpCamera <= timeToLerpCamera)
                {
                    mainCamera.orthographicSize = Mathf.SmoothStep(defaultCameraSize, interactCameraSize, timerLerpCamera / timeToLerpCamera);
                    timerLerpCamera += Time.deltaTime;
                }
                else
                {
                    GameManager.instance.ShowTakeOrLeaveUI(currentEntity.firstOption, currentEntity.secondOption, currentEntity.description);

                    if (translation > 0)
                    {
                        GameManager.instance.SelectTake();
                        firstOptionSelected = true;
                        secondOptionSelected = false;
                    }
                    else if (translation < 0)
                    {
                        GameManager.instance.SelectLeave();
                        firstOptionSelected = false;
                        secondOptionSelected = true;
                    }

                    if (Input.GetButton("Submit"))
                    {
                        if (firstOptionSelected)
                        {
                            currentEntity.FirstOption();
                        }
                        else if (secondOptionSelected)
                        {
                            currentEntity.SecondOption();
                        }

                        firstOptionSelected = true;
                        secondOptionSelected = false;
                        bringCameraCloser = false;
                        ResetCamera();
                        GameManager.instance.HideTakeOrLeaveUI();
                    }
                }

                StopMovement();
            }

            if (resetCamera)
            {
                if (timerLerpCamera <= timeToLerpCamera)
                {
                    mainCamera.orthographicSize = Mathf.SmoothStep(interactCameraSize, defaultCameraSize, timerLerpCamera / timeToLerpCamera);
                    timerLerpCamera += Time.deltaTime;
                }
                else
                {
                    resetCamera = false;
                }

                StopMovement();
            }

            ShakeCamera();
        }
        else if (GameManager.instance.IsLeaving())
        {
            ShakeCamera();
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.IsReady())
        {
            // Gravity
            toPlanetVector = planet.transform.position - _rigidbody.position;
            _rigidbody.AddForce(toPlanetVector.normalized * (1f - Mathf.Clamp01((toPlanetVector.magnitude - planetRadius) / maxDistanceToPlanet)) * gravityForce);
            _rigidbody.rotation = Quaternion.FromToRotation(transform.up, -toPlanetVector.normalized) * _rigidbody.rotation;

            // Movement
            if (Mathf.Abs(Vector3.Dot(transform.forward, _rigidbody.velocity)) < maxSpeed)
            {
                if (movingForward)
                {
                    _rigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
                }
                else if (movingBackward)
                {
                    _rigidbody.AddForce(-transform.forward * speed, ForceMode.Impulse);
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

            if (_rigidbody.velocity.magnitude > 0.1f)
            {
                head.localPosition = new Vector3(head.localPosition.x, Mathf.Lerp(startHeadHeight, targetHeadHeight, headMoveTimer / timeToMoveHead), head.localPosition.z);
                
                if (head.localPosition.y >= originalHeadPosition.y + maxHeadHeight)
                {
                    startHeadHeight = head.localPosition.y;
                    targetHeadHeight = originalHeadPosition.y;

                    headMoveTimer = 0;
                }
                else if (head.localPosition.y <= originalHeadPosition.y)
                {
                    startHeadHeight = head.localPosition.y;
                    targetHeadHeight = originalHeadPosition.y + maxHeadHeight;

                    headMoveTimer = 0;
                }
                headMoveTimer += Time.deltaTime;
            }

            for (int i = 0; i < followers.Count; i++)
            {
                Rigidbody rigid = followers[i].GetComponent<Rigidbody>();
                Vector3 gravityVector = planet.transform.position - rigid.position;
                rigid.AddForce(gravityVector.normalized * (1f - Mathf.Clamp01((gravityVector.magnitude - planetRadius) / maxDistanceToPlanet)) * gravityForce);
                rigid.rotation = Quaternion.FromToRotation(followers[i].up, -gravityVector.normalized) * rigid.rotation;

                Vector3 toPreviousVector;
                if (i == 0)
                {
                    toPreviousVector = transform.position - followers[i].position;
                }
                else
                {
                    toPreviousVector = followers[i - 1].position - followers[i].position;
                }
                float magnitude = toPreviousVector.magnitude;

                if (magnitude > 2f)
                {
                    rigid.MovePosition(rigid.position + toPreviousVector.normalized * 10f * Time.deltaTime);
                }
            }
        }
    }

    private void ShakeCamera()
    {
        if (shakeStopDurationTimer > shakeStopDuration)
        {
            Vector3 shake = Random.insideUnitCircle * Mathf.Lerp(0, maxShakeAmplitude, 1f - GameManager.instance.GetAsteroidDistanceToPlanetNormalized());
            mainCamera.transform.localPosition = originalCameraLocalPosition + shake;

            shakeStopDuration = Random.Range(0, maxShakeStopDuration);
            shakeStopDurationTimer = 0;
        }
        else
        {
            shakeStopDurationTimer += Time.deltaTime;
        }
    }

    private void StopMovement()
    {
        movingForward = false;
        movingBackward = false;
        rotateLeft = false;
        rotateRight = false;
    }

    public void AddFollower(EntityKind kind)
    {
        Vector3 point = Random.onUnitSphere * 2f;
        if (followers.Count > 0)
        {
            point = followers[followers.Count - 1].position + new Vector3(point.x, 0, point.z);
        }
        else
        {
            point = transform.position + new Vector3(point.x, 0, point.z);
        }

        if ((point - planet.transform.position).magnitude - 1f <= planet.GetRadius())
        {
            point += (point - planet.transform.position).normalized * 1f;
        }

        switch (kind)
        {
            case EntityKind.Knowledge:
                followers.Add(Instantiate(knowledgeFollowerPrefab, point, Quaternion.identity, followersFolder));
                break;
            case EntityKind.People:
                followers.Add(Instantiate(peopleFollowerPrefab, point, Quaternion.identity, followersFolder));
                break;
            case EntityKind.Nature:
                followers.Add(Instantiate(natureFollowerPrefab, point, Quaternion.identity, followersFolder));
                break;
            case EntityKind.Art:
                followers.Add(Instantiate(artFollowerPrefab, point, Quaternion.identity, followersFolder));
                break;
        }
    }

    public void Stop()
    {
        _rigidbody.velocity = Vector3.zero;

        for (int i = 0; i < followers.Count; i++)
        {
            followers[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void Interact(Entity entity)
    {
        currentEntity = entity;
        bringCameraCloser = true;
        timerLerpCamera = 0;
    }

    public void ResetCamera()
    {
        resetCamera = true;
        timerLerpCamera = 0;
    }

    public Camera GetCamera()
    {
        return mainCamera;
    }
}
