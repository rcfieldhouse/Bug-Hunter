using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRecoil : MonoBehaviour
{
    public GameObject Player;
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private bool _isAiming = false;

    [SerializeField]private float RecoilX, AimRecoilX;
    [SerializeField]private float RecoilY, AimRecoilY;
    [SerializeField]private float RecoilZ, AimRecoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;
    private float AimCorrection=0,baseAim=0,num=0;
    private bool RecoilStartPossible = true;
   private bool HasAmmo = true,_CanShoot=true;
    private PlayerInput PlayerInput;
    public WeaponSwap WeaponSwap;
    // Start is called before the first frame update
    private void SetAdsRecoil(Vector3 vec)
    {
       AimRecoilX = -vec.x;
       AimRecoilY = vec.y;
       AimRecoilZ = vec.z;
    }
    private void SetHipRecoil(Vector3 vec)
    {
        RecoilX = -vec.x;
        RecoilY = vec.y;
        RecoilZ = vec.z;
    }
    void Awake()
    {
        PlayerInput= transform.parent.parent.GetChild(0).GetComponent<PlayerInput>();
        PlayerInput.Shoot += RecoilStart;
        PlayerInput.Chamber += setRecoilPossible;
        PlayerInput.ADS += SetIsAiming;

        WeaponSwap.maginfo += getIfMagHasAmmo;
        WeaponSwap.CanShoot += CanShoot;

        WeaponSwap.BroadCastADSRecoil += SetAdsRecoil;
        WeaponSwap.BroadCastHipRecoil += SetHipRecoil;
        WeaponSwap.BroadcastSnap += SetSnap;
        Player = transform.parent.parent.GetChild(0).gameObject; 
    }
    private void OnDestroy()
    {
        PlayerInput.Shoot -= RecoilStart;
        PlayerInput.Chamber -= setRecoilPossible;
        PlayerInput.ADS -= SetIsAiming;

        WeaponSwap.maginfo -= getIfMagHasAmmo;
        WeaponSwap.CanShoot -= CanShoot;

        WeaponSwap.BroadCastADSRecoil -= SetAdsRecoil;
        WeaponSwap.BroadCastHipRecoil -= SetHipRecoil;
        WeaponSwap.BroadcastSnap -= SetSnap;
    }
    private void CanShoot(bool var)
    {
        _CanShoot = var;
    }
    private void getIfMagHasAmmo(bool var)
    {
        HasAmmo = var;
    }
    private void setRecoilPossible()
    {
        RecoilStartPossible = true;
    }
    // Update is called once per frame

    private void SetIsAiming(bool aiming)
    {
        _isAiming = aiming;
        //Debug.Log("Isaiming" + _isAiming);
    }

    private void SetSnap(Vector2 vec)
    {
        snappiness = vec.x ;
        returnSpeed = vec.y;
    }
    void Update()
    {
        if (WeaponSwap.GetWeapon().transform.GetComponent<WeaponInfo>().GetIsReloading() == true) _CanShoot = false;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        if (targetRotation.x > -0.1f)
        {
            AimCorrection = 0;
            targetRotation = Vector3.zero;
            currentRotation = Vector3.zero;
            baseAim = 0;
            RecoilStartPossible = true;
        }
        AimCorrection = Mathf.Lerp(AimCorrection, 0.0f, returnSpeed * Time.deltaTime);

        Player.GetComponent<PlayerInput>().MouseInput.y += -AimCorrection * returnSpeed* Time.deltaTime;
     
    }

    public void RecoilStart()
    {
        if (HasAmmo == true&& _CanShoot==true) { 

        if (RecoilStartPossible==true)
        {
            RecoilStartPossible = false;
            baseAim = transform.parent.GetComponent<Transform>().rotation.eulerAngles.x;
            if (baseAim > 90)
                baseAim = baseAim - 360.0f;
        }
        if (targetRotation != Vector3.zero)
        {
            num = transform.parent.GetComponent<Transform>().rotation.eulerAngles.x;
            if (num > 90)
                num = num - 360.0f;
            AimCorrection = baseAim - num;
        }
           
        if (_isAiming==false)
        targetRotation += new Vector3(RecoilX, Random.Range(-RecoilY, RecoilY), Random.Range(-RecoilZ, RecoilZ));

        if(_isAiming == true)
        targetRotation += new Vector3(AimRecoilX, Random.Range(-AimRecoilY, AimRecoilY), Random.Range(-AimRecoilZ, AimRecoilZ));

            targetRotation = new Vector3(targetRotation.x,targetRotation.y, targetRotation.z);
            Debug.Log(currentRotation.magnitude > new Vector3(AimRecoilX, Random.Range(-AimRecoilY, AimRecoilY), Random.Range(-AimRecoilZ, AimRecoilZ)).magnitude) ;
        }
    }
}
