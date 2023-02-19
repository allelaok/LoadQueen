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
    public long rank;
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
        if (ExceptedString(id.text, password.text)) return;

        GameManager.instance.userId = id.text;

        reference.Child("users").Child(id.text).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                print("login error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if ((string)snapshot.Value == password.text)
                {

                    PlayerPrefs.SetString("userId", id.text);
                    PlayerPrefs.SetString("password", password.text);
                    id.text = "";
                    password.text = "";
                    GameManager.instance.LoginSuccese();
                    print("login sucssece");
                }
                else
                {
                    print("login fail.");
                }
            }
        });
    }

    public void AutoLogin()
    {
        string userid = PlayerPrefs.GetString("userId");
        string password = PlayerPrefs.GetString("password");

        if (ExceptedString(userid, password)) return;
  
        GameManager.instance.userId = userid;

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
                    GameManager.instance.LoginSuccese();
                    print("auto login sucssece");
                }
                else
                {
                    print("auto login fail.");
                }
            }
        });
    }

    bool ExceptedString(string id, string password)
    {
        if (string.IsNullOrEmpty(id))
        {
            print("id null");
            return true;
        }
        else if (id.Contains(".") | id.Contains("$")| id.Contains("[") | id.Contains("]"))
        {
            print("id error");
            return true;
        }
        else if (string.IsNullOrEmpty(password))
        {
            print("password null");
            return true;
        }
        else if (password.Contains(".") | password.Contains("$") | password.Contains("[") | password.Contains("]"))
        {
            print("password error");
            return true;
        }

        return false;

    }


    public void SingUp()
    {
        if (ExceptedString(id.text, password.text)) return;

        GameManager.instance.userId = id.text;

        reference.Child("users").Child(id.text).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                print("sign up error");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                print("sign up 1");
                DataSnapshot snapshot = task.Result;
                if (string.IsNullOrEmpty((string)snapshot.Value))
                {
                    reference.Child("users").Child(id.text).Child("password").SetValueAsync(password.text);
                    reference.Child("users").Child(id.text).Child("score").SetValueAsync(0);
                    GameManager.instance.OnClick_CloseBtn();
                    id.text = "";
                    password.text = "";
                    print("sucssese");
                }
                else
                {
                    print("already exist");
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
                print("save score error");
                reference.Child("users").Child(GameManager.instance.userId).Child("score").SetValueAsync(score);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string str = snapshot.Value.ToString();
                if (int.Parse(str) < score)
                {
                    print("Save Score!");
                    reference.Child("users").Child(GameManager.instance.userId).Child("score").SetValueAsync(score);
                }

            }
        });


    }

    public void GetBestScore(Action callback)
    {

        reference.Child("users").Child(GameManager.instance.userId).Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get bestScore error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string str = snapshot.Value.ToString();
                GameManager.instance.bestScore = int.Parse(str);
                print("sucssese my best score");
                callback.Invoke();
            }
        });
    }
    public List<RankInfo> rankInfos = new List<RankInfo>();
    public void GetRankInfo(Action callback)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get rank error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                rankInfos.Clear();
                long topNum = 10;
                if(snapshot.ChildrenCount < topNum)
                {
                    topNum = snapshot.ChildrenCount;
                }
                int beforeScore = 0;
                int beforeRank = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    string nickName = childSnapshot.Key.ToString();

                    rank++;
                    if (rank <= topNum)
                    {
                        RankInfo info = new RankInfo();
                        info.nickName = nickName;
                        info.score = score;
                        if (beforeScore == score)
                            info.rank = beforeRank;
                        else
                        {
                            info.rank = rank;
                            beforeScore = score;
                            beforeRank = rank;
                        }

                        rankInfos.Add(info);
                    }
                    else
                    {
                        print("break");
                        break;
                    }
                }

                callback.Invoke();
            }
        });

    }

    public RankInfo targetRank = new RankInfo();
    public RankInfo myRank;
    public void GetMyRank(Action callback)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get my rank error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    rank++;

                    if (score <= GameManager.instance.bestScore)
                    {
                        //reference.Child("users").Child(GameManager.instance.userId).Child("rank").SetValueAsync(rank);
                        myRank = new RankInfo();
                        myRank.nickName = GameManager.instance.userId;
                        myRank.score = GameManager.instance.bestScore;
                        print("MyRank");
                        print(GameManager.instance.userId);
                        print(GameManager.instance.bestScore);
                        myRank.rank = rank;
                        break;
                    }

                    string nickName = childSnapshot.Key.ToString();

                    targetRank.nickName = nickName;
                    targetRank.score = score;
                    targetRank.rank = rank;

                }
               
            }

            callback.Invoke();


        });
    }


   
}