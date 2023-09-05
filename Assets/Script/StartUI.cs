using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public GameObject exit;
    private GameObject exitPanel;
    private GameObject NoExit;
    private GameObject YesExit;

    public GameObject sound;
    public Slider soundSlider;
    public AudioSource backSound;

    private void Awake()
    {
        exitPanel = GameObject.Find("ExitPanel");
        NoExit = GameObject.Find("No");
        YesExit = GameObject.Find("Yes");
        exitPanel.transform.SetAsFirstSibling(); //화면 상 맨 뒤로 보내기//처음으로 쌓임
        exitPanel.SetActive(false);

        soundSlider = GameObject.Find("Canvas").transform.Find("SoundSlider").gameObject.GetComponent<Slider>();
        soundSlider.value = 0.5f;
        soundSlider.gameObject.SetActive(false);
        backSound = GameObject.Find("BackSound").GetComponent<AudioSource>();
        backSound.volume = 0.5f;

        exit.GetComponent<Button>().onClick.AddListener(delegate
        {
            exitPanel.transform.SetAsLastSibling();
            exitPanel.SetActive(true);
        });

        NoExit.GetComponent<Button>().onClick.AddListener(delegate
        {
            exitPanel.transform.SetAsFirstSibling();
            exitPanel.SetActive(false);
        });

        YesExit.GetComponent<Button>().onClick.AddListener(delegate
        {
            Application.Quit();
        });

        sound.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (!soundSlider.gameObject.active)
            {
                soundSlider.gameObject.SetActive(true);
            }
            else
            {
                soundSlider.gameObject.SetActive(false);
            }
        });

        soundSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate
        {
            backSound.volume = soundSlider.value;
        });
    }

}

