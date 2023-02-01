using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeManager : MonoBehaviour
{
    public static GrenadeManager instance;
    public Transform StartingTransform;
    public GameObject Grenade,Fruit;
    public FruitThrow FruitThrow;

    private PreviewThrow PreviewThrow;
    public Vector3 ThrowForce = (Vector3.forward * 25 + Vector3.up * 5);
    [SerializeField] private int numGrenades=3;
    [SerializeField] private bool HasFruit=false;
    public int ThrowSelect = 0;
    // Start is called before the first frame update

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

       
        FruitThrow = GetComponentInChildren<FruitThrow>();
        PreviewThrow = gameObject.AddComponent<PreviewThrow>();

        Fruit = FruitThrow.gameObject;
        Grenade = Resources.Load<GameObject>("grenade");

        PlayerInput.Throw += BeginThrow;
        PlayerInput.WeNeedToCookJesse += CookNade;
        PlayerInput.TabThrowable += ChooseThrowable;

     
        Grenade.SetActive(false);
        Fruit.SetActive(false);
    }
    private void OnDestroy()
    {
        PlayerInput.Throw -= BeginThrow;
        PlayerInput.WeNeedToCookJesse -= CookNade;
        PlayerInput.TabThrowable -= ChooseThrowable;
    }
    public void CookNade() 
    {
       
        if (ThrowSelect == 0 && numGrenades > 0)
        {
            PreviewThrow.CookNade();
            Grenade.SetActive(true);
        }
           
        else if (ThrowSelect == 1&& HasFruit == true)
        {
            PreviewThrow.CookNade();
            Fruit.SetActive(true);
        }
        transform.parent.GetComponentInChildren<ThrowableSwap>().DisplayInfo();
    }
    public void BeginThrow(Quaternion quaternion)
    {
        if (ThrowSelect == 0)
        {
            BeginThrowGrenade(quaternion);
            PreviewThrow.Release();
        }
          
        else if (ThrowSelect == 1)
        {
            BeginThrowFruit(quaternion);
            PreviewThrow.Release();
        }
        transform.parent.GetComponentInChildren<ThrowableSwap>().DisplayInfo();
    }
    public int GetNumNades()
    {
        return numGrenades;
    }
    public void BeginThrowFruit(Quaternion quaternion)
    {
        if (HasFruit)
        {
            Fruit.SetActive(true);
            FruitThrow.ThrowFruit(quaternion * ThrowForce,StartingTransform);
            HasFruit = false;
        }
    }
    public void BeginThrowGrenade(Quaternion quaternion)
    {
      if (numGrenades > 0)
        {

            GameObject nade = Instantiate(Grenade, transform.position +transform.rotation* new Vector3(0.0f, 2.5f, 1.5f),Quaternion.identity);
            nade.GetComponent<GrenadeThrow>().ThowGrenade(quaternion * ThrowForce);
            numGrenades--;
        }
       
    }
    // Update is called once per frame
    public void GainGrenades()
    {
        numGrenades++;
    }
   public void GainGrenades(int num)
    {
        numGrenades += num;
    }
    public void SetGrenades(int num)
    {
        numGrenades = num;
    }
    public void ChooseThrowable()
    {
        ThrowSelect++;
        if (ThrowSelect > 1)
            ThrowSelect = 0;
    }
    public bool GetHasFruit()
    {
        return HasFruit;
    }
    public Transform GetFruitStart()
    {
        return StartingTransform;
    }
    public void SetHasFruit(bool foo,Transform transform)
    {
        if (foo == true)
        {
            StartingTransform = transform;
        }
        HasFruit = foo;
    }
}
