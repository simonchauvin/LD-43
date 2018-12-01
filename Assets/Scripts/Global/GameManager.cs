﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    public float introTime;
    public Color targetAtmosphereColor;
    public Color targetLightColor;

    private Player player;
    private Entity[] entities;
    private Planet planet;
    private Asteroid asteroid;
    private Camera mainCamera;
    private Light asteroidLight;
    private List<Entity> loadedEntities;

    private Canvas canvas;
    private GameObject introUI;
    private Image introPanel;
    private Text introText;
    private GameObject takeOrLeaveUI;
    private GameObject descriptionText;
    private GameObject takeSelector;
    private GameObject leaveSelector;

    private float introTimer;
    private bool ready;
    private float startAsteroidDistance;
    private Color startAtmosphereColor;
    private Color startLightColor;


    void Start ()
    {
        player = FindObjectOfType<Player>();
        entities = FindObjectsOfType<Entity>();
        planet = FindObjectOfType<Planet>();
        asteroid = FindObjectOfType<Asteroid>();
        asteroidLight = FindObjectOfType<Light>();
        loadedEntities = new List<Entity>();

        canvas = FindObjectOfType<Canvas>();
        introUI = canvas.transform.Find("IntroUI").gameObject;
        introPanel = introUI.transform.Find("Panel").GetComponent<Image>();
        introText = introUI.transform.Find("Text").GetComponent<Text>();
        introUI.SetActive(true);
        takeOrLeaveUI = canvas.transform.Find("TakeOrLeaveUI").gameObject;
        descriptionText = takeOrLeaveUI.transform.Find("DescriptionText").gameObject;
        takeSelector = takeOrLeaveUI.transform.Find("TakeSelector").gameObject;
        leaveSelector = takeOrLeaveUI.transform.Find("LeaveSelector").gameObject;
        takeOrLeaveUI.SetActive(false);

        planet.Init();
        asteroid.Init(planet.transform.position);
        player.Init(planet);
        for (int i = 0; i < entities.Length; i++)
        {
            entities[i].Init(player);
        }
        
        mainCamera = player.GetCamera();
        introTimer = 0;
        ready = false;
        startAtmosphereColor = mainCamera.backgroundColor;
        startLightColor = asteroidLight.color;
        startAsteroidDistance = (asteroid.transform.position - planet.transform.position).sqrMagnitude;
    }
	
	void Update ()
    {
        if (ready)
        {
            mainCamera.backgroundColor = Color.Lerp(targetAtmosphereColor, startAtmosphereColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance);
            asteroidLight.color = Color.Lerp(targetLightColor, startLightColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance);
        }
        else
        {
            if (introTimer <= introTime)
            {
                introPanel.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), introTimer / introTime);
                introTimer += Time.deltaTime;

                if (introTimer >= introTime * 0.5f)
                {
                    introText.enabled = false;
                }
            }
            else
            {
                introPanel.enabled = false;
                ready = true;
            }
        }
    }

    public void LoadEntity(Entity entity)
    {
        Debug.Log("Entity" + entity + "loaded");
        loadedEntities.Add(entity);
        entity.Take();
    }

    public void ShowTakeOrLeaveUI(string description)
    {
        if (!takeOrLeaveUI.activeSelf)
        {
            takeOrLeaveUI.SetActive(true);
            descriptionText.GetComponent<Text>().text = description;
            descriptionText.GetComponent<Text>().enabled = true;
            takeSelector.GetComponent<Text>().enabled = true;
            leaveSelector.GetComponent<Text>().enabled = false;
        }
    }

    public void HideTakeOrLeaveUI()
    {
        takeOrLeaveUI.SetActive(false);
    }

    public void SelectTake()
    {
        takeSelector.GetComponent<Text>().enabled = true;
        leaveSelector.GetComponent<Text>().enabled = false;
    }

    public void SelectLeave()
    {
        takeSelector.GetComponent<Text>().enabled = false;
        leaveSelector.GetComponent<Text>().enabled = true;
    }

    public bool IsReady()
    {
        return ready;
    }

    public void GameOver()
    {
        Debug.Log("Game over");
    }
}
