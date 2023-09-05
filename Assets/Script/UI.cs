using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject pause;
    public GameObject nextTurn;
    public GameObject reset;
    public GameObject giveup;
    public GameObject turnChange;
    public GameObject sound;
    private GameObject cannotDraw;
    private Text time;
    private Text move;
    private Text moveLog;
    private Text playerTurn;
    private Text OpponentCardN;
    private GameManager gamesc;
    private Hand handsc;
    private GameObject giveupPanel;
    private GameObject NoGiveup;
    private GameObject YesGiveup;

    public Slider soundSlider;
    public AudioSource backSound;

    // paused : 일시 정지 때 true, else false
    public bool paused = true;
    private float timer = 30.0f;

    private void Awake()
    {
        time = GameObject.Find("Canvas").transform.Find("Time").gameObject.GetComponent<Text>();
        move = GameObject.Find("Canvas").transform.Find("Move").gameObject.GetComponent<Text>();
        moveLog = GameObject.Find("Canvas").transform.Find("MoveLog").gameObject.GetComponent<Text>();
        playerTurn = GameObject.Find("Canvas").transform.Find("PlayerTurn").gameObject.GetComponent<Text>();
        OpponentCardN = GameObject.Find("Canvas").transform.Find("OpponentCard#").gameObject.GetComponent<Text>();
        cannotDraw = GameObject.Find("CanNotDraw");
        gamesc = GameObject.Find("GameManager").GetComponent<GameManager>();
        handsc = GameObject.Find("MyHand").GetComponent<Hand>();

        giveupPanel = GameObject.Find("GiveUpPanel");
        NoGiveup = GameObject.Find("No");
        YesGiveup = GameObject.Find("Yes");
        giveupPanel.transform.SetAsFirstSibling(); //화면 상 맨 뒤로 보내기//처음으로 쌓임
        giveupPanel.SetActive(false);

        soundSlider = GameObject.Find("Canvas").transform.Find("SoundSlider").gameObject.GetComponent<Slider>();
        soundSlider.value = 0.5f;
        soundSlider.gameObject.SetActive(false);
        backSound = GameObject.Find("BackSound").GetComponent<AudioSource>();
        backSound.volume = 0.5f;

        pause.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (0.0f < timer && timer < 30.0f) // 누군 가의 turn인 상황
            {
                if (paused)//현재 일시정지 상태 -> 재생
                {
                    handsc.ShowHand(true);
                    pause.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("UI/Iconic1024x1024")[60]; // 일시정지 버튼
                    paused = false;
                }
                else // 현재 재생 상태 -> 일시정지
                {
                    handsc.ShowHand(false);
                    pause.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("UI/Iconic1024x1024")[12]; // 재생 버튼
                    paused = true;
                }
            }
        });

        nextTurn.GetComponent<Button>().onClick.AddListener(delegate {
            handsc.PlayHand();
            gamesc.TurnEnd();
        });

        reset.GetComponent<Button>().onClick.AddListener(handsc.Reset);

        giveup.GetComponent<Button>().onClick.AddListener(delegate {
            paused = true;
            giveupPanel.transform.SetAsLastSibling();
            giveupPanel.SetActive(true);
        });

        NoGiveup.GetComponent<Button>().onClick.AddListener(delegate
        {
            giveupPanel.transform.SetAsFirstSibling();
            giveupPanel.SetActive(false);
            paused = false;
        });

        YesGiveup.GetComponent<Button>().onClick.AddListener(delegate
        {
            giveupPanel.transform.SetAsFirstSibling();
            giveupPanel.SetActive(false);
            gamesc.GiveUpWin();
        });

        turnChange.GetComponent<Button>().onClick.AddListener(gamesc.Turn);

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


    /*
     time에 남은 시간 표시
     timer의 시간 decrease
     timer에 time out -> turnEnd()
     */
    private void Update()
    {
        time.text = timer.ToString();
        if (!paused && timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0.0f)
        {
            handsc.PlayHand();
            gamesc.TurnEnd();
        }
    }

    /* AddMove : move에 해당 move를 했음을 표시해줌
        input : 
         m: 이번에 한 move
          0 ~ 20 : move의 값
          -1 : clear 
     */

    public void AddMove(int m)
    {
        if(0<=m && m < 16)
        {
            move.text += m.ToString();
        }else
        {
            switch (m)
            {
                case -1:
                    move.text = "";
                    break;
                case 16:
                    move.text += "+";
                    break;
                case 17:
                    move.text += "-";
                    break;
                case 18:
                    move.text += "*";
                    break;
                case 19:
                    move.text += "/";
                    break;
                case 20:
                    move.text += "J";
                    break;
            }
        }
    }

    public void WriteLog(int m) // m = 0 제출, m = 1 뽑음
    {
        if (m == 0)
        {
             if (gamesc.player == 0)
                moveLog.text = "Player1 : " + move.text;
            else
                moveLog.text = "Player2 : " + move.text;
        }
        else if(m == 1)
        {
            if (gamesc.player == 0)
                moveLog.text = "Player1 : Draw";
            else
                moveLog.text = "Player2 : Draw";
        }
        else
        {
            if (gamesc.player == 0)
                moveLog.text = "Player1 : Pass";
            else
                moveLog.text = "Player2 : Pass";
        }
    }

    public void CanNotDraw(int cardN)
    {
        if (cardN < 28)
        {
            cannotDraw.SetActive(false);
        }
        else
        {
            cannotDraw.SetActive(true); //뽑을 수 없다는 안내문구
        }
    }

    public void Turn()
    {
        pause.SetActive(true);
        nextTurn.SetActive(true);
        reset.SetActive(true);
        giveup.SetActive(true);
        paused = false;
        turnChange.SetActive(false);
        timer = 30.0f;
        move.text = "";
    }

    public void SetTurn()
    {
        CanNotDraw(handsc.hand[gamesc.player].Count);
        turnChange.SetActive(true);
        OpponentCardN.text = handsc.hand[(gamesc.player + 1) % 2].Count.ToString();
        if (gamesc.player == 0)
        {
            playerTurn.text = "Player 1";
        }
        else
        {
            playerTurn.text = "Player 2";
        }
    }

    public void TurnEnd()
    {
        pause.SetActive(false);
        nextTurn.SetActive(false);
        reset.SetActive(false);
        giveup.SetActive(false);
        timer = 30.0f;
        paused = true;
        time.text = "30.0";
        playerTurn.text = "";
        OpponentCardN.text = "";
    }


}

