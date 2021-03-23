using System.Collections;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public int[] score;

    void Start()
    {
        LoadScore();
    }

    public void SaveScore()
    {
        //백그라운드 비동기통신 (프라이베잇으로 테이블네임을 가져오는데 그걸 유저데이터에 저장)
        BackendAsyncClass.BackendAsync(Backend.GameInfo.GetPrivateContents, "Score", UserDataBro =>
        {
            if (UserDataBro.IsSuccess())//성공시
            {
                Param data = new Param(); //키값처리
                data.Add("Scores", score);

                if (UserDataBro.GetReturnValuetoJSON()["rows"].Count > 0)//가져온 데이터의 개수를 세서 0보다 많다 = 이미 데이터가 들어가있다
                {
                    string t_Indate = UserDataBro.GetReturnValuetoJSON()["rows"][0]["inDate"]["S"].ToString(); //수정할 식별값 가져오기
                    BackendAsyncClass.BackendAsync(Backend.GameInfo.Update, "Score", t_Indate, data, (t_callback) => //데이터로 수정해서 콜백으로 받아옴
                    {
                    });
                }
                else //데이터 없으니 새로 만듬
                {
                    BackendAsyncClass.BackendAsync(Backend.GameInfo.Insert, "Score", data, (t_callback) =>
                    {
                    });
                }
            }

        });
    }

    public void LoadScore()
    {
        BackendAsyncClass.BackendAsync(Backend.GameInfo.GetPrivateContents, "Score", UserDataBro =>
        {
            JsonData t_data = UserDataBro.GetReturnValuetoJSON();
            if (t_data.Count > 0)
            {
                JsonData t_List = t_data["rows"][0]["Scores"]["L"];
                for (int i = 0; i < t_List.Count; i++)
                {
                    var t_value = t_List[i]["N"];
                    score[i] = int.Parse(t_value.ToString());
                }

                Debug.Log("로드 완료");
            }
            else
            {
                Debug.Log("로드할 것 없음");
            }
        });
        
    }
}
