using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// PersistentScene의 유일한 Global Light 2D 래퍼.
/// SceneEnvironment에서 받은 값을 씬 진입 시 적용.
/// </summary>
[RequireComponent(typeof(Light2D))]
public class LightingService : MonoBehaviour
{
    public static LightingService Instance { get; private set; }

    public Light2D Light { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Light = GetComponent<Light2D>();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ApplyEnvironment(SceneEnvironment env)
    {
        if (env == null || Light == null) return;
        Light.intensity = env.lightIntensity;
        Light.color = env.lightColor;
    }
}
