using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform h;
    public class Target
    {
        public Vector3 pos;
        public Target next;
    }

    public static List<Vector3> pos;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
    }
    Vector3 target;
    // Update is called once per frame
    void Update()
    {
        //if (i < pos.Count)
        //    target = pos[i];
        //else
        //    target = GameManager.instance.player.transform.position;

        //h.position = Vector3.MoveTowards(h.position, target, GameManager.instance.Speed * Time.deltaTime);


    }
    bool last;
    private void LateUpdate()
    {
        if (h.position == pos[i])
        {
            i++;
            if (last)
            {

            }
        }

    }
}
