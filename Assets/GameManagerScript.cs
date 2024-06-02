using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
using System;
using Unity.VisualScripting ;
*/
public class GameManagerScript : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject clearText;
    public GameObject goalPrefab;
    public GameObject ParticlePrefab;
    //�z��̐錾
    int[,] map;
    GameObject[,] field;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        //�}�b�v�̐���
        map = new int[,]
   {
        {0,0,0,0,0 },
        {0,3,1,3,0 },
        {0,0,2,0,0 },
        {0,2,3,2,0 },
        {0,0,0,0,0 },
    };
        //�t�B�[���h�T�C�Y����
        field = new GameObject[
            map.GetLength(0),
            map.GetLength(1)
            ];
        string debugText = "";

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                debugText += map[y, x].ToString() + ",";
                if (map[y, x] == 1)
                {
                    field[y, x] = Instantiate(
                     playerPrefab, new Vector3(x, map.GetLength(0) - y, 0), Quaternion.identity);
                }
                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab, new Vector3(x, map.GetLength(0) - y, 0), Quaternion.identity);
                }
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                       goalPrefab, new Vector3(x, map.GetLength(0) - y, 0.01f), Quaternion.identity);
                }
            }
            debugText += "\n";
        }
        Debug.Log(debugText);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(1, 0));
            // MoveNumber(playerIndex, playerIndex + new Vector2Int(1,0));
            // PrintArray();
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(-1, 0));
            //MoveNumber(playerIndex,playerIndex - new Vector2Int(1,0));
            // PrintArray();
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, -1));
            // MoveNumber(playerIndex, playerIndex + new Vector2Int(0, 1));
            // PrintArray();
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2Int playerIndex = GetPlayerIndex();
            MoveNumber(playerIndex, playerIndex + new Vector2Int(0, 1));
            // MoveNumber(playerIndex, playerIndex - new Vector2Int(0, 1));
            // PrintArray();
            if (IsCleard())
            {
                clearText.SetActive(true);
            }
        }

        //�����N���A���Ă�����
        if (IsCleard())
        {
            Debug.Log("Clear");
        }

    }

    private Vector2Int GetPlayerIndex()
    {
        for (int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player")
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool MoveNumber(Vector2Int moveFrom, Vector2Int moveTo)
    {
        //�ړ��\�����f
        if (moveTo.y < 0 || moveTo.y >= field.GetLength(0)) { return false; }
        if (moveTo.x < 0 || moveTo.x >= field.GetLength(1)) { return false; }

        if (field[moveTo.y, moveTo.x] != null && field[moveTo.y, moveTo.x].tag == "Box")
        {
            Vector2Int velocity = moveTo - moveFrom;
            bool success = MoveNumber(moveTo, moveTo + velocity);
            if (!success) { return false; }
        }

        //�v���C���[�E���ς�炸�̈ړ�����
        field[moveTo.y, moveTo.x] = field[moveFrom.y, moveFrom.x];
        Vector3 moveToPosition = new Vector3(
          moveTo.x, map.GetLength(0) - moveTo.y, 0);
        field[moveFrom.y, moveFrom.x].GetComponent<Move>().MoveTo(moveToPosition);
        field[moveFrom.y, moveFrom.x] = null;

        //�p�[�e�B�N��

        const float num = 3;
        for (int i = 0; i < num; i++)
        {
            Particle.Instantiate(
                ParticlePrefab, new Vector3(moveFrom.x, map.GetLength(0) - moveFrom.y, 0),
                Quaternion.identity);
        }

        return true;
    }

    bool IsCleard()
    {
        //Vector2Int�^�̉ϒ��z��̍쐬
        List<Vector2Int> goals = new List<Vector2Int>();

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                //�i�[�ꏊ���ۂ��𔻒f
                if (map[y, x] == 3)
                {
                    //�i�[�ꏊ�̃C���f�b�N�X���T���Ă���
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }
        //�v�f����goals.Count�Ŏ擾
        for (int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                //1�ł������Ȃ�������������B��
                return false;
            }
        }
        //�������B���o�Ȃ���Ώ����B��
        return true;
    }
}