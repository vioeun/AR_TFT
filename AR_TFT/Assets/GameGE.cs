﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGE : MonoBehaviour
{
    public GameObject BlueBoard;
    public GameObject RedBoard;

    // 인식되면 3초후 게임 시작
    public GameObject GameStartCard;

    public bool isGamePlaying;

    //상대팀 보드와 내 보드 탐색
    public GameObject EnemyBoard;
    public GameObject MyBoard;
    
    //최소거리 탐색
    public GameObject [] FoundObjects;
    public GameObject enemy;
    public string TagName;
    public float shortDis;

    public static float[] ChampionAttachTimer = new float[4];

    public static GameObject[] ChampionCards = new GameObject [4];
    public int Round; //게임 라운드

    

    // Start is called before the first frame update
    void Start()
    {
        isGamePlaying = false;
        Round = 0; // 게임 라운드
        for (int i = 0; i < 4; i++)
            ChampionCards[i] = GameObject.Find("ChampionCard" + (i + 1).ToString());
    }

    // Update is called once per frame
    void Update()
    {

        for(int i=0; i< 4; i++)
            ChampionAttachTimer[i] = ChampionCards[i].GetComponent<ChampionCard>().attachTimer;

        if (BlueBoard.GetComponent<Board>().PlayerHP <= 0)
        {
            //블루팀이 짐
            //GameOver(BlueBoard, RedBoard); //이긴팀, 진팀 다른 화면 출력
        }

        /*else if (RedBoard.GetComponent<Board>().PlayerHP <= 0)
        {
            //레드팀이 짐
        }*/

        // 보드에 챔피언들을 찾음
        else if (BlueBoard.GetComponent<Board>().EqupChampionCount > 0 && BlueBoard.GetComponent<Board>().EqupChampionCount > 0) // && RedBoard.GetComponent<Board>().EqupChampionCount == 3 
        {
            if (!isGamePlaying)
            {
                if (!isGamePlaying && GameStartCard.GetComponent<MyDefaultTrackableEventHandler>().isAttach) // 게임 시작 카드 확인
                {
                    isGamePlaying = true;
                    Round++; //라운드
                    Debug.Log("startCard Check!");
                }
            }
            else if (isGamePlaying) // 추후 보드판에 붙은 캐릭터가 6개 다 인식되면 if문 추가
            {
                for (int i = 0; i < 3; i++)
                {
                    // 살아있는 챔피언 출력
                    if (BlueBoard.GetComponent<Board>().MyChampion[i].name != "EmptyGameObject")
                    {
                        Debug.Log("BLUE Team" + (i+1).ToString() + BlueBoard.GetComponent<Board>().MyChampion[i].name + "    HP:" + BlueBoard.GetComponent<Board>().MyChampion[i].GetComponent<ChampionIdentity>().ChampHP.ToString());
                        if (BlueBoard.GetComponent<Board>().MyChampion[i].GetComponent<ChampionIdentity>().ChampHP > 0) // 살아 있으면
                        {
                            //Fight
                            StartCoroutine(LookAroundEnemyChamp(BlueBoard.GetComponent<Board>().MyChampion, i));
                            

                            
                        }
                    }
                    if (RedBoard.GetComponent<Board>().MyChampion[i].name != "EmptyGameObject")
                    {
                        Debug.Log("RED Team" + (i+1).ToString() + RedBoard.GetComponent<Board>().MyChampion[i].name + "    HP:" + RedBoard.GetComponent<Board>().MyChampion[i].GetComponent<ChampionIdentity>().ChampHP.ToString());
                        if (RedBoard.GetComponent<Board>().MyChampion[i].GetComponent<ChampionIdentity>().ChampHP > 0) // 살아 있으면
                        {
                            //Fight
                            LookAroundEnemyChamp(RedBoard.GetComponent<Board>().MyChampion, i);
                        }
                    }
                }
                /*if(BlueBoard.GetComponent<Board>().MyChampion[0].GetComponent<ChampionIdentity>().ChampHP <= 0 && BlueBoard.GetComponent<Board>().MyChampion[1].GetComponent<ChampionIdentity>().ChampHP <= 0 && BlueBoard.GetComponent<Board>().MyChampion[2].GetComponent<ChampionIdentity>().ChampHP <= 0)
                {  
                    //블루 팀 라운드 패 // 남은 챔피언 수 만큼 체력 내리기
                    BlueBoard.GetComponent<Board>().PlayerHP -= Round * 7 ; // 라운드 *3 만큼
                    Debug.Log("GameOver!");
                    isGamePlaying = false;
                }
                else if (RedBoard.GetComponent<Board>().MyChampion[0].GetComponent<ChampionIdentity>().ChampHP <= 0 && RedBoard.GetComponent<Board>().MyChampion[1].GetComponent<ChampionIdentity>().ChampHP <= 0 && RedBoard.GetComponent<Board>().MyChampion[2].GetComponent<ChampionIdentity>().ChampHP <= 0)
                {
                    //레드 팀 라운드 패
                    RedBoard.GetComponent<Board>().PlayerHP -= Round*3; // 라운드 *3 만큼 체력 감소
                    isGamePlaying = false;
                }*/
            }
        }
        // BlueBoard.GetComponent<Board>().MyChampion =  
        // 이곳에서 챔피언카드를 확인하고 해당 챔피언 카드의 아이템까지 확인 

        // 각 보드에 챔피언 카드가 1장이상 붙어 있을 경우

    }

    // 가까운 적 찾기
    IEnumerator LookAroundEnemyChamp(GameObject[] OurCard, int index) // 테스트 아직 안함
    {
        if (OurCard[index].transform.parent.name == "BlueBoard")
        {
            EnemyBoard = RedBoard;
            MyBoard = BlueBoard;

        }
        if (OurCard[index].transform.parent.name == "RedBoard")
        {
            EnemyBoard = BlueBoard;
            MyBoard = RedBoard;
        }

        if (MyBoard.GetComponent<Board>().MyChampion[index].GetComponent<ChampionIdentity>().ChampHP <= 0)
        {

            OurCard[index].transform.GetChild(5).GetChild(0).GetComponent<Animator>().SetBool("Attack", false);
            OurCard[index].transform.GetChild(5).GetChild(0).GetComponent<Animator>().SetBool("Death", true);
        }
        else if (MyBoard.GetComponent<Board>().MyChampion[index].GetComponent<ChampionIdentity>().ChampHP > 0)
        {
            FoundObjects = EnemyBoard.GetComponent<Board>().MyChampion;

            // 첫번째를 기준으로 잡아주기 
            shortDis = Vector3.Distance(gameObject.transform.position, FoundObjects[0].transform.position);  // 거리
            enemy = FoundObjects[0];    // 적 GameObject

            foreach (GameObject found in FoundObjects)
            {
                // 상대가 체력이 있는지 확인
                if (found.name != "EmptyGameObject" && found.GetComponent<ChampionIdentity>().ChampHP > 0)
                {
                    float Distance = Vector3.Distance(OurCard[index].transform.position, found.transform.position);
                    if (Distance < shortDis)
                    {
                        enemy = found;
                        shortDis = Distance;
                    }
                }
            }
            if (enemy.name != "EmptyGameObject" && enemy.GetComponent<ChampionIdentity>().ChampHP > 0)
            {
                Quaternion lookOnLook = Quaternion.LookRotation(enemy.transform.GetChild(5).GetChild(0).position - OurCard[index].transform.GetChild(5).GetChild(0).position);
                OurCard[index].transform.GetChild(5).GetChild(0).rotation = Quaternion.Slerp(OurCard[index].transform.GetChild(5).GetChild(0).rotation, lookOnLook, Time.deltaTime);
                OurCard[index].transform.GetChild(5).GetChild(0).GetComponent<Animator>().SetBool("Attack", true);
                Debug.Log(enemy);
            }
            else
            {
                Debug.Log(MyBoard+"Win");
            }
            // 평타
            //Debug.Log(shortDis);
        }

        yield return new WaitForSeconds(2.0f);
    }
}
