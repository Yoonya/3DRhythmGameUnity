using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float noteSpeed = 400;

    UnityEngine.UI.Image noteImage;

    void OnEnable()//객체가 활성화될 때마다 호출되는 함수
    {
        if(noteImage == null)
            noteImage = GetComponent<UnityEngine.UI.Image>();
        noteImage.enabled = true; //노트가 반납될 때는 enabled가 false로 돌아오기 때문에 다시 쓰려면 켜야한다.
    }

    public void HideNote()
    {
        noteImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.right * noteSpeed * Time.deltaTime;  
    }

    public bool GetNoteFlag()
    {
        return noteImage.enabled; //노트의 이미지 상태를 파악
    }
}
