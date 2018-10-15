using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
[RequireComponent (typeof(Animator))]
public class FPSHandsWeapon : MonoBehaviour
{
    [SerializeField] private AudioClip m_shootClip, m_reloadClip;

    private AudioSource m_audioManager;
    private GameObject m_muzzleFlash;
    private Animator m_anim;

    private string SHOOT = "Shoot";
    private string RELOAD = "Reload";

    void Start()
    {
        this.m_muzzleFlash = this.transform.Find("Muzzle Flash").gameObject;
        this.m_muzzleFlash.SetActive(false);
        this.m_audioManager = this.GetComponent<AudioSource>();
        this.m_anim = this.GetComponent<Animator>();
    }

    public void Shoot()
    {
        if(this.m_audioManager.clip != this.m_shootClip)
        {
            this.m_audioManager.clip = this.m_shootClip;
        }
        this.m_audioManager.Play();

        StartCoroutine(this.TurnMuzzleFlashOn());

        this.m_anim.SetTrigger(SHOOT);
    }

    IEnumerator TurnMuzzleFlashOn()
    {
        
        this.m_muzzleFlash.transform.localEulerAngles = new Vector3(Random.Range(0f, 360f)
        , this.m_muzzleFlash.transform.localEulerAngles.y
        , this.m_muzzleFlash.transform.localEulerAngles.z); //讓子彈火花每次出現時都隨機轉方向
        this.m_muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        this.m_muzzleFlash.SetActive(false);
    }

    public void Reload()
    {
        StartCoroutine(this.PlayReloadSound());
        this.m_anim.SetTrigger(this.RELOAD);
    }

    IEnumerator PlayReloadSound()
    {
        yield return new WaitForSeconds(0.8f);
        if(this.m_audioManager.clip != this.m_reloadClip)
        {
            this.m_audioManager.clip = this.m_reloadClip;
        }
        this.m_audioManager.Play();
    }
}
