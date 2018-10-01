using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [SerializeField] private float m_fWalkSpeed = 6.75f;
    [SerializeField] private float m_fRunSpeed = 10;
    [SerializeField] private float m_fCrouchSpeed = 4;
    [SerializeField] private float m_fJumpSpeed = 8;
    [SerializeField] private float m_fGravity = 20;
    [SerializeField] private LayerMask m_groundLayer;

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

    // Use this for initialization
    void Awake()
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
    }

    // Update is called once per frame
    void Update()
    {
        this.PlayerMovement();
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
    }

    void PlayerCrouchingAndSprinting()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!this.m_bIsCrouching)
            {
                this.m_bIsCrouching = true;
            }
            else
            {
                if (this.CanGetUp())
                {
                    this.m_bIsCrouching = false;
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
}
