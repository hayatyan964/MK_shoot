using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    public Slider shotGauge;
    private float speed = 0;
    public float gaugeLength = 0;
    private bool shotGaugeSet = false;
    private bool gaugeReturn = false;
    private bool raycastHit = false;

    private float gaugeSpeed = 1.6f;
    
    private Vector3 spawnPos;
    private Vector3 targetPos;
    [SerializeField]
    private RawImage targetImage;

    public LayerMask groundLayer;

    private bool isTurn = false;
    private float gaugeTimer = 0.3f;

    private Ray mkray;
    private RaycastHit hit;

    private FxShoot shotController;

    void Start()
    {
        isTurn = true;
        shotController = GameObject.Find("ShootManager").GetComponent<FxShoot>();
    }


    private void Update()
    {

        if (isTurn)
        {
            if (shotGaugeSet)
            {
                targetPos = Input.mousePosition;
                targetImage.transform.position = targetPos;
                shotGaugeValue();
            }
        }
        else
        {
            if(gaugeTimer > 0)
            {
                gaugeTimer -=Time.deltaTime;
            }
            else
            {
                isTurn = true;
                gaugeTimer = 0.3f;
            }
        }
    }
    public void OnButtonDown()
    {
        if (isTurn)
        {
            Debug.Log("Down");
            shotGaugeSet = true;
        }
    }
    // ボタンを離したときの処理
    public void OnButtonUp()
    {
        if (isTurn)
        {
            Debug.Log("Up");
            shotGaugeSet = false;

            if (gaugeLength > 1f) gaugeLength = 1f;

            mkray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mkray, out hit, Mathf.Infinity, groundLayer))
            {
                spawnPos = hit.point + Vector3.up;
            }
            shotController.mkShoot(spawnPos, gaugeLength);

            gaugeLength = 0;
            isTurn = false;
        }
    }

    private void shotGaugeValue()
    {
        float frameSpeed = gaugeSpeed * Time.deltaTime;  // フレームごとのスピード

        if (gaugeReturn)
        {
            gaugeLength -= frameSpeed;
            if (gaugeLength <= 0f) // ゲージが0未満にいかないように
            {
                gaugeLength = 0f;
                gaugeReturn = false;
            }
        }
        else
        {
            gaugeLength += frameSpeed;
            if (gaugeLength >= 1f) // ゲージが1以上にならないように
            {
                gaugeLength = 1f;
                gaugeReturn = true;
            }
        }
        shotGauge.value = gaugeLength;
    }
}
