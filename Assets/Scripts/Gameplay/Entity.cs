using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Player player;


	public void Init(Player player)
    {
        this.player = player;
    }
	
	void Update ()
    {
		
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
        transform.up = (FindObjectOfType<Planet>().transform.position - transform.position).normalized;
    }

    public void Take()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
