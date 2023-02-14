using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Database;
using Firebase.Unity;
//using UnityEngine.UI;
using Firebase.Extensions;
using System.Linq;
using TMPro;

public class RankInfo
{
    public int rank;
    public string nickName;
    public int score;
}

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    [SerializeField]
    TMP_InputField id;
    [SerializeField]
    TMP_InputField password;


   

    DatabaseReference reference;

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
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        //rankInfos = Enumerable.Repeat(new RankInfo(), 10).ToArray();
    }


    public void SingIn()
    {
        if (string.IsNullOrEmpty(id.text))
        {
            print("아이디를 입력해 주세요");
            return;
        }
        else if (string.IsNullOrEmpty(password.text))
        {
            print("비밀번호를 입력해 주세요");
            return;
        }

        reference.Child("users").Child(id.text).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if ((string)snapshot.Value == password.text)
                {
                    GameManager.instance.userId = id.text;
                    GetBestScore();

                    PlayerPrefs.SetString("userId", id.text);
                    PlayerPrefs.SetString("password", password.text);
                    GameManager.instance.LoginSuccese();
                    print("로그인");
                }
                else
                {
                    print("비밀번호가 틀렸습니다.");
                }
            }
        });
    }

    public void AutoLogin()
    {
        string userid = PlayerPrefs.GetString("userId");
        string password = PlayerPrefs.GetString("password");

        if (string.IsNullOrEmpty(userid))
        {
            return;
        }
        else if (string.IsNullOrEmpty(password))
        {
            return;
        }

        reference.Child("users").Child(userid).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if ((string)snapshot.Value == password)
                {
                    GetBestScore();
                    GameManager.instance.LoginSuccese();
                    print("로그인");
                }
                else
                {
                    print("비밀번호가 틀렸습니다.");
                }
            }
        });
    }

    public void SingUp()
    {
        if (string.IsNullOrEmpty(id.text))
        {
            print("아이디를 입력해 주세요");
            return;
        }
        else if (string.IsNullOrEmpty(password.text))
        {
            print("비밀번호를 입력해 주세요");
            return;
        }

        reference.Child("users").Child(id.text).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (string.IsNullOrEmpty((string)snapshot.Value))
                {
                    reference.Child("users").Child(id.text).Child("password").SetValueAsync(password.text);
                    reference.Child("users").Child(id.text).Child("score").SetValueAsync(0);
                    reference.Child("users").Child(id.text).Child("rank").SetValueAsync(0);
                    GameManager.instance.LoginSuccese();
                    print("회원가입 성공");
                }
                else
                {
                    print("이미 사용중인 아이디");
                }
            }
        });
    }
    public void SaveScore(int score)
    {
        reference.Child("users").Child(GameManager.instance.userId).Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("실패");
                reference.Child("users").Child(GameManager.instance.userId).Child("score").SetValueAsync(score);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string str = snapshot.Value.ToString();
                if (int.Parse(str) < score)
                {
                    print("신기록 달성!");
                    reference.Child("users").Child(GameManager.instance.userId).Child("score").SetValueAsync(score);
                }

            }
        });


    }

    public void GetBestScore()
    {
        reference.Child("users").Child(GameManager.instance.userId).Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("실패");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string str = snapshot.Value.ToString();
                GameManager.instance.bestScore = int.Parse(str);
            }
        });
    }
    public RankInfo targetRank = new RankInfo();
    public RankInfo myRank;
    public List<RankInfo> rankInfos = new List<RankInfo>();
    public void GetRankInfo(Action callback)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("실패");
            }
            else if (task.IsCompleted)
            {
                print("들어옴1");
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    string nickName = childSnapshot.Key.ToString();
                    if (rank < 10)
                    {
                        RankInfo info = new RankInfo();
                        info.nickName = nickName;
                        info.score = score;
                        info.rank = rank + 1;
                        print("들어옴2");
                        rankInfos.Add(info);
                    }

                    rank++;
                    print("들어옴3");

                    if (score > GameManager.instance.bestScore)
                    {
                        targetRank.nickName = nickName;
                        targetRank.score = score;
                        targetRank.rank = rank;
                print("들어옴4");
                    }
                    else
                    {
                        reference.Child("users").Child(GameManager.instance.userId).Child("rank").SetValueAsync(rank);
                        myRank = new RankInfo();
                        myRank.nickName = nickName;
                        myRank.score = GameManager.instance.bestScore;
                        myRank.rank = rank;
                print("들어옴5");
                        break;
                    }


                    //if ((rank > 10 || ) && myRank != null)
                    //{
                    //    print("들어옴6");
                    //    break;
                    //}

                }

                print("들어옴7");
                print(rankInfos.Count);

                callback.Invoke();
            }
        });

    }

}