using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI godmodeText;
    public Image hpImage;

    public Text goldText;

    public GameObject playerDiePanel;

    void Start()
    {
        if (instance != null)
            Debug.LogError("Multiple GameManager is running");
        instance = this;

        hpImage.fillAmount = 1;
        SetGold();
        GodmodeText(false);
        ResetHP();
        //SetHP();
    }
    void Update()
    {

    }

    public void PlayerDiePanelTrue()
    {
        playerDiePanel.SetActive(true);
    }

    public void SetGold()
    {
        goldText.text = GameManager.instance.playerCoin.ToString();
    }

    public void GodmodeText(bool result)
    {
        godmodeText.enabled = result;
    }

    public void ResetHP()
    {
        hpImage.fillAmount = 1;
    }

    public void SetHP()
    {
        StartCoroutine(HpDown());
    }

    private IEnumerator HpDown()
    {
        for(int i = 0; i < 10; i++)
        {
            hpImage.fillAmount -= 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
        //hpImage.fillAmount = GameManager.instance.playerHP;
    }
}
