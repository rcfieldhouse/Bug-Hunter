using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class RaycastGun : MonoBehaviour
{

    public int gunDamage = -25;                                            // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 0.25f;                                        // Number in seconds which controls how often the player can fire
    public float weaponRange = 50f;                                        // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                        // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun
    [SerializeField]
    public Camera fpsCam;                                                // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private AudioClip gunAudio;                                        // Reference to the audio source which will play our shooting sound effect
    private LineRenderer laserLine;
  // Reference to the LineRenderer component which will display our laserline
    private ParticleSystem muzzleFlash;
    private float nextFire;                                                // Float to store the time the player will be allowed to fire again, after firing
    private Vector3 AimSpread = new Vector3(0.0f, 0.0f,0.0f);                                                                
    [SerializeField] private bool m_IsShotgun = false;

    [SerializeField] private float maxShotGunSpread = 5.0f;
    void Start()
    {
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        muzzleFlash = GetComponentInChildren<ParticleSystem>();

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioClip>();

        // Get and store a reference to our Camera by searching this GameObject and its parents
        //fpsCam = GetComponentInParent<Camera>();
     //  if (m_IsShotgun == true) { 
     //  for (int i = 0; i < 7; i++)
     //  {
     //
     //      ShotgunLines[i] = gameObject.GetComponent<LineRenderer>();
     //  }
     //  }
    }
  
    void Update()
    {
        // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
          
            // Update the time when our player can fire next
            nextFire = Time.time + fireRate;

            // Start our ShotEffect coroutine to turn our laser line on and off
          

            // Create a vector at the center of our camera's viewport
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;

            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition(0, gunEnd.position);

            if (m_IsShotgun)
            {
                //for setting the start points
                for (int i = 2; i <=15; i+=2)
                {
                    laserLine.SetPosition(i, gunEnd.position);
                }
              
            }
           
            StartCoroutine(ShotEffect());
            // Check if our raycast has hit anything
      //for basic guns
      
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                // Set the end position for our laser line 
                laserLine.SetPosition(1, hit.point);
             
           
                // Get a reference to a health script attached to the collider we hit
                HealthSystem health = hit.collider.GetComponent<HealthSystem>();

                // If there was a health script attached
                if (health != null)
                {
                    // Call the damage function of that script, passing in our gunDamage variable
                    health.ModifyHealth(gunDamage);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null)
                {
                    // Add force to the rigidbody we hit, in the direction from which it was hit
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            }
            else
            {
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
                   
            }
            


            //for shotguns shhhh i hate it 

           if (m_IsShotgun)
            {
                for (int i = 1; i <= 15; i += 2)
                {
                    AimSpread.x = Random.Range(-maxShotGunSpread, maxShotGunSpread);
                    AimSpread.y = Random.Range(-maxShotGunSpread, maxShotGunSpread);
                    AimSpread.z = Random.Range(-maxShotGunSpread, maxShotGunSpread);
                   
              
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward * weaponRange + AimSpread, out hit, weaponRange))
                {
                    // Set the end position for our laser line 
                   // laserLine.SetPosition(1, hit.point);


                    // Get a reference to a health script attached to the collider we hit
                    HealthSystem health = hit.collider.GetComponent<HealthSystem>();

                    // If there was a health script attached
                    if (health != null)
                    {
                        // Call the damage function of that script, passing in our gunDamage variable
                        health.ModifyHealth(gunDamage);
                    }

                    // Check if the object we hit has a rigidbody attached
                    if (hit.rigidbody != null)
                    {
                        // Add force to the rigidbody we hit, in the direction from which it was hit
                        hit.rigidbody.AddForce(-hit.normal * hitForce);
                    }
                    laserLine.SetPosition(i, (hit.point) );
                }
                
                else
                {
                        // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                        //laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
                        laserLine.SetPosition(i, (rayOrigin+fpsCam.transform.forward * weaponRange) + AimSpread);
                        laserLine.SetPosition(1, (rayOrigin + fpsCam.transform.forward * weaponRange) + AimSpread);
                    }
              
              
                 
                }
            }
        }
    }

 
    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        //SoundManager.instance.PlaySound(gunAudio);

        //play Shooting Effect
        muzzleFlash.Play();
        GetComponent<AudioSource>().Play();
        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }
}