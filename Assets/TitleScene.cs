using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    //�Q�[���J�n�{�^���̕ϐ�
    [SerializeField] GameObject GameStartButton;
    //�V�ѕ��\���{�^���̕ϐ�
    [SerializeField] GameObject GameExitButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //�Q�[���V�[���֑J��
    public void GoToGameScene()
    {
        SceneManager.LoadScene("GameScene",LoadSceneMode.Single);
    }

    //�Q�[�����I������
    public void GameExit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
