using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntityKind kind;
    public string description;
    public string firstOption;
    public string secondOption;

    private Player player;
    private bool processedAlready;


	public virtual void Init(Player player)
    {
        this.player = player;
        processedAlready = false;
    }
	
	public virtual void Update ()
    {
		if (GameManager.instance.IsReady())
        {

        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !processedAlready)
        {
            player.Interact(this);
        }
    }

    public virtual void FirstOption()
    {
        GameManager.instance.LoadEntity(this);

        processedAlready = true;
    }

    public virtual void SecondOption()
    {

    }
}
