using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public List<GameObject> boxNoteList = new List<GameObject>(); //노트관리

    int[] judgementRecord = new int[5];

    [SerializeField] Transform center = null; //센터
    [SerializeField] RectTransform[] timingRect = null; //판정박스 표본
    Vector2[] timingBoxs = null; //진짜 판정범위 설정된 박스

    EffectManager theEffect;
    ComboManager theComboManager;
    ScoreManager theScoreManager;
    StageManger theStageManager;
    PlayerController thePlayer;
    StatusManger theStatusManager;
    AudioManager theAudioManager;

    // Start is called before the first frame update
    void Start()
    {
        theEffect = FindObjectOfType<EffectManager>();
        theScoreManager = FindObjectOfType<ScoreManager>();
        theComboManager = FindObjectOfType<ComboManager>();
        theStageManager = FindObjectOfType<StageManger>();
        thePlayer = FindObjectOfType<PlayerController>();
        theStatusManager = FindObjectOfType<StatusManger>();


        //타이밍 박스 설정
        timingBoxs = new Vector2[timingRect.Length];

        for (int i = 0; i < timingRect.Length; i++)
        {
            //각각의 판정 범위 = 중심 - 이미지 너비/2 ~ 중심 + 이미지 너비/2
            timingBoxs[i].Set(center.localPosition.x - timingRect[i].rect.width / 2,
                              center.localPosition.x + timingRect[i].rect.width / 2);
        }
    }

    public bool CheckTiming()
    {
        for (int i = 0; i < boxNoteList.Count; i++) //리스트에 있는 노드들을 찾아서 판정박스에 있는 노트를 찾아야함
        {
            float t_notePosX = boxNoteList[i].transform.localPosition.x;

            for (int x = 0; x < timingBoxs.Length; x++)
            {
                if (timingBoxs[x].x <= t_notePosX && t_notePosX <= timingBoxs[x].y)
                {
                    //치는 순간 숨김, 잘모르겠는데 음악 코드가 맞으면 나오게 되어있는거라 노트가 중앙에 도달하지 못하고 사라지면 음악이 끊긴다한다.
                    //그래서 안보이게만 하고 객체 삭제는 범위를 벗어난 뒤에 하게된다.
                    boxNoteList[i].GetComponent<Note>().HideNote();
                    boxNoteList.RemoveAt(i);
                    //이펙트
                    if (x < timingBoxs.Length - 1)
                        theEffect.NoteHitEffect();


                    if (CheckCanNextPlate())
                    {
                        theScoreManager.IncreaseScore(x);//점수 증가
                        theStageManager.ShowNextPlate();//플레이트 등장
                        theEffect.JudgementEffect(x);//판정 연출
                        judgementRecord[x]++;//판정기록
                        theStatusManager.CheckShield(); //쉴드체크
                    }
                    else
                    {
                        theEffect.JudgementEffect(5);//이미 밟은 플레이트는 normal 판정 처리
                    }
                    AudioManager.instance.PlaySFX("Clap");
                    return true;
                }
            }

        }
        
        theComboManager.ResetCombo();
        theEffect.JudgementEffect(timingBoxs.Length); //==bad
        MissRecord();
        return false;
    }

    bool CheckCanNextPlate()
    {
        if (Physics.Raycast(thePlayer.destPos, Vector3.down, out RaycastHit t_hitInfo, 1.1f)) //접시가 아래 있나 판정
        {
            if (t_hitInfo.transform.CompareTag("BasicPlate"))
            {
                BasicPlate t_plate = t_hitInfo.transform.GetComponent<BasicPlate>();
                if (t_plate.flag) //한번 밟은 플레이트는 반응하지 않도록
                {
                    t_plate.flag = false;
                    return true;
                }
                    
            }
        }
        return false;
    }

    public int[] GetJudgementRecord()
    {
        return judgementRecord;
    }

    public void MissRecord()
    {
        judgementRecord[4]++;//판정기록, Miss기록
        theStatusManager.ResetShieldCombo();
    }

    public void Initialized()
    {
        judgementRecord[0] = 0;
        judgementRecord[1] = 0;
        judgementRecord[2] = 0;
        judgementRecord[3] = 0;
        judgementRecord[4] = 0;
    }
}
