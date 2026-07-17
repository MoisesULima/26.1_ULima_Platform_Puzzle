using System.Collections;
using UnityEngine;

/// <summary>
/// Controla el shader de disolución (SpriteDissolve) en tiempo real.
/// Permite ajustar los valores desde el Inspector y disparar la
/// animación completa del "snap" con Dissolve() o Reappear().
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDissolveController : MonoBehaviour
{
    [Header("Valores del shader (editables en tiempo real)")]
    [SerializeField, Range(0f, 1f)]
    private float dissolveAmount = 0f;

    [SerializeField]
    private float noiseScale = 40f;

    [SerializeField, Range(0f, 0.2f)]
    private float edgeWidth = 0.05f;

    [SerializeField, ColorUsage(true, true)] // true, true = permite HDR
    private Color edgeColor = new Color(1f, 0.4f, 0f, 1f) * 3f;

    [Header("Animación del snap")]
    [SerializeField, Tooltip("Duración total de la disolución en segundos")]
    private float dissolveDuration = 2f;

    [SerializeField, Tooltip("Curva de la animación (por defecto, ease in-out)")]
    private AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField, Tooltip("Partículas de polvo que se disparan al iniciar (opcional)")]
    private ParticleSystem dustParticles;

    [SerializeField, Tooltip("Destruir el GameObject al terminar la disolución")]
    private bool destroyOnComplete = true;

    // IDs de las propiedades del Shader Graph (deben coincidir con las References)
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int NoiseScaleID     = Shader.PropertyToID("_NoiseScale");
    private static readonly int EdgeWidthID      = Shader.PropertyToID("_EdgeWidth");
    private static readonly int EdgeColorID      = Shader.PropertyToID("_EdgeColor");

    private SpriteRenderer spriteRenderer;
    private Material materialInstance;
    private Coroutine currentAnimation;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // .material crea una instancia propia: cada personaje se disuelve
        // de forma independiente sin afectar al material compartido.
        materialInstance = spriteRenderer.material;
        ApplyAllValues();
    }

    private void Update()
    {
        // Aplica los valores cada frame para que los cambios del Inspector
        // se vean en tiempo real durante el Play Mode.
        ApplyAllValues();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Refleja los cambios del Inspector también en Edit Mode.
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // En Edit Mode usamos sharedMaterial para no generar instancias sueltas.
        if (!Application.isPlaying && spriteRenderer.sharedMaterial != null)
        {
            spriteRenderer.sharedMaterial.SetFloat(DissolveAmountID, dissolveAmount);
            spriteRenderer.sharedMaterial.SetFloat(NoiseScaleID, noiseScale);
            spriteRenderer.sharedMaterial.SetFloat(EdgeWidthID, edgeWidth);
            spriteRenderer.sharedMaterial.SetColor(EdgeColorID, edgeColor);
        }
    }
#endif

    private void ApplyAllValues()
    {
        if (materialInstance == null) return;

        materialInstance.SetFloat(DissolveAmountID, dissolveAmount);
        materialInstance.SetFloat(NoiseScaleID, noiseScale);
        materialInstance.SetFloat(EdgeWidthID, edgeWidth);
        materialInstance.SetColor(EdgeColorID, edgeColor);
    }

    /// <summary>
    /// Dispara la disolución completa (el "snap"). Llamable desde otros
    /// scripts, un botón de UI o un UnityEvent.
    /// </summary>
    [ContextMenu("Probar Dissolve")]
    public void Dissolve()
    {
        StartAnimation(0f, 1f, onComplete: () =>
        {
            if (destroyOnComplete)
                Destroy(gameObject);
        });

        if (dustParticles != null)
            dustParticles.Play();
    }

    /// <summary>
    /// Animación inversa: el personaje se rearma desde el polvo.
    /// </summary>
    [ContextMenu("Probar Reappear")]
    public void Reappear()
    {
        if (dustParticles != null)
            dustParticles.Stop();

        StartAnimation(1f, 0f, onComplete: null);
    }

    private void StartAnimation(float from, float to, System.Action onComplete)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(AnimateDissolve(from, to, onComplete));
    }

    private IEnumerator AnimateDissolve(float from, float to, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dissolveDuration);
            dissolveAmount = Mathf.Lerp(from, to, dissolveCurve.Evaluate(t));
            yield return null;
        }

        dissolveAmount = to;
        currentAnimation = null;
        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        // Limpia la instancia del material para evitar fugas de memoria.
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}