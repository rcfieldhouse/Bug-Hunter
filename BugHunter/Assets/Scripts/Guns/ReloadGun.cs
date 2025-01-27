using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ReloadGun : NetworkBehaviour
{
    // Start is called before the first frame update
    private float currentGun;
    private WeaponSwap gunHolder;
    private Animator gunAnimator;
    private bool Reloading = false;
    private PlayerInput PlayerInput;
    public LeftHandReloadAnim ShotgunHandReload;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            Destroy(this);
        base.OnNetworkSpawn();
    }

    public bool GetIsReloading()
    {
        return Reloading;
    }
    public void SetIsReloading(bool var)
    { 
        Reloading = var;
    }

    void Awake()
    {
        PlayerInput = transform.parent.parent.parent.GetChild(0).GetComponent<PlayerInput>();
        PlayerInput.Reload += Reload;
        gunHolder = GetComponent<WeaponSwap>();
    }
    private void OnDestroy()
    {
        PlayerInput.Reload -= Reload;
    }
 
    public void Reload()
    {
        //this is gross 
        gunAnimator = gunHolder.WeaponArray[gunHolder.GetWeaponNum()].GetComponent<Animator>();
        if (gunAnimator != null && gunHolder.WeaponArray[gunHolder.GetWeaponNum()].GetComponent<WeaponInfo>().GetCanReload()==true)
        {
            if (gunHolder.WeaponArray[gunHolder.GetWeaponNum()].GetComponent<Shotgun>() != null && gunHolder.WeaponArray[gunHolder.GetWeaponNum()].GetComponent<WeaponInfo>()._IsPrimaryWeapon == true)
                ShotgunHandReload.SetIsReloading(true);


            gunAnimator.SetBool("Reload", true);
            //gunAnimator.Play("Reload", 0, 0);
            Invoke(nameof(Wait), 0.1f);
        }
    }
    private void Wait()
    {
        gunAnimator.SetBool("Reload", false);
    }
}
