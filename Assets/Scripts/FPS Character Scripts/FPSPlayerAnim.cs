using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FPSPlayerAnim : MonoBehaviour
{
    private Animator m_anim;

    private readonly string MOVE = "Move";
    private readonly string VELOCITY_Y = "VelocityY";
    private readonly string CROUCH = "Crouch";
    private readonly string CROUCH_WALK = "CrouchWalk";

    void Awake()
    {
        this.m_anim = this.GetComponent<Animator>();
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
}
