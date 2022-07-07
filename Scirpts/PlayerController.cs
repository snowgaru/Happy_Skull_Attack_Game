using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public GameObject GunAniTargetTransform;

    public bool isShooting;

    public GameObject bulletPrefab;
    public Transform firePos;

    private MeshRenderer muzzleFlash;

    private Rigidbody playerRigidbody;
    public float dashForce = 3f;

    public BoxCollider swordCollider;

    public bool isCanMeleeAttack = true;
    public bool isCanRangeAttack = true;
    public bool isCanRolling = true;

    public bool isRolling = false;
    public bool isGodMode = false;

    public UnityEvent PerfectDodgeTimeSlowFeedback;

    public GameObject pos1;
    public GameObject Attack1Prefab;
    
    public GameObject pos2;
    public GameObject Attack2Prefab;

    bool isDead = false;

    GameObject effect2;
    GameObject effect1;

    public GameObject motionTrail;

    public AudioSource audioSource;

    public List<AudioClip> audioClips;

    #region 근접콤보공격
    bool comboPossible;
    int comboStep;

    public void CanRolling()
    {
        isCanRolling = true;
    }

    private void Attack()
    {
        isCanRolling = false;
        if(comboStep == 0)
        {
            comboStep = 1;
            anim.Play("Attack1");
            PlaySwordSound();
            Invoke("SwordEffect1Instance", 0.2f);
            Destroy(effect1, 2f);
            //anim.Play("Attack1Animation");
  
            return;
        }
        if(comboStep != 0)
        {
            if(comboPossible)
            {

                comboPossible = false;
                comboStep += 1;
            }
        }   
    }

    public void ComboPossible()
    {
        comboPossible = true;
    }

    public void Combo()
    {
        if(comboStep == 2)
        {
            PlaySwordSound();
            anim.Play("Attack2");
            Invoke("SwordEffect2Instance", 0.3f);
            
            Destroy(effect2, 2f);
            //anim.Play("Attack2Animation");
        }
        //if(comboStep == 3)
        //{ anim.Play("이름");
    }

    public void SwordEffect1Instance()
    {
        effect1 = Instantiate(Attack1Prefab, pos1.transform);
    }


    public void SwordEffect2Instance()
    {
        effect2 = Instantiate(Attack2Prefab, pos2.transform);
    }

    public void ComboReset()
    {
        swordCollider.enabled = false;
        CanRolling();
        comboPossible = false;
        comboStep = 0;

        
    }

    public void SwordColliderTrue()
    {
        swordCollider.enabled = true;
    }

    #endregion
    void Start()
    {
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
        playerRigidbody = GetComponent<Rigidbody>();

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("EffectSound");
    }

    void Update()
    {
        AttackCheck();
        DashCheck();
        ShootRay();
    }

    private void ShootRay()
    {
        RaycastHit info;
        Debug.DrawRay(transform.position, -transform.up, Color.red, 2f);
        if(Physics.Raycast(transform.position, -transform.up, out info, 1000))
        {
            if(info.transform.CompareTag("Floor"))
            {
                anim.SetTrigger("PlayerDie");
                GameManager.instance.playerCoin = 0;
                UIManager.instance.PlayerDiePanelTrue();
                isDead = true;
                StopAllCoroutines();
            }
        }
    }

    public void PerfectDodgeTimeSlowFeedbacking()
    {
        Debug.Log("때려쳐야지");
        PerfectDodgeTimeSlowFeedback?.Invoke();
    }

    private void DashCheck()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isCanRolling == true && playerRigidbody.velocity != Vector3.zero)
        {
            isCanMeleeAttack = false;
            isCanRangeAttack = false;
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        anim.SetTrigger("Rolling");
        yield return new WaitForSeconds(0.15f);
        isRolling = true;
        speed = 275;
        yield return new WaitForSeconds(0.5f);
        isRolling = false;
        speed = 200;
        isCanRangeAttack = true;
        isCanMeleeAttack = true;
    }

    void FixedUpdate()
    {
        Move();
    }

    #region 이동관련
    Rigidbody rb;
    Animator anim;

    float vr;
    float hr;

    Vector3 targetRotation;

    public float rotateSpeed = 10;
    public float speed = 10;

    void Move()
    {
        if (isDead) return;
        hr = Input.GetAxisRaw("Horizontal");
        vr = Input.GetAxisRaw("Vertical");

        Vector3 inputRaw = new Vector3(hr, 0, vr);

        if (inputRaw.sqrMagnitude > 1f)
        {
            inputRaw.Normalize();
        }

        if (inputRaw != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(inputRaw).eulerAngles;
        }

        rb.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation.x, Mathf.Round(targetRotation.y / 45) * 45, targetRotation.z), rotateSpeed * Time.deltaTime);
        Vector3 vel = inputRaw * speed * Time.deltaTime;
        rb.velocity = vel;


        if (vel.x == 0 && vel.z == 0)
        {
            anim.SetBool("Running", false);
        }
        else
        {
            anim.SetBool("Running", true);
        }
    }
    #endregion

    void AttackCheck()
    {
        if (isDead) return;

        if(Input.GetMouseButtonDown(0) && isCanMeleeAttack == true)
        {
            //근접공격
            Attack();
        }
        if(Input.GetMouseButtonDown(1)&& isCanMeleeAttack == true)
        {

            //원거리 공격
            //anim.SetTrigger("RangeAttack");
            if (!isShooting)
            {
                isCanRolling = false;
                //StopCoroutine(ShootingAnimation());
                Fire();
                StartCoroutine(ShootingAnimation());

            }
        }
    }



    private IEnumerator ShootingAnimation()
    {
        isShooting = true;
        GunAniTargetTransform.transform.DOLocalMoveY(1.33f, 0.15f).SetLoops(2,LoopType.Yoyo);
        yield return new WaitForSeconds(0.35f);
        CanRolling();
        isShooting = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.75f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.75f);


        anim.SetIKPosition(AvatarIKGoal.LeftHand, GunAniTargetTransform.transform.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, GunAniTargetTransform.transform.rotation);

        //anim.SetLookAtWeight(1);
        //anim.SetLookAtPosition(GunAniTargetTransform.position);
    }

    void Fire()
    {
        // Bullet 복사본 생성
        PlayGunSound();
        Bullet bulletPrefab = PoolManager.Instance.Pop("Bullet") as Bullet;
        bulletPrefab.GetComponent<Transform>().SetPositionAndRotation(firePos.position, firePos.rotation);
        bulletPrefab.Fire();
        StartCoroutine(ShowMuzzleFlash());

    }

    IEnumerator ShowMuzzleFlash()
    {
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;

        // 랜덤한 오프셋
        muzzleFlash.material.mainTextureOffset = offset;

        // 랜덤한 회전
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(Vector3.forward * angle);

        // 랜덤한 크기
        float scale = Random.Range(0.5f, 0.8f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(0.2f);

        muzzleFlash.enabled = false;
    }

    public IEnumerator AttackingGodMode()
    {
        if (isGodMode) yield break;
        motionTrail.SetActive(true);
        UIManager.instance.GodmodeText(true);

        isGodMode = true;
        yield return new WaitForSeconds(3f);
        UIManager.instance.GodmodeText(false);
        motionTrail.SetActive(false);
        isGodMode = false;
    }

    public IEnumerator PerfectDodge()
    {
        if (isGodMode) yield break;
        isGodMode = true;
        Debug.Log("퍼펙트닷지갓모드트루");
        UIManager.instance.GodmodeText(true);
        motionTrail.SetActive(true);
        yield return new WaitForSeconds(10f);
        UIManager.instance.GodmodeText(false);
        motionTrail.SetActive(false);
        isGodMode = false;
        Debug.Log("퍼펙트닷지갓모드펄스");
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Coin"))
        {
            PlayGoldSound();
            GameManager.instance.playerCoin += 1;
            UIManager.instance.SetGold();
            PoolManager.Instance.Push(collision.transform.GetComponent<CoinManager>());
        }

        if(collision.transform.CompareTag("Enemy") && collision.transform.GetComponent<Enemy>().isAttack)
        {

            if(isRolling)
            {
                Debug.Log("타이밍 회피");
                PerfectDodgeTimeSlowFeedbacking();
                StopCoroutine(PerfectDodge());
                StopCoroutine(AttackingGodMode());
                StartCoroutine(PerfectDodge());
                return;
            }

            if (isGodMode) return;
            anim.SetTrigger("GetHit");
            ComboReset();
            GameManager.instance.playerHP -= 1;
            UIManager.instance.SetHP();
            if(GameManager.instance.playerHP <= 0)
            {
                anim.SetTrigger("PlayerDie");
                GameManager.instance.playerCoin = 0;
                UIManager.instance.PlayerDiePanelTrue();
                isDead = true;
                StopAllCoroutines();
            }
            StopCoroutine(PerfectDodge());
            StopCoroutine(AttackingGodMode());
            StartCoroutine(AttackingGodMode());;

            Debug.Log(GameManager.instance.playerHP); 
        }

    }


    public void PlayGunSound()
    {
        audioSource.PlayOneShot(audioClips[0]);
    }

    public void PlaySwordSound()
    {
        audioSource.PlayOneShot(audioClips[1]);
    }    
    
    public void PlayGoldSound()
    {
        audioSource.PlayOneShot(audioClips[2]);
    }

}
