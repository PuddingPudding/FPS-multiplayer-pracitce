﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class FPSPlayerAnim : NetworkBehaviour
{
    [SerializeField] RuntimeAnimatorController m_pistolAnim, m_machineGunAnim;
    //AnimatorController宣告成runtime版本的話似乎就能在執行時去做替換動作

    private Animator m_anim;
    private NetworkAnimator m_networkAnim; //Trigger類動畫參數似乎無法透過直接勾選達到同步效果

    private readonly string MOVE = "Move";
    private readonly string VELOCITY_Y = "VelocityY";
    private readonly string CROUCH = "Crouch";
    private readonly string CROUCH_WALK = "CrouchWalk";

    private readonly string STAND_SHOOT = "StandShoot";
    private readonly string CROUCH_SHOOT = "CrouchShoot";
    private readonly string RELOAD = "Reload";

    void Start()
    {
        this.m_anim = this.GetComponent<Animator>();
        this.m_networkAnim = this.GetComponent<NetworkAnimator>();
    }

    public void Movement(float _fMagnitude)
    {
        this.m_anim.SetFloat(this.MOVE, _fMagnitude);
    }

    public void PlayerJump(float _fVelocity)
    {
        this.m_anim.SetFloat(this.VELOCITY_Y, _fVelocity);
    }

    public void PlayerCrouch(bool _bIsCrouching)
    {
        this.m_anim.SetBool(this.CROUCH, _bIsCrouching);
    }

    public void PlayerCrouchWalk(float _fMagnitude)
    {
        this.m_anim.SetFloat(this.CROUCH_WALK, _fMagnitude);
    }

    public void Shoot(bool _bIsStanding)
    {
        if(_bIsStanding)
        {
            this.m_anim.SetTrigger(this.STAND_SHOOT);
            this.m_networkAnim.SetTrigger(this.STAND_SHOOT);
        }
        else
        {
            this.m_anim.SetTrigger(this.CROUCH_SHOOT);
            this.m_networkAnim.SetTrigger(this.CROUCH_SHOOT);
        }
    }

    public void ReloadGun()
    {
        this.m_anim.SetTrigger(this.RELOAD);
        this.m_networkAnim.SetTrigger(this.RELOAD);
    }

    public void ChangeController(bool _bIsPistol)
    {
        if(_bIsPistol)
        {
            this.m_anim.runtimeAnimatorController = this.m_pistolAnim;
        }
        else
        {
            this.m_anim.runtimeAnimatorController = this.m_machineGunAnim;
        }
    }
}
