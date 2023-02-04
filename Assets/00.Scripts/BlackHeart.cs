using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHeart : MonoBehaviour
{
    [SerializeField]
    Transform targets;
    Transform[] pos;
    int i;
    // Start is called before the first frame update
    void Start()
    {
        pos = targets.GetComponentsInChildren<Transform>();
        i = 0;
    }
    // Update is called once per frame
    void Update()
    { 
        transform.position = Vector3.MoveTowards(transform.position, pos[i].position, GameManager.instance.Speed * Time.deltaTime);
    }
    private void LateUpdate()
    {
        if (transform.position == pos[i].position)
        {
            i++;
            if (i >= pos.Length)
                i = 0;
        }
    }

}
