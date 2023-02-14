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
        ranks = content.GetComponentsInChildren<Rank>();
    }

    public void OnClick_RankingBtn()
    {
        FirebaseManager.instance.GetRankInfo(AfterGetTopTen);
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
        ranks[10].SetInfo(FirebaseManager.instance.targetRank);
        ranks[11].SetInfo(FirebaseManager.instance.myRank);

        rankingPanel.SetActive(true);
    }
}
