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
    private bool takeSelected;
    private bool leaveSelected;
    private bool bringCameraCloser;
    private bool resetCamera;
    private Vector3 startCameraPosition;
    private float timerLerpCamera;


    public void Init(Planet planet)
    {
        this.planet = planet;

        _rigidbody = GetComponent<Rigidbody>();
        mainCamera = GetComponentInChildren<Camera>();
        cameraCloseTargetPosition = transform.Find("CameraCloseTargetPositon");
        currentEntity = null;

        toPlanetVector = planet.transform.position - _rigidbody.position;
        planetRadius = planet.GetRadius();
        movingForward = false;
        movingBackward = false;
        rotateLeft = false;
        rotateRight = false;
        takeSelected = true;
        leaveSelected = false;
        bringCameraCloser = false;
        resetCamera = false;
        startCameraPosition = mainCamera.transform.position;
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
                }
                else
                {
                    GameManager.instance.ShowTakeOrLeaveUI(currentEntity.firstOption, currentEntity.secondOption, currentEntity.description);

                    if (translation > 0)
                    {
                        GameManager.instance.SelectTake();
                        takeSelected = true;
                        leaveSelected = false;
                    }
                    else if (translation < 0)
                    {
                        GameManager.instance.SelectLeave();
                        takeSelected = false;
                        leaveSelected = true;
                    }

                    if (Input.GetButton("Submit"))
                    {
                        if (takeSelected)
                        {
                            GameManager.instance.LoadEntity(currentEntity);
                        }
                        else if (leaveSelected)
                        {

                        }

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
                }
                else
                {
                    resetCamera = false;
                }

                StopMovement();
            }
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
