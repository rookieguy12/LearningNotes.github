using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    float _xMove;
    float _yMove;
    public float MoveSpeed;
    public float XMove{
        get=>_xMove;
        set=>_xMove = value;
    }

    public float YMove{
        get=>_yMove;
        set=>_yMove = value;
    }

    void Update()
    {
        XMove = Input.GetAxis("Horizontal");
        YMove = Input.GetAxis("Vertical");
    }

    private void FixedUpdate() {
        transform.Translate(new Vector3(XMove,YMove,0) * Time.fixedDeltaTime * MoveSpeed);
    }
}
