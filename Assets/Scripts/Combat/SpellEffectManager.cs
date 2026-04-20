using UnityEngine;
using System.Collections;

/// <summary>
/// 마법 이펙트를 지상에서 연출한다.
/// SpellCommand의 visualId에 따라 ISpellVisual 전략을 선택하여 재생한다.
/// </summary>
public class SpellEffectManager : MonoBehaviour
{
    public static SpellEffectManager Instance;
    void Awake() { Instance = this; }

    public void ExecuteSpells(SpellResult result)
    {
        StartCoroutine(RunSequence(result));
    }

    IEnumerator RunSequence(SpellResult result)
    {
        foreach (var cmd in result.commands)
        {
            for (int i = 0; i < cmd.hitCount; i++)
            {
                FireAt(cmd);
                yield return new WaitForSeconds(0.12f);
            }
        }
    }

    void FireAt(SpellCommand cmd)
    {
        if (EnemyRegistry.Instance == null) return;
        Enemy target = EnemyRegistry.Instance.GetRandom();
        if (target == null) return;

        // 유물 속성 보너스 적용
        if (PlayerState.Instance != null && cmd.element != Element.None)
            cmd.damage *= (1f + PlayerState.Instance.GetElementBonus(cmd.element));

        var visual = VisualRegistry.Get(cmd.visualId);
        var ctx = new SpellVisualContext
        {
            command = cmd,
            target = target,
            targetPosition = target.transform.position,
            elementColor = GetElementColor(cmd.element),
            vfxRoot = transform
        };
        StartCoroutine(visual.Play(ctx));
    }

    public static Color GetElementColor(Element e)
    {
        switch (e)
        {
            case Element.Fire: return new Color(1f, 0.4f, 0f);
            case Element.Water: return new Color(0.3f, 0.5f, 1f);
            case Element.Wind: return new Color(0.4f, 1f, 0.5f);
            case Element.Earth: return new Color(0.6f, 0.4f, 0.2f);
            case Element.Darkness: return new Color(0.5f, 0.2f, 0.8f);
            default: return Color.cyan;
        }
    }
}
