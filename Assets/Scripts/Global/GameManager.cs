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

    public Transform startPlayerPosition;
    public float introTime;
    public float timeToQuit;
    public float timeToLeave;
    public Color targetAtmosphereColor;
    public Color targetLightColor;

    [System.Serializable]
    public class WinConditionsSettings
    {
        [Range(0, 1)]
        public float minEntityKindCountPercentage;
        [Range(0, 1)]
        public float minKnowledgeProportion;
        [Range(0, 1)]
        public float minPeopleProportion;
        [Range(0, 1)]
        public float minNatureProportion;
        [Range(0, 1)]
        public float minArtProportion;
    }
    public WinConditionsSettings winConditionSettings;

    [System.Serializable]
    public class WinTexts
    {
        public string lost;
        public string win;
        public string tooFewKnowledge;
        public string tooFewPeople;
        public string tooFewNature;
        public string tooFewArt;
        public string enoughKnowledge;
        public string enoughPeople;
        public string enoughNature;
        public string enoughArt;
    }
    public WinTexts winTexts;

    private Player player;
    private Entity[] entities;
    private Planet planet;
    private Asteroid asteroid;
    private Camera mainCamera;
    private Light sunlight;
    private List<Entity> loadedEntities;

    private Image introPanel;
    private Text titleText;
    private Text introText;
    private Text creditsText;
    private Text endText1;
    private Text endText2;
    private Text endText3;
    private Text endText4;
    private Text endText5;
    private GameObject takeOrLeaveUI;
    private Text firstOptionText;
    private Text secondOptionText;
    private Text descriptionText;
    private GameObject takeSelector;
    private GameObject leaveSelector;

    private float introTimer;
    private float quitTimer;
    private float leaveTimer;
    private bool ready;
    private bool pressedStart;
    private bool leaving;
    private bool canRestart;
    private bool hasRestarted;
    private Color startAtmosphereColor;
    private Color startLightColor;
    private Color fadeQuitColor;
    private Dictionary<EntityKind, int> totalEntityKindCount;


    void Start ()
    {
#if !UNITY_EDITOR
        Cursor.visible = false;
#endif
        Init();
    }

    private void Init()
    {
        player = FindObjectOfType<Player>();
        entities = FindObjectsOfType<Entity>();
        planet = FindObjectOfType<Planet>();
        asteroid = FindObjectOfType<Asteroid>();
        sunlight = FindObjectOfType<Light>();
        loadedEntities = new List<Entity>();

        Transform introUI = GameObject.Find("MenuCanvas").transform.Find("IntroUI").transform;
        introPanel = introUI.Find("Panel").GetComponent<Image>();
        introPanel.enabled = true;
        titleText = introUI.Find("Title").GetComponent<Text>();
        titleText.enabled = true;
        introText = introUI.Find("Intro").GetComponent<Text>();
        introText.enabled = false;
        creditsText = introUI.Find("Credits").GetComponent<Text>();
        creditsText.enabled = true;
        endText1 = introUI.Find("EndText1").GetComponent<Text>();
        endText1.enabled = false;
        endText2 = introUI.Find("EndText2").GetComponent<Text>();
        endText2.enabled = false;
        endText3 = introUI.Find("EndText3").GetComponent<Text>();
        endText3.enabled = false;
        endText4 = introUI.Find("EndText4").GetComponent<Text>();
        endText4.enabled = false;
        endText5 = introUI.Find("EndText5").GetComponent<Text>();
        endText5.enabled = false;
        takeOrLeaveUI = GameObject.Find("InGameCanvas").transform.Find("TakeOrLeaveUI").gameObject;
        firstOptionText = takeOrLeaveUI.transform.Find("FirstOptionText").GetComponent<Text>();
        secondOptionText = takeOrLeaveUI.transform.Find("SecondOptionText").GetComponent<Text>();
        descriptionText = takeOrLeaveUI.transform.Find("DescriptionText").GetComponent<Text>();
        takeSelector = takeOrLeaveUI.transform.Find("FirstOptionSelector").gameObject;
        leaveSelector = takeOrLeaveUI.transform.Find("SecondOptionSelector").gameObject;
        takeOrLeaveUI.gameObject.SetActive(false);

        planet.Init();
        asteroid.Init(planet);
        player.Init(planet, startPlayerPosition.position);
        for (int i = 0; i < entities.Length; i++)
        {
            entities[i].Init(player);
        }

        mainCamera = player.GetCamera();
        introTimer = 0;
        quitTimer = 0;
        leaveTimer = 0;
        ready = false;
        pressedStart = false;
        leaving = false;
        canRestart = false;
        hasRestarted = true;
        startAtmosphereColor = mainCamera.backgroundColor;
        startLightColor = sunlight.color;
        totalEntityKindCount = new Dictionary<EntityKind, int>();
        totalEntityKindCount.Add(EntityKind.Neutral, 0);
        totalEntityKindCount.Add(EntityKind.Knowledge, 0);
        totalEntityKindCount.Add(EntityKind.People, 0);
        totalEntityKindCount.Add(EntityKind.Nature, 0);
        totalEntityKindCount.Add(EntityKind.Art, 0);
        for (int i = 0; i < entities.Length; i++)
        {
            totalEntityKindCount[entities[i].kind] = totalEntityKindCount[entities[i].kind] + 1;
        }
        Debug.Log("Total neutral entities: " + totalEntityKindCount[EntityKind.Neutral]);
        Debug.Log("Total knowledge entities: " + totalEntityKindCount[EntityKind.Knowledge]);
        Debug.Log("Total people entities: " + totalEntityKindCount[EntityKind.People]);
        Debug.Log("Total nature entities: " + totalEntityKindCount[EntityKind.Nature]);
        Debug.Log("Total art entities: " + totalEntityKindCount[EntityKind.Art]);
    }

    void Update ()
    {
        if (ready)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                quitTimer = 0;
                introPanel.enabled = true;
            }
            if (Input.GetButton("Cancel"))
            {
                introPanel.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), quitTimer / timeToQuit);
                fadeQuitColor = introPanel.color;
                quitTimer += Time.deltaTime;
                if (quitTimer >= timeToQuit)
                {
                    Application.Quit();
                }
            }
            else if (quitTimer > 0)
            {
                introPanel.color = Color.Lerp(new Color(0, 0, 0, 0), fadeQuitColor, quitTimer / timeToQuit);
                quitTimer -= Time.deltaTime;
            }
            else
            {
                introPanel.enabled = false;
            }

            mainCamera.backgroundColor = Color.Lerp(targetAtmosphereColor, startAtmosphereColor, GetAsteroidDistanceToPlanetNormalized());
            sunlight.color = Color.Lerp(targetLightColor, startLightColor, GetAsteroidDistanceToPlanetNormalized());
        }
        else
        {
            if (leaving)
            {
                introPanel.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), leaveTimer / timeToLeave);
                leaveTimer += Time.deltaTime;
                if (leaveTimer >= timeToLeave)
                {
                    leaving = false;
                    StartCoroutine(ShowEndScreen());
                }
            }
            else if (canRestart)
            {
                if (Input.anyKeyDown)
                {
                    Init();
                }
            }
            else if (hasRestarted && Input.anyKeyDown)
            {
                pressedStart = true;
                hasRestarted = false;
                titleText.enabled = false;
                creditsText.enabled = false;

                introText.enabled = true;
            }

            if (pressedStart)
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
                    pressedStart = false;
                }
            }
        }
    }

    private IEnumerator ShowEndScreen()
    {
        yield return new WaitForSeconds(Random.Range(1, 2));

        endText1.enabled = true;

        yield return new WaitForSeconds(Random.Range(1, 2));

        endText2.enabled = true;

        yield return new WaitForSeconds(Random.Range(1, 2));

        endText3.enabled = true;

        yield return new WaitForSeconds(Random.Range(1, 2));

        endText4.enabled = true;

        yield return new WaitForSeconds(Random.Range(1, 2));

        endText5.enabled = true;

        canRestart = true;

        Debug.Log("End");
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

        ready = false;
        leaving = true;
        leaveTimer = 0;
        introPanel.enabled = true;

        float knowledgeScore = 0,
            peopleScore = 0,
            natureScore = 0,
            artScore = 0;
        for (int i = 0; i < loadedEntities.Count; i++)
        {
            switch(loadedEntities[i].kind)
            {
                case EntityKind.Knowledge:
                    knowledgeScore++;
                    break;
                case EntityKind.People:
                    peopleScore++;
                    break;
                case EntityKind.Nature:
                    natureScore++;
                    break;
                case EntityKind.Art:
                    artScore++;
                    break;
            }
        }
        knowledgeScore = knowledgeScore / totalEntityKindCount[EntityKind.Knowledge];
        peopleScore = peopleScore / totalEntityKindCount[EntityKind.People];
        natureScore = natureScore / totalEntityKindCount[EntityKind.Nature];
        artScore = artScore / totalEntityKindCount[EntityKind.Art];
        float totalScore = knowledgeScore + peopleScore + natureScore + artScore,
            knowLedgeProportion = knowledgeScore / 4f,
            peopleProportion = peopleScore / 4f,
            natureProportion = natureScore / 4f,
            artProportion = artScore / 4f;
        Debug.Log("Knowledge score: " + knowledgeScore);
        Debug.Log("People score: " + peopleScore);
        Debug.Log("Nature score: " + natureScore);
        Debug.Log("Art score: " + artScore);
        Debug.Log("Total score: " + totalScore);

        List<WinType> winTypes = new List<WinType>();
        if (knowledgeScore < winConditionSettings.minEntityKindCountPercentage || knowLedgeProportion < winConditionSettings.minKnowledgeProportion)
        {
            winTypes.Add(WinType.TooFewKnowledge);
        }
        else
        {
            winTypes.Add(WinType.EnoughKnowledge);
        }

        if (peopleScore < winConditionSettings.minEntityKindCountPercentage || peopleProportion < winConditionSettings.minPeopleProportion)
        {
            winTypes.Add(WinType.TooFewPeople);
        }
        else
        {
            winTypes.Add(WinType.EnoughPeople);
        }

        if (natureScore < winConditionSettings.minEntityKindCountPercentage || natureProportion < winConditionSettings.minNatureProportion)
        {
            winTypes.Add(WinType.TooFewNature);
        }
        else
        {
            winTypes.Add(WinType.EnoughNature);
        }

        if (artScore < winConditionSettings.minEntityKindCountPercentage || artProportion < winConditionSettings.minArtProportion)
        {
            winTypes.Add(WinType.TooFewArt);
        }
        else
        {
            winTypes.Add(WinType.EnoughArt);
        }

        // Lost
        string[] texts = new string[5];
        if (winTypes.Contains(WinType.TooFewKnowledge) || winTypes.Contains(WinType.TooFewPeople) || winTypes.Contains(WinType.TooFewNature) || winTypes.Contains(WinType.TooFewArt))
        {
            texts = new string[winTypes.Count + 1];
            for (int i = 0; i < winTypes.Count; i++)
            {
                switch (winTypes[i])
                {
                    case WinType.TooFewKnowledge:
                        texts[i] = winTexts.tooFewKnowledge;
                        break;
                    case WinType.TooFewPeople:
                        texts[i] = winTexts.tooFewPeople;
                        break;
                    case WinType.TooFewNature:
                        texts[i] = winTexts.tooFewNature;
                        break;
                    case WinType.TooFewArt:
                        texts[i] = winTexts.tooFewArt;
                        break;
                    case WinType.EnoughKnowledge:
                        texts[i] = winTexts.enoughKnowledge;
                        break;
                    case WinType.EnoughPeople:
                        texts[i] = winTexts.enoughPeople;
                        break;
                    case WinType.EnoughNature:
                        texts[i] = winTexts.enoughNature;
                        break;
                    case WinType.EnoughArt:
                        texts[i] = winTexts.enoughArt;
                        break;
                }
            }
            texts[4] = winTexts.lost;
        }
        else // Win
        {
            texts[0] = winTexts.win;

            if (knowLedgeProportion > peopleProportion && knowLedgeProportion > natureProportion && knowLedgeProportion > artProportion) // First Knowledge
            {
                texts[1] += "Through labor and ingenuity.";
                if (peopleProportion > natureProportion && peopleProportion > artProportion) // Second People
                {
                    texts[2] += "Together as one.";
                    if (natureProportion > artProportion) // Third Nature
                    {
                        texts[3] += "You held on to your faith.";
                    }
                    else if (artProportion > natureProportion) // Third Art
                    {
                        texts[3] += "You dived into the unknown.";
                    }
                }
                else if (natureProportion > peopleProportion && natureProportion > artProportion) // Second Nature
                {
                    texts[2] += "Knowing you had it in you.";
                    if (peopleProportion > artProportion) // Third People
                    {
                        texts[3] += "You stood up as one.";
                    }
                    else if (artProportion > peopleProportion) // Third Art
                    {
                        texts[3] += "You never lost hope.";
                    }
                }
                else if (artProportion > peopleProportion && artProportion > natureProportion) // Second Art
                {
                    texts[2] += "Holding on to your dreams.";
                    if (peopleProportion > natureProportion) // Third People
                    {
                        texts[3] += "You were legion.";
                    }
                    else if (natureProportion > peopleProportion) // Third Nature
                    {
                        texts[3] += "You knew deep down.";
                    }
                }
            }
            else if (peopleProportion > knowLedgeProportion && peopleProportion > natureProportion && peopleProportion > artProportion) // First People
            {
                texts[1] += "Because you had each other.";
                if (knowLedgeProportion > natureProportion && knowLedgeProportion > artProportion) // Second Knowledge
                {
                    texts[2] += "Knowing the dangers ahead.";
                    if (natureProportion > artProportion) // Third Nature
                    {
                        texts[3] += "You stayed strong.";
                    }
                    else if (artProportion > natureProportion) // Third Art
                    {
                        texts[3] += "You kept looking.";
                    }
                }
                else if (natureProportion > knowLedgeProportion && natureProportion > artProportion) // Second Nature
                {
                    texts[2] += "With faith and strength.";
                    if (knowLedgeProportion > artProportion) // Third Knowledge
                    {
                        texts[3] += "You knew the way.";
                    }
                    else if (artProportion > knowLedgeProportion) // Third Art
                    {
                        texts[3] += "You never stopped searching.";
                    }
                }
                else if (artProportion > knowLedgeProportion && artProportion > natureProportion) // Second Art
                {
                    texts[2] += "Spirits leading the way.";
                    if (knowLedgeProportion > natureProportion) // Third Knowledge
                    {
                        texts[3] += "You are free.";
                    }
                    else if (natureProportion > knowLedgeProportion) // Third Nature
                    {
                        texts[3] += "You're all there is.";
                    }
                }
            }
            else if (natureProportion > knowLedgeProportion && natureProportion > peopleProportion && natureProportion > artProportion) // First Nature
            {
                texts[1] += "Confident that you would remember.";
                if (knowLedgeProportion > peopleProportion && knowLedgeProportion > artProportion) // Second Knowledge
                {
                    texts[2] += "Knowing all the answers.";
                    if (peopleProportion > artProportion) // Third People
                    {
                        texts[3] += "You could stay united.";
                    }
                    else if (artProportion > peopleProportion) // Third Art
                    {
                        texts[3] += "You kept trying.";
                    }
                }
                else if (peopleProportion > knowLedgeProportion && peopleProportion > artProportion) // Second People
                {
                    texts[2] += "Sure to be surrounded.";
                    if (knowLedgeProportion > artProportion) // Third Knowledge
                    {
                        texts[3] += "You could forge the future.";
                    }
                    else if (artProportion > knowLedgeProportion) // Third Art
                    {
                        texts[3] += "You could choose.";
                    }
                }
                else if (artProportion > knowLedgeProportion && artProportion > peopleProportion) // Second Art
                {
                    texts[2] += "Revived through hope.";
                    if (knowledgeScore > peopleProportion) // Third Knowledge
                    {
                        texts[3] += "You were certain of what's ahead.";
                    }
                    else if (peopleProportion > knowLedgeProportion) // Third People
                    {
                        texts[3] += "You held on to each other.";
                    }
                }
            }
            else if (artProportion > knowLedgeProportion && artProportion > peopleProportion && artProportion > natureProportion) // First Art
            {
                texts[1] += "Through your hopes and dreams.";
                if (knowLedgeProportion > peopleProportion && knowLedgeProportion > natureProportion) // Second Knowledge
                {
                    texts[2] += "Knowing you could do it all.";
                    if (peopleProportion > natureProportion) // Third People
                    {
                        texts[3] += "You remained united.";
                    }
                    else if (natureProportion > peopleProportion) // Third Nature
                    {
                        texts[3] += "You kept believing.";
                    }
                }
                else if (peopleProportion > knowLedgeProportion && peopleProportion > natureProportion) // Second People
                {
                    texts[2] += "Moving forward hand in hand.";
                    if (knowLedgeProportion > natureProportion) // Third Knowledge
                    {
                        texts[3] += "You knew your worth.";
                    }
                    else if (natureProportion > knowLedgeProportion) // Third Nature
                    {
                        texts[3] += "You never let go.";
                    }
                }
                else if (natureProportion > knowLedgeProportion && natureProportion > peopleProportion) // Second Nature
                {
                    texts[2] += "Believing that you had a purpose.";
                    if (knowLedgeProportion > peopleProportion) // Third Knowledge
                    {
                        texts[3] += "You realized you could do anything.";
                    }
                    else if (peopleProportion > knowLedgeProportion) // Third People
                    {
                        texts[3] += "You clung to one another.";
                    }
                }
            }
        }

        endText1.text += texts[0] + "\n";
        endText2.text += texts[1] + "\n";
        endText3.text += texts[2] + "\n";
        endText4.text += texts[3] + "\n";
        endText5.text += texts[4] + "\n";
    }

    public float GetAsteroidDistanceToPlanetNormalized()
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
