using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{ 
    public int cardId = -1;
    private Hand handsc;
    private GameManager gamesc;

    private void Awake()
    {
        handsc = GameObject.Find("MyHand").GetComponent<Hand>();
        gamesc = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ShowCard(bool show)
    {
        if (!show)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Back");
            return;
        }
        if(0<=cardId/3 && cardId / 3 < 16)
        {
            string path = "Sprite/";
            int tmp = cardId / 3;
            path += tmp.ToString();
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(path);
        }
        else
        {
            switch (cardId / 3)
            {
                case 16:
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/+");
                    break;
                case 17:
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/-");
                    break;
                case 18:
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/X");
                    break;
                case 19:
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/%");
                    break;
                case 20:
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Joker");
                    break;
            }
            
        }  
    }

    private void OnMouseDown()
    {
        handsc.PlayCard(gamesc.player, this);
    }

    public void MoveTo(float x, float y, float time, bool destroy)
    {
        IEnumerator coroutine = Move(x, y, time, destroy);
        StartCoroutine(coroutine);
    }

    private IEnumerator Move(float endx, float endy, float time, bool destroy)
    {
        float startx, starty;
        startx = transform.position.x;
        starty = transform.position.y;
        for(int i =0; i<16; i++)
        {
            transform.position += new Vector3((endx-startx)/16, (endy-starty)/16,0.0f);
            yield return new WaitForSeconds(time / 16);
        }
        if (destroy)
        {
           Destroy(this.gameObject);
        }
    }

    public void FlipCard(bool show)
    {
        StartCoroutine(Flip(show));
    }
    
    private IEnumerator Flip(bool show)
    {
        for (int i = 0; i <= 90; i += 5)
        {
            transform.rotation = Quaternion.Euler(0, i, 0);
            yield return new WaitForSeconds(0.02f);
        }

        ShowCard(show);

        for (int i = 80; i >= 0; i -= 5)
        {
            transform.rotation = Quaternion.Euler(0, i, 0);
            yield return new WaitForSeconds(0.02f);
        }
    }
}

