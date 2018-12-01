using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Color startColor;
    public Color targetColor;

    private Player player;
    private Planet planet;
    private Asteroid asteroid;

    private float startAsteroidDistance;


    void Start ()
    {
        player = FindObjectOfType<Player>();
        planet = FindObjectOfType<Planet>();
        asteroid = FindObjectOfType<Asteroid>();

        planet.Init();
        asteroid.Init(planet.transform.position);
        player.Init(planet);

        startAsteroidDistance = (asteroid.transform.position - planet.transform.position).sqrMagnitude;
    }
	
	void Update ()
    {
        //Debug.Log(Color.Lerp(startColor, targetColor, (asteroid.transform.position - planet.transform.position).sqrMagnitude / startAsteroidDistance));
    }

    public void GameOver()
    {
        Debug.Log("Game over");
    }
}
