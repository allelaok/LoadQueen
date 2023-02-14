using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rank : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text rank;
    [SerializeField]
    TMPro.TMP_Text nickName;

    [SerializeField]
    TMPro.TMP_Text score;

    public void SetInfo(RankInfo rankInfo = null)
    {
        if (rankInfo == null)
        {
            rank.text = string.Empty;
            nickName.text = string.Empty;
            score.text = string.Empty;
        }
        else
        {
            rank.text = rankInfo.rank.ToString();
            nickName.text = rankInfo.nickName;
            score.text = rankInfo.score.ToString();
        }
    }

}
