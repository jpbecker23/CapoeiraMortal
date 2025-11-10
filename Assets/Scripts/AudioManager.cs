using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Gerenciador central de áudio do jogo.
/// Singleton que controla todos os efeitos sonoros e música de fundo.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    
    [Header("Configurações de Volume")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Clips de Áudio")]
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] dodgeSounds;
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private AudioClip[] uiSounds;
    [SerializeField] private AudioClip[] backgroundMusic;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        LoadAudioSettings();
        PlayBackgroundMusic(0); // Toca primeira música
    }
    
    private void InitializeAudioSources()
    {
        // Criar AudioSource para BGM se não existir
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            if (bgmMixerGroup != null)
                bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        }
        
        // Criar AudioSource para SFX se não existir
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            if (sfxMixerGroup != null)
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }
    
    /// <summary>
    /// Toca um efeito sonoro uma vez.
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        
        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }
    
    /// <summary>
    /// Toca um som de ataque aleatório.
    /// </summary>
    public void PlayAttackSound(int attackIndex = -1)
    {
        if (attackSounds == null || attackSounds.Length == 0) return;
        
        int index = attackIndex >= 0 && attackIndex < attackSounds.Length 
            ? attackIndex 
            : Random.Range(0, attackSounds.Length);
        
        PlaySFX(attackSounds[index]);
    }
    
    /// <summary>
    /// Toca um som de hit aleatório.
    /// </summary>
    public void PlayHitSound()
    {
        if (hitSounds == null || hitSounds.Length == 0) return;
        
        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
        PlaySFX(clip);
    }
    
    /// <summary>
    /// Toca um som de esquiva.
    /// </summary>
    public void PlayDodgeSound()
    {
        if (dodgeSounds == null || dodgeSounds.Length == 0) return;
        
        AudioClip clip = dodgeSounds[Random.Range(0, dodgeSounds.Length)];
        PlaySFX(clip);
    }
    
    /// <summary>
    /// Toca um som de morte.
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSounds == null || deathSounds.Length == 0) return;
        
        AudioClip clip = deathSounds[Random.Range(0, deathSounds.Length)];
        PlaySFX(clip, 0.8f);
    }
    
    /// <summary>
    /// Toca um som de UI.
    /// </summary>
    public void PlayUISound(int soundIndex = 0)
    {
        if (uiSounds == null || uiSounds.Length == 0) return;
        
        if (soundIndex >= 0 && soundIndex < uiSounds.Length)
        {
            PlaySFX(uiSounds[soundIndex], 0.7f);
        }
    }
    
    /// <summary>
    /// Toca música de fundo.
    /// </summary>
    public void PlayBackgroundMusic(int musicIndex)
    {
        if (backgroundMusic == null || backgroundMusic.Length == 0) return;
        if (bgmSource == null) return;
        
        if (musicIndex >= 0 && musicIndex < backgroundMusic.Length)
        {
            bgmSource.clip = backgroundMusic[musicIndex];
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }
    
    /// <summary>
    /// Para a música de fundo.
    /// </summary>
    public void StopBackgroundMusic()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }
    
    /// <summary>
    /// Define o volume master.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveAudioSettings();
    }
    
    /// <summary>
    /// Define o volume de SFX.
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveAudioSettings();
    }
    
    /// <summary>
    /// Define o volume de BGM.
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;
        SaveAudioSettings();
    }
    
    private void ApplyVolumes()
    {
        if (masterMixerGroup != null)
        {
            float db = masterVolume > 0 ? 20f * Mathf.Log10(masterVolume) : -80f;
            masterMixerGroup.audioMixer.SetFloat("MasterVolume", db);
        }
        
        if (sfxMixerGroup != null)
        {
            float db = sfxVolume > 0 ? 20f * Mathf.Log10(sfxVolume) : -80f;
            sfxMixerGroup.audioMixer.SetFloat("SFXVolume", db);
        }
        
        if (bgmMixerGroup != null)
        {
            float db = bgmVolume > 0 ? 20f * Mathf.Log10(bgmVolume) : -80f;
            bgmMixerGroup.audioMixer.SetFloat("BGMVolume", db);
        }
    }
    
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
    }
    
    private void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        ApplyVolumes();
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

