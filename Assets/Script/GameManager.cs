using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Scene 전환에 필요

public class GameManager : MonoBehaviour
{
    //player : 현재 turn인 player의 #
    public int player; //0, 1
    private GameObject myHand;
    private Hand handsc;
    private Stock stocksc;
    private Wastepile wastepilesc;
    private UI uisc;
    //Panel
    private GameObject WinPanel1;//use when Player1 win
    private GameObject WinPanel2;//use when Player2 win

    private void Awake()
    {
        myHand = GameObject.Find("MyHand");
        handsc = myHand.GetComponent<Hand>();
        stocksc = GameObject.Find("Stock").GetComponent<Stock>();
        wastepilesc = GameObject.Find("Wastepile").GetComponent<Wastepile>();
        uisc = GameObject.Find("GameManager").GetComponent<UI>();

        WinPanel1 = GameObject.Find("WinPanel1"); //GameObject.Find("Canvas").transform.Find("WinPanel1");
        WinPanel2 = GameObject.Find("WinPanel2");
    }

    private void Start()
    {
        Deal();
        WinPanel1.SetActive(false);
        WinPanel2.SetActive(false);
    }

    private void Deal()
    {
        handsc.hand[0] = new List<int>();
        handsc.hand[1] = new List<int>();
        stocksc.stock = new List<int>();
        wastepilesc.wastepile = new List<int>();

        player = 0;

        for (int i = 0; i < 63; i++) //stock에 0~62 저장
        {
            stocksc.stock.Add(i);
        }

        IEnumerator coroutine = DealAnim(10);
        StartCoroutine(coroutine);   
    }

    public void Turn()
    {
        handsc.Turn(player);
        uisc.Turn();
    }

    public void TurnEnd()
    {
        StartCoroutine("ChangePlayerAnim");
        handsc.TurnEnd();
        uisc.TurnEnd();
        if (handsc.Win(player))
        {//someone win
            Win();
        }
        player = (player + 1) % 2;
    }

    public void GiveUpWin()
    {
        player = (player + 1) % 2;
        Win();
    }

    private void Win()
    {
        if (player == 0)
        {
            WinPanel1.transform.SetAsLastSibling(); //맨 위에 띄우기
            WinPanel1.SetActive(true);
        }
        else
        {
            WinPanel2.transform.SetAsLastSibling(); //맨 위에 띄우기
            WinPanel2.SetActive(true);
        }
    }

    /* Decode : 카드의 고유 번호를 통해 카드가 표현하는 값을 리턴
      input : card : 카드의 고유번호
      output : 카드가 표현하는 값
       - 0 ~ 15 : 해당 숫자 값
       - 16 ~ 19 : 순서대로 +, -, *, /
       - 20 : 조커
       - -1: 에러
     */
    public int Decode(int card)
    {
        if (0 <= card && card < 63)
        {
            return card / 3;
        }
        else
        {
            Debug.Log("Out of Bounds");
            return -1;
        }
    }

    /* CardType : 카드의 고유번호를 이용해어 카드의 종류를 리턴
       input : card : 카드의 고유번호
       output: 카드의 종류
        - 0 : 숫자
        - 1 : 연산자
        - 2 : 조커
        - -1 : 에러
    */
    public int CardType(int card)
    {
        if(0<= card && card < 63)
        {
            if (card < 48) return 0;
            else if (card < 60) return 1;
            else return 2;
        }
        else
        {
            Debug.Log("Out of Bounds");
            return -1;
        }
    }

    private IEnumerator DealAnim(int card)
    {
        for (int i = 0; i < card; i++) // player1,2에 각 10장의 카드 나눠줌
        {
            handsc.Draw(0);
            yield return new WaitForSeconds(0.01f);
            handsc.Draw(1);
            yield return new WaitForSeconds(0.01f);
        }
        uisc.SetTurn();
    }

    private IEnumerator ChangePlayerAnim()
    {
        yield return new WaitForSeconds(0.1f);
        float opponentx = GameObject.Find("Opponent").transform.position.x;
        float opponenty = GameObject.Find("Opponent").transform.position.y;
        handsc.transform.GetChild(0).gameObject.GetComponent<Card>().MoveTo(opponentx, opponenty, 0.5f, false);
        GameObject.Find("Opponent").GetComponent<Card>().MoveTo(handsc.transform.position.x, handsc.transform.position.y, 0.5f, false);
        yield return new WaitForSeconds(1.0f);
        uisc.SetTurn();
        handsc.transform.GetChild(0).transform.position = new Vector3(handsc.transform.position.x, handsc.transform.position.y,0);
        GameObject.Find("Opponent").transform.position = new Vector3(opponentx, opponenty, 0);
    }
}
 