using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d; //리듬게임 오차를 방지하기 위해 더 좋은 double로

    

    [SerializeField] Transform tfNoteAppear = null;

    TimingManager theTimingManager;
    EffectManager theEffectmanager;
    ComboManager theComboManager;

    void Start()
    {
        theComboManager = FindObjectOfType<ComboManager>();
        theEffectmanager = FindObjectOfType<EffectManager>();
        theTimingManager = GetComponent<TimingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManger.instance.isStartGame)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= 60d / bpm) // 60/120 은 1비트당 0.5초
            {
                GameObject t_note = ObjectPool.instance.noteQueue.Dequeue();
                t_note.transform.position = tfNoteAppear.position;
                t_note.SetActive(true);

                theTimingManager.boxNoteList.Add(t_note);//노트 박스리스트에 추가
                currentTime -= 60d / bpm; //currentTime을 0으로 초기화 시키면 아직 쌓여있는 deltatime이 사라져 그만큼 시간적오차가 누적된다.
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            if (collision.GetComponent<Note>().GetNoteFlag())
            {
                theTimingManager.MissRecord();
                theEffectmanager.JudgementEffect(4);
                theComboManager.ResetCombo();
            }
            theTimingManager.boxNoteList.Remove(collision.gameObject);//리스트 노트 제거

            
            ObjectPool.instance.noteQueue.Enqueue(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }

    public void RemoveNote()
    {
        GameManger.instance.isStartGame = false;
        for (int i = 0; i < theTimingManager.boxNoteList.Count; i++)
        {
            theTimingManager.boxNoteList[i].SetActive(false);
            ObjectPool.instance.noteQueue.Enqueue(theTimingManager.boxNoteList[i]); 
        }
        theTimingManager.boxNoteList.Clear();
    }
}
