using UnityEngine;

public class MissileAppear : MonoBehaviour
{
    private Boss _boss;
    private Animator _anim;

    private void Awake()
    {
        _boss = GetComponentInParent<Boss>();
    }

    private void MissileTrigger()
    {
        _boss.GetMissile();
    }
}