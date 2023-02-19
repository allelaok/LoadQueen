using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    Transform content;
    [SerializeField]
    GameObject rankingPanel;
    Rank[] ranks;

    private void Start()
    {
        //https://giseung.tistory.com/19
        float h = (float)2778;
        float w = (float)1284;

        if(h / w > Screen.height / Screen.width)
        {

        }


        ranks = content.GetComponentsInChildren<Rank>();
    }

    public void OnClick_RankingBtn()
    {
        FirebaseManager.instance.GetRankInfo(AfterGetTopTen);
        FirebaseManager.instance.GetMyRank(AfterGetMyRank);
    }

    public void AfterGetTopTen()
    {
        for (int i = 0; i < 10; i++)
        {
            if(i < FirebaseManager.instance.rankInfos.Count)
            ranks[i].SetInfo(FirebaseManager.instance.rankInfos[i]);
            else
            ranks[i].SetInfo();
        }
    }

    public void AfterGetMyRank()
    {
        if (FirebaseManager.instance.targetRank.rank == 0)
            FirebaseManager.instance.targetRank = null;

        ranks[10].SetInfo(FirebaseManager.instance.targetRank);
        ranks[11].SetInfo(FirebaseManager.instance.myRank);
        rankingPanel.SetActive(true);
    }
}
