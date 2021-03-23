using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//프로그래밍에서 오브젝트를 생성하거나 파괴하는 작업은 꽤나 무거운 작업으로 분류된다.
//오브젝트 풀링 = 미리 객체를 생성하고 필요할 때마다 가져다 쓰고 돌려놓는 것
[System.Serializable]
public class ObjectInfo
{
    public GameObject goPrefab;
    public int count;
    public Transform tfPoolParent;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] ObjectInfo[] objectInfo = null; //노트 종류를 늘릴 수 있는 배열

    public Queue<GameObject> noteQueue = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        noteQueue = InsertQueue(objectInfo[0]);
    }

    Queue<GameObject> InsertQueue(ObjectInfo p_objectInfo)
    {
        Queue<GameObject> t_queue = new Queue<GameObject>();
        for (int i = 0; i < p_objectInfo.count; i++)
        {
            GameObject t_clone = Instantiate(p_objectInfo.goPrefab, transform.position, Quaternion.identity);
            t_clone.SetActive(false);
            if (p_objectInfo.tfPoolParent != null)
                t_clone.transform.SetParent(p_objectInfo.tfPoolParent);
            else
                t_clone.transform.SetParent(this.transform);

            t_queue.Enqueue(t_clone);
        }

        return t_queue;
    }

}
