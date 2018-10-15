using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(FPSPlayerAnim))]
public class FPSController : NetworkBehaviour
{
    [SerializeField] private float m_fWalkSpeed = 6.75f;
    [SerializeField] private float m_fRunSpeed = 10;
    [SerializeField] private float m_fCrouchSpeed = 4;
    [SerializeField] private float m_fJumpSpeed = 8;
    [SerializeField] private float m_fGravity = 20;
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private WeaponManager m_weaponManager;
    [SerializeField] private WeaponManager m_handsWeaponManager;
    [SerializeField] private GameObject m_playerHolder, m_weaponsHolder;
    [SerializeField] private GameObject[] m_arrWeaponsFPS;
    [SerializeField] private FPSMouse[] m_arrMouseLook;
    [SerializeField] private Camera m_mainCam;

    private FPSWeapon m_currentWeapon;
    private FPSHandsWeapon m_curHandsWeapon;
    private float m_fFireRate = 15f;
    private float m_fNextTimeToFire = 0f;

    private float m_fSpeed;
    private bool m_bIsMoving, m_bIsGrounded, m_bIsCrouching;
    private float m_fInputX, m_fInputY;
    private float m_fInputXSet, m_fInputYSet;
    private float m_fInputModifyFactor;
    private bool m_bLimitDiagonalSpeed = true;
    private float m_fAnitiBumpFactor = 0.75f;

    private Transform m_firstPersonView;
    private Transform m_firstPersonCameara;
    private Vector3 m_firstPersonViewRotation;
    private CharacterController m_charController;
    private Vector3 m_moveDir = Vector3.zero;

    private float m_fRayDis;
    private float m_fDefaultControllerHeight;
    private Vector3 m_defaulCamPos;
    private float m_fCamHeight;

    private FPSPlayerAnim m_playerAnim;

