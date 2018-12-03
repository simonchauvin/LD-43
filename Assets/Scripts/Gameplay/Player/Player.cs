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
    public float timeToLerpCamera;
    public float maxShakeAmplitude;
    public float maxShakeStopDuration;

    private Planet planet;

    private Rigidbody _rigidbody;
    private Camera mainCamera;
    private Transform cameraCloseTargetPosition;
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
    private Vector3 startCameraPosition;
    private Vector3 initialCameraLocalPosition;
    private float shakeStopDuration;
    private float shakeStopDurationTimer;
    private float timerLerpCamera;


    public void Init(Planet planet, Transform start)
    {
        this.planet = planet;
        transform.position = start.position;
        transform.rotation = start.rotation;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = Vector3.zero;
        mainCamera = GetComponentInChildren<Camera>();
        cameraCloseTargetPosition = transform.Find("CameraCloseTargetPositon");
        currentEntity = null;

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
        startCameraPosition = mainCamera.transform.position;
        initialCameraLocalPosition = mainCamera.transform.localPosition;
        shakeStopDuration = 0;
        shakeStopDurationTimer = 0;
        timerLerpCamera = 0;
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
                    mainCamera.transform.position = Vector3.Lerp(startCameraPosition, cameraCloseTargetPosition.position, Mathf.SmoothStep(0, 1, timerLerpCamera / timeToLerpCamera));
                    timerLerpCamera += Time.deltaTime;

                    initialCameraLocalPosition = mainCamera.transform.localPosition;
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
                    mainCamera.transform.position = Vector3.Lerp(cameraCloseTargetPosition.position, startCameraPosition, Mathf.SmoothStep(0, 1, timerLerpCamera / timeToLerpCamera));
                    timerLerpCamera += Time.deltaTime;

                    initialCameraLocalPosition = mainCamera.transform.localPosition;
                }
                else
                {
                    resetCamera = false;
                }

                StopMovement();
            }

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
        }
    }

    private void ShakeCamera()
    {
        if (shakeStopDurationTimer > shakeStopDuration)
        {
            Vector3 shake = Random.insideUnitCircle * Mathf.Lerp(0, maxShakeAmplitude, 1f - GameManager.instance.GetAsteroidDistanceToPlanetNormalized());
            mainCamera.transform.localPosition = initialCameraLocalPosition + shake;

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

    public void Interact(Entity entity)
    {
        currentEntity = entity;
        bringCameraCloser = true;
        startCameraPosition = mainCamera.transform.position;
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
