using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    public Transform m_EnemyPos;
    public SlingShot m_SlingShot;
    private List<GameObject> m_BirdList;


    #region UI部分

    //开挂人生
    [SerializeField] private Button mCheatBtn;

    //动如脱兔
    [SerializeField] private Button mDartBtn;

    //左边格子节点
    [SerializeField] private Transform mFloorTrans;

    [SerializeField] private Transform mCheatsTrans;

    //游戏正式开启
    [SerializeField] private Transform mGameStartTrans;

    #endregion


    public int EnemyHp { get; set; } = 10;

    [HideInInspector] public GameState m_GameState;

    //private List<GameObject> m_EnemyList;
    [SerializeField] private GameObject[] m_Birds;

    //[SerializeField] private GameObject[] m_Enemys;
    [SerializeField] private GameObject[] m_CreateNodes;
    public bool isFinishThrow = true;

    private void Awake()
    {
        instance = this;
        m_BirdList = new List<GameObject>();
        mCheatBtn.onClick.AddListener(OnCheatsBtnClick);
        mDartBtn.onClick.AddListener(OnDartBtnClick);
    }

    private void Start()
    {
        //StartCoroutine(nameof(RandomCreateBird));
        RandomCreateBird();
        m_GameState = GameState.START;
    }

    private void Update()
    {
        Debug.Log("isFinishThrow'" + isFinishThrow);
        if (isFinishThrow == false)
        {
            return;
        }

        switch (m_GameState)
        {
            case GameState.START:
                m_SlingShot.SetSlingshotLineActive(false);
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("Bird"))
                        {
                            m_SlingShot.m_BirdObj = hit.collider.gameObject;
                            Debug.Log($"当前点击的小鸟是 ： {m_SlingShot.m_BirdObj.name}");
                            m_SlingShot.m_SlingshotState = SlingshotState.IDLE;
                            m_GameState = GameState.PLAYING;
                        }
                    }
                }

                break;
            case GameState.BIRD_MOVING_TO_SLINGSHOT:
                break;
            case GameState.PLAYING:
                //isFinishThrow = false;
                break;
            case GameState.WON:
                //TODO 胜利后
                break;
            case GameState.LOST:
                //TODO 失败后
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    //开挂人生
    public void OnCheatsBtnClick()
    {
        DisPlayGameStartNeedWindow();
        m_SlingShot.m_TrajectoryLineRenderer.transform.gameObject.SetActive(true);
        //TODO 隐藏CheatsNode 展示floor 展示GameStartNode
    }

    //飞就完了
    public void OnDartBtnClick()
    {
        m_SlingShot.m_TrajectoryLineRenderer.transform.gameObject.SetActive(false);
        DisPlayGameStartNeedWindow();
        m_SlingShot.m_ThrowSpeed = 7.5f;
    }

    //展示游戏需要的界面窗口
    private void DisPlayGameStartNeedWindow()
    {
        mCheatsTrans.transform.gameObject.SetActive(false);

        mFloorTrans.transform.gameObject.SetActive(true);
        mGameStartTrans.transform.gameObject.SetActive(true);
    }

    private void RandomCreateEnemy(Transform parentTrans)
    {
        int index = UnityEngine.Random.Range(0, m_Birds.Length);
        GameObject obj = m_Birds[index];
        Instantiate(obj, parentTrans);
    }

    public void RandomCreateBird()
    {
        // yield return null;
        // yield return null;
        //yield return new WaitForSeconds(1f);

        for (int i = 0; i < m_CreateNodes.Length; i++)
        {
            if (m_CreateNodes[i].transform.childCount == 0)
            {
                //0.17
                //RandomCreatBird(m_CreateNodes[i].transform);
                int index = Random.Range(0, m_Birds.Length);
                GameObject oblBird = Instantiate(m_Birds[index], m_CreateNodes[i].transform);
                oblBird.name = m_Birds[index].name;
                Debug.Log(oblBird.name);
                if (oblBird.name == "RedBird")
                {
                    oblBird.transform.localPosition = new Vector3(0, 0.17f, 0);
                }
            }
        }
    }
}