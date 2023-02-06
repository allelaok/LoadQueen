using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public STATE state;


    int dieCnt;
    int tornadoCnt;
    int score;
    int point = 1;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text bestScoreText;
    [SerializeField]
    Image[] lifeImg;
    [SerializeField]
    Image[] tornadoImg;
    [SerializeField]
    TMPro.TMP_Text fScore;
    //[SerializeField]
    //List<Transform> PlayerAndHeart = new List<Transform>();
    List<Heart> hearts = new List<Heart>();
    [SerializeField]
    GameObject heartPool;
    SpriteRenderer animSprite;
    [SerializeField]
    Transform subwayPool;
    Transform[] subways;
    [HideInInspector]
    public Camera cam;
    [SerializeField]
    Transform tornado;
    [SerializeField]
    Animator anim;
    Vector3 initSize;
    int inverse = 1;
    int bestScore;

    private void Start()
    {
        state = STATE.Defualt;
        fScore.text = "";
        animSprite = GetComponentInChildren<SpriteRenderer>();
        //PlayerAndHeart.Add(transform);
        inversedTime = inverseTime;
        heartPos = heartPositions.GetComponentsInChildren<Transform>();
        subways = subwayPool.GetComponentsInChildren<Transform>();
        cam = GetComponentInChildren<Camera>();
        //anim = GetComponentInChildren<Animator>();
        initSize = transform.localScale;
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
            case (STATE.CamMove):
                CamMove();
                break;
            case (STATE.Tornado):
                Tornado();
                break;
            case (STATE.Defualt):
                break;
            case (STATE.Die):
                break;
            case (STATE.GameOver):
                break;
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefs.SetInt("BestScore", 0);
        }
    }

    void Ready()
    {
            tornado.gameObject.SetActive(false);
        for(int i = 0; i < GameManager.instance.LifeCnt; i++)
        {
            lifeImg[i].enabled = true;
        }

        for (int i = 0; i < GameManager.instance.TornadoCnt; i++)
        {
            tornadoImg[i].enabled = true;
        }
        scoreText.text = "0";
        initPos();
        DestroyAllHearts();
        CreateHeart();
        tornadoCnt = GameManager.instance.TornadoCnt;
        dieCnt = 0;
        state = STATE.Play;
        bestScore = PlayerPrefs.GetInt("BestScore");
        bestScoreText.text = bestScore.ToString();
        targetIndx.Clear();
        target.Clear();
    }

    List<Vector3> target = new List<Vector3>();
    List<int> targetIndx = new List<int>();
    public void Move()
    {
        transform.position += transform.forward * GameManager.instance.Speed * Time.deltaTime;
        //for (int i = 0; i < hearts.Count; i++)
        //{
        //    hearts[i].transform.position += hearts[i].transform.forward * GameManager.instance.Speed * Time.deltaTime;

            //Vector3 targetPos;
            //if (targetIndx[i] < target.Count)
            //    targetPos = target[targetIndx[i]];
            //else if (targetIndx[i] < 0)
            //    targetPos = hearts[i - 1].position;
            //else
            //    targetPos = transform.position;

            //    hearts[i].position = Vector3.MoveTowards(hearts[i].position, targetPos, GameManager.instance.Speed * Time.deltaTime);

            //if (hearts[i].position == targetPos)
            //{
            //    targetIndx[i]++;
            //    if(i == hearts.Count - 2)
            //    {
            //        for(int j = 0; j < targetIndx.Count; j++)
            //        {
            //            targetIndx[j]--;
            //        }
            //    }
            //}
        //}

        if (Input.GetKeyDown(KeyCode.A) || GameManager.instance.turnLeft)
        {
            transform.rotation *= Quaternion.Euler(new Vector3(0, -1 * inverse * 45, 0));
            //target.Add(transform.position);
            //StartCoroutine(PetRot(0, -1 * inverse));
            for(int i = 0; i < hearts.Count; i++)
            {
                if (i - 1 < 0)
                {
                    TurnInfo info = new TurnInfo(transform.position, transform.forward);
                    hearts[i].targets.Enqueue(info);
                }
            }
            GameManager.instance.turnLeft = false;
        }
        else if(Input.GetKeyDown(KeyCode.D) || GameManager.instance.turnRight)
        {
            //target.Add(transform.position);
            transform.rotation *= Quaternion.Euler(new Vector3(0, inverse * 45, 0));
            for (int i = 0; i < hearts.Count; i++)
            {
                TurnInfo info = new TurnInfo(transform.position, transform.forward);
                hearts[i].targets.Enqueue(info);
            }
            //StartCoroutine(PetRot(0, 1 * inverse));
            GameManager.instance.turnRight = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) || GameManager.instance.tornado)
        {
            tornadoCnt--;
            if(tornadoCnt > 0)
            {
                tornadoImg[tornadoCnt].enabled = false;
                state = STATE.Tornado;
            }
            GameManager.instance.tornado = false;
        }
    }

    
    void Tornado()
    {
        if (tornado.gameObject.activeSelf == false)
            tornado.gameObject.SetActive(true);

        transform.Rotate(new Vector3(0, 300 * Time.deltaTime, 0));
        transform.localScale -= Vector3.one * GameManager.instance.Speed * 0.2f * Time.deltaTime;
        if (transform.localScale.x < 0.1f)
        {
            transform.localScale = initSize;
            tornado.gameObject.SetActive(false);
            SelectSubway();
        }
    }

    void SelectSubway(Transform beforeSubway = null)
    {
        transform.GetComponentInChildren<SpriteRenderer>().enabled = false;
        int subwayIdx = Random.Range(1, subways.Length);
        if (beforeSubway != null)
        {
            while (beforeSubway == subways[subwayIdx])
            {
                subwayIdx = Random.Range(1, subways.Length);
            }
        }
        subways[subwayIdx].gameObject.layer = LayerMask.NameToLayer("SelectedSubway");
        this.subwayIdx = subwayIdx;
        target.Add(subways[subwayIdx].position);

        state = STATE.CamMove;
    }

    int subwayIdx;
    void CamMove()
    {
        transform.position += (subways[subwayIdx].position - transform.position).normalized * GameManager.instance.Speed * 3 * Time.deltaTime;
        if (Vector3.Distance(transform.position, subways[subwayIdx].position) < 0.5f)
        {
            transform.GetComponentInChildren<SpriteRenderer>().enabled = true;
            state = STATE.Play;
            StopAllCoroutines();
            for(int i = 0; i< hearts.Count; i++)
            {
                hearts[i].targets.Clear();
            }
            StartCoroutine(PetSubway(0, subways[subwayIdx]));
        }
    }

    IEnumerator PetRot(int petIdx, int right)
    {
        if (petIdx < hearts.Count)
        {
                yield return new WaitForSeconds(GameManager.instance.Interval);
            hearts[petIdx].transform.rotation *= Quaternion.Euler(new Vector3(0, right * 45, 0));
            if (++petIdx < hearts.Count)
            {
                StartCoroutine(PetRot(petIdx, right));
            }
        }
    }
    void initPos()
    {
            inverse = 1;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
    //Transform last;
    private void OnTriggerEnter(Collider other)
    {
        if (state != STATE.Play) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyAllHearts();
            dieCnt++;
            lifeImg[GameManager.instance.LifeCnt - dieCnt].enabled = false;

            if (dieCnt < GameManager.instance.LifeCnt)
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

            Heart heart = other.gameObject.AddComponent<Heart>();
            if (hearts.Count == 0)
            {
                other.transform.position = transform.position - transform.forward * GameManager.instance.Speed * GameManager.instance.Interval;
                other.transform.rotation = transform.rotation;
                //heart.targets = new Queue<Vector3>();
            }
            else
            {
                Heart lastHeart = hearts[hearts.Count - 1];
                heart.targets = new Queue<TurnInfo>(lastHeart.targets);
                other.transform.position = lastHeart.transform.position - lastHeart.transform.forward * GameManager.instance.Speed * GameManager.instance.Interval;
                other.transform.rotation = lastHeart.transform.rotation;
            }
            hearts.Add(heart);
            score += point;
            if (score > bestScore)
                bestScoreText.text = score.ToString();
            scoreText.text = score.ToString();

            //if (targetIndx.Count - 1 < 0)
            //    targetIndx.Add(target.Count);
            //else
            //    targetIndx.Add(targetIndx[targetIndx.Count - 1] - 1);


            CreateHeart();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("RedCapsule"))
        {
            inversedTime = 0;
            inverse = -1;
            anim.SetInteger("Inverse", inverse);
            GameManager.instance.SetSpring(other.transform);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Subway"))
        {
            SelectSubway(other.transform);
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Stair"))
        {
            Stair stair = other.transform.GetComponent<Stair>();
            stair.otherStair.gameObject.layer = LayerMask.NameToLayer("SelectedStair");
            StopAllCoroutines();
            StartCoroutine(PetSubway(0, stair.otherStair));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SelectedSubway"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Subway");
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("SelectedStair"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Stair");
        }
    }
    IEnumerator Subway(int petIdx, Transform pos)
    {
        transform.position = pos.position;
        transform.rotation = pos.rotation;

        yield return new WaitForSeconds(GameManager.instance.Interval);
        StartCoroutine(PetSubway(petIdx, pos));
    }

    IEnumerator PetSubway(int petIdx, Transform pos)
    {
        yield return new WaitForSeconds(GameManager.instance.Interval);
        if (petIdx < hearts.Count)
        {
            hearts[petIdx].transform.position = pos.position;
            hearts[petIdx].transform.rotation = pos.rotation;
            if (++petIdx < hearts.Count)
            {
                StartCoroutine(PetSubway(petIdx, pos));
            }
        }
    }

    float inverseTime = 5f;
    float inversedTime;
    void InverseTime()
    {
        if (inverse == 1) return;
        inversedTime += Time.deltaTime;

        if (inversedTime >= inverseTime)
        {
            animSprite.color = new Color(1, 1, 1, 1);
            inverse = 1;
            anim.SetInteger("Inverse", inverse);
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
        for (int i = 1; i < hearts.Count; i++)
        {
            Destroy(hearts[i].gameObject);
        }
        hearts.Clear();
    }

    [SerializeField]
    Transform heartPositions;
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

        Transform heartPrefab = heartPool.transform.GetChild(0);
        heartPrefab.SetParent(null, true);
        heartPrefab.position = heartPos[idx].position;
        heartPrefab.rotation = Quaternion.identity;
        GameManager.instance.nowTarget = heartPrefab;
    }

  
    private void GameOver()
    {
        state = STATE.GameOver;
        fScore.text = score.ToString();
        GameManager.instance.loginPanel.SetActive(true);
        //FirebaseManager.instance.SaveScore(score);
        print("GameOver");
        if(score > bestScore)
            PlayerPrefs.SetInt("BestScore", score);
    }

}
