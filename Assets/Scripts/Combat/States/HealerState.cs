using UnityEngine;

public class HealerState : MonoBehaviour, IEnemyState
{
    public float healCooldown = 3f;
    public float healAmount = 15f;
    private float _timer;

    public void Tick(float dt) { _timer += dt; }
    public bool CanHeal() => _timer >= healCooldown;
    public void ResetCooldown() { _timer = 0f; }
}
