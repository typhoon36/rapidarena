using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어를 직접 조종
public class Player_Ctrl : Base_Ctrl
{
    #region Camera
    Cam_Ctrl m_CamCtrl;
    #endregion

    #region KeyBoard
    float h, v = 0;
    float m_MoveVel = 5f;
    float m_RunVel = 10f;

    CharacterController m_CharCtrl;
    Vector3 m_MoveDir = Vector3.zero;
    Vector3 m_Velocity = Vector3.zero; // 추가된 변수
    #endregion

    #region Jump    
    float m_Gravity = -10f;
    float m_JumpForce = 2f;
    bool m_IsGrounded;
    #endregion

    #region Sound
    [Header("Audio Clip")]
    [SerializeField] private AudioClip m_WalkSound;
    [SerializeField] private AudioClip m_RunSound;
    AudioSource m_ASource;
    #endregion

    #region HP
    //HP들만 있는것은 UI 매니저가 관리하기에
    float m_MaxHP = 440;
    float m_CurHP = 440;
    #endregion

    Weapon_Base m_GunBase;

    #region ChangeWepaon
    [SerializeField] private Weapon_Base[] m_weapons;
    private Weapon_Base currentWeapon;
    #endregion

    void Awake()
    {
        #region FPS Cameara
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        m_CamCtrl = GetComponent<Cam_Ctrl>();
        #endregion

        #region Init
        m_CharCtrl = GetComponent<CharacterController>();
        m_Anim = GetComponentInChildren<Animator>();

        m_ASource = GetComponent<AudioSource>();

        m_GunBase = GetComponentInChildren<AsGun_Ctrl>();
        #endregion
    }

    void Start()
    {
        // 모든 무기를 비활성화
        foreach (var weapon in m_weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        if (m_weapons.Length > 0)
        {
            // 첫 번째 무기(기본 나이프) 활성화
            currentWeapon = m_weapons[2]; 
            currentWeapon.gameObject.SetActive(true);
            UI_Mgr.Inst.SetWeapon(currentWeapon);
        }
        UI_Mgr.Inst.UpdateHPBar(m_CurHP, m_MaxHP);

    }

    void Update()
    {
        // 키보드 움직임
        KeyMovement();
        // 점프
        Jump();

        // 카메라
        UpdateRot();

        #region 총기 적용
        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.StartWAtt(); 
        }
        else if (Input.GetMouseButton(0) && currentWeapon.WeaponSetting.IsAutoAttack)
        {
            currentWeapon.StartWAtt();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            currentWeapon.StopWeaponAction();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            currentWeapon.StartWAtt(1);
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

        #endregion

        #region 총기 교체
        // 마우스 휠로 무기 변경
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
            ChangeWeapon(m_weapons[CurIdx]);
        }

        #endregion

        m_Velocity.y += m_Gravity * Time.deltaTime;

        // 캐릭터 컨트롤러에 이동 적용
        m_CharCtrl.Move((m_MoveDir + m_Velocity) * Time.deltaTime);
        m_IsGrounded = m_CharCtrl.isGrounded;

        if (m_IsGrounded && m_Velocity.y < 0)
            m_Velocity.y = -2f;

        // 공중에 있을 때 Idle 상태로 전환
        if (!m_IsGrounded && DefState != State.DefState.Idle)
        {
            DefState = State.DefState.Idle;
            PlaySound(null, false);
        }

        // 플레이어가 멈췄을 때 Idle 상태로 전환
        if (m_IsGrounded && m_MoveDir == Vector3.zero && DefState != State.DefState.Idle)
        {
            DefState = State.DefState.Idle;
            PlaySound(null, false);
        }

        // 피격 테스트용 코드 (실제 게임에서는 적의 공격 로직에서 호출)
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(50);
        }

    }

    #region Camera 회전 업데이트
    void UpdateRot()
    {
        float a_MouseX = Input.GetAxis("Mouse X");
        float a_MouseY = Input.GetAxis("Mouse Y");

        m_CamCtrl.UpdateRot(a_MouseX, a_MouseY);
    }
    #endregion

    #region 키보드 움직임
    void KeyMovement()
    {
        //# 기본 움직임 
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 a_Dir = new Vector3(h, 0, v);
        a_Dir = transform.TransformDirection(a_Dir);

        if (h == 0 && v == 0)
        {
            if (DefState != State.DefState.Idle && DefState != State.DefState.Inspect && DefState != State.DefState.Reload)
            {
                DefState = State.DefState.Idle;
                PlaySound(null, false); // Idle 상태에서는 소리를 재생하지 않음
            }
        }
        else
        {
            //# 달리기(상태 확인후 속도 적용)
            if (m_IsGrounded && Input.GetKey(KeyCode.LeftShift))
            {
                a_Dir *= m_RunVel;
                if (DefState != State.DefState.Run)
                {
                    DefState = State.DefState.Run;
                    PlaySound(m_RunSound, true);
                }
            }
            //# 걷기상태시
            else if (m_IsGrounded)
            {
                a_Dir *= m_MoveVel;
                if (DefState != State.DefState.Walk)
                {
                    DefState = State.DefState.Walk;
                    PlaySound(m_WalkSound, true);
                }
            }
        }

        //# 캐릭터 컨트롤러에 적용
        m_MoveDir.x = a_Dir.x;
        m_MoveDir.z = a_Dir.z;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && m_IsGrounded)
        {
            m_Velocity.y = Mathf.Sqrt(m_JumpForce * -2f * m_Gravity);
            DefState = State.DefState.Jump;
        }
    }
    #endregion

    #region Audio
    void PlaySound(AudioClip clip, bool loop)
    {
        if (m_ASource == null) return;

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
    #endregion

    #region 총기교체
    public void ChangeWeapon(Weapon_Base newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);
        UI_Mgr.Inst.SetWeapon(currentWeapon);

        // 무기 변경 후 애니메이션 상태 업데이트
        UpdateAnimationState();
    }




    #endregion

    #region Damage
    public void TakeDamage(float damage)
    {
        m_CurHP -= damage;
        if (m_CurHP < 0) m_CurHP = 0;

        UI_Mgr.Inst.ShowDamagePanel();
        UI_Mgr.Inst.UpdateHPBar(m_CurHP, m_MaxHP);

        DefState = State.DefState.Damaged;
    }
    #endregion

    #region ToggleIsAttMode
    public void ToggleIsAttackMode()
    {
        currentWeapon.WeaponSetting.IsAutoAttack = !currentWeapon.WeaponSetting.IsAutoAttack;
        UI_Mgr.Inst.UpdateGunModeText(currentWeapon.WeaponType, currentWeapon.WeaponSetting.IsAutoAttack);
    }
    #endregion

}
