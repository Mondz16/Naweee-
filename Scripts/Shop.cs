using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerSkins
{
    public string skinName;
    public Sprite design;
    public Color color;

    public PlayerSkins(string skinName, Sprite design,Color color)
    {
        this.skinName = skinName;
        this.design = design;
        this.color = color;
    }
}

[System.Serializable]
public class BoostShop
{
    public string name;
    public int boostLevel;
}


public class Shop : MonoBehaviour
{
    public Image[] skinPreview;
    public Image[] colorButtons;
    public Image selectBtn;

    public PlayerSkins[] playerSkins;
    public BoostShop[] boostShop;
    public GameObject[] boostLvlHandler;
    public Scrollbar previewScrollbar;
    public TextMeshProUGUI[] coinTxt;

    public int turboCost;
    public int slowmoCost;
    public TextMeshProUGUI turboCostTxt;
    public TextMeshProUGUI slowmoCostTxt;
    // Start is called before the first frame update
    void Start()
    {
        turboCost = 50 *(GameManager.Instance.playerData.boosts[0].boostLevel + 1);
        turboCostTxt.text = turboCost.ToString();
        slowmoCost = 50 * (GameManager.Instance.playerData.boosts[1].boostLevel + 1);
        slowmoCostTxt.text = slowmoCost.ToString();
        for (int i = 0; i < coinTxt.Length; i++) coinTxt[i].text = PlayerPrefs.GetInt("Coins").ToString();
        for (int i = 0; i < boostShop.Length; i++) BoostLevelValue(i);
    }

    private void BoostLevelValue(int a)
    {
        switch (a)
        {
            case 0:
                GameManager.Instance.playerData.boosts[0].boostLevel = PlayerPrefs.GetInt("Turbo");
                for (int i = 0; i < GameManager.Instance.playerData.boosts[0].boostLevel; i++)
                {
                    Color temp = boostLvlHandler[0].transform.GetChild(i).GetComponent<Image>().color;
                    temp.a = 1;
                    boostLvlHandler[0].transform.GetChild(i).GetComponent<Image>().color = temp;
                }

                if (GameManager.Instance.playerData.boosts[0].boostLevel >= 5)
                {
                    boostLvlHandler[0].transform.parent.gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.red;
                    boostLvlHandler[0].transform.parent.gameObject.transform.GetChild(1).transform.GetChild(0).
                        GetComponent<TextMeshProUGUI>().text = "Max";
                    boostLvlHandler[0].transform.parent.gameObject.transform.GetChild(1).transform.GetChild(0).
                        GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                break;
            case 1:
                GameManager.Instance.playerData.boosts[1].boostLevel = PlayerPrefs.GetInt("Slowmo");
                for (int i = 0; i < GameManager.Instance.playerData.boosts[1].boostLevel; i++)
                {
                    Color temp = boostLvlHandler[1].transform.GetChild(i).GetComponent<Image>().color;
                    temp.a = 1;
                    boostLvlHandler[1].transform.GetChild(i).GetComponent<Image>().color = temp;
                }

                if (GameManager.Instance.playerData.boosts[1].boostLevel >= 5)
                {
                    boostLvlHandler[1].transform.parent.gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.red;
                    boostLvlHandler[1].transform.parent.gameObject.transform.GetChild(1).transform.GetChild(0).
                        GetComponent<TextMeshProUGUI>().text = "Max";
                    boostLvlHandler[1].transform.parent.gameObject.transform.GetChild(1).transform.GetChild(0).
                        GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                break;
            default:
                break;
        }
    }

    public void UpgradeTurbo()
    {
        if (GameManager.Instance.playerData.coin >= turboCost)
        {
            if (GameManager.Instance.playerData.boosts[0].boostLevel < 5)
            {
                GameManager.Instance.playerData.boosts[0].boostLevel += 1;

                PlayerPrefs.SetInt("Turbo", GameManager.Instance.playerData.boosts[0].boostLevel);
                BoostLevelValue(0);
                PlayerPrefs.SetInt("Coins", GameManager.Instance.playerData.coin - turboCost);
                GameManager.Instance.playerData.coin = PlayerPrefs.GetInt("Coins");
                turboCost *= (GameManager.Instance.playerData.boosts[0].boostLevel + 1);
                turboCostTxt.text = turboCost.ToString();

                for (int i = 0; i < coinTxt.Length; i++) coinTxt[i].text = GameManager.Instance.playerData.coin.ToString();

            }
        }
    }

    public void UpgradeSlowmo()
    {
        if (GameManager.Instance.playerData.coin >= slowmoCost)
        {
            if (GameManager.Instance.playerData.boosts[1].boostLevel < 5)
            {
                GameManager.Instance.playerData.boosts[1].boostLevel += 1;

                PlayerPrefs.SetInt("Slowmo", GameManager.Instance.playerData.boosts[1].boostLevel);
                BoostLevelValue(1);
                PlayerPrefs.SetInt("Coins", GameManager.Instance.playerData.coin - slowmoCost);
                GameManager.Instance.playerData.coin = PlayerPrefs.GetInt("Coins"); 
                slowmoCost *= (GameManager.Instance.playerData.boosts[1].boostLevel + 1);
                slowmoCostTxt.text = slowmoCost.ToString();

                for (int i = 0; i < coinTxt.Length; i++) coinTxt[i].text = GameManager.Instance.playerData.coin.ToString();

            }
        }
    }

    public void changeSkin(int i)
    {
        for (int a = 0; a < skinPreview.Length; a++)
        {
            if (skinPreview[a].color != colorButtons[i].color)
            {
                skinPreview[a].color = colorButtons[i].color;
            }
        }

        if (GameManager.Instance.playerData.playerSkin.color != colorButtons[i].color) selectSkin(false);
        else selectSkin(true);
    }

    public void selectSkin(bool t)
    {
        if (t)
        {
            Color temp = selectBtn.color;
            temp.a = 0;
            selectBtn.color = temp;
            Debug.Log(selectBtn.color.a);
            selectBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            selectBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Selected";
            if (previewScrollbar.value > .75f)
            {
                playerSkins[2].color = skinPreview[2].color;
                GameManager.Instance.playerData.playerSkin = playerSkins[2];
            }
            else if (previewScrollbar.value < .75f && previewScrollbar.value > .45f)
            {
                playerSkins[1].color = skinPreview[1].color;
                GameManager.Instance.playerData.playerSkin = playerSkins[1];
            }
            else if (previewScrollbar.value < .45)
            {
                playerSkins[0].color = skinPreview[0].color;
                GameManager.Instance.playerData.playerSkin = playerSkins[0];
            }
        }
        else
        {
            Color temp = selectBtn.color;
            temp.a = 1;
            selectBtn.color = temp; 
            selectBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            selectBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Select";
        }
    }
}
