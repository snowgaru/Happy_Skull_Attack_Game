using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public int playerCoin;
    public int playerAttackLevel;

    public int[] playerAttackLsit = { 4, 8, 12, 16, 20 , 0 };
    public int[] playerAttackUpgradeLsit = { 5, 10, 20, 35, 50, 0 };

    public Text coinText;
    public Text nextUpgradeText;
    public Text needUpgradeText;
    public Text errorMessage;

    private bool isSetting;

    public GameObject SettingPanel;
    public GameObject UpgradePanel;

    public Scrollbar BackgroundScroll;
    public Scrollbar EffectScroll; 
    public float BackgroundValue;
    public float EffectValue;

    public Text SettingOrUpgradeText;
    void Awake()
    {
        playerCoin = PlayerPrefs.GetInt("playerCoin");
        playerAttackLevel = PlayerPrefs.GetInt("playerAttack");
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        PlayBackgroundSound();
        SetText();
        SoundLoad();
    }

    public void SettingAndUpgradeChange()
    {
        isSetting = isSetting ? false : true;
        if(isSetting)
        {
            SettingPanel.SetActive(true);
            UpgradePanel.SetActive(false);
            SettingOrUpgradeText.text = "강화하기";
        }
        else
        {
            SettingPanel.SetActive(false);
            UpgradePanel.SetActive(true);
            SettingOrUpgradeText.text = "설정하기";
        }
    }

    public void SoundSave()
    {
        BackgroundValue = BackgroundScroll.value;
        EffectValue = EffectScroll.value;
        PlayerPrefs.SetFloat("BackgroundSound", BackgroundValue);
        PlayerPrefs.SetFloat("EffectSound", EffectValue);
    }

    public void SoundLoad()
    {
        BackgroundValue = PlayerPrefs.GetFloat("BackgroundSound");
        EffectValue = PlayerPrefs.GetFloat("EffectSound");
        BackgroundScroll.value = BackgroundValue;
        EffectScroll.value = EffectValue;
    }

    void Update()
    {
        
    }

    void SetText()
    {
        coinText.text = playerCoin.ToString();
        nextUpgradeText.text = playerAttackLsit[playerAttackLevel + 1].ToString();
        needUpgradeText.text = "필요 코인 : " + playerAttackUpgradeLsit[playerAttackLevel + 1].ToString();
    }

    public void Upgrade()
    {
        if(playerAttackUpgradeLsit[playerAttackLevel + 1] < playerCoin)
        {
            if (playerAttackLevel >= 5)
            {
                errorMessage.text = "최대 강화입니다.";
                return;
            }
            playerCoin -= playerAttackUpgradeLsit[playerAttackLevel + 1];
            playerAttackLevel++;
            PlayerPrefs.SetInt("playerCoin", playerCoin);
            PlayerPrefs.SetInt("playerAttack", playerAttackLevel);
        }
        else
        {
            errorMessage.text = "돈이 부족합니다.";
        }
        SetText();
    }


    public void StartGame()
    {
        SceneManager.LoadScene("GameScenes");
    }

    public AudioSource audioSource;

    public AudioClip audioClips;
    public AudioClip audioClip;

    public void PlayBackgroundSound()
    {
        audioSource.volume = PlayerPrefs.GetFloat("BackgroundSound");
        //audioSource.PlayOneShot(audioClips);
        audioSource.Play();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
