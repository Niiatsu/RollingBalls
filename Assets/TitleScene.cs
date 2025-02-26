using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    //ゲーム開始ボタンの変数
    [SerializeField] GameObject GameStartButton;
    //遊び方表示ボタンの変数
    [SerializeField] GameObject GameExitButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //ゲームシーンへ遷移
    public void GoToGameScene()
    {
        SceneManager.LoadScene("GameScene",LoadSceneMode.Single);
    }

    //ゲームを終了する
    public void GameExit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
