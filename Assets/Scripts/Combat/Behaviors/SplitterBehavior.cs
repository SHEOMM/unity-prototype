using UnityEngine;

/// <summary>분열체: 사망 시 60% 크기의 2마리로 분열한다.</summary>
[EnemyBehaviorId("splitter")]
public class SplitterBehavior : IEnemyBehavior
{
    public bool Tick(Enemy enemy, float dt) => true;
    public float ModifyIncomingDamage(Enemy enemy, float dmg, Element el) => dmg;

    public bool OnDeath(Enemy enemy)
    {
        if (enemy.Data.scale * enemy.transform.localScale.x < 0.2f)
            return true; // 너무 작으면 더 이상 분열하지 않음

        for (int i = 0; i < 2; i++)
        {
            var offset = new Vector3(i == 0 ? -0.3f : 0.3f, 0, 0);
            var go = new GameObject("Enemy_Split");
            go.transform.position = enemy.transform.position + offset;
            var child = go.AddComponent<Enemy>();
            var sprite = EnemySpriteGenerator.GenerateEnemySprite(enemy.Data);
            child.Initialize(enemy.Data, sprite);
            go.AddComponent<EnemyView>();
            go.AddComponent<StatusIconView>();
            child.maxHP = enemy.Data.baseHP * 0.4f;
            child.currentHP = child.maxHP;
            child.transform.localScale = enemy.transform.localScale * 0.6f;
        }

        Object.Destroy(enemy.gameObject);
        return false;
    }
}
