using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 모든 씬 Boot 스크립트의 공통 베이스. 템플릿 메소드 패턴으로 씬 진입 불변 조건을 강제한다.
///
/// Start() 순서:
/// 1. SetActiveScene → 이후 OnBoot 내부의 new GameObject()가 올바른 씬에 parent된다
/// 2. SceneEnvironment가 할당되어 있으면 카메라/조명에 적용
/// 3. 상속자가 구현한 OnBoot() 호출 (scene-specific 로직)
///
/// 신규 씬 추가 시: 이 클래스를 상속하고 environment 필드에 해당 씬의 SceneEnvironment SO를 할당하면 된다.
/// </summary>
public abstract class SceneBootBase : MonoBehaviour
{
    [Tooltip("이 씬의 카메라/조명/배경 환경 데이터. 씬 진입 시 자동 적용.")]
    [SerializeField] protected SceneEnvironment environment;

    protected virtual void Start()
    {
        // 1. 활성 씬 먼저 (new GameObject() 이전)
        SceneManager.SetActiveScene(gameObject.scene);

        // 2. 환경 적용
        if (environment != null)
        {
            CameraService.Instance?.ApplyEnvironment(environment);
            LightingService.Instance?.ApplyEnvironment(environment);
        }
        else
        {
            Debug.LogWarning($"[{GetType().Name}] SceneEnvironment 미할당 — Inspector에서 할당하세요.");
        }

        // 3. 씬별 초기화
        OnBoot();
    }

    /// <summary>씬별 초기화 로직. SceneBootBase가 활성 씬 설정 + 환경 적용을 먼저 처리한 뒤 호출된다.</summary>
    protected abstract void OnBoot();

    /// <summary>씬에 PlayerHPBar HUD를 부착한다. 전투 외 씬에서도 HP 노출. 중복 부착 방지.</summary>
    protected void EnsureHud()
    {
        if (GetComponent<PlayerHPBar>() == null)
            gameObject.AddComponent<PlayerHPBar>();
    }
}
