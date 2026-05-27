using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX")]
    [SerializeField] private AudioClip chainLightningClip;
    [SerializeField] private AudioClip burnClip;
    [SerializeField] private AudioClip hitClip;

    [SerializeField]
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlayChainLightning()
    {
        sfxSource.PlayOneShot(chainLightningClip);
    }

    public void PlayBurn()
    {
        sfxSource.PlayOneShot(burnClip);
    }

    public void PlayHit()
    {
        sfxSource.PlayOneShot(hitClip);
    }
}