using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public PoolingListSO InitList;

    public List<Transform> points = new List<Transform>();

    public int playerCoin = 0;
    public int playerHP = 0;

    public int currentMonsterCount = 0;

    public int maxMonsterCount = 4;

    //public int playerAttack = 0;
    public int playerAttackLevel = 1;


    public int[] playerAttackLsit = { 4, 8, 12, 16, 20 };
    public int[] playerAttackUpgradeLsit = { 5, 10, 20, 35, 50 };


    public AudioSource audioSource;

    public AudioClip audioClips;

    void Awake()
    {
        playerAttackLevel = PlayerPrefs.GetInt("playerAttack");
        playerCoin = PlayerPrefs.GetInt("playerCoin");

        if (instance != null)
            Debug.LogError("Multiple GameManager is running");
        instance = this;

        PoolManager.Instance = new PoolManager(transform); //풀매니저 생성

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayBackgroundSound();

        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        // List 타입 변수 사용
        //spawnPointGroup?.GetComponentsInChildren<Transform>(points);

        foreach (Transform item in spawnPointGroup)
        {
            points.Add(item);
        }
        CreatePool();

        //Invoke(("CreateMonster"), 2);
        //Invoke(("CreateMonster"), 4);
        
        
        //InvokeRepeating(("CreateMonster"), 2, 5);
    }

    void CreateMonster()
    {
        if (currentMonsterCount >= maxMonsterCount) return;
        // 몬스터의 불규칙한 위치 생성
        int idx = Random.Range(0, points.Count);

        currentMonsterCount++;
        //Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);

        // 오브젝트 풀에서 몬스터 추출
        Enemy monster = PoolManager.Instance.Pop("VikingSlime") as Enemy;

        // 몬스터의 위치와 회전 설정
        monster?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        monster?.Check();
        //monster?.GetComponent<GameObject>().SetActive(true);
    }

    private void CreatePool()
    {
        foreach (PoolingPair pair in InitList.list)
        {
            if (PoolManager.Instance == null) Debug.Log("널");
            PoolManager.Instance.CreatePool(pair.prefab, pair.poolCnt);
        }

    }

    public void SmithScene()
    {
        PlayerPrefs.SetInt("playerCoin", playerCoin);
        PlayerPrefs.SetInt("playerAttack", playerAttackLevel);
        SceneManager.LoadScene("Smithy");

    }

    public void PlayBackgroundSound()
    {
        audioSource.volume = PlayerPrefs.GetFloat("BackgroundSound");
        audioSource.Play();
    }
}
