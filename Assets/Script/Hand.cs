using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public GameObject cardPrefab;
    private GameObject opponent;
    private Stock stocksc;
    private GameManager gamesc;
    private Wastepile wastepilesc;
    private UI uisc;
    public List<int>[] hand = new List<int>[2];
    /* playState : 현제 turn에서 어떤 방식으로 낼지에 대한 변수
       0 : 미정
       1 : 연산자 + 숫자
       2 : 조커 + 숫자
     */
    private int playState = 0;
    // playingCards: 낸 카드 고유번호 저장
    private Card[] playingCards = new Card[2];
    //moveResult: move를 내면 나오는 값, move의 값이 invalid이면 -1을 저장
    private int moveResult = -1;

    private void Awake()
    {
        stocksc = GameObject.Find("Stock").GetComponent<Stock>();
        gamesc = GameObject.Find("GameManager").GetComponent<GameManager>();
        wastepilesc = GameObject.Find("Wastepile").GetComponent<Wastepile>();
        uisc = GameObject.Find("GameManager").GetComponent<UI>();
        opponent = GameObject.Find("Opponent");
    }

    // ShowHand: hand에 있는 카드를 보여주거나 뒷면이 보이게 함

    public void ShowHand(bool show)
    {
        foreach (Transform child in this.transform)
        {
            if (child != this.transform.GetChild(0))
            {
                child.gameObject.GetComponent<Card>().FlipCard(show);
            }
        }
    }

    /* Draw : stock에 있는 카드 1장을 해당 player의 hand로 이동
        * 단, player의 hand에 있는 카드의 수가 28 미만일 때
        * 아니면 카드를 뽑을 수 없다는 문구를 보여줌
       input : player : 뽑는 player의 번호
     * Log 출력
     */

    public bool Draw(int player)
    {
        uisc.CanNotDraw(hand[player].Count);
        if (hand[player].Count < 28)
        {
            int n = Random.Range(0, stocksc.stock.Count); //index 0 ~ stock length 중 random number
            hand[player].Add(stocksc.stock[n]);
            stocksc.stock.RemoveAt(n);
            GameObject newCard = Instantiate(cardPrefab,new Vector3(stocksc.gameObject.transform.position.x, stocksc.gameObject.transform.position.y, 0.0f), Quaternion.identity);
            if(player == gamesc.player)
            {
                newCard.GetComponent<Card>().MoveTo(transform.position.x, transform.position.y, 0.5f,true);
            }
            else
            {
                newCard.GetComponent<Card>().MoveTo(opponent.transform.position.x, opponent.transform.position.y, 0.5f,true);
            }
            
            if (stocksc.stock.Count <= 0)
            {
                stocksc.Reshuffle();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /* PlayCard : player가 card를 play를 함
        * card가 play 가능한지 확인
        * play 가능하면 play 하기
        * play 가능하지 않으면 처음부터 다시 고르게 만듦
       input :
            player : 뽑는 player의 번호
            card : 뽑힌 카드의 고유번호
     */
    public void PlayCard(int player, Card card)
    {
        if(moveResult == -1)
        {
            if (playState == 0) // 아직 어떤 방식으로 낼 지 미정
            {
                if (gamesc.CardType(card.cardId) == 0) // 숫자
                {
                    if (wastepilesc.lastCard == -1) // 현재 wastepile에 카드가 없음, 모든 숫자 카드 가능
                    {
                        uisc.AddMove(gamesc.Decode(card.cardId));
                        moveResult = gamesc.Decode(card.cardId);
                        playingCards[0] = card;
                    }
                    else // 현재 wastepile에 카드가 있음
                    {
                        if (wastepilesc.lastCard - gamesc.Decode(card.cardId)!=0 &&(Mathf.Abs(wastepilesc.lastCard - gamesc.Decode(card.cardId)) <= 2 || Mathf.Abs(wastepilesc.lastCard - gamesc.Decode(card.cardId)) >= 14))// 낼 수 있는 숫자 카드
                        {
                            uisc.AddMove(gamesc.Decode(card.cardId));
                            moveResult = gamesc.Decode(card.cardId);
                            playingCards[0] = card;
                        }
                        else // 낼 수 없는 숫자 카드
                        {
                            Reset();
                        }
                    }
                }
                else // 연산자 or 조커가 뽑힘
                {
                    if (wastepilesc.lastCard != -1)// wastepile에 카드가 있음
                    {
                        playState = gamesc.CardType(card.cardId); //연산자 + 숫자 or 조커 + 숫자인 방식으로 결정
                        uisc.AddMove(gamesc.Decode(card.cardId));
                        playingCards[0] = card;
                    }
                    else //wastepile에 카드 없음 -> 가능하지 않는 경우의 수
                    {
                        Reset();
                    }
                }
            }
            else //연산자 + 숫자 or 조커 + 숫자인 방식
            {
                if (gamesc.CardType(card.cardId) == 0) // 두번째 카드로 숫자가 뽑힌 경우
                {
                    if (playState == 1)// 연산자 + 숫자
                    {
                        int value; // 연산 후의 값
                        switch (gamesc.Decode(playingCards[0].cardId))
                        {
                            case 16:
                                value = wastepilesc.lastCard + gamesc.Decode(card.cardId);
                                break;
                            case 17:
                                value = wastepilesc.lastCard - gamesc.Decode(card.cardId);
                                break;
                            case 18:
                                value = wastepilesc.lastCard * gamesc.Decode(card.cardId);
                                break;
                            case 19:
                                if (gamesc.Decode(card.cardId) == 0) value = -1; // 뽑은 카드 ==0 -> 안 됨
                                else if (wastepilesc.lastCard % gamesc.Decode(card.cardId) != 0)
                                { // 나눈 값 != 자연수 -> 안됨
                                    value = -1;
                                }
                                else
                                {
                                    value = wastepilesc.lastCard / gamesc.Decode(card.cardId);
                                }
                                break;
                            default:
                                value = -1;
                                break;
                        }
                        if (0 <= value && value < 16) // 결과값이 범위 내 있음
                        {
                            uisc.AddMove(gamesc.Decode(card.cardId));
                            playingCards[1] = card;
                            moveResult = value;
                        }
                        else //결과값이 범위 내에 없음
                        {
                            Reset();
                        }
                    }
                    else // 조커 + 숫자, 모든 숫자 가능
                    {
                        uisc.AddMove(gamesc.Decode(card.cardId));
                        playingCards[1] = card;
                        moveResult = gamesc.Decode(card.cardId);
                    }
                }
                else //나머지 : 연산자 or 조커 + 연산자 or 조커 -> nonplayable 한 경우, 처음부터 다시 뽑기
                {
                    Reset();
                }
            }
        } 
    }
    // Reset : 내가 play하고픈 카드들을 초기화함
    public void Reset()
    {
        uisc.AddMove(-1);
        playState = 0;
        moveResult = -1;
    }
    /* Turn : player의 차례에 카드 정렬, 카드 보여줌
     * input : player : turn에 카드를 내는 player의 번호 
     */
    public void Turn(int player)
    {
        SortList(player);
        this.transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 0; i < hand[player].Count; i++)
        {
            Card tmp = Instantiate(cardPrefab, transform.position, Quaternion.identity, this.transform).GetComponent<Card>();
            tmp.MoveTo((i % 7) * 0.8f + transform.position.x, (i / 7) * 1.2f+ transform.position.y, 0.5f,false);
            tmp.cardId = hand[player][i];
            tmp.FlipCard(true);
        }
    }

    //TurnEnd : player의 차례가 끝났을 때 Reset(), 카드를 화면상에서 없앰
    public void TurnEnd()
    {
        Reset();
        this.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine("TurnEndAnim");
    }

    //PlayHand : 뽑은 카드가 적절하면 내고 적절하지 않으면 Draw()
    public void PlayHand()
    {
        if (moveResult != -1)// play 가능한 combination of cards를 뽑은 상태
        {
            wastepilesc.Discard(gamesc.player, playingCards[0]);
            if (playState != 0) //playingCards 중 2개 다 사용중
            {
                wastepilesc.Discard(gamesc.player, playingCards[1]);
            }
            wastepilesc.ChangeLastCard(moveResult, playState == 1);
            uisc.WriteLog(0); //Log
            uisc.AddMove(-1);
        }
        else // play 가능한 combination of cards를 뽑지 못한 상태
        {
            if (Draw(gamesc.player))
            {
                //draw 된 상태
                uisc.WriteLog(1); //Log draw
            }
            else
            {
                //draw하지 않고 그냥 pass 된 상태
                uisc.WriteLog(2); //Log Pass
            }
        }
    }

    private void SortList(int player)
    {
        hand[player].Sort(0, hand[player].Count, null);
    }

    public bool Win(int player)
    {
        if (hand[player].Count < 1)
        {
            return true;
        }
        return false;
    }

    private IEnumerator TurnEndAnim()
    {
        ShowHand(false);
        foreach (Transform child in this.transform)
        {
            if (child != this.transform.GetChild(0))
            {
                child.gameObject.GetComponent<Card>().MoveTo(transform.position.x, transform.position.y, 0.5f, true);
            }
        }
        yield return new WaitForSeconds(0.6f);
    }
}
