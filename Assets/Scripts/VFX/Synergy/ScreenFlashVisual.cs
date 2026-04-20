using System.Collections;
using UnityEngine;

/// <summary>
/// 전체 화면 순간 플래시 + 카메라 쉐이크. 가장 강한 시너지(Combo/궁극기 톤)에서 사용.
/// UIFactory.MakePixel로 카메라 전체 덮는 오버레이 스프라이트 생성 후 페이드.
/// CameraService.Shake로 짧은 흔들림 병행.
/// </summary>
[SynergyVisualId("screen_flash")]
public class ScreenFlashVisual : ISynergyVisual
{
    public IEnumerator Play(SynergyVisualContext ctx)
    {
        var cam = CameraService.Instance?.Camera;
        if (cam == null) yield break;

        float h = cam.orthographicSize * 2f;
        float w = h * cam.aspect;

        var fx = new GameObject("FX_ScreenFlash");
        fx.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0);
        fx.transform.localScale = new Vector3(w, h, 1f);

        var sr = fx.AddComponent<SpriteRenderer>();
        sr.sprite = UIFactory.MakePixel();
        var c = ctx.ElementColor;
        sr.color = new Color(c.r, c.g, c.b, 0.6f);
        sr.sortingOrder = GameConstants.SortingOrder.SynergyScreenFlash;

        // Shake 병행 (있으면)
        CameraService.Instance?.Shake(0.25f, 0.25f);

        float duration = 0.25f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(0.6f, 0f, t);
            sr.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        Object.Destroy(fx);
    }
}
