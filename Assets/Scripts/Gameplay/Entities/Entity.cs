using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string description;
    public string firstOption;
    public string secondOption;

    private Player player;


	public virtual void Init(Player player)
    {
        this.player = player;
    }
	
	public virtual void Update ()
    {
		if (GameManager.instance.IsReady())
        {

        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.Interact(this);
        }
    }

    public void Align()
    {
        transform.up = (transform.position - FindObjectOfType<Planet>().transform.position).normalized;
        transform.position = FindObjectOfType<Planet>().transform.position + (transform.position - FindObjectOfType<Planet>().transform.position).normalized * (FindObjectOfType<Planet>().GetComponent<Collider>().bounds.extents.x + GetComponent<MeshRenderer>().bounds.extents.y);
    }

    public virtual void FirstOption()
    {
        GameManager.instance.LoadEntity(this);

        GetComponent<MeshRenderer>().enabled = false;
    }

    public virtual void SecondOption()
    {

    }
}
