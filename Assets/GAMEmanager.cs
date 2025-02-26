using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GAMEmanager : MonoBehaviour
{
    //�^�C�}�[�e�L�X�g�̕ϐ�
    public TextMeshProUGUI timerText;
    private float timerCount = 0f;      //�o�ߎ���
    //�����`�[����\������e�L�X�g�̕ϐ�
    public TextMeshProUGUI blueWinText; //�`�[��
    public TextMeshProUGUI redWinText;  //�ԃ`�[��
    //�X�^�[�g�p�A���s���t�����Ƃ��p�̃p�l���ϐ�
    [SerializeField] GameObject Panel;
    //�X�^�[�g�p�e�L�X�g�̕ϐ�
    public TextMeshProUGUI startText;
    private bool isGameStarted = false; //�Q�[���J�n�t���O
    //���X�^�[�g����{�^���̕ϐ�
    [SerializeField] GameObject RestartButton;
    //�^�C�g���֖߂�{�^���̕ϐ�
    [SerializeField] GameObject ReturnTitle;

    // Start is called before the first frame update
    void Start()
    {
        isGameStarted = true;
        timerText.text = "";
        RestartButton.SetActive(false);
        ReturnTitle.SetActive(false);
        Time.timeScale = 0.0f;  //�Q�[������U��~������i�X�^�[�g�̂��߁j
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

        //�J�E���g�_�E���\��
        while (countDown > 0)
        {
            Panel.SetActive(true);
            startText.text = countDown.ToString();
            //�Q�[����~���ł��J�E���g�_�E����������
            yield return new WaitForSecondsRealtime(1.5f);
            
            countDown--;
        }

        //�X�^�[�g�̕\��
        startText.text = "Start!";
        //1�b�ԕ\��
        yield return new WaitForSecondsRealtime(1f);

        //�J�E���g�_�E���e�L�X�g���\���ɂ��ăQ�[���J�n
        startText.text = "";
        Panel.SetActive(false);
        Time.timeScale = 1.0f;  //�Q�[���ĊJ
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
        //�J�E���g�_�E�����͏������Ȃ�
        if (isGameStarted) return;

        //BluePlayer��RedPlayer�̃I�u�W�F�N�g�����擾
        int blueCount = GameObject.FindGameObjectsWithTag("BluePlayer").Length;
        int redCount = GameObject.FindGameObjectsWithTag("RedPlayer").Length;

        timerCount += Time.deltaTime;
        UpdateTimerUI();

        //�ǂ��炩�̃`�[���̐l����0�ɂȂ�����
        if (blueCount == 0)
        {
            Panel.SetActive(true);
            RestartButton.SetActive(true);
            ReturnTitle.SetActive(true);
            redWinText.text = "RED TEAM WIN!!!";
            Time.timeScale = 0.0f;  //�Q�[�����I��������
        }
        else if(redCount == 0)
        {
            Panel.SetActive(true);
            RestartButton.SetActive(true);
            ReturnTitle.SetActive(true);
            blueWinText.text = "BLUE TEAM WIN!!!";
            Time.timeScale = 0.0f;  //�Q�[�����I��������
        }
    }
}
