using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    int dieCnt;
    [HideInInspector]
    public  STATE state = STATE.Ready;


    float score;
    float point = 55.25f;
    [SerializeField]
    TMPro.TMP_Text scoreText;
    [SerializeField]
    TMPro.TMP_Text lifeText;
    [SerializeField]
    TMPro.TMP_Text fScore;
    //[SerializeField]
    List<Transform> playerAndHearts = new List<Transform>();
    [SerializeField]
    GameObject heartPool;
    SpriteRenderer animSprite;
    [SerializeField]
    Transform subwayPool;
    Transform[] subways;

    private void Start()
    {
        fScore.text = "";
        animSprite = GetComponentInChildren<SpriteRenderer>();
        playerAndHearts.Add(transform);
        inversedTime = inverseTime;
        heartPos = positions.GetComponentsInChildren<Transform>();
        subways = subwayPool.GetComponentsInChildren<Transform>();
        
    }

    private void Update()
    {
        switch (state)
        {
            case (STATE.Ready):
                Ready();
                break;
            case (STATE.Start):
                break;
            case (STATE.Play):
                Move();
                InverseTime();
                break;
            case (STATE.Die):
                break;
            case (STATE.GameOver):
                break;
        }
    }

    private void LateUpdate()
    {
        if(playerAndHearts.Count - 1 > 0)
        playerAndHearts[playerAndHearts.Count - 1].transform.rotation = Quaternion.identity;
    }

    void Ready()
    {
        lifeText.text = "0";
        scoreText.text = "0";
        initPos();
        DestroyAllHearts();
        CreateHeart();
        dieCnt = 0;
        state = STATE.Play;
    }


    int inverse = 1;
    public void Move()
    {
        for (int i = 0; i < playerAndHearts.Count - 1; i++)
        {
            playerAndHearts[i].position += playerAndHearts[i].forward * GameManager.instance.Speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(PerRot(0, -1 * inverse));
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(PerRot(0, 1 * inverse));
        }
    }

   
    IEnumerator PerRot(int petIdx, int right)
    {
        if (petIdx < playerAndHearts.Count - 1)
        {
            playerAndHearts[petIdx].rotation *= Quaternion.Euler(new Vector3(0, right * 45, 0));
            if (++petIdx < playerAndHearts.Count - 1)
            {
                yield return new WaitForSeconds(GameManager.instance.Interval);
                StartCoroutine(PerRot(petIdx, right));
            }
        }
    }
    void initPos()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
    //Transform last;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyAllHearts();
            dieCnt++;
            lifeText.text = (GameManager.instance.Life - dieCnt).ToString();

            if (dieCnt < GameManager.instance.Life)
            {
                CreateHeart();
                initPos();
            }
            else
            {
                GameOver();
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Heart"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Default");
            if (playerAndHearts.Count == 1)
            {
                other.transform.position = transform.position - transform.forward * GameManager.instance.Speed * GameManager.instance.Interval;
                other.transform.rotation = transform.rotation;
            }
            else
            {
                Transform lastPet = playerAndHearts[playerAndHearts.Count - 2];
                other.transform.position = lastPet.position - lastPet.forward * GameManager.instance.Speed * GameManager.instance.Interval;
                other.transform.rotation = lastPet.rotation;
            }
            score += point;
            scoreText.text = score.ToString();
            CreateHeart();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("RedCapsule"))
        {
            inversedTime = 0;
            inverse = -1;
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Subway"))
        {
            int subwayIdx = Random.Range(1, subways.Length);
           while(other.transform == subways[subwayIdx])
            {
                 subwayIdx = Random.Range(1, subways.Length);
            }
            subways[subwayIdx].gameObject.layer = LayerMask.NameToLayer("SelectedSubway");
            StopAllCoroutines();
            StartCoroutine(PerSubway(0, subways[subwayIdx]));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SelectedSubway"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Subway");
        }
    }
    IEnumerator PerSubway(int petIdx, Transform pos)
    {
        if (petIdx < playerAndHearts.Count - 1)
        {
            playerAndHearts[petIdx].position = pos.position;
            playerAndHearts[petIdx].rotation = pos.rotation;
            if (++petIdx < playerAndHearts.Count - 1)
            {
                yield return new WaitForSeconds(GameManager.instance.Interval);
                StartCoroutine(PerSubway(petIdx, pos));
            }
        }
    }
    float inverseTime = 5f;
    float inversedTime;
    void InverseTime()
    {
        if (inversedTime >= inverseTime)
        {
            animSprite.color = new Color(1, 1, 1, 1);
            inverse = 1;
        }
        else
        {
            if (inversedTime % 0.5f < 0.25f)
                animSprite.color = new Color(1, 1, 1, 0.5f);
            else
                animSprite.color = new Color(1, 1, 1, 1);
            inversedTime += Time.deltaTime;
        }
    }


    void DestroyAllHearts()
    {
        for (int i = 1; i < playerAndHearts.Count; i++)
        {
            Destroy(playerAndHearts[i].gameObject);
        }
        playerAndHearts.Clear();
        playerAndHearts.Add(transform);
    }

    [SerializeField]
    Transform positions;
    Transform[] heartPos;

    int beforePosIndx = 0;
    int idx;
    void CreateHeart()
    {
            idx = Random.Range(1, heartPos.Length);
        while (idx == beforePosIndx)
        {
            idx = Random.Range(1, heartPos.Length);
        }

        print(idx);

        Transform heartPrefab = heartPool.transform.GetChild(0);
        heartPrefab.SetParent(null, true);
        heartPrefab.position = heartPos[idx].position;
        heartPrefab.rotation = Quaternion.identity;
        playerAndHearts.Add(heartPrefab.transform);
        GameManager.instance.nowTarget = heartPrefab;
    }

    private void GameOver()
    {
        state = STATE.GameOver;
        fScore.text = score.ToString();
        GameManager.instance.startPanel.SetActive(true);
        print("GameOver");
    }

}
