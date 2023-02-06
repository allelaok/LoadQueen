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
            // �÷��̾� ��ġ���� ��� ��Ʈ ����
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].position = transform.position - transform.forward * 2 * (i + 1) * 0.5f;
            }

        }
        else if (points.Count > pointIdx - 1 && hearts.Length > heartIdx)
        {
            // ���� ����Ʈ�� ���� ����Ʈ�� �Ÿ�
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

            // �Ÿ��� ������ ���
            if (dis < 2 * 0.5f - tmpDis)
            {
                pointIdx++;
                SetHeartsPosition(pointIdx, heartIdx, true, dis + tmpDis);
            }
            // �Ÿ��� ������ ���
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

                // ��Ʈ ��ġ
                hearts[heartIdx].position = pos;
                hearts[heartIdx].forward = forword;
                heartIdx++;
                if (hearts.Length > heartIdx)
                    SetHeartsPosition(pointIdx, heartIdx, false);
                else
                {
                    // ������ ����Ʈ ����
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
        // ���� ����Ʈ�� �ִ� ���
        if (points.Count > pointIdx)
        {
            // �÷��̾�� ������ ����Ʈ���� �Ÿ�
            float dis;
            if (pointIdx == 1)
                dis = Vector3.Distance(transform.position, points[points.Count - pointIdx]);
            else
                dis = Vector3.Distance(points[points.Count - pointIdx + 1], points[points.Count - pointIdx]);
            // �Ÿ��� ������ ���
            if (dis < 2 * 0.5f)
            {
                pointIdx++;
                Test(pointIdx, heartIdx);
                #region
                //if (points.Count > pointIdx)
                //{
                //    // ���� ����Ʈ���� �Ÿ�
                //    float dis2 = Vector3.Distance(points[points.Count - pointIdx + 1], points[points.Count - pointIdx]);
                //    // �Ÿ��� ������ ���
                //    // �Ÿ��� ����� ���
                //}
                //// ���� ����Ʈ�� ���� ���
                //else
                //{
                //    // ��Ʈ�� �����ϴ°��
                //    if (hearts.Length > heartIdx)
                //    {
                //        // ù��° ��Ʈ ����
                //        hearts[heartIdx].position = points[points.Count - pointIdx + 1] - (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]).normalized * (2 * 0.5f - dis1);
                //        hearts[heartIdx].forward = (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]);
                //        heartIdx++;
                //        // ù��° ��Ʈ �������� ������ ��Ʈ ����
                //        for (int i = heartIdx; i < hearts.Length; i++)
                //        {
                //            hearts[i].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * (i + 1 - heartIdx) * 0.5f;
                //        }
                //    }
                //    // ��Ʈ�� ���� ���
                //    else
                //    {
                //        return;
                //    }

                //}
                #endregion
            }
            // �Ÿ��� ����� ���
            else
            {
                // ù��° ��Ʈ ��ġ
                if (heartIdx == 0)
                    hearts[heartIdx].position = transform.position - transform.forward * 2 * 0.5f;
                else
                    hearts[heartIdx].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * 0.5f;
                heartIdx++;
                Test(pointIdx, heartIdx);
            }
        }
        // ���� ����Ʈ�� ���� ���
        else
        {
            //if (pointIdx == 1)
            //{
            // �÷��̾� ��ġ���� ��� ��Ʈ ����
            for (int i = pointIdx - 1; i < hearts.Length; i++)
            {
                if (pointIdx < 1)
                    hearts[i].position = transform.position - transform.forward * 2 * (i + 1) * 0.5f;
            }
            //}
            //else
            //{
            //    // ��Ʈ�� �����ϴ°��
            //    if (hearts.Length > heartIdx)
            //    {
            //        // ù��° ��Ʈ ����
            //        hearts[heartIdx].position = points[points.Count - pointIdx + 1] - (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]).normalized * (2 * 0.5f - dis1);
            //        hearts[heartIdx].forward = (points[points.Count - pointIdx + 1] - points[points.Count - pointIdx]);
            //        heartIdx++;
            //        // ù��° ��Ʈ �������� ������ ��Ʈ ����
            //        for (int i = heartIdx; i < hearts.Length; i++)
            //        {
            //            hearts[i].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * (i + 1 - heartIdx) * 0.5f;
            //        }
            //    }
            //    // ��Ʈ�� ���� ���
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

            // �Ÿ��� ����� ���
    void Test2(int pointIdx, int heartIdx)
    {
        // ��Ʈ�� �ִ°��
        if (hearts.Length > heartIdx)
        {
            // ù��° ��Ʈ ��ġ
            if (heartIdx == 0)
                hearts[heartIdx].position = transform.position - transform.forward * 2 * 0.5f;
            else
                hearts[heartIdx].position = hearts[heartIdx - 1].position - hearts[heartIdx - 1].forward * 2 * 0.5f;
            heartIdx++;
            // ù��° ��Ʈ�� ���� ����Ʈ���� �Ÿ�
            float dis3 = Vector3.Distance(hearts[heartIdx - 1].position, points[points.Count - pointIdx]);
            // �Ÿ��� ������ ���
            if (dis3 < 2 * 0.5f)
            {
                Test(pointIdx, heartIdx);
            }
            // �Ÿ��� ����� ���
            else
            {
                Test2(pointIdx, heartIdx);
            }

        }
        // ��Ʈ�� ���� ���
        else
        {
            for(int i = points.Count - pointIdx; i > -1; i--)
            {
                points.RemoveAt(i);
            }
        }
    }

}
