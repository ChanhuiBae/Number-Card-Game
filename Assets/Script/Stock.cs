using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stock : MonoBehaviour
{
    public List<int> stock;
    private Wastepile wastepilesc;
    private Hand handsc;
    private GameManager gamesc;
    private UI uisc;

    private void Awake()
    {
        handsc = GameObject.Find("MyHand").GetComponent<Hand>();
        gamesc = GameObject.Find("GameManager").GetComponent<GameManager>();
        wastepilesc = GameObject.Find("Wastepile").GetComponent<Wastepile>();
        uisc = GameObject.Find("GameManager").GetComponent<UI>();
    }

    // Reshuffle : wastepile에 있는 마지막 카드를 제외한 모든 카드를 stock로 이동
    public void Reshuffle()
    {
        stock.InsertRange(0, wastepilesc.wastepile);
        stock.RemoveAt(wastepilesc.wastepile.Count - 1);
        wastepilesc.wastepile.RemoveAll(x => x < wastepilesc.wastepile.Count); // 어떻게 지워지는지 확인 필요
    }

    void OnMouseDown()
    {
        if (!uisc.paused && handsc.Draw(gamesc.player))
        {
            gamesc.TurnEnd();
        }

    }
}
