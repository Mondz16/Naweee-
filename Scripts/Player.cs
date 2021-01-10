using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private Color[] destroyEffectColor;
    [SerializeField]private ParticleSystem deathEffect;
    [SerializeField] private ParticleSystem[] particleEffect;
    [SerializeField] private SpriteRenderer skin;

    private void Start()
    {
        skin.sprite = GameManager.Instance.playerData.playerSkin.design;
        skin.color = GameManager.Instance.playerData.playerSkin.color;
        var particleSpeed1 = particleEffect[0].main;
        var particleSpeed2 = particleEffect[1].main;
        particleSpeed1.startSpeed = ObjectHandler.Instance.speed * 10;
        particleSpeed2.startSpeed = ObjectHandler.Instance.speed * 10;
        particleSpeed1.startColor = GameManager.Instance.playerData.playerSkin.color;
        Color temp = particleSpeed1.startColor.color;
        temp.a = .5f;
        particleSpeed1.startColor = temp;
        particleSpeed2.startColor = GameManager.Instance.playerData.playerSkin.color;
        destroyEffectColor[0] = particleSpeed1.startColor.color;
        destroyEffectColor[1] = particleSpeed2.startColor.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 direction = Camera.main.ScreenToWorldPoint(touch.position) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (direction.x >= 1)
                GameManager.Instance.PlayerMovement(gameObject, rotation, direction);
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, .1f);
                Debug.Log("Not greater than 1");
            }
        }
        else
        {
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (direction.x >= 1)
                GameManager.Instance.PlayerMovement(gameObject, rotation, direction);
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, .1f);
                Debug.Log("Not greater than 1");
            }
        }
    }

    public void Dead()
    {
        var ps = deathEffect.main;
        var ps1 = deathEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        ps.startColor = destroyEffectColor[0];
        ps1.startColor = destroyEffectColor[1];
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
