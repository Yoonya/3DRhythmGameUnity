using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static bool s_canPressKey = true; //골에 도착시 못움직이게 하려고

    [SerializeField] float moveSpeed = 3;
    Vector3 dir = new Vector3();
    public Vector3 destPos = new Vector3();
    Vector3 originPos = new Vector3();

    [SerializeField] float spinSpeed = 270;
    Vector3 rotDir = new Vector3();
    Quaternion destRot = new Quaternion();

    [SerializeField] float recoilPosY = 0.25f;//들썩임, 반동 표현
    [SerializeField] float recoilSpeed = 1.5f;

    bool canMove = true; //굴러가는 동안에 또 노트판정이 나오면 급발진됨, 그것을 막아주기 위해 구를때는 노트 판정없도록
    bool isFalling = false;//떨어지는지 아닌지

    [SerializeField] Transform fakeCube = null; // 가짜 큐브를 먼저 돌려놓고 그 돌아간 만큼의 값을 목표 회전값으로 삼음
    [SerializeField] Transform realCube = null;

    TimingManager theTimingManager;
    CameraController theCam;
    Rigidbody myRigid;
    StatusManger theStatus;

    // Start is called before the first frame update
    void Start()
    {
        theTimingManager = FindObjectOfType<TimingManager>();
        theCam = FindObjectOfType<CameraController>();
        theStatus = FindObjectOfType<StatusManger>();
        myRigid = GetComponentInChildren<Rigidbody>();
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManger.instance.isStartGame)
        {
            CheckFaling();
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
            {
                if (canMove && s_canPressKey && !isFalling)
                {
                    Calc();
                    //판정체크
                    if (theTimingManager.CheckTiming())
                    {
                        StartAction();
                    }
                }
            }
        }
    }

    public void Initialized()
    {
        transform.position = Vector3.zero;
        destPos = Vector3.zero;
        realCube.localPosition = Vector3.zero;
        canMove = true;
        s_canPressKey = true;
        isFalling = false;
        myRigid.useGravity = false;
        myRigid.isKinematic = true;
    }

    void Calc()
    {
        //방향계산
        dir.Set(Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal")); //vertical = 입력값 방향키위, w = 1 반대는 -1, 아니면 0
        //이동 목표값 계산
        destPos = transform.position + new Vector3(-dir.x, 0, dir.z);//큐브 x축이 반대로 움직이게 되있어서 여기서 반대로
        //회전 목표값 계산
        rotDir = new Vector3(-dir.z, 0f, -dir.x);
        fakeCube.RotateAround(transform.position, rotDir, spinSpeed); //원래 공전에서 사용 (공전 대상, 회전축, 회전값)을 이용한 편법회전 구현
        destRot = fakeCube.rotation;
    }

    void StartAction()
    {
        StartCoroutine(MoveCo());
        StartCoroutine(SpinCo());
        StartCoroutine(RecoilCo());
        StartCoroutine(theCam.ZoomCam());
    }

    IEnumerator MoveCo()
    {
        canMove = false;
        while (Vector3.SqrMagnitude(transform.position - destPos) >= 0.001f) //distance = a와 b의 좌표차이 반환, sqlMagnitude = 제곱근을 반환 4=2
        {
            transform.position = Vector3.MoveTowards(transform.position, destPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = destPos; //근사값으로 이동하지 정확한 값이 아니기 때문에 마지막에 조정
        canMove = true;
    }

    IEnumerator SpinCo()
    {
        while (Quaternion.Angle(realCube.rotation, destRot) > 0.5f)
        {
            realCube.rotation = Quaternion.RotateTowards(realCube.rotation, destRot, spinSpeed * Time.deltaTime);
            yield return null;
        }

        realCube.rotation = destRot; 
    }

    IEnumerator RecoilCo()
    {
        while (realCube.position.y < recoilPosY)
        {
            realCube.position += new Vector3(0, recoilSpeed * Time.deltaTime, 0);
            yield return null;
        }

        while (realCube.position.y > 0)
        {
            realCube.position -= new Vector3(0, recoilSpeed * Time.deltaTime, 0);
            yield return null;
        }

        realCube.localPosition = new Vector3(0, 0, 0);
    }

    void CheckFaling()
    {
        if (!isFalling && canMove)
        {
            if (!Physics.Raycast(transform.position, Vector3.down, 1.1f))
            {
                Falling();
            }
        }
    }

    void Falling()
    {
        isFalling = true;
        myRigid.useGravity = true;
        myRigid.isKinematic = false;
    }

    public void ResetFalling()
    {
        theStatus.DecreaseHp(1);
        AudioManager.instance.PlaySFX("Falling");

        if (!theStatus.GetIsDead())
        {
            isFalling = false;
            myRigid.useGravity = false;
            myRigid.isKinematic = true;
            transform.position = originPos;
            realCube.localPosition = new Vector3(0, 0, 0); //자식개체도 원래대로
        }
   
    }
}

   
