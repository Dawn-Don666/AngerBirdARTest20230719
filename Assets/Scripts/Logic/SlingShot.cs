using System;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    //弹弓中间的向量
    private Vector3 m_SlingshotMiddleVector;
    [HideInInspector] public SlingshotState m_SlingshotState;
    public GameObject m_BirdObj;
    public Transform m_LeftSlingshotOrigin, m_RightSlingshotOrigin;
    public LineRenderer m_SlingshotLine1;
    public LineRenderer m_SlingshotLine2;
    public LineRenderer m_TrajectoryLineRenderer;
    public Transform m_BirdWaitPos;
    public float m_ThrowSpeed;
    public bool isFinishThrow = true;
    [HideInInspector] public float m_TimeSinceThrown;

    private void Start()
    {
        m_SlingshotLine1.sortingLayerName = "Foreground";
        m_SlingshotLine2.sortingLayerName = "Foreground";
        m_TrajectoryLineRenderer.sortingLayerName = "Foreground";

        m_SlingshotLine1.startWidth = 0.1f;
        m_SlingshotLine1.endWidth = 0.1f;

        m_SlingshotLine2.startWidth = 0.1f;
        m_SlingshotLine2.endWidth = 0.1f;

        m_TrajectoryLineRenderer.startWidth = 0.08f;
        m_TrajectoryLineRenderer.endWidth = 0.08f;


        //m_SlingshotState = SlingshotState.IDLE;
        m_SlingshotLine1.SetPosition(0, m_LeftSlingshotOrigin.position);
        m_SlingshotLine2.SetPosition(0, m_RightSlingshotOrigin.position);

        m_SlingshotMiddleVector = new Vector3((m_LeftSlingshotOrigin.position.x + m_RightSlingshotOrigin.position.x) / 2,
            (m_LeftSlingshotOrigin.position.y + m_RightSlingshotOrigin.position.y) / 2, 0);

        SetSlingshotLineActive(false);
    }

    private void Update()
    {
        if (!isFinishThrow) return;

        switch (m_SlingshotState)
        {
            case SlingshotState.IDLE:
                InitializeBird();
                DisplaySlingshotLineRenderers();
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("Bird"))
                        {
                            m_SlingshotState = SlingshotState.USER_PULLING;
                        }

                        GameManager.Instance.isFinishThrow = false;
                    }
                }
                break;
            case SlingshotState.USER_PULLING:
                DisplaySlingshotLineRenderers();
                if (Input.GetMouseButton(0))
                {
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    location.z = 0;
                    if (Vector3.Distance(location, m_SlingshotMiddleVector) > 1.5f)
                    {
                        var maxPosition = (location - m_SlingshotMiddleVector).normalized * 1.5f + m_SlingshotMiddleVector;
                        m_BirdObj.transform.position = maxPosition;
                    }
                    else
                    {
                        m_BirdObj.transform.position = location;
                    }
                    float distance = Vector3.Distance(m_SlingshotMiddleVector, m_BirdObj.transform.position);
                    DisplayTrajectoryLineRenderer2(distance);
                }
                else
                {
                    SetSlingshotLineActive(false);
                    m_TimeSinceThrown = Time.time;
                    float distance = Vector3.Distance(m_SlingshotMiddleVector, m_BirdObj.transform.position);
                    if (distance > 1)
                    {
                        SetSlingshotLineActive(false);
                        m_SlingshotState = SlingshotState.BIRD_FLYING;
                        ThrowBird(distance);
                    }
                    else
                    {
                        m_BirdObj.transform.positionTo(distance / 10, //duration
                                m_BirdWaitPos.transform.position). //final position
                            setOnCompleteHandler((x) =>
                        {
                            x.complete();
                            x.destroy();
                            InitializeBird();
                        });
                    }
                }
                break;
            case SlingshotState.BIRD_FLYING:
                break;
        }
    }

    private void InitializeBird()
    {
        m_BirdObj.transform.position = m_BirdWaitPos.position;
        m_SlingshotState = SlingshotState.IDLE;
        SetSlingshotLineActive(true);
    }

    void DisplaySlingshotLineRenderers()
    {
        m_SlingshotLine1.SetPosition(1, m_BirdObj.transform.position);
        m_SlingshotLine2.SetPosition(1, m_BirdObj.transform.position);
    }

    public void SetSlingshotLineActive(bool active)
    {
        m_SlingshotLine1.enabled = active;
        m_SlingshotLine2.enabled = active;
    }

    private void ThrowBird(float distance)
    {
        Vector3 velocity = m_SlingshotMiddleVector - m_BirdObj.transform.position;
        m_BirdObj.GetComponent<Bird>().OnThrow();
        m_BirdObj.GetComponent<Rigidbody>().velocity = new Vector2(velocity.x, velocity.y) * m_ThrowSpeed * distance;
        BirdThrown?.Invoke(this, EventArgs.Empty);
    }
    public event EventHandler BirdThrown;


    void DisplayTrajectoryLineRenderer2(float distance)
    {
        SetTrajectoryLineRendererActive(true);
        Vector3 v3 = m_SlingshotMiddleVector - m_BirdObj.transform.position;
        int segmentCount = 15;
        float segmentScale = 2;
        Vector3[] segments = new Vector3[segmentCount];

        segments[0] = m_BirdObj.transform.position;

        Vector3 segVelocity = new Vector3(v3.x, v3.y) * m_ThrowSpeed * distance;

        Vector3.Angle(segVelocity, new Vector2(1, 0));
        for (int i = 1; i < segmentCount; i++)
        {
            float time2 = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics.gravity * Mathf.Pow(time2, 2);
        }

        m_TrajectoryLineRenderer.SetVertexCount(segmentCount);
        for (int i = 0; i < segmentCount; i++)
            m_TrajectoryLineRenderer.SetPosition(i, segments[i]);
    }

    public void SetTrajectoryLineRendererActive(bool active)
    {
        m_TrajectoryLineRenderer.enabled = active;
    }
}
