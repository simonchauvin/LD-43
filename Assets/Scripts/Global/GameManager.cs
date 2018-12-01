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

    public float introTime;
    public Color targetAtmosphereColor;
    public Color targetLightColor;

    private Player player;
    private Entity[] entities;
    private Planet planet;
    private Asteroid asteroid;
    private Camera mainCamera;
    private Light sunlight;
    private List<Entity> loadedEntities;

    private Transform introUI;
    private Image introPanel;
    private Text introText;
    private GameObject takeOrLeaveUI;
    private Text firstOptionText;
    private Text secondOptionText;
    private Text descriptionText;
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
        sunlight = FindObjectOfType<Light>();
        loadedEntities = new List<Entity>();

        introUI = GameObject.Find("MenuCanvas").transform.Find("IntroUI").transform;
        introPanel = introUI.Find("Panel").GetComponent<Image>();
        introText = introUI.Find("Text").GetComponent<Text>();
        introUI.gameObject.SetActive(true);
        takeOrLeaveUI = GameObject.Find("InGameCanvas").transform.Find("TakeOrLeaveUI").gameObject;
        firstOptionText = takeOrLeaveUI.transform.Find("FirstOptionText").GetComponent<Text>();
        secondOptionText = takeOrLeaveUI.transform.Find("SecondOptionText").GetComponent<Text>();
        descriptionText = takeOrLeaveUI.transform.Find("DescriptionText").GetComponent<Text>();
        takeSelector = takeOrLeaveUI.transform.Find("FirstOptionSelector").gameObject;
        leaveSelector = takeOrLeaveUI.transform.Find("SecondOptionSelector").gameObject;
        takeOrLeaveUI.gameObject.SetActive(false);

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
        startLightColor = sunlight.color;
        startAsteroidDistance = (asteroid.transform.position - planet.transform.position).sqrMagnitude;
    }
	
	void Update ()
    {
        if (ready)
        {
            mainCamera.backgroundColor = Color.Lerp(targetAtmosphereColor, startAtmosphereColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance);
            sunlight.color = Color.Lerp(targetLightColor, startLightColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance);
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
    }

    public void ShowTakeOrLeaveUI(string firstOption, string secondOption, string description)
    {
        if (!takeOrLeaveUI.activeSelf)
        {
            takeOrLeaveUI.SetActive(true);
            descriptionText.GetComponent<Text>().text = description;
            firstOptionText.text = firstOption;
            secondOptionText.text = secondOption;
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

    public void LeavePlanet()
    {
        Debug.Log("Leave planet now");
    }

    public float GetAsteroidDistanceToPlanet()
    {
        return asteroid.GetDistanceToPlanetNormalized();
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
