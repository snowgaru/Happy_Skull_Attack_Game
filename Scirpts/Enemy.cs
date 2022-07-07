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


    [Header("체력")]
    public float hp;

    [Header("현재체력")]
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
        // 목적지까지 남은 거리로 화전 여부 판단
        if (agent.remainingDistance >= 2.0f)
        {
            // 에이전트의 회전 값
            Vector3 direction = agent.desiredVelocity;

            // 회전 각도 산출
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 구면 선형보간 함수로 부드러운 회전 처리
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
        // 몬스터의 상태를 체크하는 코루틴
        StartCoroutine(CheckMonsterState());

        // 상태에 따라 몬스터 행동 수행 코루틴
        StartCoroutine(MonsterAction());
    }

    IEnumerator CheckMonsterState()
    {
        while (!PlayerDie())
        {

            yield return new WaitForSeconds(0.3f);

            // 플레이어 죽음 상태시 코루틴 멈춤
            if (state == State.PLAYERDIE)
            {
                yield break;
            }

            // 몬스터 죽음 상태시 코루틴 멈춤
            if (state == State.DIE)
            {
                yield break;
            }


            // 몬스터의 캐릭터 사이의 거리 측정
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
            //Debug.Log("현재 몬스터 액션");
            switch (state)
            {
                case State.IDLE:
                    // 추적 중지
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    // 추적 대상 좌표로 이동
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
            // 총알 삭제
            if (collision.collider.CompareTag("Bullet"))
            {


                //해야하는거 이펙트 위치 정확하게 해서 출력하기

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


            // 피격 애니메이션 실행
            anim.SetTrigger(hashHit);


            // 충돌 지점
            Vector3 pos = collision.GetContact(0).point;
            // 충알의 충돌 지점의 법선 벡터
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            // 혈흔 효과 생성
            //ShowBloodEffect(pos, rot);

            // 몬스터 hp 차감
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
            //Debug.Log("뒤져있음");
            return true;
        }
        else
        {
            //Debug.Log(monsterData.isDie);
            //Debug.Log("플레이어 살아있음");
            return false;
        }
        */
    }
}
