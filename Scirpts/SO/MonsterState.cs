using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Monster")]
public class MonsterState : ScriptableObject
{
    [Header("원거리 방어력")]
    public float rangeDEF;

    [Header("공격력")]
    public float atk;

    [Header("추격 사정거리")]
    public float traceDist = 10.0f;

    [Header("공격 사정거리")]
    public float attackDist = 2.0f;

    [Header("사망판정")]
    public bool isDie = false;

    [Header("폭발하는지 체크")]
    public bool isBomb;
}
