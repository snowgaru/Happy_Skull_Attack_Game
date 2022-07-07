using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class Boss : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        ATTACK2,
        DIE,
        PLAYERDIE
    }

    public float hp;
    public float currHp;

    // 몬스터의 현재 상태
    public State state = State.IDLE;
    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    public float attackDist = 2.0f;
    // 몬스터 사망 여부
    public bool isDie = false;

    private Transform monsterTransform;
    private Transform targetTransform;
    private Animator anim;
    private NavMeshAgent agent;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack2 = Animator.StringToHash("IsAttack2");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");


    public GameObject pos1;
    public GameObject Attack1Prefab;

    public GameObject pos2;
    public GameObject Attack2Prefab;

    GameObject effect2;
    GameObject effect1;

    public void AttackEffect1Instance()
    {
        effect1 = Instantiate(Attack1Prefab, pos1.transform);
        effect1.transform.localScale = pos1.transform.localScale;
        effect1.transform.position = pos1.transform.position;
        effect1.transform.rotation = pos1.transform.rotation;
        Destroy(effect1, 1);
    }

    public bool delay;

    public void EndAttack()
    {
        delay = true;
        state = State.TRACE;
        anim.SetBool(hashAttack, false);
        anim.SetBool(hashAttack2, false);
        voidvoid();
    }

    public void voidvoid()
    {
        Invoke("CompleteDelay", 3f);
    }

    public void CompleteDelay()
    {
        delay = false;
    }

    void Awake()
    {
        currHp = hp;

        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        // 자동회전 기능 비활성화
        agent.updateRotation = false;
        anim = GetComponent<Animator>();
    }


    void OnEnable()
    {

        state = State.IDLE;

        isDie = false;

        // 몬스터의 상태를 체크하는 코루틴
        StartCoroutine(CheckMonsterState());

        // 상태에 따라 몬스터 행동 수행 코루틴
        StartCoroutine(MonsterAction());
    }

    void Update()
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

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
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

            if (distance <= attackDist && delay == false)
            {
                int random = Random.Range(0, 1);
                if(random == 0)
                {
                    state = State.ATTACK;
                }
                else if(random == 1)
                {
                    state = State.ATTACK2;
                }
            }

            else if (distance <= traceDist)
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
        while (!isDie)
        {
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
                    anim.SetBool(hashAttack2, false);
                    break;

                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                        
                case State.ATTACK2:
                    anim.SetBool(hashAttack2, true);
                    break;

                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);

                    GetComponent<CapsuleCollider>().enabled = false;

                    SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>();
                    foreach (SphereCollider sphere in spheres)
                    {
                        sphere.enabled = false;
                    }

                    yield return new WaitForSeconds(3.0f);

                    this.gameObject.SetActive(false);
                    break;
                //case State.PLAYERDIE:
                //    StopAllCoroutines();

                //    agent.isStopped = true;
                //    anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.3f));
                //    anim.SetTrigger(hashPlayerDie);

                //    GetComponent<CapsuleCollider>().enabled = false;
                //    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
