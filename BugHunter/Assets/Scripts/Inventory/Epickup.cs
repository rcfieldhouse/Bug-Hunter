using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epickup : MonoBehaviour
{
    // this script is attached to collectable items and adds an item to the list when colliding with the player
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float PickupRange;
    private PlayerInput PlayerInput;
    private GameObject Player;

    //private void OnDestroy()
    //{
    //    PlayerInput.Interact -= Pickup;
    //}
    // adds the item to the inventory list then destroys it's self
    void Pickup()
    {
      if(Physics.CheckSphere(transform.position, PickupRange, whatIsPlayer)==true)
        {

           Player.GetComponent<GrenadeManager>().SetHasFruit(true,gameObject.transform);
           Player.GetComponent<GUIHolder>().PickupPrompt.SetActive(false);
           this.gameObject.SetActive(false);
        }    
    }
    public void ResetFruit()
    {
        this.gameObject.SetActive(true);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, PickupRange);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player = other.gameObject;
            PlayerInput = Player.GetComponent<PlayerInput>();
            Player.GetComponent<GUIHolder>().PickupPrompt.SetActive(true);
            PlayerInput.Interact += Pickup;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Player.GetComponent<GUIHolder>().PickupPrompt.SetActive(false);
            Player = null;
            PlayerInput.Interact -= Pickup;
            PlayerInput = null;
        }
    }
}
