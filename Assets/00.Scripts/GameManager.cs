using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum STATE
{
    Ready,
    Start,
    Play,
    Defualt,
    CamMove,
    Tornado,
    Subway,
    Die,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        Screen.SetResolution(1080, 1920, false);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float Speed { get { return speed; }  set { speed = value; } }
    public float baseSpeed { get { return 5; } }
    public float Interval { get { return interval; } }
    public int LifeCnt { get { return lifeCnt; } }
    public int TornadoCnt { get { return tornadoCnt; } }
    //public STATE State { set { state = value; }  get { return state; } }
    //public GameObject loginPanel;

    public Player player;
    [SerializeField]
    float speed = 2f;
    int lifeCnt = 3;
    int tornadoCnt = 3;
    float interval = 1.5f;
    public int bestScore;

    [SerializeField]
    Image arrow;
    public Transform nowTarget;

    [SerializeField]
    Transform[] springPositions;
    Dictionary<Transform, Transform[]> springPos = new Dictionary<Transform, Transform[]>();
    //public FirebaseManager loginManager;

    public bool lastColl;

    Dictionary<Transform, int> before = new Dictionary<Transform, int>();
    [SerializeField]
    Transform[] springs;

    public string userId;
    public string password;
    [SerializeField]
    GameObject startCanvas;
    [SerializeField]
    GameObject startPanel;
    [SerializeField]
    GameObject loginPanel;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.state = STATE.Ready;

        for(int i = 0; i < springs.Length; i++)
        {
            if(springPos.ContainsKey(springs[i]) == false)
            {
                before.Add(springs[i], -1);
                springPos.Add(springs[i], springPositions[i].GetComponentsInChildren<Transform>());
                SetSpring(springs[i]);
            }
        }
        startPanel.SetActive(false);
        Obj.SetActive(false);
        signUpObj.SetActive(false);
        loginObj.SetActive(false);
        rankingPanel.SetActive(false);
        loginPanel.SetActive(true);
        FirebaseManager.instance.AutoLogin();
    }
    public void LoginSuccese()
    {
        loginPanel.SetActive(false);
        rankingPanel.SetActive(false);
        startPanel.SetActive(true);
    }
    public void SetSpring(Transform spring)
    {
        int idx = Random.Range(1, springPos[spring].Length);
        while (idx == before[spring])
        {
            idx = Random.Range(1, springPos[spring].Length);
        }

        spring.position = springPos[spring][idx].position;
        spring.rotation = Quaternion.identity;
        before[spring] = idx;
    }


    // Update is called once per frame
    void Update()
    {
        if (nowTarget)
        {
            Vector3 dir = nowTarget.position - player.transform.position;
            int i = 1;
            if (dir.x > 0)
            {
                i = -1;
            }
            float ang = Vector3.Angle(dir, Vector3.forward);
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, ang * i));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    private void LateUpdate()
    {
        player.cam.transform.up = Vector3.forward;
    }
    [HideInInspector]
    public bool turnLeft;
    [HideInInspector]
    public bool turnRight;
    [HideInInspector]
    public bool tornado;
    public void OnClickLeftBtn()
    {
        turnLeft = true;
    }
    public void OnClickRightBtn()
    {
        turnRight = true;
    }
    public void OnClickTornado()
    {
        tornado = true;
    }

    [SerializeField]
    GameObject signUpObj;
    [SerializeField]
    GameObject loginObj;
    [SerializeField]
    GameObject rankingPanel;
    [SerializeField]
    GameObject Obj;
    public void OnClick_SignUpBtn()
    {
        Obj.SetActive(true);
        loginObj.SetActive(false);
        signUpObj.SetActive(true);
    }

    public void OnClick_LogInBtn()
    {
        Obj.SetActive(true);
        signUpObj.SetActive(false);
        loginObj.SetActive(true);
    }

    public void OnClick_PlayBtn()
    {
        FirebaseManager.instance.GetBestScore(EndGetMyScore);
    }

    void EndGetMyScore()
    {

        startCanvas.SetActive(false);
        player.state = STATE.Ready;
    }



    public void GameOver()
    {
        startCanvas.SetActive(true);
    }

    public void OnClick_CloseBtn()
    {
        Obj.SetActive(false);
    }
}
