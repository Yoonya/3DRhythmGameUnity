using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class Login : MonoBehaviour
{
    [SerializeField] InputField id = null;
    [SerializeField] InputField pw = null;

    DatabaseManager theDatabase;

    // Start is called before the first frame update
    void Start()
    {
        theDatabase = FindObjectOfType<DatabaseManager>();
        Backend.Initialize(InitializeCallback); //뒤끝서버초기화
    }

    void InitializeCallback()
    {
        if (Backend.IsInitialized)
        {
            Debug.Log(Backend.Utils.GetServerTime()); //현재 시간
            Debug.Log(Backend.Utils.GetGoogleHash()); //모바일<->뒤끝서버 통신에 필요한 구글 해시키
        }
        else
            Debug.Log("초기화 실패 (인터넷 문제 등등)");
    }

    public void BtnRegist()
    {
        string t_id = id.text;
        string t_pw = pw.text;

        BackendReturnObject bro = Backend.BMember.CustomSignUp(t_id, t_pw, "Test"); //회원가입 함수

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입 완료");
            theDatabase.LoadScore();
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("회원가입 실패");
        }
    }

    public void BtnLogin()
    {
        string t_id = id.text;
        string t_pw = pw.text;

        BackendReturnObject bro = Backend.BMember.CustomLogin(t_id, t_pw); //로그인 함수

        if (bro.IsSuccess())
        {
            Debug.Log("로그인 완료");
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("로그인 실패");
        }
    }
}
