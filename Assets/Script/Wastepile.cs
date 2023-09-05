using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wastepile : MonoBehaviour
{
    private Hand handsc;
    private GameManager gamesc;
    public List<int> wastepile;
    public int lastCard = -1;

    private void Awake()
    {
        handsc = GameObject.Find("MyHand").GetComponent<Hand>();
        gamesc = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = null;
    }

    public void Discard(int player, Card card)
    {
        if (handsc.hand[player].Contains(card.cardId))
        {
            wastepile.Add(card.cardId);
            handsc.hand[player].Remove(card.cardId);
            card.gameObject.transform.parent = null;
            card.MoveTo(transform.position.x, transform.position.y, 0.05f, true);
        }
        else
        {
            Debug.Log("해당 card가 player hand에 존재하지 않습니다.");
        }
    }

    public void ChangeLastCard(int card, bool blue)
    {
        lastCard = card;
        
        string path = "Sprite/";
        path += card.ToString();
        if (blue) path += 'r';
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);
    }

}
