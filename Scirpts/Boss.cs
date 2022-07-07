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

    // ������ ���� ����
    public State state = State.IDLE;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    // ���� �����Ÿ�
    public float attackDist = 2.0f;
    // ���� ��� ����
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

        // �ڵ�ȸ�� ��� ��Ȱ��ȭ
        agent.updateRotation = false;
        anim = GetComponent<Animator>();
    }


    void OnEnable()
    {

        state = State.IDLE;

        isDie = false;

        // ������ ���¸� üũ�ϴ� �ڷ�ƾ
        StartCoroutine(CheckMonsterState());

        // ���¿� ���� ���� �ൿ ���� �ڷ�ƾ
        StartCoroutine(MonsterAction());
    }

    void Update()
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

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
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
