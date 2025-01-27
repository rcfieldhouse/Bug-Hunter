using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssaultIcons : MonoBehaviour
{
    [SerializeField]
    private Image ClassAbility;
     private float fillTime = 5.1f,StoppedTime;
    bool Stopped = false;
    

    // Start is called before the first frame update
    void Awake()
    {
        ClassAbility.fillAmount = 1.0f;
        transform.GetChild(0).gameObject.SetActive(false);
        Dodge.Dodged+=UseDodge;
        fillTime = 5.1f;
    }
    private void OnEnable()
    {
        StopAllCoroutines();
        if (Stopped == true)
        {
            StopAllCoroutines();
            fillTime += (Time.time - StoppedTime);
            StartCoroutine(RechargeAbility(5.0f));
        }
    }
    private void OnDisable()
    {
        if (fillTime < 5)
        {
            Stopped = true;
            StoppedTime = Time.time;
        }
        else Stopped = false;
    }
    public void SetIcon()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        Dodge.Dodged -= UseDodge;
    }
    void UseDodge(float Amount)
    {
        fillTime = 0;
        transform.GetChild(0).gameObject.SetActive(true);
        ClassAbility.fillAmount = 0.0f;
        StartCoroutine(RechargeAbility(Amount)); 
    }
    private IEnumerator RechargeAbility(float RechargeTime)
    {
        while (fillTime < RechargeTime)
        {
            fillTime+=Time.deltaTime;
            ClassAbility.fillAmount = Mathf.Lerp(0.0f,1.0f,fillTime/RechargeTime);
            yield return null;
        }
      
    }
}
