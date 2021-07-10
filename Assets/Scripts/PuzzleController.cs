using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuzzleController : MonoBehaviour
{
    private int columnCount = 6;
    private int rowCount = 5;
    public int removeCount = 3;

    public Sprite[] orbImage;
    public Vector3[] orbPos;
    public Orb[] orbs;

    public bool isFade = false;
    public bool isLock = false;
    public int combo = 0;
    public int count = 0;
    public List<List<Orb>> orbGroups = new List<List<Orb>>();

    [System.Serializable]
    public struct Combo
    {
        public Sprite type;
        public int count;
    }

    public Combo[] orbCombo;

    public bool hasRemove = false;


    void Start()
    {
        InitGrid();
        SavePosition();

        do
        {
            hasRemove = false;
            ResetOrbStatus();
            OrbGroup();
            OrbCombo();
            OrbRemoveInit();

        } while (hasRemove);

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

    }


    private void InitGrid()
    {
        orbs = new Orb[30];
        orbs = GetComponentsInChildren<Orb>();

        for (int r = 0; r < rowCount; r++)
        {
            for(int c = 0; c < columnCount; c++)
            {
                orbs[r * columnCount + c].row = r;
                orbs[r * columnCount + c].column = c;
                orbs[r * columnCount + c].ChangeImage();
            }
        }

    }


    void FindMembers(Orb orb, int groupNum)
    {
        foreach(Orb linkOrb in orb.linkOrbs)
        {
            if(linkOrb.type.sprite == orb.type.sprite && linkOrb.group == false)
            {
                orbGroups[groupNum].Add(linkOrb);
                linkOrb.group = true;
                linkOrb.groupNum = groupNum;

                FindMembers(linkOrb, groupNum);
            }
        }
    }


    public void ResetOrbStatus()
    {
        foreach(Orb orb in orbs)
        {
            orb.group = false;
            orb.groupNum = -1;
            orb.removed = false;
            orb.removeText = "";
            orb.linkOrbs.Clear();
            orb.CheckLinkOrbs();
        }
    }


    public void OrbGroup()
    {
        orbGroups.Clear();

        Array.Sort(orbs, delegate (Orb orb1, Orb orb2)
        {
            return orb1.id.CompareTo(orb2.id);
        });

        foreach(Orb orb in orbs)
        {
            if(orb.group == false)
            {
                orbGroups.Add(new List<Orb>());
                int groupNum = orbGroups.Count - 1;
                orbGroups[groupNum].Add(orb);
                orb.group = true;
                orb.groupNum = groupNum;

                FindMembers(orb, groupNum);
            }
        }

    }



    public void OrbCombo()
    {
        orbCombo = new Combo[orbGroups.Count];

        foreach(List<Orb> orbGroup in orbGroups)
        {
            int comboIndex = orbGroups.IndexOf(orbGroup);
            orbCombo[comboIndex].type = orbGroup[0].type.sprite;
            orbCombo[comboIndex].count = 0;

            foreach (Orb orb in orbGroup)
            {
                FindRemoveOrb(orb, comboIndex);
            }

        }
    }


    void FindRemoveOrb(Orb orb, int comboIndex)
    {
        int orbCountV = 0;
        int orbCountH = 0;

        RaycastHit2D[] hitsR = Physics2D.LinecastAll(orb.transform.position, orb.transform.position + orb.transform.right * 10, 256);
        RaycastHit2D[] hitsU = Physics2D.LinecastAll(orb.transform.position, orb.transform.position + orb.transform.up * 10, 256);

        foreach(RaycastHit2D hit in hitsR)
        {
            if(hit.collider.GetComponent<Orb>().type.sprite == orb.type.sprite)
            {
                orbCountH++;
            }
            else
            {
                break;
            }
        }

        foreach (RaycastHit2D hit in hitsU)
        {
            if (hit.collider.GetComponent<Orb>().type.sprite == orb.type.sprite)
            {
                orbCountV++;
            }
            else
            {
                break;
            }
        }

        if (orbCountH >= removeCount)
        {
            for(int i = 0; i < orbCountH; i++)
            {
                Orb hitOrb = hitsR[i].collider.GetComponent<Orb>();

                if(hitOrb.removed == false)
                {
                    orbCombo[comboIndex].count += 1;
                }

                hitOrb.removed = true;
                hitOrb.removeText = "C";
            }
        }

        if (orbCountV >= removeCount)
        {
            for (int i = 0; i < orbCountV; i++)
            {
                Orb hitOrb = hitsU[i].collider.GetComponent<Orb>();

                if (hitOrb.removed == false)
                {
                    orbCombo[comboIndex].count += 1;
                }

                hitOrb.removed = true;
                hitOrb.removeText = "C";
            }
        }

    }


    void OrbRemoveInit()
    {
        foreach(List<Orb> orbGroup in orbGroups)
        {
            foreach(Orb orb in orbGroup)
            {
                if(orb.removed == true)
                {
                    hasRemove = true;
                    orb.ChangeImage();
                }
            }
        }
    }


    public bool OrbRemove(List<Orb> orbGroup)
    {
        bool isRemove = false;
        
        foreach (Orb orb in orbGroup)
        {
            if (orb.removed == true)
            {
                hasRemove = true;
                isRemove = true;
                orb.Fade();
            }
        }

        if (isRemove)
        {
            return true;
        }
        else
        {
            return false;
        }
    
    }


    private void SavePosition()
    {
        orbPos = new Vector3[30];
        for (int i = 0; i < transform.childCount; i++)
        {
            orbPos[i] = transform.GetChild(i).position;
        }
    }


    public void AllCheckFall()
    {
        for (int i = 29; i >= -29; i--)
        {
            foreach (Orb pz in orbs)
            {
                if (pz.id == i)
                {
                    pz.CheckFall();
                }

            }
        }

        isFade = false;

    }

}
