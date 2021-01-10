using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data" , menuName = "DataBank")]
public class PlayerDataBank : ScriptableObject
{
    public string Username;
    public Difficulty userDifficulty;

    public PlayerSkins playerSkin;
    public BoostShop[] boosts;
    public int level;
    public int skillPoints;
    public float experience;
    public int coin;
    public int highScore;
}
