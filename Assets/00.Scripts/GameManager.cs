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
        SetResolution();
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

    /* 해상도 설정하는 함수 */
    public void SetResolution()
    {
        //int setWidth = 1284; // 사용자 설정 너비
        //int setHeight = 2278; // 사용자 설정 높이

        //int deviceWidth = Screen.width; // 기기 너비 저장
        //int deviceHeight = Screen.height; // 기기 높이 저장

        //Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        //if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        //{
        //    float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
        //    Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        //}
        //else // 게임의 해상도 비가 더 큰 경우
        //{
        //    float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
        //    Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        //}
    }

    public void LoginSuccese()
    {
        FirebaseManager.instance.GetBestScore(EndGetMyScore);


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

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetResolution();
        }
#endif
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
        startCanvas.SetActive(false);
        player.state = STATE.Ready;
    }

    void EndGetMyScore()
    {
        loginPanel.SetActive(false);
        rankingPanel.SetActive(false);
        startPanel.SetActive(true);
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
