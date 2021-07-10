using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Orb : MonoBehaviour
{
    public SpriteRenderer type;

    public int row, column;

    PuzzleController puzzleController;
    public int id = 0;

    private bool isMoving = false;
    public bool isCount = false;

    public bool group = false;
    public int groupNum;
    public List<Orb> linkOrbs = new List<Orb>();

    public bool removed = false;
    public string removeText;


    private void Awake()
    {
        puzzleController = transform.parent.GetComponent<PuzzleController>();
        type = GetComponent<SpriteRenderer>();
    }


    void Start()
    {
        
    }


    public void ChangeImage()
    {        
        type.sprite = puzzleController.orbImage[Random.Range(0, 6)];
    }
        

    void Update()
    {

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, puzzleController.orbPos[id], 0.12f);

            if (transform.position == puzzleController.orbPos[id])
            {
                isMoving = false;
            }

        }

    }


    private void OnMouseDrag()
    {
        if(puzzleController.isLock == false)
        {
            Camera cam = Camera.main;
            Vector3 newPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Bounds bound = transform.parent.GetComponent<BoxCollider>().bounds;
            Vector3 min = bound.min;
            Vector3 max = bound.max;

            newPos = new Vector3(Mathf.Clamp(newPos.x, min.x, max.x), Mathf.Clamp(newPos.y, min.y, max.y), 0);

            transform.position = new Vector3(newPos.x, newPos.y, 0);

            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }


    private void OnMouseUp()
    {
        if(puzzleController.isLock == false)
        {
            transform.position = puzzleController.orbPos[id];
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            puzzleController.isLock = true;

            StartCoroutine(FadeWait());
        }
    }


    IEnumerator FadeWait()
    {
        bool isRemove;

        do
        {
            puzzleController.hasRemove = false;
            puzzleController.ResetOrbStatus();
            puzzleController.OrbGroup();
            puzzleController.OrbCombo();

            foreach(List<Orb> orbGroup in puzzleController.orbGroups)
            {
                isRemove = puzzleController.OrbRemove(orbGroup);

                if (isRemove)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                
            }
            
            puzzleController.AllCheckFall();
            yield return new WaitForSeconds(0.6f);


        } while (puzzleController.hasRemove == true);

        puzzleController.isLock = false;  
        
    }


    public void CheckLinkOrbs()
    {
        RaycastHit2D[] hitsR = Physics2D.LinecastAll(puzzleController.orbPos[id], puzzleController.orbPos[id] + transform.right * 10, 256);
        RaycastHit2D[] hitsL = Physics2D.LinecastAll(puzzleController.orbPos[id], puzzleController.orbPos[id] - transform.right * 10, 256);
        RaycastHit2D[] hitsU = Physics2D.LinecastAll(puzzleController.orbPos[id], puzzleController.orbPos[id] + transform.up * 10, 256);
        RaycastHit2D[] hitsD = Physics2D.LinecastAll(puzzleController.orbPos[id], puzzleController.orbPos[id] - transform.up * 10, 256);

        if (hitsR.Length > 1)
        {
            linkOrbs.Add(hitsR[1].collider.GetComponent<Orb>());
        }

        if (hitsL.Length > 1)
        {
            linkOrbs.Add(hitsL[1].collider.GetComponent<Orb>());
        }

        if (hitsU.Length > 1)
        {
            linkOrbs.Add(hitsU[1].collider.GetComponent<Orb>());
        }

        if (hitsD.Length > 1)
        {
            linkOrbs.Add(hitsD[1].collider.GetComponent<Orb>());
        }
    }


    public void Fade()
    {
        GetComponent<Animation>().Play("PuzzleFade");
    }


    public void MoveToUp()
    {
        id *= -1;
        transform.position = new Vector3(transform.position.x, -transform.position.y, 0);
        ChangeImage();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!puzzleController.isLock)
        {
            foreach (Orb pz in puzzleController.orbs)
            {
                if (pz.id == collision.GetComponent<PuzzleSlot>().id)
                {
                    int temp = id;
                    int tempC = column;
                    int tempR = row;
                    id = pz.id;
                    column = pz.column;
                    row = pz.row;

                    pz.id = temp;
                    pz.column = tempC;
                    pz.row = tempR;
                    pz.isMoving = true;
                    pz.GetComponent<SpriteRenderer>().sortingOrder = 0;
                }
            }
        }

    }


    public void CheckFall()
    {
        isMoving = true;
        
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, transform.position - transform.up * 10, 256);

        if (hits.Length == 1)
        {
            id = Mathf.Abs(id % 6) + 24;
        }
        else
        {
            id = hits[1].collider.GetComponent<Orb>().id - 6;

        }
        
    }


}
