using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy :PoolableMono
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE,
        PLAYERDIE
    }


    [Header("ü��")]
    public float hp;

    [Header("����ü��")]
    public float currHp;


    public MonsterState monsterData;

    public State state = State.IDLE;

    private Transform monsterTransform;
    private Transform targetTransform;
    private NavMeshAgent agent;
    private Animator anim;

    private Transform hitAniPos;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    public bool isAttack = false;
    public bool isDie = false;
    public Transform createPos;

    private SphereCollider col;

    public UnityEvent TimeSlowEffectFeedback = null;

    public GameObject hitAni;
    public GameObject hitAni2;

    void Awake()
    {
        currHp = hp;
        col = GetComponent<SphereCollider>();
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        anim = GetComponent<Animator>();
        hitAniPos = GameObject.Find("hitAniPos").transform;
    }
    private void Update()
    {   
        // ���������� ���� �Ÿ��� ȭ�� ���� �Ǵ�
        if (agent.remainingDistance >= 2.0f)
        {
            // ������Ʈ�� ȸ�� ��
            Vector3 direction = agent.desiredVelocity;

            // ȸ�� ���� ����
            Quaternion rotation = Quaternion.LookRotation(direction);

            // ���� �������� �Լ��� �ε巯�� ȸ�� ó��
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
        }
    }

    public void ColliderFalse()
    {
        col.enabled = false;
    }

    public void ColliderTrue()
    {
        col.enabled = true;
    }

    public void IsAttackTrue()
    {
        isAttack = true;
    }

    public void IsAttackFalse()
    {
        isAttack = false;
    }

    public void Check()
    {

        state = State.IDLE;
        currHp = hp;
        isDie = false;
        monsterData.isDie = false;
        //this.gameObject.SetActive(true);
        //Debug.Log(monsterData.isDie);
        // ������ ���¸� üũ�ϴ� �ڷ�ƾ
        StartCoroutine(CheckMonsterState());

        // ���¿� ���� ���� �ൿ ���� �ڷ�ƾ
        StartCoroutine(MonsterAction());
    }

    IEnumerator CheckMonsterState()
    {
        while (!PlayerDie())
        {

            yield return new WaitForSeconds(0.3f);

            // �÷��̾� ���� ���½� �ڷ�ƾ ����
            if (state == State.PLAYERDIE)
            {
                yield break;
            }

            // ���� ���� ���½� �ڷ�ƾ ����
            if (state == State.DIE)
            {
                yield break;
            }


            // ������ ĳ���� ������ �Ÿ� ����
            float distance = Vector3.Distance(monsterTransform.position, targetTransform.position);

            if (distance <= monsterData.attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= monsterData.traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!PlayerDie())
        {
            //Debug.Log("���� ���� �׼�");
            switch (state)
            {
                case State.IDLE:
                    // ���� ����
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    // ���� ��� ��ǥ�� �̵�
                    agent.SetDestination(targetTransform.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    isDie = true;
                    monsterData.isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);
                    GameManager.instance.currentMonsterCount--;
                    //GetComponent<CapsuleCollider>().enabled = false;

                    yield return new WaitForSeconds(0.7f);

                    yield return new WaitForSeconds(0.1f);
                    //CoinManager coin = PoolManager.Instance.Pop("Coin") as CoinManager;
                    //coin.GetComponent<Transform>().position = this.transform.position;
                    //this.gameObject.SetActive(false);
                    //PoolManager.Instance.Push(this);
                    //state = State.IDLE;

                    //StopAllCoroutines();
                    //gameObject.SetActive(false);
                    break;
                case State.PLAYERDIE:
                    //StopAllCoroutines();

                    //agent.isStopped = true;
                    //anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.3f));
                    //anim.SetTrigger(hashPlayerDie);

                    //GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void EnemyDieFunction()
    {
        CoinManager coin = PoolManager.Instance.Pop("Coin") as CoinManager;
        coin.GetComponent<Transform>().position = this.createPos.position;
        //this.gameObject.SetActive(false);
        ColliderTrue();
        PoolManager.Instance.Push(this);
        state = State.IDLE;
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") || collision.collider.CompareTag("Sword") && currHp > 0)
        {

            //TimeSlowEffectFeedback?.Invoke();
            // �Ѿ� ����
            if (collision.collider.CompareTag("Bullet"))
            {


                //�ؾ��ϴ°� ����Ʈ ��ġ ��Ȯ�ϰ� �ؼ� ����ϱ�

                Vector3 effectPos = collision.GetContact(0).point;
                GameObject hitEffect = Instantiate<GameObject>(hitAni, effectPos, Quaternion.identity, hitAniPos);
                Destroy(hitEffect, 0.1f);

                //Invoke("DestoryHitAni");
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                //player.GetComponent<PlayerController>().StartSwordTimeSlowFeedback();
                //ollision.transform.GetComponent<Bullet>();

                PoolManager.Instance.Push(collision.transform.GetComponent<Bullet>());

            }
            else if(collision.collider.CompareTag("Sword"))
            {
                Vector3 effectPos = collision.GetContact(0).point;
                GameObject hitEffect = Instantiate<GameObject>(hitAni2, effectPos, Quaternion.identity, hitAniPos);
                Destroy(hitEffect, 0.25f);
            }


            // �ǰ� �ִϸ��̼� ����
            anim.SetTrigger(hashHit);


            // �浹 ����
            Vector3 pos = collision.GetContact(0).point;
            // ����� �浹 ������ ���� ����
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            // ���� ȿ�� ����
            //ShowBloodEffect(pos, rot);

            // ���� hp ����
            currHp -= GameManager.instance.playerAttackLsit[GameManager.instance.playerAttackLevel];

            if (currHp <= 0)
            {
                state = State.DIE;
            }
        }
    }

    public override void Reset()
    {
        //StopAllCoroutines();
        //state = State.IDLE;
        ////monsterData.currHp = monsterData.hp;
        //monsterData.isDie = false;
    }

    private bool PlayerDie()
    {
        return isDie;
        /*
        //Debug.Log(monsterData.isDie);
        if(monsterData.isDie == true)
        {
            //Debug.Log(monsterData.isDie);
            //Debug.Log("��������");
            return true;
        }
        else
        {
            //Debug.Log(monsterData.isDie);
            //Debug.Log("�÷��̾� �������");
            return false;
        }
        */
    }
}
