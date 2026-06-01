using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX")]
    [SerializeField] private AudioClip chainLightningClip;
    [SerializeField] private AudioClip burnClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip normalHitClip;
    [SerializeField] private AudioClip wetClip;
    [SerializeField] private AudioClip ignoreDefenseClip;
    [SerializeField] private AudioClip pierceClip;

    [SerializeField]
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlayChainLightning()
    {
        sfxSource.PlayOneShot(chainLightningClip, 0.25f); 
    }

    public void PlayBurn()
    {
        sfxSource.PlayOneShot(burnClip, 0.4f);
    }

    public void PlayExplosion()
    {
        sfxSource.PlayOneShot(explosionClip, 0.4f);
    }

    public void PlayNormalHit()
    {
        sfxSource.PlayOneShot(normalHitClip, 3f);
    }
    
    public void PlayWet()
    {
        sfxSource.PlayOneShot(wetClip, 1f);
    }

    public void PlayIgnoreDefenseClip()
    {
        sfxSource.PlayOneShot(ignoreDefenseClip);
    }

    public void PlayPierce()
    {
        sfxSource.PlayOneShot(pierceClip);
    }
}