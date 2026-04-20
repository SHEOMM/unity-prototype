using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 배경 렌더러. 전달된 Set의 스프라이트를 카메라 뷰 내 지정 Y 범위에 채워 렌더.
/// 원본 비율은 유지하되 대상 사각형 비율에 맞게 가운데를 **잘라서** 사용 (cover-fit).
/// 종횡비가 틀어진 비균일 스트레치(scaleX ≠ scaleY)는 쓰지 않는다.
///
/// Space: 단일 SpriteRenderer (천상 영역 또는 Map 전체)
/// Ground: layers[] 개수만큼 SpriteRenderer 자식 스택 (뒤→앞 순서, sortingOrder 오름차순)
/// </summary>
public class BackgroundView : MonoBehaviour
{
    SpriteRenderer _spaceRenderer;
    readonly List<SpriteRenderer> _groundRenderers = new List<SpriteRenderer>();

    Sprite _spaceGenSprite;
    readonly List<Sprite> _groundGenSprites = new List<Sprite>();

    /// <summary>Space 배경을 y ∈ [yMin, yMax] 영역에 cover-fit (가운데 크롭).</summary>
    public void ApplySpace(SpaceBackgroundSet set, Camera cam, float yMin, float yMax, int sortingOrder)
    {
        if (set == null || set.sprite == null || cam == null) return;
        if (_spaceRenderer == null) _spaceRenderer = CreateLayer("Space", sortingOrder);
        else _spaceRenderer.sortingOrder = sortingOrder;

        _spaceRenderer.color = set.tint;
        ReplaceSpaceSprite(null);
        FitToRect(_spaceRenderer, set.sprite, cam, yMin, yMax, isSpace: true);
    }

    /// <summary>Ground 레이어들을 y ∈ [yMin, yMax] 영역에 cover-fit. layer[0]이 가장 뒤.</summary>
    public void ApplyGround(GroundBackgroundSet set, Camera cam, float yMin, float yMax, int sortingOrderBase)
    {
        if (set == null || cam == null) return;

        // 기존 레이어 정리 (Refresh 대응)
        foreach (var r in _groundRenderers) if (r != null) Destroy(r.gameObject);
        _groundRenderers.Clear();
        DisposeGroundGenSprites();

        var layers = set.ResolveLayers();
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null) continue;
            var sr = CreateLayer($"Ground_{i}", sortingOrderBase + i);
            sr.color = Color.white;
            FitToRect(sr, layers[i], cam, yMin, yMax, isSpace: false);
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

    /// <summary>
    /// 대상 직사각형(카메라 너비 × [yMin, yMax])을 채우도록 원본 스프라이트의 가운데를
    /// 크롭한 새 Sprite를 만들어 할당한다. 균일 스케일만 사용 → 왜곡 없음.
    /// </summary>
    void FitToRect(SpriteRenderer renderer, Sprite source, Camera cam, float yMin, float yMax, bool isSpace)
    {
        if (source == null) return;
        float targetW = cam.orthographicSize * 2f * cam.aspect;
        float targetH = yMax - yMin;
        if (targetW <= 0f || targetH <= 0f) return;

        float targetAspect = targetW / targetH;
        var cropped = CreateCoverCroppedSprite(source, targetAspect);
        if (isSpace) ReplaceSpaceSprite(cropped);
        else _groundGenSprites.Add(cropped);
        renderer.sprite = cropped;

        Vector2 nativeSize = cropped.bounds.size;
        // 크롭 결과는 정확히 targetAspect 비율이므로 X/Y 스케일이 동일해진다.
        float scale = nativeSize.x > 0.0001f ? targetW / nativeSize.x : 1f;

        renderer.transform.position = new Vector3(cam.transform.position.x, (yMin + yMax) * 0.5f, 0f);
        renderer.transform.localScale = new Vector3(scale, scale, 1f);
    }

    /// <summary>
    /// 원본 스프라이트의 텍스처 rect에서 targetAspect에 맞는 중앙 크롭 rect로 새 Sprite 생성.
    /// Sprite.Create는 텍스처를 재사용하고 서브 rect만 참조하므로 메모리/픽셀 복사 없음.
    /// </summary>
    static Sprite CreateCoverCroppedSprite(Sprite source, float targetAspect)
    {
        Rect srcRect = source.rect; // 텍스처 픽셀 좌표
        float srcAspect = srcRect.width / srcRect.height;
        Rect cropRect;

        if (srcAspect > targetAspect)
        {
            // 원본이 더 가로로 길다 → 좌우를 잘라낸다
            float newW = srcRect.height * targetAspect;
            float xOff = (srcRect.width - newW) * 0.5f;
            cropRect = new Rect(srcRect.x + xOff, srcRect.y, newW, srcRect.height);
        }
        else
        {
            // 원본이 더 세로로 길다 → 상하를 잘라낸다
            float newH = srcRect.width / targetAspect;
            float yOff = (srcRect.height - newH) * 0.5f;
            cropRect = new Rect(srcRect.x, srcRect.y + yOff, srcRect.width, newH);
        }

        return Sprite.Create(source.texture, cropRect, new Vector2(0.5f, 0.5f), source.pixelsPerUnit);
    }

    void ReplaceSpaceSprite(Sprite next)
    {
        if (_spaceGenSprite != null) Destroy(_spaceGenSprite);
        _spaceGenSprite = next;
    }

    void DisposeGroundGenSprites()
    {
        foreach (var s in _groundGenSprites) if (s != null) Destroy(s);
        _groundGenSprites.Clear();
    }

    void OnDestroy()
    {
        ReplaceSpaceSprite(null);
        DisposeGroundGenSprites();
    }
}
