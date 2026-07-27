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

    // 클립별 중첩 재생 제어용
    private Dictionary<AudioClip, int> playCountInWindow = new Dictionary<AudioClip, int>();
    private Dictionary<AudioClip, float> windowStartTime = new Dictionary<AudioClip, float>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    /// <summary>
    /// 같은 클립이 짧은 시간(windowDuration) 안에 maxOverlap번까지만 겹쳐 재생되도록 제한.
    /// 그 이상 호출되면 소리 없이 무시됨(효과 자체는 그대로 적용됨).
    /// </summary>
    private void PlayLimited(AudioClip clip, float volume, int maxOverlap, float windowDuration = 0.12f)
    {
        if (clip == null)
            return;

        float now = Time.time;

        // 윈도우가 없거나 만료됐으면 새로 시작
        if (!windowStartTime.TryGetValue(clip, out float startTime) || now - startTime > windowDuration)
        {
            windowStartTime[clip] = now;
            playCountInWindow[clip] = 0;
        }

        int count = playCountInWindow[clip];

        if (count >= maxOverlap)
            return; // 이미 3번 겹쳤으면 더 이상 재생 안 함

        playCountInWindow[clip] = count + 1;
        sfxSource.PlayOneShot(clip, volume);
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
        //sfxSource.PlayOneShot(wetClip, 1f);      // 최대 3번까지만 중첩 재생되도록 수정
        PlayLimited(wetClip, 1f, 3);
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