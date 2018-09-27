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

    private float m_fSpeed;
    private bool m_bIsMoving, m_bIsGrounded, m_bIsCruouching;
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

    // Use this for initialization
    void Awake()
    {
        this.m_firstPersonView = this.transform.Find("FPS View").transform;
        //transform.Find會從自己的遊戲物件子列表中去尋找對應的物件，這個方法會比GameObject.find還要快
        this.m_charController = this.GetComponent<CharacterController>();
        this.m_fSpeed = this.m_fWalkSpeed;
        this.m_bIsMoving = false;
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
            this.m_fInputXSet = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.m_fInputXSet = -1;
        }
        else
        {
            this.m_fInputXSet = 0;
        }

        this.m_fInputY = Mathf.Lerp(this.m_fInputY, this.m_fInputYSet, Time.deltaTime * 19);
        this.m_finputX = Mathf.Lerp(this.m_fInputX, this.m_fInputXSet, Time.deltaTime * 19);
    }
}
