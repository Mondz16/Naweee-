using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string objName;
        public GameObject obj;
        public int size;
    }

    public Pool[] pool;
    public Dictionary<string, Queue<GameObject>> objectPool;

    public Transform[] spawnPoints;
    public float speed;
    public float timer;
    public float timeBeforeSpawn;
    public float timeBetweenSpawn;
    public bool started = false;
    bool snowBoost = false;
    private int gemCounter;
    [SerializeField]float timerBeforeEvent = 50;
    public int ChangeRotation = 0;
    float timerBeforeBoostDisappear = 10;

    #region Singleton
    private static ObjectHandler _instance;

    public static ObjectHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void Start()
    {
        if (pool.Length >= 1)
        {
            objectPool = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pool)
            {
                Queue<GameObject> objPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject objSpawned = Instantiate(pool.obj);
                    objSpawned.SetActive(false);
                    objPool.Enqueue(objSpawned);
                }

                objectPool.Add(pool.objName, objPool);
            }

            StartCoroutine(TimeBeforeSpawn());
        }
    }

    public GameObject ObjectToSpawn(string tag)
    {
        if (!objectPool.ContainsKey(tag)) return null;

        int temp = Random.Range(0, 50);
        GameObject objectToSpawn = objectPool[tag].Dequeue();

        if (objectToSpawn.name != "CoinObject(Clone)")
        {
            objectToSpawn.GetComponent<SpriteRenderer>().color =
               new Color(
               GameManager.Instance.background.GetComponent<SpriteRenderer>().color.r,
               GameManager.Instance.background.GetComponent<SpriteRenderer>().color.g,
               GameManager.Instance.background.GetComponent<SpriteRenderer>().color.b,
               255);
        }
        objectToSpawn.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

        if (temp <= 40 && objectToSpawn.name == "CoinObject(Clone)") objectToSpawn.SetActive(false);
        else objectToSpawn.SetActive(true);
        objectPool[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    IEnumerator TimeBeforeSpawn()
    {
        yield return new WaitForSeconds(timeBeforeSpawn);
        started = true;
    }

    private void Update()
    {
        if (started == true)
        {
            if (GameManager.Instance.playerData.boosts[0].boostLevel > 0 || GameManager.Instance.playerData.boosts[1].boostLevel > 0)
            {
                if (timerBeforeBoostDisappear <= 0)
                {
                    UIManager.Instance.turboBoost.SetActive(false);
                    if (!UIManager.Instance.effectTimeSlider.gameObject.activeInHierarchy && snowBoost == false 
                        && GameManager.Instance.playerData.boosts[1].boostLevel > 0)
                    {
                        UIManager.Instance.slowmoBoost.SetActive(true);
                        snowBoost = true;
                    }
                }
                else timerBeforeBoostDisappear -= 1;
            }

            if (timer <= 0)
            {
                if (UIManager.Instance.score >= 100)
                {
                    if (timerBeforeEvent >= 1) timerBeforeEvent -= 1;
                    else
                    {
                        if (speed <= 15) speed += .2f;
                        float temp = 100 + (UIManager.Instance.score * .5f);
                        timerBeforeEvent = temp;
                    }

                    //Debug.Log(timerBeforeEvent.ToString());

                    if (timerBeforeEvent <= 0) ChangeRotation = Random.Range(0,3);

                    GameManager.Instance.OnChangeRotationObjects();
                }

                ObjectToSpawn(pool[Random.Range(0, pool.Length)].objName);
                UIManager.Instance.ScoreCount();
                UIManager.Instance.ExpCount();

                if (gemCounter >= 10)
                {
                    UIManager.Instance.coinsPerObjects += 1;
                    gemCounter = 0;
                }
                else gemCounter += 1;

                timer = timeBetweenSpawn;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
