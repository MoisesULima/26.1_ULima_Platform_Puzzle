using UnityEngine;

/// <summary>
/// Controla el shader de hue swap (recoloreo selectivo) en tiempo real.
/// Permite ajustar el color objetivo, el color nuevo y los filtros desde
/// el Inspector, cambiar de "skin" por código, y mantener/volver a la
/// versión original mediante EffectStrength (0 = original, 1 = recoloreado).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class HueSwappper : MonoBehaviour
{
    [Header("Colores del recoloreo")]
    [SerializeField, Tooltip("Color a detectar en el sprite (ej. el azul del polo)")]
    private Color targetColor = new Color(0.1f, 0.3f, 0.8f, 1f);

    [SerializeField, Tooltip("Color de destino (ej. rojo)")]
    private Color newColor = new Color(0.8f, 0.1f, 0.1f, 1f);

    [Header("Filtros de selección")]
    [SerializeField, Range(0f, 0.5f)]
    private float hueTolerance = 0.1f;

    [SerializeField, Range(0f, 0.2f)]
    private float hueSmoothness = 0.05f;

    [SerializeField, Range(0f, 1f)]
    private float saturationMin = 0.3f;

    [Header("Intensidad del efecto")]
    [SerializeField, Range(0f, 1f),
     Tooltip("0 = sprite original, 1 = recoloreo completo. Requiere el nodo EffectStrength en el grafo.")]
    private float effectStrength = 1f;

    // IDs de las propiedades del Shader Graph (deben coincidir con las References)
    private static readonly int TargetColorID    = Shader.PropertyToID("_TargetColor");
    private static readonly int NewColorID        = Shader.PropertyToID("_NewColor");
    private static readonly int HueToleranceID    = Shader.PropertyToID("_HueTolerance");
    private static readonly int HueSmoothnessID   = Shader.PropertyToID("_HueSmoothness");
    private static readonly int SaturationMinID   = Shader.PropertyToID("_SaturationMin");
    private static readonly int EffectStrengthID  = Shader.PropertyToID("_EffectStrength");

    private SpriteRenderer spriteRenderer;
    private Material materialInstance;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Instancia propia: cada personaje puede tener su propio color
        // sin afectar a los demás que comparten el mismo shader.
        materialInstance = spriteRenderer.material;
        ApplyAllValues();
    }

    private void Update()
    {
        // Aplica cada frame para ver los cambios del Inspector en tiempo real.
        ApplyAllValues();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // En Edit Mode escribimos sobre el sharedMaterial para previsualizar.
        if (!Application.isPlaying && spriteRenderer.sharedMaterial != null)
            WriteTo(spriteRenderer.sharedMaterial);
    }
#endif

    private void ApplyAllValues()
    {
        if (materialInstance != null)
            WriteTo(materialInstance);
    }

    private void WriteTo(Material mat)
    {
        mat.SetColor(TargetColorID, targetColor);
        mat.SetColor(NewColorID, newColor);
        mat.SetFloat(HueToleranceID, hueTolerance);
        mat.SetFloat(HueSmoothnessID, hueSmoothness);
        mat.SetFloat(SaturationMinID, saturationMin);
        mat.SetFloat(EffectStrengthID, effectStrength);
    }

    // ---- API pública para usar desde otros scripts o UI ----

    /// <summary>Cambia el color de destino en caliente (selector de skins).</summary>
    public void SetNewColor(Color color)
    {
        newColor = color;
        effectStrength = 1f;
        ApplyAllValues();
    }

    /// <summary>Vuelve al sprite original sin quitar el material.</summary>
    [ContextMenu("Mostrar original")]
    public void ShowOriginal()
    {
        effectStrength = 0f;
        ApplyAllValues();
    }

    /// <summary>Reactiva el recoloreo.</summary>
    [ContextMenu("Mostrar recoloreado")]
    public void ShowRecolored()
    {
        effectStrength = 1f;
        ApplyAllValues();
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}