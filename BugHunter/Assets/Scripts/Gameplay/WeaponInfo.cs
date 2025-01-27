using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class WeaponInfo : MonoBehaviour
{
    //this script has data for weapons and is used to change data within classes that need the data from the weapons
    [Range(0, 1)] [SerializeField] [Tooltip("How fast the gun reaches the peak of it's recoil")]
    private float RecoilRotIntensity;

    [Range(0, 1)]
    [SerializeField]
    [Tooltip("How fast the gun reaches the peak of it's recoil")]
    private float RecoilOffsetIntensity;

    [Range(0, 1)]
    [SerializeField]
    [Tooltip("How fast the gun reaches the peak of it's recoil")]
    private float Weight;
    [HideInInspector] public float RecoilTimer;

    [SerializeField]
    FMODUnity.EventReference reloadSound;


    [SerializeField] [Tooltip("Ammo in the current clip")] 
    private int ammoInMag;

    [SerializeField] [Tooltip("Maximum Possible Ammo In a clip")] 
    private int maxAmmo;

    [SerializeField] [Tooltip("Ammo in the current Reserve")] 
    public int magSize = 1;

   

    [SerializeField] [Tooltip("Maximum Possible Ammo that can be in the reserve clip")] 
    private int reserveAmmo = 1;
    [Range(0, 2)]
    [Tooltip("Time taken to Aim Down Sights")]
    public float ADSTime = 0;

    [Range(0, 2)]
    [Tooltip("Time taken to Aim Down Sights")]
    public float EquipTime = 0;
    [Range(0, 5)] [SerializeField] [Tooltip("Camera FOV Zoom Amount When Aiming Down Sight")] 
    private float AdsZoomScale=0;
       [Range(0, 5)] [SerializeField] [Tooltip("how close weapon is to camera")] 
    private float WeaponAdsZoomScale=0;
   // public static Action<bool> maginfo,CanShoot;

    [Range(0, 10)] [HideInInspector] [Tooltip("Amount of time to Reload Timer")] 
    public WaitForSeconds ReloadTimer= new WaitForSeconds(1.0f);

    [Range(0, 10)] [Tooltip("Amount of time to Reload Timer")] 
    public float _reloadTimer = 1.0f; 

    [Range(0,20)][SerializeField] [Tooltip("Per-Axis Recoil")] 
    public float RecoilX, AimRecoilX;
    [Range(0,20)][SerializeField] [Tooltip("Per-Axis Recoil")] 
    public float RecoilY, AimRecoilY;
    [Range(0,10)][SerializeField] [Tooltip("Per-Axis Recoil")] 
    public float RecoilZ, AimRecoilZ;
    [Range(0,10)][SerializeField] [Tooltip("How fast the gun reaches the peak of it's recoil")] 
    private float snappiness;
    [Range(0,5)][SerializeField] [Tooltip("How fast the gun centres itself after a shot")] 
    private float returnSpeed;
    [HideInInspector] public bool IsPaused=false;
    private PlayerInput PlayerInput;
    public bool _CanShoot = true, _CanReload = false, _isReloading = false, tempTimer = true;
    public bool _IsPrimaryWeapon = false;
    public bool CancelReload=false;

   public Vector3 Startpos;
   public Quaternion StartRot;
    // Start is called before the first frame update
    void Awake()
    {
        Startpos = transform.localPosition;
        StartRot = transform.localRotation;
        ReloadTimer = new WaitForSeconds(_reloadTimer);         
        ammoInMag = magSize;
       
        Invoke(nameof(Wait), 0.1f);
        if(GetComponent<Gun>()!=null)
        RecoilTimer = GetComponent<Gun>().FireRate;
    }

    private void OnEnable()
    {
        transform.localPosition = Startpos;
        transform.localRotation = StartRot;
    }
    void Wait()
    {
        PlayerInput = transform.parent.parent.parent.parent.GetComponentInChildren<PlayerInput>();
        PlayerInput.Reload += Reload;
    }
    private void OnDestroy()
    {
        if(PlayerInput)
        PlayerInput.Reload -= Reload;
    }
    public bool GetIsReloading()
    {
        return _isReloading;
    }
    public void SetIsReloading(bool var)
    {
        _isReloading = var;
    }
    public void SetCanShoot(bool CanShoot)
    {
        
        _CanShoot = CanShoot;
    }
    public bool GetCanShoot()
    {
        return _CanShoot;
    }
    public void Update()
    {
       
        if (ammoInMag <= 0) _CanShoot = false;

        if(_isReloading==true) _CanShoot = false;

        if (ammoInMag == magSize||reserveAmmo==0)
            _CanReload = false;
        else if(_isReloading==false) _CanReload = true;

        //if (gameObject.activeInHierarchy == true)
        //{
        //    maginfo.Invoke(hasAmmo());
        //    CanShoot.Invoke(GetCanShoot());
        //}
        if (_isReloading == true) _CanShoot = false;
        if (IsPaused == true)
            _CanShoot = false;

    
    }
    private void LateUpdate()
    {
        if (_isReloading == false && transform.localPosition != Startpos)
        {
            transform.localPosition = Startpos;
            transform.localRotation = StartRot;
        }
    }
    public void SetPaused(bool var)
    {
        IsPaused=var;
    }
    public void OnDisable()
    {
        StopCoroutine(SetBulletCount(true));
    }
    public bool hasAmmo()
    {
        return ammoInMag > 0;
    }
    public Vector4 GetRecoilInfo()
    {
        return new Vector4(RecoilRotIntensity, RecoilOffsetIntensity, RecoilTimer, Weight);
       
    }
    //0 for hipfire 1 for ads
    public Vector3 GetCameraRecoilInfo(int num)
    {
        if (num == 0)
        {
            return new Vector3(RecoilX, RecoilY, RecoilZ);
        }
        if (num == 1)
        {
            return new Vector3(AimRecoilX, AimRecoilY, AimRecoilZ);
        }
        return Vector3.zero;

    }
    public Vector2 GetSnap()
    {
        return new Vector2(snappiness, returnSpeed);
    }
    public float GetADSZoom()
    {
        return WeaponAdsZoomScale;
    }
    public float GetZoom()
    {
        return AdsZoomScale;
    }
   
    public void Reload()
    {
       if(GetComponent<Shotgun>() != null && _IsPrimaryWeapon == true){
            CancelReload = false;
            StartCoroutine(ReloadShotgun());
        }
       else if (_CanReload == true)
        {          
            _CanShoot = false;
            if (gameObject.activeInHierarchy == true)
                StartCoroutine(SetBulletCount(true));    
        }
       
    }
    public IEnumerator ReloadShotgun()
    {
        _isReloading = true;
        _CanReload = false;
        _CanShoot = false;
        gameObject.GetComponentInParent<ReloadGun>().SetIsReloading(true);
        yield return ReloadTimer;

            if (reserveAmmo > 1&& ammoInMag < magSize)
            {
                reserveAmmo -=1;
                ammoInMag += 1;
            }
       // _CanShoot = true;
        if (ammoInMag < magSize&&CancelReload==false)
        {
            Debug.Log("reloading shotgun");
            StopAllCoroutines();
            StartCoroutine(ReloadShotgun());           
        }
        else if (ammoInMag >= magSize||CancelReload==true)
        { 
            GetComponentInChildren<LeftHandReloadAnim>().SetIsReloading(false); 
            GetComponent<Animator>().SetBool("Reload", false);
            GetComponent<Animator>().SetBool("DoneReloading", true);
            Debug.Log("Finsished  shotgun");
        }
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool("DoneReloading", false);
        gameObject.GetComponentInParent<ReloadGun>().SetIsReloading(false);
        _isReloading = false;
        Debug.Log("reloading shotgun");

    }
    // Update is called once per frame
    //alter mag to subtract a bullet or fill it full on reload 
    public void SetBulletCount()
    {
        if (ammoInMag > 0)
            ammoInMag--;
    }
    public int GetMag()
    {
        return ammoInMag;
    }
    public int GetReserveAmmo()
    {
        return reserveAmmo;
    }
    public bool GetCanReload()
    {
        return _CanReload;
    }
    public void SetMag(int Amount)
    {
        ammoInMag = Amount;
    }
    public void SetReserveAmmo(int Amount)
    {
        reserveAmmo = Amount;
        if (reserveAmmo > maxAmmo)
        {
            reserveAmmo = maxAmmo;
        }
    }
    public int GetMaxBullets()
    {
        return maxAmmo;
    }
    public IEnumerator SetBulletCount(bool var)
    {
        _isReloading = true;
        _CanReload = false;
        _CanShoot = false;
        
        
        gameObject.GetComponentInParent<ReloadGun>().SetIsReloading(true);

        if (tempTimer)
        {
            tempTimer = false;
            FMODUnity.RuntimeManager.PlayOneShot(reloadSound);
        }
           

        yield return ReloadTimer;

        if (var)
        {

            if (reserveAmmo > magSize - ammoInMag)
            {
                reserveAmmo -= magSize - ammoInMag;
                ammoInMag = magSize;
            }
            else
            {
                ammoInMag += reserveAmmo;
                reserveAmmo = 0;
            }

        }
      
        _CanShoot = true;


        
        yield return new WaitForSeconds(0.5f);
        tempTimer = true;
        gameObject.GetComponentInParent<ReloadGun>().SetIsReloading(false);
        _isReloading = false;

    }
    public void SetMaxBullets()
    {
        reserveAmmo = maxAmmo;
    }
}