    // Use this for initialization
    void Start()
    {
        this.m_firstPersonView = this.transform.Find("FPS View").transform;
        //transform.Find會從自己的遊戲物件子列表中去尋找對應的物件，這個方法會比GameObject.find還要快
        this.m_charController = this.GetComponent<CharacterController>();
        this.m_fSpeed = this.m_fWalkSpeed;
        this.m_bIsMoving = false;

        this.m_fRayDis = this.m_charController.height + this.m_charController.radius;
        print("射線距離 " + this.m_fRayDis);
        this.m_fDefaultControllerHeight = this.m_charController.height;
        this.m_defaulCamPos = this.m_firstPersonView.localPosition;
        this.m_playerAnim = this.GetComponent<FPSPlayerAnim>();

        this.m_weaponManager.Weapons[0].SetActive(true);
        this.m_currentWeapon = this.m_weaponManager.Weapons[0].GetComponent<FPSWeapon>();

        this.m_handsWeaponManager.Weapons[0].SetActive(true);
        this.m_curHandsWeapon = this.m_handsWeaponManager.Weapons[0].GetComponent<FPSHandsWeapon>();

        if(this.isLocalPlayer)
        {
            this.m_playerHolder.layer = LayerMask.NameToLayer("Player");
            foreach(Transform child in m_playerHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            for(int i = 0; i<m_arrWeaponsFPS.Length; i++)
            {
                m_arrWeaponsFPS[i].layer = LayerMask.NameToLayer("Player");
            }
            m_weaponsHolder.layer = LayerMask.NameToLayer("Enemy");
            foreach(Transform child in m_weaponsHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
        //if (!this.isLocalPlayer)
        else
        {
            this.m_playerHolder.layer = LayerMask.NameToLayer("Enemy");
            foreach (Transform child in m_playerHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            for (int i = 0; i < m_arrWeaponsFPS.Length; i++)
            {
                m_arrWeaponsFPS[i].layer = LayerMask.NameToLayer("Enemy");
            }
            m_weaponsHolder.layer = LayerMask.NameToLayer("Player");
            foreach (Transform child in m_weaponsHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            for(int i = 0; i < m_arrMouseLook.Length; i++)
            {
                m_arrMouseLook[i].enabled = false;
            }
            this.m_mainCam.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //如果我們不是該玩家
        if(this.isLocalPlayer)
        {
            this.PlayerMovement();
            this.SelectWeapon();
        }        
    }

    private void PlayerMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.m_fInputYSet = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.m_fInputYSet = -1;
        }
        else
        {
            this.m_fInputYSet = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.m_fInputXSet = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.m_fInputXSet = 1;
        }
        else
        {
            this.m_fInputXSet = 0;
        }

        this.m_fInputY = Mathf.Lerp(this.m_fInputY, this.m_fInputYSet, Time.deltaTime * 19);
        this.m_fInputX = Mathf.Lerp(this.m_fInputX, this.m_fInputXSet, Time.deltaTime * 19);
        if (this.m_fInputXSet != 0 && this.m_fInputYSet != 0 && this.m_bLimitDiagonalSpeed)
        {
            this.m_fInputModifyFactor = Mathf.Lerp(this.m_fInputModifyFactor, 0.75f, Time.deltaTime * 19);
        }
        else
        {
            this.m_fInputModifyFactor = Mathf.Lerp(this.m_fInputModifyFactor, 1, Time.deltaTime * 19);
        }

        this.m_firstPersonViewRotation = Vector3.Lerp(this.m_firstPersonViewRotation, Vector3.zero, Time.deltaTime * 5);
        //if(this.m_firstPersonViewRotation.magnitude > 0)
        //{
        //    print("FPSView之旋轉角度為 " + this.m_firstPersonViewRotation);
        //} //2018/9/30經實測以後發現上下這兩行其實沒啥用(也許只是暫時)
        this.m_firstPersonView.localEulerAngles = this.m_firstPersonViewRotation;
        //上面這兩行我他媽有點看不懂

        if (this.m_bIsGrounded)
        {
            this.PlayerCrouchingAndSprinting();
            this.m_moveDir = new Vector3(this.m_fInputX * this.m_fInputModifyFactor, -this.m_fAnitiBumpFactor
                , this.m_fInputY * this.m_fInputModifyFactor);
            this.m_moveDir = transform.TransformDirection(this.m_moveDir) * this.m_fSpeed;
            this.PlayerJump();
        }
        this.m_moveDir.y -= this.m_fGravity * Time.deltaTime;
        this.m_bIsGrounded = (this.m_charController.Move(this.m_moveDir * Time.deltaTime) & CollisionFlags.Below) != 0;
        //檢測底下有沒有碰撞

        this.m_bIsMoving = this.m_charController.velocity.magnitude > 0.15f;

        this.HandleAnimations();
    }

    void PlayerCrouchingAndSprinting()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!this.m_bIsCrouching)
            {
                this.m_bIsCrouching = true;
                this.m_playerAnim.PlayerCrouch(this.m_bIsCrouching);
            }
            else
            {
                if (this.CanGetUp())
                {
                    this.m_bIsCrouching = false;
                    this.m_playerAnim.PlayerCrouch(this.m_bIsCrouching);
                }
            }
            StopCoroutine(MoveCamCrouch());
            StartCoroutine(MoveCamCrouch());
        }


        if (this.m_bIsCrouching)
        {
            this.m_fSpeed = this.m_fCrouchSpeed;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                this.m_fSpeed = this.m_fRunSpeed;
            }
            else
            {
                this.m_fSpeed = this.m_fWalkSpeed;
            }
        }
    }

    bool CanGetUp()
    {
        bool bOutput = true;
        Ray groundRay = new Ray(this.transform.position, this.transform.up); //向上的射線
        RaycastHit groundHit;

        if (Physics.SphereCast(groundRay, this.m_charController.radius + 0.05f
            , out groundHit, this.m_fRayDis, this.m_groundLayer))
        {//假如向上打出射線，有含括到地形類別的東西的話，那就看成無法起身
            //SphereCast的意思有點像是製造一個圓柱體射線
            print("站不起來啦幹");
            if (Vector3.Distance(this.transform.position, groundHit.point) < 2.3f)
            {

                bOutput = false;
            }
        }

        return bOutput;
    }

