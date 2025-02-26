using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneBGM : MonoBehaviour
{
    //�I�[�f�B�I�\�[�X
    �@public AudioSource GameBGM;
    �@public AudioSource VictorySE;

    //����SE���Ǘ�����bol
    private bool isVictory = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayGameBGM();
    }

    public void PlayGameBGM()
    {
        StopAllBGM();
        GameBGM.Play();
    }

    public void PlayVictorySE()
    {
        StopAllBGM();
        VictorySE.Play();
    }

    public void StopAllBGM()
    {
        GameBGM.Stop();
        VictorySE.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //BluePlayer��RedPlayer�̃I�u�W�F�N�g�����擾
        int blueCount = GameObject.FindGameObjectsWithTag("BluePlayer").Length;
        int redCount = GameObject.FindGameObjectsWithTag("RedPlayer").Length;

        if (blueCount == 0 && !isVictory)
        {
            PlayVictorySE();
            isVictory = true;
        }
        else if (redCount == 0 && !isVictory)
        {
            PlayVictorySE();
            isVictory = true;
        }
    }
}
