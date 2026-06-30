// ─── Nota de diseno ──────────────────────────────────────────────
// AudioManager es una EXCEPCION CONSCIENTE a la regla de componentes
// autonomos del proyecto. Es un singleton global persistente, un patron
// justificado solo para sistemas transversales como el audio (cualquier
// script de cualquier escena necesita disparar SFX/musica sin acoplarse).
// NO es el patron a copiar para el resto de componentes: los demas deben
// seguir siendo autonomos y comunicarse por referencias o eventos.
// ─────────────────────────────────────────────────────────────────

using System.Collections.Generic;
using UnityEngine;

// ─── Enums de audio ──────────────────────────────────────────────

/// <summary>
/// Identificadores de efectos de sonido. Usar el enum en vez de strings
/// da autocompletado y evita errores de tipeo. Ampliable segun el juego.
/// </summary>
public enum SoundEffect
{
    Jump,
    Land,
    Collect,
    PlayerDeath,
    EnemyDeath,
    ButtonClick
}

/// <summary>
/// Identificadores de pistas de musica. Ampliable segun el juego.
/// </summary>
public enum MusicTrack
{
    MainMenu,
    Level,
    Victory
}

/// <summary>
/// Singleton global y persistente entre escenas que centraliza el audio.
/// Reproduce SFX con PlayOneShot (se solapan sin cortarse) y musica con
/// play/stop/pause directo (sin fade). Mapea cada enum a su AudioClip via
/// listas serializadas en el Inspector, convertidas a Dictionary en Awake
/// para busqueda O(1). Expone control de volumen separado para SFX y musica.
/// Acceso global via AudioManager.Instance desde cualquier escena/script.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // ─── Singleton ───────────────────────────────────────────────

    /// <summary>Instancia global accesible desde cualquier script.</summary>
    public static AudioManager Instance { get; private set; }

    // ─── Tipos serializables de mapeo ────────────────────────────

    // Par enum -> clip para SFX. Editable como lista en el Inspector
    // porque Unity no serializa Dictionary directamente.
    [System.Serializable]
    private struct SoundEffectEntry
    {
        [Tooltip("Efecto de sonido al que corresponde este clip")]
        public SoundEffect effect;

        [Tooltip("Clip de audio que se reproduce para este efecto")]
        public AudioClip clip;
    }

    // Par enum -> clip para musica. Mismo motivo que SoundEffectEntry.
    [System.Serializable]
    private struct MusicTrackEntry
    {
        [Tooltip("Pista de musica a la que corresponde este clip")]
        public MusicTrack track;

        [Tooltip("Clip de audio que se reproduce para esta pista")]
        public AudioClip clip;
    }

    // ─── Configuracion en el Inspector ───────────────────────────

    [Header("Mapeo de SFX")]

    [Tooltip("Asigna un AudioClip a cada efecto de sonido del enum SoundEffect")]
    [SerializeField] private List<SoundEffectEntry> _soundEffects = new List<SoundEffectEntry>();

    [Header("Mapeo de Musica")]

    [Tooltip("Asigna un AudioClip a cada pista del enum MusicTrack")]
    [SerializeField] private List<MusicTrackEntry> _musicTracks = new List<MusicTrackEntry>();

    [Header("Volumen inicial")]

    [Tooltip("Volumen inicial de los efectos de sonido (0 a 1)")]
    [Range(0f, 1f)]
    [SerializeField] private float _sfxVolume = 1f;

    [Tooltip("Volumen inicial de la musica (0 a 1)")]
    [Range(0f, 1f)]
    [SerializeField] private float _musicVolume = 1f;

    // ─── Estado interno ──────────────────────────────────────────

    // AudioSource creados por codigo en Awake (no se configuran a mano).
    private AudioSource _sfxSource;
    private AudioSource _musicSource;

    // Diccionarios para busqueda O(1) construidos desde las listas.
    private Dictionary<SoundEffect, AudioClip> _sfxMap;
    private Dictionary<MusicTrack, AudioClip> _musicMap;

    // Pista actualmente sonando, para no reiniciarla si se vuelve a pedir.
    private bool _hasCurrentTrack;
    private MusicTrack _currentTrack;

    // ─── Ciclo de vida ───────────────────────────────────────────

    private void Awake()
    {
        // Patron singleton: si ya existe una instancia, este duplicado se destruye.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
        BuildDictionaries();
    }

    // Crea los dos AudioSource por codigo y aplica el volumen inicial.
    private void CreateAudioSources()
    {
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.volume = _sfxVolume;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.volume = _musicVolume;
    }

    // Convierte las listas serializadas en diccionarios para busqueda O(1).
    private void BuildDictionaries()
    {
        _sfxMap = new Dictionary<SoundEffect, AudioClip>(_soundEffects.Count);
        foreach (SoundEffectEntry entry in _soundEffects)
            _sfxMap[entry.effect] = entry.clip;

        _musicMap = new Dictionary<MusicTrack, AudioClip>(_musicTracks.Count);
        foreach (MusicTrackEntry entry in _musicTracks)
            _musicMap[entry.track] = entry.clip;
    }

    // ─── API publica ─────────────────────────────────────────────

    /// <summary>
    /// Reproduce un efecto de sonido con PlayOneShot, de modo que varios
    /// SFX pueden sonar a la vez sin cortarse entre si. Si el efecto no
    /// tiene clip asignado, muestra un Debug.LogWarning y no reproduce nada.
    /// </summary>
    public void PlaySFX(SoundEffect effect)
    {
        if (!_sfxMap.TryGetValue(effect, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"[AudioManager] SFX sin clip asignado: {effect}");
            return;
        }

        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    /// <summary>
    /// Reproduce una pista de musica en bucle. Si la misma pista ya esta
    /// sonando no la reinicia. Si la pista no tiene clip asignado, muestra
    /// un Debug.LogWarning y no reproduce nada.
    /// </summary>
    public void PlayMusic(MusicTrack track)
    {
        // No reinicia la pista si ya es la que esta sonando.
        if (_hasCurrentTrack && _currentTrack == track && _musicSource.isPlaying)
            return;

        if (!_musicMap.TryGetValue(track, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"[AudioManager] Musica sin clip asignado: {track}");
            return;
        }

        _musicSource.clip = clip;
        _musicSource.Play();

        _currentTrack = track;
        _hasCurrentTrack = true;
    }

    /// <summary>
    /// Detiene la musica por completo y olvida la pista actual.
    /// </summary>
    public void StopMusic()
    {
        _musicSource.Stop();
        _hasCurrentTrack = false;
    }

    /// <summary>
    /// Pausa la musica en su posicion actual sin perder la pista.
    /// </summary>
    public void PauseMusic()
    {
        _musicSource.Pause();
    }

    /// <summary>
    /// Reanuda la musica pausada desde la posicion en que se detuvo.
    /// </summary>
    public void UnPauseMusic()
    {
        _musicSource.UnPause();
    }

    /// <summary>
    /// Ajusta en caliente el volumen de los efectos de sonido (0 a 1).
    /// Pensado para sliders del menu de opciones.
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        _sfxSource.volume = _sfxVolume;
    }

    /// <summary>
    /// Ajusta en caliente el volumen de la musica (0 a 1).
    /// Pensado para sliders del menu de opciones.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        _musicSource.volume = _musicVolume;
    }
}