    IEnumerator MoveCamCrouch()
    {
        if (this.m_bIsCrouching)
        {
            this.m_charController.height = this.m_fDefaultControllerHeight / 1.5f;
            this.m_fCamHeight = this.m_defaulCamPos.y / 1.5f;
        }
        else
        {
            this.m_charController.height = this.m_fDefaultControllerHeight;
            this.m_fCamHeight = this.m_defaulCamPos.y;
        }
        this.m_charController.center = new Vector3(0, this.m_charController.height / 2, 0);

        while (Mathf.Abs(this.m_fCamHeight - this.m_firstPersonView.localPosition.y) > 0.01f)
        { //上面會設定說新的攝影機位置應該在哪，而這邊則會檢測說假如實際攝影機
          //跟新位置還有差距的話，就會用Lerp的方式漸漸移過去
            this.m_firstPersonView.localPosition = Vector3.Lerp(this.m_firstPersonView.localPosition
                , new Vector3(this.m_defaulCamPos.x, this.m_fCamHeight, this.m_defaulCamPos.z)
                , Time.deltaTime * 11);

            yield return null;
        }
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.m_bIsCrouching)
            {
                if (this.CanGetUp())
                {
                    this.m_bIsCrouching = false;
                    this.m_playerAnim.PlayerCrouch(this.m_bIsCrouching);
                    StopCoroutine(MoveCamCrouch());
                    StartCoroutine(MoveCamCrouch());
                }
            }
            else
            {
                this.m_moveDir.y = this.m_fJumpSpeed;
            }
        }
    }
    void HandleAnimations()
    {
        this.m_playerAnim.Movement(this.m_charController.velocity.magnitude);
        this.m_playerAnim.PlayerJump(this.m_charController.velocity.y);

        if (this.m_bIsCrouching && this.m_charController.velocity.magnitude > 0)
        {
            this.m_playerAnim.PlayerCrouchWalk(this.m_charController.velocity.magnitude);
        }
        //this.m_playerAnim.PlayerCrouch(this.m_bIsCrouching); //原本想說每一禎呼叫，後來改掉
        //this.m_playerAnim.PlayerCrouchWalk(this.m_charController.velocity.magnitude);

        if(Input.GetMouseButtonDown(0) && Time.time > this.m_fNextTimeToFire)
        {
            this.m_fNextTimeToFire = Time.time + 1f / this.m_fFireRate;
            if(!this.m_bIsCrouching) //如果不是蹲著(也就是站著或跳躍)
            {
                this.m_playerAnim.Shoot(true);
            }
            else
            {
                this.m_playerAnim.Shoot(false);
            }
            this.m_currentWeapon.Shoot();
            this.m_curHandsWeapon.Shoot();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            this.m_playerAnim.ReloadGun();
            this.m_curHandsWeapon.Reload();
        }
    }

    void SelectWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(!m_handsWeaponManager.Weapons[0].activeInHierarchy)
            {
                for(int i = 0; i < m_handsWeaponManager.Weapons.Count; i++)
                {
                    this.m_handsWeaponManager.Weapons[i].SetActive(false);
                }
                this.m_curHandsWeapon = null;
                this.m_handsWeaponManager.Weapons[0].SetActive(true);
                this.m_curHandsWeapon = this.m_handsWeaponManager.Weapons[0].GetComponent<FPSHandsWeapon>();
            }

            if (!this.m_weaponManager.Weapons[0].activeInHierarchy)
            {
                for(int i = 0; i<this.m_weaponManager.Weapons.Count; i++)
                {
                    this.m_weaponManager.Weapons[i].SetActive(false);
                }
                this.m_currentWeapon = null;
                this.m_weaponManager.Weapons[0].SetActive(true);
                this.m_currentWeapon = this.m_weaponManager.Weapons[0].GetComponent<FPSWeapon>();
                this.m_playerAnim.ChangeController(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!m_handsWeaponManager.Weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < m_handsWeaponManager.Weapons.Count; i++)
                {
                    this.m_handsWeaponManager.Weapons[i].SetActive(false);
                }
                this.m_curHandsWeapon = null;
                this.m_handsWeaponManager.Weapons[1].SetActive(true);
                this.m_curHandsWeapon = this.m_handsWeaponManager.Weapons[1].GetComponent<FPSHandsWeapon>();
            }

            if (!this.m_weaponManager.Weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < this.m_weaponManager.Weapons.Count; i++)
                {
                    this.m_weaponManager.Weapons[i].SetActive(false);
                }
                this.m_currentWeapon = null;
                this.m_weaponManager.Weapons[1].SetActive(true);
                this.m_currentWeapon = this.m_weaponManager.Weapons[1].GetComponent<FPSWeapon>();
                this.m_playerAnim.ChangeController(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!m_handsWeaponManager.Weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < m_handsWeaponManager.Weapons.Count; i++)
                {
                    this.m_handsWeaponManager.Weapons[i].SetActive(false);
                }
                this.m_curHandsWeapon = null;
                this.m_handsWeaponManager.Weapons[2].SetActive(true);
                this.m_curHandsWeapon = this.m_handsWeaponManager.Weapons[2].GetComponent<FPSHandsWeapon>();
            }

            if (!this.m_weaponManager.Weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < this.m_weaponManager.Weapons.Count; i++)
                {
                    this.m_weaponManager.Weapons[i].SetActive(false);
                }
                this.m_currentWeapon = null;
                this.m_weaponManager.Weapons[2].SetActive(true);
                this.m_currentWeapon = this.m_weaponManager.Weapons[2].GetComponent<FPSWeapon>();
                this.m_playerAnim.ChangeController(false);
            }
        }
    }
}
