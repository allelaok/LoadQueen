using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public Camera cam;
    public Transform[] hearts;
    public List<Vector3> points;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * 2 * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation *= Quaternion.Euler(new Vector3(0, -45, 0));
            points.Add(transform.position);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation *= Quaternion.Euler(new Vector3(0, 45, 0));
            points.Add(transform.position);
        }
    }

    private void LateUpdate()
    {
        SetHeartsPosition(1, 0);
        cam.transform.up = Vector3.forward;
    }

    void SetHeartsPosition(int pointIdx, int heartIdx, bool pointDis = true, float tmpDis =0)
    {
        if (points.Count == 1)
        {
            // 플레이어 위치부터 모든 하트 나열
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].position = transform.position - transform.forward * 2 * (i + 1) * 0.5f;
            }

        }
        else if (points.Count > pointIdx - 1 && hearts.Length > heartIdx)
        {
            // 현재 포인트와 다음 포인트의 거리
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

            // 거리가 부족한 경우
            if (dis < 2 * 0.5f - tmpDis)
            {
                pointIdx++;
                SetHeartsPosition(pointIdx, heartIdx, true, dis + tmpDis);
            }
            // 거리를 충족한 경우
            else
            {
                Vector3 pos;
                Vector3 forword;
                if (pointDis)
                {
                    if (pointIdx == 1)
                    {
                        pos = Vector3.Lerp(transform.position, points[points.Count - pointIdx], (2 * 0.5f - tmpDis) / dis);
                        forword = transform.position - points[points.Count - pointIdx];
                    }
                    else
                    {
                        pos = Vector3.Lerp(points[points.Count - pointIdx + 1], points[points.Count - pointIdx], (2 * 0.5f - tmpDis) / dis);
                        forword = points[points.Count - pointIdx + 1] - points[points.Count - pointIdx];

                    }
                }
                else
                {
                    pos = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * 0.5f;
                    forword = hearts[heartIdx - 1].forward;
                }

                // 하트 배치
                hearts[heartIdx].position = pos;
                hearts[heartIdx].forward = forword;
                heartIdx++;
                if (hearts.Length > heartIdx)
                    SetHeartsPosition(pointIdx, heartIdx, false);
                else
                {
                    // 나머지 포인트 삭제
                    for(int i = points.Count - pointIdx -1; i > -1; i--)
                    {
                        points.RemoveAt(i);
                    }
                }
            }
        }
    }


    void Test(int pointIdx, int heartIdx)
    {
        //int pointIdx = 1;
        //int heartIdx = 0;
        // 다음 포인트가 있는 경우
        if (points.Count > pointIdx)
        {
            // 플레이어와 마지막 포인트와의 거리
            float dis;
            if (pointIdx == 1)
                dis = Vector3.Distance(transform.position, points[points.Count - pointIdx]);
            else
                dis = Vector3.Distance(points[points.Count - pointIdx + 1], points[points.Count - pointIdx]);
            // 거리가 부족한 경우
            if (dis < 2 * 0.5f)
            {
                pointIdx++;
                Test(pointIdx, heartIdx);
                #region
                //if (points.Count > pointIdx)
                //{
                //    // 다음 포인트와의 거리
                //    float dis2 = Vector3.Distance(points[points.Count - pointIdx + 1], points[points.Count - pointIdx]);
                //    // 거리가 부족한 경우
                //    // 거리가 충분한 경우
                //}
                //// 다음 포인트가 없는 경우
                //else
                //{
                //    // 하트가 존재하는경우
                //    if (hearts.Length > heartIdx)
                //    {
                //        // 첫번째 하트 나열
                //        hearts[heartIdx].position = points[points.Count - pointIdx + 1] - (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]).normalized * (2 * 0.5f - dis1);
                //        hearts[heartIdx].forward = (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]);
                //        heartIdx++;
                //        // 첫번째 하트 기준으로 나머지 하트 나열
                //        for (int i = heartIdx; i < hearts.Length; i++)
                //        {
                //            hearts[i].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * (i + 1 - heartIdx) * 0.5f;
                //        }
                //    }
                //    // 하트가 없는 경우
                //    else
                //    {
                //        return;
                //    }

                //}
                #endregion
            }
            // 거리가 충분한 경우
            else
            {
                // 첫번째 하트 배치
                if (heartIdx == 0)
                    hearts[heartIdx].position = transform.position - transform.forward * 2 * 0.5f;
                else
                    hearts[heartIdx].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * 0.5f;
                heartIdx++;
                Test(pointIdx, heartIdx);
            }
        }
        // 다음 포인트가 없는 경우
        else
        {
            //if (pointIdx == 1)
            //{
            // 플레이어 위치부터 모든 하트 나열
            for (int i = pointIdx - 1; i < hearts.Length; i++)
            {
                if (pointIdx < 1)
                    hearts[i].position = transform.position - transform.forward * 2 * (i + 1) * 0.5f;
            }
            //}
            //else
            //{
            //    // 하트가 존재하는경우
            //    if (hearts.Length > heartIdx)
            //    {
            //        // 첫번째 하트 나열
            //        hearts[heartIdx].position = points[points.Count - pointIdx + 1] - (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]).normalized * (2 * 0.5f - dis1);
            //        hearts[heartIdx].forward = (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]);
            //        heartIdx++;
            //        // 첫번째 하트 기준으로 나머지 하트 나열
            //        for (int i = heartIdx; i < hearts.Length; i++)
            //        {
            //            hearts[i].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * (i + 1 - heartIdx) * 0.5f;
            //        }
            //    }
            //    // 하트가 없는 경우
            //    else
            //    {
            //        for (int i = points.Count - pointIdx; i > -1; i--)
            //        {
            //            points.RemoveAt(i);
            //        }
            //    }
            //}

        }
    }

            // 거리가 충분한 경우
    void Test2(int pointIdx, int heartIdx)
    {
        // 하트가 있는경우
        if (hearts.Length > heartIdx)
        {
            // 첫번째 하트 배치
            if (heartIdx == 0)
                hearts[heartIdx].position = transform.position - transform.forward * 2 * 0.5f;
            else
                hearts[heartIdx].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * 0.5f;
            heartIdx++;
            // 첫번째 하트와 현재 포인트와의 거리
            float dis3 = Vector3.Distance(hearts[heartIdx - 1].position, points[points.Count - pointIdx]);
            // 거리가 부족한 경우
            if (dis3 < 2 * 0.5f)
            {
                Test(pointIdx, heartIdx);
            }
            // 거리가 충분한 경우
            else
            {
                Test2(pointIdx, heartIdx);
            }

        }
        // 하트가 없는 경우
        else
        {
            for(int i = points.Count - pointIdx; i > -1; i--)
            {
                points.RemoveAt(i);
            }
        }
    }

}
