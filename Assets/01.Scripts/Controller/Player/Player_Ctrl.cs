using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;



public class Player_Ctrl : Base_Ctrl
{
    [HideInInspector] public PhotonView pv = null;

    public Transform m_CamPos;

    #region KeyBoard
    float h, v = 0;
    float m_MoveVel = 5f;
    Vector3 m_MoveDir = Vector3.zero;
    float m_RunVel = 10f;
    Vector3 m_Velocity = Vector3.zero;
    CharacterController m_CharCtrl;
    #endregion

    #region Jump    
    float m_Gravity = -10f;
    float m_JumpForce = 2f;
    bool IsGround;
    #endregion

    #region Sound
    [Header("Audio Clip")]
    [SerializeField] private AudioClip m_WalkSound;
    [SerializeField] private AudioClip m_RunSound;
    AudioSource m_ASource;
    #endregion

    #region References
    Weapon_Base m_GunBase;
    GameState m_GameState;
    Cam_Ctrl m_CamCtrl;
    StickyBomb m_Sbomb;
    #endregion

    #region ChangeWeapon
    [SerializeField] private Weapon_Base[] m_weapons;
    private Weapon_Base currentWeapon;
    #endregion



    void Awake()
    {
        pv = GetComponent<PhotonView>();
        m_CharCtrl = GetComponent<CharacterController>();
        m_Anim = GetComponentInChildren<Animator>();
        m_ASource = GetComponent<AudioSource>();
        m_GunBase = GetComponentInChildren<Weapon_Base>();
        m_CamCtrl = GetComponent<Cam_Ctrl>();
        m_Sbomb = GetComponentInChildren<StickyBomb>();
    }

    void Start()
    {

        foreach (var weapon in m_weapons)
            weapon.gameObject.SetActive(false);

        if (m_weapons.Length > 0)
        {
            currentWeapon = m_weapons[0];
            currentWeapon.gameObject.SetActive(true);
            Game_Mgr.Inst.SetWeapon(currentWeapon);
        }

        if (pv.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = m_CamPos;
            Camera.main.GetComponent<Cam_Ctrl>().Init(gameObject);
        }

    }

    void Update()
    {
        if (Ready_Mgr.m_GameState == GameState.Ready) return;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null)
            return;

        if (!pv.IsMine)
            return;

        if (pv.IsMine)
        {
            KeyMovement();
            Jump();
            RotateCam();
            IsChange();
            HandleWeaponActions();

            m_Velocity.y += m_Gravity * Time.deltaTime;
            m_CharCtrl.Move((m_MoveDir + m_Velocity) * Time.deltaTime);
            IsGround = m_CharCtrl.isGrounded;

            if (IsGround && m_Velocity.y < 0)
                m_Velocity.y = -2f;

            if (!IsGround && PlayerState != DefState.Idle)
            {
                PlayerState = DefState.Idle;
                PlaySound(null, false);
            }

            if (IsGround && m_MoveDir == Vector3.zero && PlayerState != DefState.Idle)
            {
                PlayerState = DefState.Idle;
                PlaySound(null, false);
            }

            if (Input.GetKeyDown(KeyCode.G) && isInArea)
            {
                m_Sbomb.SpawnBomb();
                Game_Mgr.Inst.ShowMessage(5f, true);
            }
        }
    }

    bool isInArea = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Area"))
        {
            isInArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Area"))
        {
            isInArea = false;
        }
    }

    void KeyMovement()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 a_Dir = new Vector3(h, 0, v);
        a_Dir = transform.TransformDirection(a_Dir);

        if (h == 0 && v == 0)
        {
            if (PlayerState != DefState.Idle && PlayerState != DefState.Inspect && PlayerState != DefState.Reload)
            {
                PlayerState = DefState.Idle;
                PlaySound(null, false);
            }
        }
        else
        {
            if (IsGround && Input.GetKey(KeyCode.LeftShift))
            {
                a_Dir *= m_RunVel;
                if (PlayerState != DefState.Run)
                {
                    PlayerState = DefState.Run;
                    PlaySound(m_RunSound, true);
                }
            }
            else if (IsGround)
            {
                a_Dir *= m_MoveVel;
                if (PlayerState != DefState.Walk)
                {
                    PlayerState = DefState.Walk;
                    PlaySound(m_WalkSound, true);
                }
            }
        }

        m_MoveDir.x = a_Dir.x;
        m_MoveDir.z = a_Dir.z;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGround)
        {
            m_Velocity.y = Mathf.Sqrt(m_JumpForce * -2f * m_Gravity);
            PlayerState = DefState.Jump;
        }
    }

    void RotateCam()
    {
        // 마우스 입력에 따라 카메라 회전 업데이트
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Camera.main.GetComponent<Cam_Ctrl>().UpdateRot(mouseX, mouseY);
    }

    void PlaySound(AudioClip clip, bool loop)
    {
        if (m_ASource.isPlaying)
        {
            m_ASource.Stop();
        }

        if (clip != null)
        {
            m_ASource.clip = clip;
            m_ASource.loop = loop;
            m_ASource.Play();
        }
    }

    public void C_Weapon(Weapon_Base newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);
        Game_Mgr.Inst.SetWeapon(currentWeapon);

        UpdateAnimationState();
    }

    void IsChange()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int CurIdx = System.Array.IndexOf(m_weapons, currentWeapon);
            if (scroll > 0)
            {
                CurIdx = (CurIdx + 1) % m_weapons.Length;
            }
            else if (scroll < 0)
            {
                CurIdx = (CurIdx - 1 + m_weapons.Length) % m_weapons.Length;
            }
            C_Weapon(m_weapons[CurIdx]);
        }
    }

    public void ToggleIsAttackMode()
    {
        currentWeapon.WeaponSetting.IsAutoAttack = !currentWeapon.WeaponSetting.IsAutoAttack;
        Game_Mgr.Inst.UpdateGunModeText(currentWeapon.WeaponType, currentWeapon.WeaponSetting.IsAutoAttack);
    }

    void HandleWeaponActions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.StartWAtt(0, pv.Owner.ActorNumber);
        }
        else if (Input.GetMouseButton(0) && currentWeapon.WeaponSetting.IsAutoAttack)
        {
            currentWeapon.StartWAtt(0, pv.Owner.ActorNumber);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            currentWeapon.StopWeaponAction();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            currentWeapon.StartWAtt(1, pv.Owner.ActorNumber);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            currentWeapon.StopWeaponAction(1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            currentWeapon.Inspect();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleIsAttackMode();
        }
    }
}

