using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GAMEmanager : MonoBehaviour
{
    //タイマーテキストの変数
    public TextMeshProUGUI timerText;
    private float timerCount = 0f;      //経過時間
    //勝利チームを表示するテキストの変数
    public TextMeshProUGUI blueWinText; //青チーム
    public TextMeshProUGUI redWinText;  //赤チーム
    //スタート用、勝敗が付いたとき用のパネル変数
    [SerializeField] GameObject Panel;
    //スタート用テキストの変数
    public TextMeshProUGUI startText;
    private bool isGameStarted = false; //ゲーム開始フラグ
    //リスタートするボタンの変数
    [SerializeField] GameObject RestartButton;
    //タイトルへ戻るボタンの変数
    [SerializeField] GameObject ReturnTitle;

    // Start is called before the first frame update
    void Start()
    {
        isGameStarted = true;
        timerText.text = "";
        RestartButton.SetActive(false);
        ReturnTitle.SetActive(false);
        Time.timeScale = 0.0f;  //ゲームを一旦停止させる（スタートのため）
        Panel.SetActive(false);
        blueWinText.text = "";
        redWinText.text = "";

        StartCoroutine(StartTextOpen());
    }
    public void AgainGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void returnTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Title",LoadSceneMode.Single);
    }

    IEnumerator StartTextOpen()
    {
        int countDown = 3;

        //カウントダウン表示
        while (countDown > 0)
        {
            Panel.SetActive(true);
            startText.text = countDown.ToString();
            //ゲーム停止中でもカウントダウンがすすむ
            yield return new WaitForSecondsRealtime(1.5f);
            
            countDown--;
        }

        //スタートの表示
        startText.text = "Start!";
        //1秒間表示
        yield return new WaitForSecondsRealtime(1f);

        //カウントダウンテキストを非表示にしてゲーム開始
        startText.text = "";
        Panel.SetActive(false);
        Time.timeScale = 1.0f;  //ゲーム再開
        isGameStarted = false;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {timerCount:F2}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //カウントダウン中は処理しない
        if (isGameStarted) return;

        //BluePlayerとRedPlayerのオブジェクト数を取得
        int blueCount = GameObject.FindGameObjectsWithTag("BluePlayer").Length;
        int redCount = GameObject.FindGameObjectsWithTag("RedPlayer").Length;

        timerCount += Time.deltaTime;
        UpdateTimerUI();

        //どちらかのチームの人数が0になったら
        if (blueCount == 0)
        {
            Panel.SetActive(true);
            RestartButton.SetActive(true);
            ReturnTitle.SetActive(true);
            redWinText.text = "RED TEAM WIN!!!";
            Time.timeScale = 0.0f;  //ゲームを終了させる
        }
        else if(redCount == 0)
        {
            Panel.SetActive(true);
            RestartButton.SetActive(true);
            ReturnTitle.SetActive(true);
            blueWinText.text = "BLUE TEAM WIN!!!";
            Time.timeScale = 0.0f;  //ゲームを終了させる
        }
    }
}
