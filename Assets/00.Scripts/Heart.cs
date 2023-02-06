using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TurnInfo 
{
    public Vector3 position;
    public Vector3 forword;
    
    public TurnInfo(Vector3 position, Vector3 forword)
    {
        this.position = position;
        this.forword = forword;
    }
}

public class Heart : MonoBehaviour
{

    public Queue<TurnInfo> targets = new Queue<TurnInfo>();
    public Transform beforeHeart;
    TurnInfo target = new TurnInfo(Vector3.one * -1, Vector3.forward);

    private void Update()
    {
    }

    private void LateUpdate()
    {
        if (target.position == Vector3.one * -1)
        {
            if (targets.Count > 0)
            {
                target = targets.Dequeue();
            }
        }

        if (target.position == Vector3.one * -1)
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, GameManager.instance.Speed * Time.deltaTime);
        //transform.position += transform.forward * GameManager.instance.Speed * Time.deltaTime;
        else
            transform.position = Vector3.MoveTowards(transform.position, target.position, GameManager.instance.Speed * Time.deltaTime);

        if (transform.position == target.position)
        {
            transform.forward = target.forword;
            if (targets.Count > 0)
            {
                target = targets.Dequeue();
            }
            else
                target.position = Vector3.one * -1;
        }
    }
}