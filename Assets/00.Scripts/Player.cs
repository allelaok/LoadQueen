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
    TMPro.TMP_Text scoreText;
    [SerializeField]
    TMPro.TMP_Text bestScoreText;
    [SerializeField]
    Image[] lifeImg;
    [SerializeField]
    Image[] tornadoImg;
    //[SerializeField]
    //List<Transform> PlayerAndHeart = new List<Transform>();
    List<Transform> hearts = new List<Transform>();
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
    //public Transform[] hearts;
    public List<Vector3> points;

    private void Start()
    {
        state = STATE.Defualt;
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
                SetHeartsPosition(1, 0);
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
    }

    private void LateUpdate()
    {
    }

    void Ready()
    {
        tornado.gameObject.SetActive(false);
        for (int i = 0; i < GameManager.instance.LifeCnt; i++)
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
        bestScoreText.text = GameManager.instance.bestScore.ToString();
        targetIndx.Clear();
        points.Clear();
        points.Add(transform.position);
        score = 0;
        inverse = 1;
        GameManager.instance.Speed = GameManager.instance.baseSpeed;
    }

    //List<Vector3> target = new List<Vector3>();
    List<int> targetIndx = new List<int>();
    public void Move()
    {
        transform.position += transform.forward.normalized * GameManager.instance.Speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A) || GameManager.instance.turnLeft)
        {
            transform.rotation *= Quaternion.Euler(new Vector3(0, -1 * inverse * 45, 0));
            points.Add(transform.position);

            GameManager.instance.turnLeft = false;
        }
        else if(Input.GetKeyDown(KeyCode.D) || GameManager.instance.turnRight)
        {
            transform.rotation *= Quaternion.Euler(new Vector3(0, inverse * 45, 0));
            points.Add(transform.position);

            GameManager.instance.turnRight = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) || GameManager.instance.tornado)
        {
            tornadoCnt--;
            if(tornadoCnt > 0)
            {
                tornadoImg[tornadoCnt].enabled = false;
                points.Clear();
                state = STATE.Tornado;
            }
            GameManager.instance.tornado = false;
        }
    }

    void SetHeartsPosition(int pointIdx, int heartIdx, bool pointDis = true, float tmpDis = 0)
    {
        if(points.Count == 0)
        {
            // ???? ???? ???????? ??????
            for (int i = 0; i < hearts.Count; i++)
                hearts[i].position = Vector3.one * -1;
        }
        else if (points.Count == 1)
        {
            float dis = Vector3.Distance(transform.position, points[0]);
            // ???????? ???????? ???? ???? ????
            for (int i = 0; i < hearts.Count; i++)
            {
                if (dis > GameManager.instance.Interval * (i + 1))
                    hearts[i].position = transform.position - transform.forward * GameManager.instance.Interval * (i + 1);
            }
        }
        else if (points.Count > pointIdx - 1 && hearts.Count > heartIdx)
        {
            // ???? ???????? ???? ???????? ????
            float dis;
            if (pointDis)
            {
                if (pointIdx == 1)
                    dis = Vector3.Distance(transform.position, points[points.Count - pointIdx]);
                else
                    dis = Vector3.Distance(points[points.Count - pointIdx + 1], points[points.Count - pointIdx]);
            }
            else
            {
                dis = Vector3.Distance(hearts[heartIdx - 1].position, points[points.Count - pointIdx]);
            }

            // ?????? ?????? ????
            if (dis < GameManager.instance.Interval - tmpDis)
            {
                pointIdx++;
                SetHeartsPosition(pointIdx, heartIdx, true, dis + tmpDis);
            }
            // ?????? ?????? ????
            else
            {
                Vector3 pos;
                Vector3 forword;
                if (pointDis)
                {
                    if (pointIdx == 1)
                    {
                        pos = Vector3.Lerp(transform.position, points[points.Count - pointIdx], (GameManager.instance.Interval - tmpDis) / dis);
                        forword = transform.position - points[points.Count - pointIdx];
                    }
                    else
                    {
                        pos = Vector3.Lerp(points[points.Count - pointIdx + 1], points[points.Count - pointIdx], (GameManager.instance.Interval - tmpDis) / dis);
                        forword = points[points.Count - pointIdx + 1] - points[points.Count - pointIdx];

                    }
                }
                else
                {
                    pos = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * 0.5f;
                    forword = hearts[heartIdx - 1].forward;
                }

                // ???? ????
                hearts[heartIdx].position = pos;
                hearts[heartIdx].forward = forword;
                heartIdx++;
                if (hearts.Count > heartIdx)
                    SetHeartsPosition(pointIdx, heartIdx, false);
                else
                {
                    // ?????? ?????? ????
                    for (int i = points.Count - pointIdx - 1; i > -1; i--)
                    {
                        points.RemoveAt(i);
                    }
                }
            }
        }
    }
    void Tornado()
    {
        if (tornado.gameObject.activeSelf == false)
            tornado.gameObject.SetActive(true);
        transform.Rotate(new Vector3(0, 300 * Time.deltaTime, 0));
        transform.localScale -= Vector3.one * GameManager.instance.baseSpeed * Time.deltaTime;
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
        target = subways[subwayIdx];
        state = STATE.CamMove;
    }

    Transform target;
    void CamMove()
    {
        transform.position += (target.position - transform.position).normalized * GameManager.instance.Speed * 10 * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            transform.forward = target.forward;
            points.Add(target.position);
            transform.GetComponentInChildren<SpriteRenderer>().enabled = true;
            state = STATE.Play;
        }
    }

    void initPos()
    {
        inverse = 1;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != STATE.Play) return;
        // ???? ????
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyAllHearts();
            dieCnt++;
            lifeImg[GameManager.instance.LifeCnt - dieCnt].enabled = false;
            GameManager.instance.Speed = GameManager.instance.baseSpeed;

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
        // ????
        else if (other.gameObject.layer == LayerMask.NameToLayer("Heart"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Wall");
            print(other.gameObject.name);
            hearts.Add(other.transform);

            // ???? ????
            score += point;
            GameManager.instance.Speed = GameManager.instance.baseSpeed + (hearts.Count + 1) * 0.2f;
            print(GameManager.instance.Speed);
            if (score > GameManager.instance.bestScore)
            {
                GameManager.instance.bestScore = score;
                bestScoreText.text = score.ToString();
            }
            scoreText.text = score.ToString();

            CreateHeart();
        }
        // ????
        else if (other.gameObject.layer == LayerMask.NameToLayer("RedCapsule"))
        {
            inversedTime = 0;
            inverse = -1;
            anim.SetInteger("Inverse", inverse);
            GameManager.instance.SetSpring(other.transform);
        }
        // ??????
        else if (other.gameObject.layer == LayerMask.NameToLayer("Subway"))
        {
            points.Clear();
            SelectSubway(other.transform);
        }
        // ????
        else if (other.gameObject.layer == LayerMask.NameToLayer("Stair"))
        {
            Stair stair = other.transform.GetComponent<Stair>();
            stair.otherStair.gameObject.layer = LayerMask.NameToLayer("SelectedStair");
            transform.GetComponentInChildren<SpriteRenderer>().enabled = false;
            target = stair.otherStair.transform;
            points.Clear();
            state = STATE.CamMove;
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

    float inverseTime = 5f;
    float inversedTime;
    void InverseTime()
    {
        if (inverse == 1) 
        {
            inverseTime = 5f;
            animSprite.color = new Color(1, 1, 1, 1);
            //anim.SetInteger("Inverse", inverse);
            return;
        }

        inversedTime += Time.deltaTime;

        if (inversedTime >= inverseTime)
        {
            animSprite.color = new Color(1, 1, 1, 1);
            inverse = 1;
            //anim.SetInteger("Inverse", inverse);
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
        for (int i = 0; i < hearts.Count; i++)
        {
            Destroy(hearts[i].gameObject);
        }
        if (nowHeart)
        {
            Destroy(nowHeart.gameObject);
        }
        hearts.Clear();
    }

    [SerializeField]
    Transform heartPositions;
    Transform[] heartPos;

    int beforePosIndx = 0;
    int idx;
    GameObject nowHeart;
    void CreateHeart()
    {
        idx = Random.Range(1, heartPos.Length);
        while (idx == beforePosIndx)
        {
            idx = Random.Range(1, heartPos.Length);
        }
        beforePosIndx = idx;
        Transform heartPrefab = heartPool.transform.GetChild(0);
        heartPrefab.SetParent(null, true);
        heartPrefab.position = heartPos[idx].position;
        heartPrefab.rotation = Quaternion.identity;
        nowHeart = heartPrefab.gameObject;
        GameManager.instance.nowTarget = heartPrefab;
    }

  
    private void GameOver()
    {
        state = STATE.GameOver;
        GameManager.instance.GameOver();
        FirebaseManager.instance.SaveScore(score);
        inverse = 1;
        print("GameOver");
    }

}
