using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AutomaticRifle : Gun
{
    public bool _IsAssaultRifle = false, _IsAutoPistol = false;
    public override void WeaponUpgrades(int lvl)
    {
        if (_IsAssaultRifle)
            switch (lvl)
            {
                case 1:
                    GetComponent<WeaponInfo>().ReloadTimer = new WaitForSeconds(GetComponent<WeaponInfo>()._reloadTimer *= 0.7f);
                    break;
                case 2:
                    GetComponent<WeaponInfo>().magSize += 10;
                    break;
                case 3:
                    GetComponent<WeaponInfo>().RecoilX *= 0.5f;
                    GetComponent<WeaponInfo>().AimRecoilX *= 0.5f;
                    GetComponent<WeaponInfo>().RecoilY *= 0.5f;
                    GetComponent<WeaponInfo>().AimRecoilY *= 0.5f;
                    break;

            }
        else if (_IsAutoPistol)
            switch (lvl)
            {
                case 1:
                    GetComponent<WeaponInfo>().RecoilX *= 0.7f;
                    GetComponent<WeaponInfo>().AimRecoilX *= 0.7f;
                    GetComponent<WeaponInfo>().RecoilY *= 0.7f;
                    GetComponent<WeaponInfo>().AimRecoilY *= 0.7f;
                    break;
                case 2:
                    GetComponent<WeaponInfo>().ReloadTimer = new WaitForSeconds(GetComponent<WeaponInfo>()._reloadTimer *= 0.7f);
                    break;
                case 3:
                    CritMultiplier *= 1.15f;
                    break;

            }
    }
    private bool _IsShooting = false;
    // Start is called before the first frame update
    private void Awake()
    {
       // Debug.Log("gun is awake");

        Player = transform.parent.parent.parent.parent.GetChild(0).GetComponent<PlayerInput>();
        HitMarkers = transform.parent.parent.parent.parent.GetComponentInChildren<GUIHolder>().HitMarkers;
        info = GetComponent<WeaponInfo>();
        LazerLine = GetComponent<LineRenderer>();
        MuzzleFlash = GetComponentInChildren<ParticleSystem>();
        Camera = transform.parent.GetComponentInParent<Camera>();
        GunEnd = MuzzleFlash.transform;
        Player.Shoot += SetShootingTrue;
        Player.Shoot += Shoot;
        Player.ADS += AIM;
        Player.Chamber += SetShootingFalse;
        Player.Sprinting += SetIsSprinting;
    }
    private void OnDestroy()
    {
        Player.Shoot -= SetShootingTrue;
        Player.Shoot -= Shoot;
        Player.ADS -= AIM;
        Player.Chamber -= SetShootingFalse;
    }
    private void SetShootingTrue()
    {
       _IsShooting = true;
    }
    private void SetShootingFalse()
    {
        _IsShooting = false;
    }
    public override void Shoot()
    {
        base.Shoot();
        if (info.GetCanShoot() == true && gameObject.activeInHierarchy == true && info._isReloading == false&&_IsSprinting==false)
        {
            //For hipfire spray
            Vector3 Spread;
            Spread.x = RandomGaussian(-ShotSpread.x, ShotSpread.x);
            Spread.y = RandomGaussian(-ShotSpread.y, ShotSpread.y);
            Spread.z = 0.0f;
            //for ads accuracy
            if (_IsAiming == true)
                Spread *= (1 - ADS_Accuracy);
            //gun info
            info.SetCanShoot(false);
            info.SetBulletCount();
            NextFire = Time.time + FireRate;

            GameObject Shell = Instantiate(AmmoCasingPrefab);
            Shell.transform.position = CasingEjectPoint.position;
            Shell.GetComponent<Rigidbody>().velocity = Player.gameObject.GetComponent<Rigidbody>().velocity;
            Shell.transform.rotation = this.transform.rotation;
            Shell.transform.Rotate(Vector3.up, -90.0f);
            Shell.GetComponent<Rigidbody>().AddForce(CasingEjectPoint.transform.rotation * Vector3.right * 10.0f, ForceMode.Impulse);
            Shell.GetComponent<Rigidbody>().AddForce(CasingEjectPoint.transform.rotation*Vector3.right*10.0f, ForceMode.Impulse);

            //Bullet raycast
            Vector3 RayOrigin = Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit Hit;

            StartCoroutine(ShotEffect());
            if (Physics.Raycast(RayOrigin, Camera.transform.forward * WeaponRange + Spread, out Hit, 500))
            {
                //Damage
                LazerLine.SetPosition(1, Hit.point);
                HealthSystem Health = FindHealth(Hit.collider);
              
                DoDamage(Health, Hit.collider.isTrigger, Hit.point, Hit);
               
                if (Hit.rigidbody != null)
                    Hit.rigidbody.AddForce(-Hit.normal * HitForce);

            }
            else
                LazerLine.SetPosition(1, RayOrigin + (Camera.transform.forward * WeaponRange) + Spread);
        }
    }
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }

        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
    public override void Update()
    {
        if (_IsShooting == true&&_IsSprinting==false)
            Player.AutomaticBandAid();

        base.Update();
    }
}
