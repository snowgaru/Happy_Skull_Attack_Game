using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Monster")]
public class MonsterState : ScriptableObject
{
    [Header("���Ÿ� ����")]
    public float rangeDEF;

    [Header("���ݷ�")]
    public float atk;

    [Header("�߰� �����Ÿ�")]
    public float traceDist = 10.0f;

    [Header("���� �����Ÿ�")]
    public float attackDist = 2.0f;

    [Header("�������")]
    public bool isDie = false;

    [Header("�����ϴ��� üũ")]
    public bool isBomb;
}
