using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public GameObject Player;
    private Vector3 offset = new Vector3(-0.02f, 0.04f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        PlayerInput.Look += Aim;
    }

    // Update is called once per frame
    void Update()
    {
      //  WeaponHolder.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, WeaponHolder.transform.rotation.eulerAngles.y, transform.eulerAngles.z);
        //WeaponHolder.transform.position = Player.transform.position + WeaponHolder.transform.rotation*Vector3.up;
    }
    private void Aim(Quaternion quaternion)
    {
        gameObject.transform.localRotation = quaternion;
        gameObject.transform.position = Player.transform.position - (transform.localRotation * Vector3.up) + (transform.localRotation * offset);
    }
}
