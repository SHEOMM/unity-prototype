using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 배경 렌더러. 전달된 Set의 스프라이트를 카메라 뷰 내 지정 Y 범위에 맞춰 스트레치 렌더.
///
/// Space: 단일 SpriteRenderer (천상 영역 또는 Map 전체)
/// Ground: layers[] 개수만큼 SpriteRenderer 자식 스택 (뒤→앞 순서, sortingOrder 오름차순)
///
/// 사용:
///   var go = new GameObject("Background"); go.AddComponent&lt;BackgroundView&gt;()
///     .ApplySpace(spaceSet, cam, yMin, yMax, baseSortingOrder);
/// </summary>
public class BackgroundView : MonoBehaviour
{
    SpriteRenderer _spaceRenderer;
    readonly List<SpriteRenderer> _groundRenderers = new List<SpriteRenderer>();

    /// <summary>Space 배경을 y ∈ [yMin, yMax] 영역에 스트레치.</summary>
    public void ApplySpace(SpaceBackgroundSet set, Camera cam, float yMin, float yMax, int sortingOrder)
    {
        if (set == null || set.sprite == null || cam == null) return;
        if (_spaceRenderer == null) _spaceRenderer = CreateLayer("Space", sortingOrder);
        else _spaceRenderer.sortingOrder = sortingOrder;

        _spaceRenderer.sprite = set.sprite;
        _spaceRenderer.color = set.tint;
        FitToRect(_spaceRenderer.transform, _spaceRenderer.sprite, cam, yMin, yMax);
    }

    /// <summary>Ground 레이어들을 y ∈ [yMin, yMax] 영역에 스트레치. layer[0]이 가장 뒤.</summary>
    public void ApplyGround(GroundBackgroundSet set, Camera cam, float yMin, float yMax, int sortingOrderBase)
    {
        if (set == null || cam == null) return;

        // 기존 레이어 정리 (Refresh 대응)
        foreach (var r in _groundRenderers) if (r != null) Destroy(r.gameObject);
        _groundRenderers.Clear();

        var layers = set.ResolveLayers();
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null) continue;
            var sr = CreateLayer($"Ground_{i}", sortingOrderBase + i);
            sr.sprite = layers[i];
            sr.color = Color.white;
            FitToRect(sr.transform, sr.sprite, cam, yMin, yMax);
            _groundRenderers.Add(sr);
        }
    }

    SpriteRenderer CreateLayer(string name, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = sortingOrder;
        return sr;
    }

    /// <summary>스프라이트 transform을 카메라 너비 전체 × [yMin,yMax] 높이로 맞춤 (비균일 스케일 허용).</summary>
    static void FitToRect(Transform t, Sprite sprite, Camera cam, float yMin, float yMax)
    {
        if (sprite == null) return;
        float targetW = cam.orthographicSize * 2f * cam.aspect;
        float targetH = yMax - yMin;
        if (targetW <= 0f || targetH <= 0f) return;

        Vector2 nativeSize = sprite.bounds.size;  // PPU 반영된 월드 단위 크기
        float scaleX = nativeSize.x > 0.0001f ? targetW / nativeSize.x : 1f;
        float scaleY = nativeSize.y > 0.0001f ? targetH / nativeSize.y : 1f;

        t.position = new Vector3(cam.transform.position.x, (yMin + yMax) * 0.5f, 0f);
        t.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
