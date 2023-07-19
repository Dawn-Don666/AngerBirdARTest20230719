using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Animator m_EnemyAnimator;

    public Transform pointA;
    public Transform pointB;

    private NavMeshAgent agent;

    private bool isMovingToB = false;

    private Animator animator;
    private BoxCollider mEnemyBoxColl;


    private void Awake()
    {
        m_EnemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        mEnemyBoxColl = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        SetDestination(pointB.position);
    }


    private void Update()
    {
        if (agent == null) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // 到达目标位置
            if (isMovingToB)
            {
                SetDestination(pointA.position);
            }
            else
            {
                SetDestination(pointB.position);
            }

            isMovingToB = !isMovingToB;
        }
    }

    private void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);

        // 设置动画状态
        bool isRunning = agent.remainingDistance > agent.stoppingDistance;
        m_EnemyAnimator.SetBool("IsRunning", isRunning);
    }

    // 动画事件：动画播放完毕
    public void OnAnimationFinished()
    {
        // 动画播放完毕后切换目标位置
        if (isMovingToB)
            SetDestination(pointA.position);
        else
            SetDestination(pointB.position);

        isMovingToB = !isMovingToB;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bird")
        {
            GameManager.Instance.EnemyHp--;
            Vector3 lastScale = gameObject.transform.localScale;

            agent.speed += agent.speed + 0.1f;
            gameObject.transform.position = pointA.transform.position;

            gameObject.transform.localScale = new Vector3
            (
                lastScale.x + 0.1f,
                lastScale.y - 0.1f,
                lastScale.z - 0.1f
            );
            mEnemyBoxColl.size = new Vector3
            (
                mEnemyBoxColl.size.z - 0.01f,
                mEnemyBoxColl.size.y - 0.01f,
                mEnemyBoxColl.size.z - 0.01f
            );


            Debug.Log("GameManager.Instance.EnemyHp :"  + GameManager.Instance.EnemyHp);
        }
    }
}