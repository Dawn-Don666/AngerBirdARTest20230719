using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private void Start()
    {
        // GetComponent<TrailRenderer>().enabled = false;
        // GetComponent<TrailRenderer>().sortingLayerName = "Foreground";
        GetComponent<Rigidbody>().isKinematic = true;
        //GetComponent<CapsuleCollider>().radius = Constants.BirdColliderRadiusBig;
        State = BirdState.BEFORE_THROWN;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            GameManager.Instance.m_GameState = GameState.START;
            GameManager.Instance.isFinishThrow = true;
            GameManager.Instance.m_SlingShot.SetTrajectoryLineRendererActive(false);
            State = BirdState.BEFORE_THROWN;

            Destroy(gameObject);
            GameManager.Instance.RandomCreateBird();
        }
        else
        {
            GameManager.Instance.m_GameState = GameState.START;
            GameManager.Instance.isFinishThrow = true;
            GameManager.Instance.m_SlingShot.SetTrajectoryLineRendererActive(false);
            State = BirdState.BEFORE_THROWN;
            //TODO 状态为没打到
            Destroy(gameObject);
            GameManager.Instance.RandomCreateBird();
        }
    }

    //抛出的时候
    public void OnThrow()
    {
        //GetComponent<TrailRenderer>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false; //2D
        //GetComponent<CapsuleCollider>().radius = Constants.BirdColliderRadiusNormal; //2D
        State = BirdState.THROWN;
    }

    public BirdState State
    {
        get;
        private set;
    }
}
