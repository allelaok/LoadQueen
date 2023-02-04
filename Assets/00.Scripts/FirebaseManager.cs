//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using Firebase;
//using Firebase.Database;
//using Firebase.Unity;
//using UnityEngine.UI;
//using Firebase.Extensions;

//public class FirebaseManager : MonoBehaviour
//{
//    public static FirebaseManager instance;

//    [SerializeField]
//    InputField id;
//    [SerializeField]
//    InputField password;

//    string userId;


//    DatabaseReference reference;

//    private void Awake()
//    {
//        if(instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        reference = FirebaseDatabase.DefaultInstance.RootReference;
//        //DataLoad();
//        SaveScore(10);

//    }

//    public void SingIn()
//    {
//        if (string.IsNullOrEmpty(id.text))
//        {
//            print("아이디를 입력해 주세요");
//            return;
//        }
//        else if (string.IsNullOrEmpty(password.text))
//        {
//            print("비밀번호를 입력해 주세요");
//            return;
//        }

//        reference.Child("users").Child(id.text).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsFaulted)
//            {
//            }
//            else if (task.IsCompleted)
//            {
//                DataSnapshot snapshot = task.Result;
//                if ((string)snapshot.Value == password.text)
//                {
//                    GameManager.instance.GameStart();
//                    userId = id.text;
//                    print("로그인");
//                }
//                else
//                {
//                    print("비밀번호가 틀렸습니다.");
//                }
//            }
//        });
//    }

//    public void SingUp()
//    {
//        if (string.IsNullOrEmpty(id.text))
//        {
//            print("아이디를 입력해 주세요");
//            return;
//        }
//        else if (string.IsNullOrEmpty(password.text))
//        {
//            print("비밀번호를 입력해 주세요");
//            return;
//        }

//        reference.Child("users").Child(id.text).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsFaulted)
//            {
//                // Handle the error...
//            }
//            else if (task.IsCompleted)
//            {
//                DataSnapshot snapshot = task.Result;
//                if (string.IsNullOrEmpty((string)snapshot.Value))
//                {
//                    reference.Child("users").Child(id.text).Child("password").SetValueAsync(password.text);
//                    reference.Child("users").Child(id.text).Child("score").SetValueAsync(0);
//                    print("회원가입 성공");
//                }
//                else
//                {
//                    print("이미 사용중인 아이디");
//                }
//            }
//        });
//    }
//    public void SaveScore(float score)
//    {


//        reference.Child("users").Child("test").Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
//        {
//            if (task.IsFaulted)
//            {
//                // Handle the error...
//                print("실패");
//            }
//            else if (task.IsCompleted)
//            {
//                DataSnapshot snapshot = task.Result;
//                print("최고기록: " + (float)snapshot.Value);
//                print("지금기록: " + score);
//                if ((float)snapshot.Value < score)
//                {
//                    print("최고기록 갱신");
//                    reference.Child("users").Child("test").Child("score").SetValueAsync(score);
//                }
//            }
//        });
//    }

//    public void ShowTopTen()
//    {
//        reference/*.Child("users")*/.GetValueAsync().ContinueWith(task => {
//            if (task.IsCompleted)
//            { // 성공적으로 데이터를 가져왔으면
//                DataSnapshot snapshot = task.Result;
//                // 데이터를 출력하고자 할때는 Snapshot 객체 사용함
//                print(snapshot.Children);
                
//                foreach (DataSnapshot data in snapshot.Children)
//                {
//                    float rank = (float)data.Child("Score").Value;
//                print("데이터 가져옴");
//                    print(rank);
//                }
//            }
//            else
//            {
//                print("데이터 가져오기 실패");
//            }
//        });
//    }
//    long strLen;
//    string[] strRank;
//    bool textLoadBool;
//    void DataLoad()
//    {
//        reference.Child("users").GetValueAsync().ContinueWith(task =>
//        {
//            if (task.IsFaulted)
//            {
//                DataLoad();
//            }
//            else if (task.IsCompleted)
//            {
//                DataSnapshot snapshot = task.Result;

//                int count = 0;
//                strLen = snapshot.ChildrenCount;
//                strRank = new string[strLen];

//                foreach (DataSnapshot data in snapshot.Children)
//                {
//                    IDictionary rankInfo = (IDictionary)data.Value;
//                    strRank[count] = rankInfo["score"].ToString();
//                    print(strRank[count]);
//                    count++;
//                }
//                textLoadBool = true;
//            }
//        });
//    }

//    public Text[] Rank = new Text[7];
//    void TextLoad()
//    {
//        textLoadBool = false;
//        try
//        {
//            Array.Sort(strRank);
//        }
//        catch(NullReferenceException e)
//        {
//            return;
//        }

//        for (int i = 0; i < Rank.Length; i++)
//        {
//            //Text UI 에 현재 가지고있는 str 길이 까지만 보여주기 위함.
//            if (strLen <= i) return;
//            Rank[i].text = strRank[i];
//        }
//    }
//}