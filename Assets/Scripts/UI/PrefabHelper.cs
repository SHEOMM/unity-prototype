using UnityEngine;

/// <summary>
/// Prefab Instantiate를 안전하게 감싸는 정적 헬퍼.
///
/// <para>패턴: Boot 스크립트 또는 View가 <c>[SerializeField] GameObject prefab</c>로
/// Inspector에서 받은 Prefab을 이 메서드로 Instantiate. Prefab이 null이거나 요구된
/// 컴포넌트가 없으면 <c>Debug.LogError</c>로 빠른 실패하여 런타임 NRE를 방지한다.</para>
///
/// <example>
/// <code>
/// [SerializeField] GameObject rewardCardPrefab;
///
/// var card = PrefabHelper.Spawn&lt;RewardCardView&gt;(rewardCardPrefab, transform);
/// if (card == null) return;
/// card.transform.position = new Vector3(x, y, 0);
/// card.Bind(choice);
/// </code>
/// </example>
/// </summary>
public static class PrefabHelper
{
    /// <summary>
    /// Prefab을 Instantiate하고 요구된 컴포넌트를 반환.
    /// Prefab/컴포넌트 누락 시 LogError + null 반환 (호출 측에서 null 체크 필수).
    /// </summary>
    public static T Spawn<T>(GameObject prefab, Transform parent = null) where T : Component
    {
        if (prefab == null)
        {
            Debug.LogError($"[PrefabHelper] Prefab is null when spawning component {typeof(T).Name}");
            return null;
        }

        var go = Object.Instantiate(prefab, parent);
        var comp = go.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogError($"[PrefabHelper] Prefab '{prefab.name}' is missing component {typeof(T).Name}");
            Object.Destroy(go);
            return null;
        }
        return comp;
    }

    /// <summary>
    /// Prefab을 Instantiate하고 GameObject 자체를 반환. 컴포넌트 검증 없이 단순 복제.
    /// 컴포넌트가 필요하다면 제네릭 오버로드(<see cref="Spawn{T}"/>) 사용 권장.
    /// </summary>
    public static GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("[PrefabHelper] Prefab is null");
            return null;
        }
        return Object.Instantiate(prefab, parent);
    }
}
