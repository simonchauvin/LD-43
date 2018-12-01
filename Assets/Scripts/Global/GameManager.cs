using System.Collections;
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
    private GameObject takeOrLeaveUI;
    private GameObject takeSelector;
    private GameObject leaveSelector;

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

        canvas = FindObjectOfType<Canvas>();
        takeOrLeaveUI = canvas.transform.Find("TakeOrLeaveUI").gameObject;
        takeSelector = takeOrLeaveUI.transform.Find("TakeSelector").gameObject;
        leaveSelector = takeOrLeaveUI.transform.Find("LeaveSelector").gameObject;
        takeOrLeaveUI.SetActive(false);
        loadedEntities = new List<Entity>();

        planet.Init();
        asteroid.Init(planet.transform.position);
        player.Init(planet);
        for (int i = 0; i < entities.Length; i++)
        {
            entities[i].Init(player);
        }

        mainCamera = player.GetCamera();
        startAtmosphereColor = mainCamera.backgroundColor;
        startLightColor = asteroidLight.color;
        startAsteroidDistance = (asteroid.transform.position - planet.transform.position).sqrMagnitude;
    }
	
	void Update ()
    {
        mainCamera.backgroundColor = Color.Lerp(targetAtmosphereColor, startAtmosphereColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance);
        asteroidLight.color = Color.Lerp(targetLightColor, startLightColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance);
    }

    public void LoadEntity(Entity entity)
    {
        Debug.Log("Entity" + entity + "loaded");
        loadedEntities.Add(entity);
        entity.Take();
    }

    public void ShowTakeOrLeaveUI()
    {
        if (!takeOrLeaveUI.activeSelf)
        {
            takeOrLeaveUI.SetActive(true);
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

    public void GameOver()
    {
        Debug.Log("Game over");
    }
}
