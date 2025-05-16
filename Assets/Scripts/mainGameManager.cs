using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class mainGameManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    public Text timerText;
    private float timeRemaining = 60f;

    private int score = 0;
    public bool isGameover = false;

    private FxShoot shotController;
    [SerializeField]
    private Canvas mainGameCanvas;
    [SerializeField]
    private Canvas resultCanvas;
    [SerializeField]
    private Button reStartButton;
    [SerializeField]
    private Button TitleButton;
    [SerializeField]
    private Button EndButton;

    [SerializeField]
    private Text resultScoreText;
    [SerializeField]
    private Text resultEvText;

    void Start()
    {
        resultCanvas.enabled = false;
        mainGameCanvas.enabled = true;

        shotController = GameObject.Find("ShootManager").GetComponent<FxShoot>();
        shotController.StageMenkoLoader();
        scoreText.text = score.ToString();

        reStartButton.onClick.AddListener(Restart);
        TitleButton.onClick.AddListener(ToTitle);
        EndButton.onClick.AddListener(GameOver);
    }

    void Update()
    {
        // �^�C�}�[���܂��c���Ă���ꍇ
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            // ��������������\��
            //timerText.text = ( "�c�莞��:" +Mathf.Floor(timeRemaining).ToString());
            timerText.text = (Mathf.Floor(timeRemaining).ToString());
        }
        else
        {
            // �^�C�}�[���I�������ꍇ��0�b��\��
            timerText.text = "0";
            isGameover = true;
            result();
        }

        if (!isGameover && shotController.totalMenkoCount < 2)
        {
            shotController.StageMenkoLoader(); // �����R���Đ���
        }
    }
    public void increaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    private void result()
    {
        mainGameCanvas.enabled = false;
        resultCanvas.enabled = true;
        resultScoreText.text = "�X�R�A" + score;
        if (score >= 20) //20�ȏ�@�B�l
        {
            resultEvText.text = "�B�l";
        }
        else if (score >= 15) //15�ȏ� ���l
        {
            resultEvText.text = "���l";
        }
        else if (score >= 10) //10�ȏ�v�������R��[ ?
        {
            resultEvText.text = "�v�������R��[";
        }
        else if (score >= 7) //7�ȏ�@��
        {
            resultEvText.text = "��";
        }
        else if (score >= 5) //5�ȏ�@������
        {
            resultEvText.text = "������";
        }
        else //5�ȉ��@����΂�܂��傤�I
        {
            resultEvText.text = "����΂�܂��傤�I";
        }
    } 

    private void Restart()
    {
        Debug.Log("���X�^�[�g");
        SceneManager.LoadScene(1);
    }
    private void ToTitle()
    {
        Debug.Log("�^�C�g�����");
        SceneManager.LoadScene(0);
    }
    private void GameOver()
    {
        Debug.Log("�Q�[���I��");
        Application.Quit();
    }
}
