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
//            print("���̵� �Է��� �ּ���");
//            return;
//        }
//        else if (string.IsNullOrEmpty(password.text))
//        {
//            print("��й�ȣ�� �Է��� �ּ���");
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
//                    print("�α���");
//                }
//                else
//                {
//                    print("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
//                }
//            }
//        });
//    }

//    public void SingUp()
//    {
//        if (string.IsNullOrEmpty(id.text))
//        {
//            print("���̵� �Է��� �ּ���");
//            return;
//        }
//        else if (string.IsNullOrEmpty(password.text))
//        {
//            print("��й�ȣ�� �Է��� �ּ���");
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
//                    print("ȸ������ ����");
//                }
//                else
//                {
//                    print("�̹� ������� ���̵�");
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
//                print("����");
//            }
//            else if (task.IsCompleted)
//            {
//                DataSnapshot snapshot = task.Result;
//                print("�ְ���: " + (float)snapshot.Value);
//                print("���ݱ��: " + score);
//                if ((float)snapshot.Value < score)
//                {
//                    print("�ְ��� ����");
//                    reference.Child("users").Child("test").Child("score").SetValueAsync(score);
//                }
//            }
//        });
//    }

//    public void ShowTopTen()
//    {
//        reference/*.Child("users")*/.GetValueAsync().ContinueWith(task => {
//            if (task.IsCompleted)
//            { // ���������� �����͸� ����������
//                DataSnapshot snapshot = task.Result;
//                // �����͸� ����ϰ��� �Ҷ��� Snapshot ��ü �����
//                print(snapshot.Children);
                
//                foreach (DataSnapshot data in snapshot.Children)
//                {
//                    float rank = (float)data.Child("Score").Value;
//                print("������ ������");
//                    print(rank);
//                }
//            }
//            else
//            {
//                print("������ �������� ����");
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
//            //Text UI �� ���� �������ִ� str ���� ������ �����ֱ� ����.
//            if (strLen <= i) return;
//            Rank[i].text = strRank[i];
//        }
//    }
//}