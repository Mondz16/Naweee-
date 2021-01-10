using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    private void Update()
    {
        GameManager.Instance.ObstacleMovement(ObjectHandler.Instance.speed, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Stopper")
        {
            gameObject.SetActive(false);
            gameObject.transform.position = ObjectHandler.Instance.spawnPoints
                [Random.Range(0, ObjectHandler.Instance.spawnPoints.Length)].position;
        }

        if (collision.tag == "Player" && this.name != "CoinObject(Clone)")
        {
            GameManager.Instance.GameOver();
            gameObject.transform.position = ObjectHandler.Instance.spawnPoints
                [Random.Range(0, ObjectHandler.Instance.spawnPoints.Length)].position;
        }

        if (collision.tag == "Player" && this.name == "CoinObject(Clone)")
        {
            UIManager.Instance.coinsInGame += 1; 
            UIManager.Instance.coinCounter.text = UIManager.Instance.coinsInGame.ToString();
            GameManager.Instance.audioCoin.Play();
            gameObject.SetActive(false);
        }

        if (collision.tag == "Obstacle")
        {
            gameObject.SetActive(false);
        }
    }
}
