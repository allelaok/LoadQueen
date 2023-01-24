using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum STATE
{
    Ready,
    Start,
    Play,
    Die,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float Speed { get { return speed; } }
    public float Interval { get { return interval; } }
    public int Life { get { return life; } }
    //public STATE State { set { state = value; }  get { return state; } }
    public GameObject startPanel;

    Player player;
    Camera cam;

    float speed = 3f;
    int life = 3;
    float interval = 0.2f;

    [SerializeField]
    Image arrow;
    public Transform nowTarget;

    // Start is called before the first frame update
    void Start()
    {
        startPanel.SetActive(true);
        player = FindObjectOfType<Player>();
        player.state = STATE.Ready;
        cam = player.GetComponentInChildren<Camera>();
    }

    public void OnClick_Start()
    {
        startPanel.SetActive(false);
        player.state = STATE.Ready;
    }

    // Update is called once per frame
    void Update()
    {
        if (nowTarget)
        {
          Vector3 dir =   nowTarget.position- player.transform.position;
            int i = 1;
            if (dir.x > 0)
            {
                i = -1;
            }
           float ang =  Vector3.Angle(dir, Vector3.forward);
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, ang * i));
        }
    }

    private void LateUpdate()
    {
        cam.transform.up = Vector3.forward;
    }
}
