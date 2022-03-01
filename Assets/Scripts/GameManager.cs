using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Trail;
    public Rigidbody PlayerRb;
    public Rigidbody[] Balls = new Rigidbody[15];
    public Camera MainCamera;
    public float Force = 4;
    public float Direction = 270;
    public int GameState;
    public int Turn;
    public int Player1 = 0;
    public int Player2 = 0;
    public bool[] TakenBalls = new bool[15];
    public Text[] PRs = new Text[2];
    public Text ForceText;
    public Text Guide;
    public Scrollbar ForceScale;
    public AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        GameState = 0;
        Turn = 0;
        for (int i = 0; i < 15; ++i)
        {
            TakenBalls[i] = false;
        }
        ForceScale.size = Force / 20;
        AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Turn == 0)
        {
            PRs[0].fontSize = 35;
            PRs[1].fontSize = 20;
        }
        else
        {
            PRs[1].fontSize = 35;
            PRs[0].fontSize = 20;
        }

        if (GameState == 0)
        {
            // Adjusting the camera
            float angle = (float)((Direction + 30) * Math.PI / 180);
            Vector3 position;

            position.x = (float)(Player.transform.position.x - 1.8f * Math.Sin(angle));
            position.z = (float)(Player.transform.position.z - 1.8f * Math.Cos(angle));
            position.y = Player.transform.position.y + 0.6f;

            MainCamera.transform.position = position;
            MainCamera.transform.rotation = Quaternion.Euler(0, Direction, 0);

            // Adjusting the trail
            angle = (float)(Direction * Math.PI / 180);
            position.x = (float)(Player.transform.position.x + 0.2f * Math.Sin(angle));
            position.z = (float)(Player.transform.position.z + 0.2f * Math.Cos(angle));
            position.y = Player.transform.position.y - 0.06f;
            
            Trail.transform.position = position;
            Trail.transform.rotation = Quaternion.Euler(0, Direction, 0);

            // Getting user commands
            if (Input.GetKey("d"))
                ++Direction;
            if (Input.GetKey("a"))
                --Direction;
            if (Input.GetKey(KeyCode.RightArrow))
                Direction += 0.2f;
            if (Input.GetKey(KeyCode.LeftArrow))
                Direction -= 0.2f;
            if (Input.GetKey("s"))
                Force = Math.Min(20f, Force + 0.2f);
            if (Input.GetKey("w"))
                Force = Math.Max(1f, Force - 0.2f);
            if (Input.GetKey("l"))
            {
                PlayerRb.AddForce((float)(Force * Math.Sin(angle)), 0, (float)(Force * Math.Cos(angle)), ForceMode.VelocityChange);
                GameState = 1;

                Vector3 cameraPosition;
                cameraPosition.x = 0;
                cameraPosition.y = 2.9f;
                cameraPosition.z = -4;
                MainCamera.transform.position = cameraPosition;
                MainCamera.transform.rotation = Quaternion.Euler(30, 0, 0);
                Trail.SetActive(false);
                ForceText.gameObject.SetActive(false);
                ForceScale.gameObject.SetActive(false);
                Guide.gameObject.SetActive(false);
                Thread.Sleep(100);
            }

            ForceScale.size = Force / 20;
        }
        else // If GameState == 1
        {
            if (Math.Abs(PlayerRb.velocity.x) > 0.001
                || Math.Abs(PlayerRb.velocity.y) > 0.001
                || Math.Abs(PlayerRb.velocity.z) > 0.001)
                return;
            for (int i = 0; i < 15; ++i)
            {
                if (Math.Abs(Balls[i].velocity.x) > 0.001
                    || Math.Abs(Balls[i].velocity.y) > 0.001
                    || Math.Abs(Balls[i].velocity.z) > 0.001)
                    return;
            }
            for (int i = 0; i < 15; ++i)
            {
                if (TakenBalls[i] == false && Balls[i].position.y < 0.15)
                {
                    if (Turn == 0)
                        ++Player1;
                    else
                        ++Player2;
                    TakenBalls[i] = true;
                }
            }
            if (PlayerRb.position.y < 0.15)
            {
                Vector3 reset;
                reset.x = 2.85f;
                reset.y = 0.19f;
                reset.z = 0;
                Player.transform.position = reset;
            }
            PRs[0].text = "Player 1: " + Player1;
            PRs[1].text = "Player 2: " + Player2;
            GameState = 0;
            Turn = Turn == 0 ? 1 : 0;
            Trail.SetActive(true);
            ForceText.gameObject.SetActive(true);
            ForceScale.gameObject.SetActive(true);
            Guide.gameObject.SetActive(true);
        }
    }
}
