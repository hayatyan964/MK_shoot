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
        // タイマーがまだ残っている場合
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            // 整数部分だけを表示
            //timerText.text = ( "残り時間:" +Mathf.Floor(timeRemaining).ToString());
            timerText.text = (Mathf.Floor(timeRemaining).ToString());
        }
        else
        {
            // タイマーが終了した場合は0秒を表示
            timerText.text = "0";
            isGameover = true;
            result();
        }

        if (!isGameover && shotController.totalMenkoCount < 2)
        {
            shotController.StageMenkoLoader(); // メンコを再生成
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
        resultScoreText.text = "スコア" + score;
        if (score >= 20) //20以上　達人
        {
            resultEvText.text = "達人";
        }
        else if (score >= 15) //15以上 名人
        {
            resultEvText.text = "名人";
        }
        else if (score >= 10) //10以上プロメンコやー ?
        {
            resultEvText.text = "プロメンコやー";
        }
        else if (score >= 7) //7以上　匠
        {
            resultEvText.text = "匠";
        }
        else if (score >= 5) //5以上　すごい
        {
            resultEvText.text = "すごい";
        }
        else //5以下　がんばりましょう！
        {
            resultEvText.text = "がんばりましょう！";
        }
    } 

    private void Restart()
    {
        Debug.Log("リスタート");
        SceneManager.LoadScene(1);
    }
    private void ToTitle()
    {
        Debug.Log("タイトル画面");
        SceneManager.LoadScene(0);
    }
    private void GameOver()
    {
        Debug.Log("ゲーム終了");
        Application.Quit();
    }
}
