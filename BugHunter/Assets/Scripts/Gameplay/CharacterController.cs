using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public GameObject CameraMain,CameraCrouch,CameraDodge, CameraManager,WeaponCamera,PlayerCamera;
    private bool SuspendMovement = false, OnZipline=false;
    private Transform StartPos, EndPos;
    public float FootStepTime = 0.5f;
    bool _IsOnLadder = false;
    [SerializeField] private LayerMask m_WhatIsGround;

    [Range(0, 1)][SerializeField] public float m_CrouchSpeed = 0.5f;
    [Range(0, 1)] [SerializeField] public float m_LadderSpeed = 0.5f;
    [Range(0, 1)][SerializeField] private float SpeedSlider = .5f;
    public event Action Jumped = delegate { };
    public WaitForSeconds RollTimer = new WaitForSeconds(0.75f);

    [SerializeField] public Vector3 mover,JumpForce = new Vector3 (0.0f,25.0f,0.0f);
     private bool m_Grounded = true;
    private PlayerInput Player;
    private CapsuleCollider coll;

    private HealthSystem Health;

    private void Start()
    {
        disableCams(false);
        CameraMain.SetActive(true);
     
    }
    private void Awake()
    {
        
        Player = GetComponent<PlayerInput>();
        Rigidbody = GetComponent<Rigidbody>();
        // PlayerInput.UseAbility += Dodge;
        Player.Crouching += SwitchCamCrouch;
        Player.JumpAction += Jump;
        Player.Move += Move;
        coll = GetComponent<CapsuleCollider>();
        //this line sets default cam to main
 
        disableCams(false);
        CameraMain.SetActive(true);

        Invoke(nameof(GetHealthSys),0.5f);
    }
        void GetHealthSys()
    {
        Health = GetComponent<HealthSystem>();
        Health.OnTakeDamage += HandleDamage;
    }
    private void HandleDamage(int obj)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Player_Hurt");
    }

    private void OnDestroy()
    {
        //Health.OnTakeDamage -= HandleDamage;
        Player.Crouching -= SwitchCamCrouch;
        Player.JumpAction -= Jump;
        Player.Move -= Move;
        //Health.OnTakeDamage -= HandleDamage;
    }
    public void SetIfOnLadder(bool var)
    {   
        SuspendMovement = var;
        _IsOnLadder = var;
        SwitchLadderCam(var);
        Rigidbody.velocity = Vector3.zero;
    }
    public void SetZiplinePoint(Transform Start, Transform End)
    {
        OnZipline = true; 
        StartPos = Start;
        EndPos = End;
    }
    public void ExitZipLine()
    {
        OnZipline = false;
    }
    public bool GetIfOnLadder()
    {
        return _IsOnLadder;
    }
    public GameObject[] GetCameras()
    {//this is for dodge roll fam
        EnableCams();
        return new GameObject[5] { CameraMain, CameraCrouch, CameraDodge, PlayerCamera, WeaponCamera };
    }
    //functions for delegates 
    //happy programmer noises ;)
    public void EnableCams()
    {
        CameraCrouch.SetActive(true);
        CameraDodge.SetActive(true);
        CameraMain.SetActive(true);
        WeaponCamera.SetActive(true);
    }

    public void disableCams(bool var)
    {   //set all cameras to false, if called by a function that is setting that camera to false
        //set default cam to main
        CameraCrouch.SetActive(false);
        CameraDodge.SetActive(false);
        CameraMain.SetActive(!var);
    }
  
  
    private void SwitchLadderCam(bool var)
    { 
        disableCams(var);
        CameraDodge.SetActive(var);
    }
 
    public void SetSuspendMovement(bool foo)
    {
        SuspendMovement = foo;
    }


    private void SwitchCamCrouch(bool var)
    {
        disableCams(var);
        CameraCrouch.SetActive(var);       
    }
    public void Jump()
    {
        if (isGrounded())
            Joomp();
    }
    public void Joomp()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Jump");
        GetComponent<PlayerAnimatorScript>().Jump();
        Rigidbody.velocity += JumpForce;
        if(isGrounded())Jumped.Invoke();
    }

    
    private void Move(Vector2 move, float SpeedMod)
    {
        //Perform an initial save when he player moves once
        // unfortunetly can't use Start() functions bc some Start() functions
        // are called before others which produces null references when saving

        move = move.normalized;
        if (SuspendMovement == false)
        {
            //This is cursed, do not move it.
            if (move != Vector2.zero)
                FootStepTimer(SpeedMod);

            // was 4.0f
            SpeedMod *= 20.0f;
            mover = transform.right * move.x + transform.forward * move.y;

            Rigidbody.velocity = new Vector3(mover.x * SpeedSlider * SpeedMod, Rigidbody.velocity.y, mover.z * SpeedSlider * SpeedMod);
        }      
       else if (gameObject.GetComponent<Dodge>()!=null  )
       {
            if (GetComponent<Dodge>().GetRollVector() != Vector3.zero) {
                
                Rigidbody.velocity = GetComponent<Dodge>().GetRollVector() * 12;
            }
        }
        if (_IsOnLadder == true)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Climb_Watchtower");
            SpeedMod *= 10.0f;
            // Rigidbody.velocity = Vector3.zero;
            Rigidbody.velocity = new Vector3(0.0f, SpeedMod*move.y/m_LadderSpeed, 0.0f);
        }
        if (OnZipline == true)
        {
            // Zipline.ogg
            Rigidbody.velocity = SpeedMod*Vector3.Normalize( EndPos.position- StartPos.position);
        }
        Rigidbody.angularVelocity = Vector3.zero;      
    }

    private void FootStepTimer(float speedMod)
    {
        if(FootStepTime >= 0.5)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Footsteps/Footsteps_Grass");
            FootStepTime = 0;
        }
        FootStepTime += Time.deltaTime * UnityEngine.Random.Range(0.8f, 1.3f) * speedMod;
    }

    public bool isGrounded(float x)
    {
        return Physics.Raycast(coll.bounds.center - Vector3.down / 10, Vector3.down, x, m_WhatIsGround);
    }
    // Update is called once per frame
    void Update()
    {
        GetComponent<PlayerAnimatorScript>().Land(isGrounded(3));
        //dev hack
        if (Input.GetKeyDown(KeyCode.Period))
        {
            //#if UNITY_EDITOR
            //FMODUnity.RuntimeManager.PlayOneShot("event:/Konami_Code");
            //#endif
            Rigidbody.position = new Vector3(170.0f, 25.0f, 420.0f);
        }
        gameObject.transform.rotation = Quaternion.Euler(0.0f, CameraManager.transform.eulerAngles.y, 0.0f);
    }


    public bool isGrounded()
    {
        m_Grounded = Physics.Raycast(coll.bounds.center - Vector3.down / 10, Vector3.down,2.4f, m_WhatIsGround);
        return Physics.Raycast(coll.bounds.center - Vector3.down / 10, Vector3.down, 2.4f, m_WhatIsGround);
    }
 
}
