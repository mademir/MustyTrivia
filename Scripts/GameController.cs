using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    [Header("Custom")]
    public bool playersReady = false;
    public bool freezePlayers = false;
    public bool started = false;
    public bool loadingQuestion = false;
    public bool timeUp = false;
    public bool timerStarted = false;
    public bool notEnoughPlayers = false;
    public bool gameOver = false;
    public int Level;
    public int restartLives = 5;
    public Coroutine counter;
    private string qsS;
    private string[] qs;
    private string[] q = new string[5];
    public GameObject UI;
    public GameObject UIB;
    public GameObject PLAT;
    public GameObject[] doors = new GameObject[4];
    public GameObject menu;

    [ClientRpc] private void RpcGameOver()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        GameObject.Find("Base").SetActive(false);
        GameObject.Find("MSG").GetComponent<Text>().text = "GAME OVER";
        GameObject.Find("Platform").transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("CanvasBoard").transform.GetChild(4).gameObject.SetActive(true);    //Start text

        for (int i = 1; i <= 5; i++)
        {
            UI.transform.GetChild(i).GetComponent<Text>().text = "";
        }

        for (int i = 0; i < players.Length; i++)        //loop to check if all players are on a door
        {
            players[i].GetComponent<MultPlayerController>().lives = restartLives;
            players[i].GetComponent<Rigidbody>().position = new Vector3(Random.Range(-10f, 10f) * (Random.Range(-1f, 1f) > 0 ? -1 : 1), 16, Random.Range(-10f, 10f));
        }

        UIB.transform.GetChild(6).gameObject.SetActive(true);
        PLAT.transform.GetChild(7).gameObject.SetActive(true);
        for (int i = 0; i < 4; i++) UIB.transform.GetChild(i).gameObject.SetActive(false);
    }

    [ClientRpc] private void RpcStarting()
    {
        GameObject.Find("S").transform.GetChild(0).gameObject.SetActive(false);  //Deactivate start door
        GameObject.Find("Start").SetActive(false);  //Deactivate start door

        GameObject.Find("MSG").GetComponent<Text>().text = "";
        GameObject.Find("CanvasUI").transform.GetChild(0).gameObject.SetActive(true);

        UIB.transform.GetChild(6).gameObject.SetActive(false);
        PLAT.transform.GetChild(7).gameObject.SetActive(false);
        for (int i = 0; i < 4; i++) UIB.transform.GetChild(i).gameObject.SetActive(true);
    }

    [ClientRpc] private void RpcTimer(float i, float n)
    {
        //if (!started) GameObject.Find("MSG").GetComponent<Text>().text = "in " + -(i - n) + "..";
        //else GameObject.Find("Ct").GetComponent<Text>().text = (-(i - n)).ToString();
    }

    [ClientRpc] private void RpcQuestionTimeUp()
    {
        GameObject.Find("Ct").GetComponent<Text>().text = "";
        //var doors = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < doors.Length; i++)
        {    //for every door
            if (!doors[i].GetComponent<DoorTrigger>().correct)  //if the door is not the correct option
            {
                doors[i].transform.GetChild(0).gameObject.SetActive(false);  //Deactivate the door
            }
        }
    }

    [ClientRpc] private void RpcSetMsg(string str) => GameObject.Find("MSG").GetComponent<Text>().text = str;
    [ClientRpc] private void RpcSetCt(string str) => GameObject.Find("Ct").GetComponent<Text>().text = str;

    [ClientRpc] private void RpcReactivateDoors()
    {
        GameObject.Find("Platform").SetActiveRecursively(true);
        GameObject.Find("S").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Colours").gameObject.SetActive(false);
    }

    [ClientRpc] private void RpcResetCorrect()
    {
        //var doors = GameObject.FindGameObjectsWithTag("Door");


        for (int i = 0; i < doors.Length; i++)  //for every door
        {
            doors[i].GetComponent<DoorTrigger>().correct = false;
        }
    }

    [ClientRpc] private void RpcDisplayQuestion(string[] q)
    {
        //var doors = GameObject.FindGameObjectsWithTag("Door");
        UI.transform.GetChild(1).GetComponent<Text>().text = q[0];
        for (int i = 1; i < q.Length; i++)
        {
            if (q[i][2] != '!') q[i] = q[i].Substring(3);
            else
            {
                //correct option
                doors[i - 1].GetComponent<DoorTrigger>().correct = true;
                q[i] = q[i].Substring(4);
            }
            UI.transform.GetChild(i + 1).GetComponent<Text>().text = q[i];
        }

    }

    [ClientRpc] private void RpcSetFreeze(bool freeze) => freezePlayers = freeze;

    [Server]
    void Update()
    {
        if (!isServer) return;
        var players = GameObject.FindGameObjectsWithTag("Player");

        if (started)
        {
            if (!loadingQuestion)
            {
                if (menu.GetComponent<Menu>().restart)
                {
                    menu.GetComponent<Menu>().restart = false;
                    RpcGameOver();
                    Level = 0;
                    gameOver = false;
                    started = false;
                    playersReady = false;
                }

                playersReady = true;
                gameOver = true;
                for (int i = 0; i < players.Length; i++)        //loop to check if all players are on a door
                {
                    if (!(players[i].GetComponent<MultPlayerController>().isOnDoor || players[i].GetComponent<MultPlayerController>().lives <= 0))
                    {
                        playersReady = false;
                        //break;
                    }

                    if (players[i].GetComponent<MultPlayerController>().lives > 0) gameOver = false;
                }

                if (gameOver)
                {
                    RpcGameOver();
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //GameObject.Find("Base").SetActive(false);
                    //GameObject.Find("MSG").GetComponent<Text>().text = "GAME OVER";
                    //GameObject.Find("Platform").transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    //GameObject.Find("CanvasBoard").transform.GetChild(4).gameObject.SetActive(true);    //Start text

                    //for (int i = 1; i <= 5; i++)
                    //{
                    //    UI.transform.GetChild(i).GetComponent<Text>().text = "";
                    //}

                    //for (int i = 0; i < players.Length; i++)        //loop to check if all players are on a door
                    //{
                    //    players[i].GetComponent<MultPlayerController>().lives = restartLives;
                    //    players[i].GetComponent<Rigidbody>().position = new Vector3(Random.Range(-10f, 10f) * (Random.Range(-1f, 1f) > 0 ? -1 : 1), 16, Random.Range(-10f, 10f));
                    //}

                    //UIB.transform.GetChild(6).gameObject.SetActive(true);
                    //PLAT.transform.GetChild(7).gameObject.SetActive(true);
                    //for (int i = 0; i < 4; i++) UIB.transform.GetChild(i).gameObject.SetActive(false);
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    gameOver = false;
                    started = false;
                    playersReady = false;
                }

                if (playersReady)                               //if all players are on a door, deactivate false options and load next question
                {
                    if (!timerStarted)
                    {
                        counter = StartCoroutine(Timer(3));
                        timerStarted = true;
                        Debug.Log("TIMER STARTED");
                    }

                    if (timeUp)
                    {
                        timeUp = false;
                        timerStarted = false;

                        //Debug.Log("Everyone ready");
                        RpcSetFreeze(true);
                        //freezePlayers = true;

                        RpcQuestionTimeUp();
                        //GameObject.Find("Ct").GetComponent<Text>().text = "";
                        //var doors = GameObject.FindGameObjectsWithTag("Door");
                        //for (int i = 0; i < doors.Length; i++)
                        //{    //for every door
                        //    if (!doors[i].GetComponent<DoorTrigger>().correct)  //if the door is not the correct option
                        //    {
                        //        doors[i].transform.GetChild(0).gameObject.SetActive(false);  //Deactivate the door
                        //    }
                        //}

                        playersReady = false;

                        //Load next question
                        loadingQuestion = true;
                        StartCoroutine(LoadQuestion());
                    }
                }
                else if (timerStarted)
                {
                    timerStarted = false;
                    timeUp = false;
                    StopCoroutine(counter);
                    RpcSetCt("");
                    //GameObject.Find("Ct").GetComponent<Text>().text = "";
                    Debug.Log("TIMER STOPPED");
                }
            }
        }
        else //If not yet started
        {
            if (players.Length < 2)
            {
                notEnoughPlayers = true;
                RpcSetMsg("Not enough players. Invite someone to play together.");
                //GameObject.Find("MSG").GetComponent<Text>().text = "Not enough players. Invite someone to play together.";
                return;
            }
            else if (notEnoughPlayers)
            {
                RpcSetMsg("");
                //GameObject.Find("MSG").GetComponent<Text>().text = "";
                notEnoughPlayers = false;
            }

            playersReady = true;
            for (int i = 0; i < players.Length; i++)
            {

                if (!(players[i].GetComponent<MultPlayerController>().isReady))
                {
                    playersReady = false;
                    break;
                }
            }

            if (playersReady)
            {
                Debug.Log("Everyone ready");

                if (!timerStarted)
                {
                    counter = StartCoroutine(Timer(5));
                    timerStarted = true;
                    Debug.Log("TIMER STARTED");
                }


                if (timeUp) //Start
                {
                    timeUp = false;
                    timerStarted = false;

                    RpcStarting();
                    //GameObject.Find("S").transform.GetChild(0).gameObject.SetActive(false);  //Deactivate start door
                    //GameObject.Find("Start").SetActive(false);  //Deactivate start door

                    //GameObject.Find("MSG").GetComponent<Text>().text = "";
                    //GameObject.Find("CanvasUI").transform.GetChild(0).gameObject.SetActive(true);

                    //UIB.transform.GetChild(6).gameObject.SetActive(false);
                    //PLAT.transform.GetChild(7).gameObject.SetActive(false);
                    //for (int i = 0; i < 4; i++) UIB.transform.GetChild(i).gameObject.SetActive(true);
                    
                    StartCoroutine(LoadQuestion(true));
                    playersReady = false;
                    started = true;
                }
            }
            else if (timerStarted)
            {
                timerStarted = false;
                timeUp = false;
                StopCoroutine(counter);
                RpcSetMsg("");
                //GameObject.Find("MSG").GetComponent<Text>().text = "";
                Debug.Log("TIMER STOPPED");
            }
        }

    }

    IEnumerator LoadQuestion(bool initial = false)
    {
        if (!initial)
        {
            yield return new WaitForSeconds(3);
            RpcReactivateDoors();
            //GameObject.Find("Platform").SetActiveRecursively(true);
            //GameObject.Find("S").transform.GetChild(0).gameObject.SetActive(false);
            //GameObject.Find("Colours").gameObject.SetActive(false);
            playersReady = false;
        }
        
        RpcResetCorrect();
        //var doors = GameObject.FindGameObjectsWithTag("Door");


        //for (int i = 0; i < doors.Length; i++)  //for every door
        //{
        //    doors[i].GetComponent<DoorTrigger>().correct = false;
        //}

        //////////////////////////////////////////////////////////////////////
        q[0] = qs[Level * 10];
        q[1] = qs[Level * 10 + 2];
        q[2] = qs[Level * 10 + 4];
        q[3] = qs[Level * 10 + 6];
        q[4] = qs[Level * 10 + 8];

        q[0] = q[0].Substring(0, q[0].IndexOf("Trivia Question:")) + q[0].Substring(q[0].IndexOf(":")+2);

        RpcDisplayQuestion(q);
        //UI.transform.GetChild(1).GetComponent<Text>().text = q[0];
        //for (int i = 1; i < q.Length; i++)
        //{
        //    if (q[i][2] != '!') q[i] = q[i].Substring(3);
        //    else
        //    {
        //        //correct option
        //        doors[i-1].GetComponent<DoorTrigger>().correct = true;
        //        q[i] = q[i].Substring(4);
        //    }
        //    UI.transform.GetChild(i+1).GetComponent<Text>().text = q[i];
        //}

        //////////////////////////////////////////////////////////////////////
        Level++;
        if (!initial)
        {
            yield return new WaitForSeconds(3.5f);
            RpcSetFreeze(false);
            //freezePlayers = false;
            yield return new WaitForSeconds(5);
            loadingQuestion = false;
        }
    }

    IEnumerator Timer(float n)
    {
        for (float i = 0f; i < n; i++)
        {
            Debug.Log("Ready in " + -(i-n));
            RpcTimer(i, n);
            if (!started) RpcSetMsg("in " + -(i - n) + "..");
            else RpcSetCt((-(i - n)).ToString());
            yield return new WaitForSeconds(1);
        }

        timeUp = true;
    }



    void Start()
    {
        Level = 0;

        qsS = @"1. Trivia Question: What is the name for the Jewish New Year?

a) Hanukkah

b) Yom Kippur

c) Kwanza

d)! Rosh Hashanah

2. Trivia Question: How many blue stripes are there on the U.S. flag?

a) 6

b) 7

c) 13

d)! 0

3. Trivia Question: Which one of these characters is not friends with Harry Potter?

a) Ron Weasley

b) Neville Longbottom

c)! Draco Malfoy

d) Hermione Granger

4. Trivia Question: What is the color of Donald Duck’s bowtie?

a)! Red

b) Yellow

c) Blue

d) White

5. Trivia Question: What was the name of the band Lionel Richie was a part of?

a) King Harvest

b) Spectrums

c)! Commodores

d) The Marshall Tucker Band

6. Trivia Question: Which animal does not appear in the Chinese zodiac?

a) Dragon

b) Rabbit

c) Dog

d)! Hummingbird

7. Trivia Question: Which country held the 2016 Summer Olympics?

a) China

b)! Ireland

c) Brazil

d) Italy

8. Trivia Question: Which planet is the hottest?

a)! Venus

b) Saturn

c) Mercury

d) Mars

9. Trivia Question: Who was the only U.S. President to resign?

a) Herbert Hoover

b)! Richard Nixon

c) George W. Bush

d) Barack Obama

10. Trivia Question: What does the “D” in “D-Day” stand for?

a) Dooms

b) Dark

c)! Day 

d) Dunkirk

11. Trivia Question: In which city can you find the Liberty Bell?

a) Washington, D.C.

b) Boston

c)! Philadelphia

d) Manhattan 

12. Trivia Question: In Pirates of the Caribbean, what was Captain Jack Sparrow’s ship’s name?

a) The Marauder

b)! The Black Pearl

c) The Black Python

d) The Slytherin

13. Trivia Question: According to Forrest Gump, “life was like…”

a) A bag of lemons

b) A handful of roses

c) A lollipop

d)! A box of chocolates

14. Trivia Question: Linda and Bob from Bob’s Burgers have 3 kids. Which one of these characters is not one of them?

a) Louise

b) Gene

c)! Jimmy

d) Tina

15. Trivia Question: The British band One Direction (rip) was made up of Harry, Louis, Niall, Zayn, and…

a) Paul

b) Callum

c) Kevin

d)! Liam

16. Trivia Question: What is the rarest blood type?

a) O

b) A

c) B

d)! AB-Negative

17. Trivia Question: Holly Golightly is a character in which film?

a)! Breakfast at Tiffanys

b) Pretty In Pink

c) Funny Face

d) Singing In The Rain

18. Trivia Question: In The Wizard of Oz, the Tin Man wanted to see the Wizard about getting…

a) A brain

b) An oil can

c) A dog

d)! A heart

19. Trivia Question: Which U.S. state is known as the sunflower state?

a) Florida

b) California

c) Maine

d)! Kansas

20. Trivia Question: Which one of these characters isn’t a part of the Friends group?

a) Rachel

b) Joey

c)! Gunther

d) Monica

21. Trivia Question: How many bones are there in the human body?

a)! 206

b) 205

c) 201

d) 209

22. Trivia Question: Which famous singer released a song called “Adore You”?

a)! Harry Styles

b) Dua Lipa

c) Halsey

d) Shawn Mendes

23. Trivia Question: Fe is the chemical symbol for…

a) Zinc

b) Hydrogen

c) Fluorine

d)! Iron

24. Trivia Question: How old do you have to be to enter the hunger games?

a)! 12

b) 11

c) 10

d) 15

25. Trivia Question: What language is the most spoken worldwide?

a)! Chinese

b) Spanish

c) Arabic

d) English

26. Trivia Question: What year did Barbie come out?

a) 1958

b)! 1959

c) 1956

d) 1961

27. Trivia Question: What is Shakespeare’s shortest tragedy?

a)! Macbeth

b) Hamlet

c) Romeo & Juliet

d) Othello

28. Trivia Question: What is the #1 cookie in the U.S.?

a) Chips Ahoy!

b) Milano

c) Girl Scout Thin Mints

d)! Oreo

29. Trivia Question: How many hearts does an octopus have?

a) 1

b) 2

c)! 3

d) 4

30. Trivia Question: Who wrote The Scarlett Letter?

a) Shakespeare

b) Stephen King

c)! Nathanial Hawthorne

d) Ernest Hemingway

31. Trivia Question: Which social media platform came out in 2003?

a)! Myspace

b) Twitter

c) Facebook

d) Tumblr

32. Trivia Question: Which planet in our solar system is the largest?

a)! Jupiter

b) Saturn

c) Neptune

d) Earth

33. Trivia Question: The Powerpuff Girls are 3 distinct colors. What are they?

a) Red, yellow, green

b) Yellow, blue, green

c)! Blue, green, red

d) Green, purple, orange

34. Trivia Question: Which boyband sings the song “I Want It That Way”?

a) One Direction

b)! Backstreet Boys

c) NSYNC

d) New Kids On The Block

35. Trivia Question: Who painted the Sistine Chapel ceiling?

a) Picasso

b) Da Vinci

c)! Michelangelo

d) Van Gogh

36. Trivia Question: In which state did the Salem Witch Trials take place?

a) Washington

b) Virginia

c)! Massachusetts

d) Pennsylvania

37. Trivia Question: Which ocean is the largest?

a) Indian

b)! Pacific

c) Atlantic

d) Arctic

38. Trivia Question: Which New York City building is the tallest:

a) Empire State Building

b) Bank of America Tower

c)! One World Trade Center

d) Statue of Liberty

39. Trivia Question: What does the “E” in Chuck E. Cheese stand for?

a) Ernest

b) Edward

c)! Entertainment

d) Extra

40. Trivia Question: Which country gifted the Statue of Liberty to the U.S.?

a) Germany

b) China

c)! France

d) Italy

41. Trivia Question: Who painted the Mona Lisa?

a) Van Gogh

b)! da Vinci

c) Picasso

d) Monet

42. Trivia Question: In which city were Anne Frank and her family in hiding?

a) Paris

b)! Amsterdam

c) Brussels

d) Frankfurt

43. Trivia Question: There are 4 best friends on the TV show Pretty Little Liars: Hanna, Emily, Aria, and…

a) Mona

b) Charlie

c) Alison

d)! Spencer

44. Trivia Question: In which decade does the Netflix series Stranger Things take place?

a) the ‘70s

b)! the ‘80s

c) the ‘90s

d) the early 2000s

45. Trivia Question: Which country consumes the most chocolate?

a) Spain

b) Germany

c) North America

d)! Switzerland

46. Trivia Question: What is Sodium Chloride?

a)! Salt

b) Sugar

c) Chlorine

d) Bleach

47. Trivia Question: Which astrological sign is a crab?

a) Pisces

b) Aquarius

c)! Cancer

d) Virgo

48. Trivia Question: In the Bible, how does the Virgin Mary learn of her pregnancy with baby Jesus?

a) God tells her

b)! the angel Gabriel tells her

c) she has a dream about it

d) a doctor tells her

49. Trivia Question: Who, in the Harry Potter series, is Tom Riddle?

a) a student in Harry’s class

b) a professor at Hogwarts

c) Harry’s birth father

d)! Voldemort

50. Trivia Question: The movie The Social Network is about which social media platform:

a)! Facebook

b) Myspace

c) Instagram

d) Twitter

51. Trivia Question: Who wrote the songs for The Lion King? 

a)! Elton John

b) Phil Collins

c) Celine Dion

d) Stevie Wonder

52. Trivia Question: How many daughters does Barack Obama have?

a) 0

b) 1

c)! 2

d) 3

53. Trivia Question: Which Disney princess sings “Just Around The Riverbend”?

a) Snow White

b)! Pocahontas

c) Elsa

d) Belle

54. Trivia Question: Which biblical narrative is connected to Palm Sunday?

a)! Jesus’ entry into Jerusalem

b) Jesus’ resurrection

c) Jesus feeding the thousands

d) Nothing, it just means to go to church on Sunday/the day of rest

55. Trivia Question: In Harry Potter and the Sorcerer’s Stone, who gives Harry the invisibility cloak?

a) Ron

b) Snape

c)! Dumbledore

d) No one; he just finds it

56. Trivia Question: How many boroughs are there in New York City?

a) 4

b)! 5

c) 6

d) 10

57. Trivia Question: In the U.S. version of The Office, Michael Scott burns his foot on:

a) hot water

b) pavement/cement

c) rocks on fire

d)! a George Foreman Grill

58. Trivia Question: The superstition believes that when the groundhog sees his shadow, it means:

a) early spring

b)! 6 more weeks of winter

c) it’s going to rain

d) a tornado is coming

59. Trivia Question: What is the longest river in the world?

a) Amazon

b) Congo

c)! Nile

d) Hudson

60. Trivia Question: How many days are in February during a leap year?

a) 28

b)! 29

c) 30

d) 31

61. Trivia Question: How many degrees are in a circle?

a)! 360

b) 180

c) 150

d) 359

62. Trivia Question: Which city is known as the City of Love?

a) Rome

b) Barcelona

c) New York City

d)! Paris

63. Trivia Question: As an adult, how many teeth should you have in your mouth?

a) 35

b)! 32

c) 30

d) 42

64. Trivia Question: What was the name of the boy who won Willy Wonka’s factory?

a) Charlie Baxter

b) Charlie Brown

c) Charlie Bones

d)! Charlie Bucket

65. Trivia Question: Edward Scissorhands is known for cutting:

a) hair

b) bushes

c) clothes

d)! everything

66. Trivia Question: In which city would you find the Fisherman’s Bastion?

a) Rome

b)! Budapest

c) Barcelona

d) Athens

67. Trivia Question: Which U.S. president doesn’t/didn’t have a dog in the White House?

a)! Trump

b) Obama

c) Bush

d) Lincoln

68. Trivia Question: What does the “U” stand for in “UFO”?

a)! Unidentified

b) Under

c) United

d) Unique

69. Trivia Question: Which U.S. state is known as “America’s Dairyland”?

a) Minnesota

b) Iowa

c) Pennsylvania

d)! Wisconsin

70. Trivia Question: Usher found a young boy singing on YouTube and made him into a famous singer. What’s that kid’s name?

a) Niall Horan

b) Jaden Smith

c) Shawn Mendes

d)! Justin Bieber

71. Trivia Question: Which Olympic sport is Michael Phelps known for?

a) Snowboarding

b) Skiing

c) Running

d)! Swimming

72. Trivia Question: What is the complementary color of green?

a) blue

b) yellow

c)! red

d) purple

73. Trivia Question: Han Solo is a character from which movie series:

a) Harry Potter

b)! Star Wars

c) Lord of the Rings

d) Indiana Jones

74. Trivia Question: In Men and Black, what are the two FBI agents hunting?

a) serial killers

b) ghosts

c)! aliens

d) time travelers

75. Trivia Question: The most recent seasons of American Idol have the judges Katy Perry, Lionel Richie, and…

a) Trace Adkins

b)! Luke Bryan

c) Blake Shelton

d) Keith Urban

76. Trivia Question: How many Harry Potter books are there?

a)! 7

b) 8

c) 6

d) 10

77. Trivia Question: What breed of dog is the most popular in the U.S.?

a) Pug

b) Dalmatian

c) Beagle

d)! Golden Retriever

78. Trivia Question: Which rapper was known for his album Blue Slide Park?

a) J Cole

b) Post Malone

c) Eminem

d)! Mac Miller

79. Trivia Question: How many sides does a hexagon have?

a) 5

b)! 6

c) 7

d) 8

80. Trivia Question: In which city was Ferris Bueller’s Day Off filmed?

a) Pittsburgh

b)! Chicago

c) NYC

d) San Francisco

81. Trivia Question: The UK is made up of the following countries: England, Ireland, Wales, and…

a) France

b) Hungary

c)! Scotland

d) Austria

82. Trivia Question: How many elements are there on the periodic table?

a) 112

b)! 118

c) 120

d) 143

83. Trivia Question: Where is the United Nations Headquarters?

a) D.C.

b)! NYC

c) Philadelphia

d) Orlando

84. Trivia Question: What famous singer sings with Taylor Swift in her song “Me!”?

a)! Brendan Urie

b) Shawn Mendes

c) Ellie Goulding

d) Halsey

85. Trivia Question: In what year did women get the right to vote?

a) 1910

b)! 1920

c) 1930

d) 1940

86. Trivia Question: Where in the United States is the largest aquarium?

a) New Jersey

b) Maine

c) California

d)! Georgia

87. Trivia Question: There are 5 great lakes in the United States: Lake Michigan, Lake Superior, Lake Ontario, Lake Erie, and…

a)! Lake Huron

b) Lake Hartwell

c) Lake Tahoe

d) Great Bear Lake

88. Trivia Question: Neil Armstrong was the first man…

a) on Mars

b) on a spacecraft alone

c)! on the Moon

d) to travel to the sun

89. Trivia Question: What does “FBI” stand for?

a) Female Body Inspector

b)! Federal Bureau of Investigation

c) Federal Business of Investigation

d) Federal Bureau of Inspection

90. Trivia Question: What is the deadliest snake?

a) Python

b) Cobra

c) Cobra

d)! Black Mamba

91. Trivia Question: In The Office, what object of Dwight’s does Jim put in jello?

a) computer mouse

b) wallet

c)! stapler

d) coffee mug

92. Trivia Question: In Friends, how many times has Ross been married?

a) only once

b)! 3 times

c) twice

d) more than 3 times

93. Trivia Question: What is a group of lions called?

a) Squad

b) Pack

c) Herd

d)! Pride

94. Trivia Question: How many keys are on a piano?

a) 86

b) 87

c)! 88

d) 89

95. Trivia Question: What was the name of the Greek mythological woman who had snakes for hair?

a) Pandora

b) Helen

c) Cassiopeia

d)! Medusa

96. Trivia Question: What do you call a baby goat?

a)! Kid

b) Goatee

c) Child

d) Baby Goat

97. Trivia Question: According to Phineas and Ferb, there are __ days of summer vacation?

a) 90

b) 103

c)! 104

d) 110

98. Trivia Question: What is the most populous city in Canada?

a)! Toronto

b) Ontario

c) Quebec

d) Vancouver

99. Trivia Question: The Da Vinci Code opens with a murder in which museum:

a) The Guggenheim

b)! The Louvre

c) The Van Gogh museum

d) The Metropolitan Museum of Art

100. Trivia Question: From which TV show is the family of Roses: Johnny, Moira, David, and Alexis?

a) Bob’s Burgers

b)! Schitt’s Creek

c) Parenthood

d) 7th Heaven 

101. Trivia Question: After The Simpsons, what is the longest-running TV show?

a)! Law & Order

b) Grey’s Anatomy

c) Criminal Minds

d) NCIS

1o2. Trivia Question: Which Disney princess sings “A Dream Is A Wish Your Heart Makes”?

a) Belle

b)! Cinderella

c) Jasmine

d) Sleeping Beauty

103. Trivia Question: How often does the moon orbit the Earth?

a) every 7 days

b)! every 27 days

c) every 30 days

d) every 365 days

104. Trivia Question: In Greek Mythology, who is the Queen of the Underworld?

a) Pandora

b) Medusa

c) Helen

d)! Persephone

105. Trivia Question: How many points are a touchdown worth?

a) 5

b)! 6

c) 7

d) 8

106. Trivia Question: How many feet are in a mile?

a) 1,037

b) 5,288

c)! 5,280

d) 6,201

107.Trivia Question: Where is the Oval Office located in the White House?

a) North Wing

b) South Wing

c) East Wing

d)! West Wing

108. Trivia Question: At what temperature (Fahrenheit) does water freeze?

a)! 32 degrees

b) 40 degrees

c) -10 degrees

d) 0 degrees

109. Trivia Question: Where in Pennsylvania is The Office based?

a) Philadelphia

b) Pittsburgh

c)! Scranton

d) Lancaster

110. Trivia Question: In the movie Good Will Hunting, which college does Skylar attend?

a)! Harvard

b) Yale

c) Columbia

d) UCLA

111. Trivia Question: Where in California is Disneyland located?

a) Malibu

b) Huntington Beach

d) Los Angeles

d)! Anaheim

112. Trivia Question: “I see dead people” is a line from which horror film…

a)! The Sixth Sense

b) The Grudge

c) The Shining

d) The Exorcist 

113. Trivia Question: Who founded Microsoft?

a) Bill Hader

b) Steve Jobs

c)! Bill Gates

d) Mark Zuckerberg

114. Trivia Question: In which city was the movie National Treasure filmed?

a) Washington D.C.

b) NYC

c)! Philadelphia

d) Roanoke

115. Trivia Question: Which classic novel has the line “Stay Gold, Ponyboy”?

a) The Catcher in the Rye

b) 1984

c)! The Outsiders

d) Catch-22

116. Trivia Question: What was the name of Harry Potter’s pet owl?

a)! Hedwig

b) Luna

c) Dobby

d) Fluffy

117. Trivia Question: Which Disney princess had 3 fairy godmothers?

a) Cinderella

b) Snow White

c)! Sleeping Beauty

d) Jasmine

118. Trivia Question: Which band came back together in 2019?

a) The Naked Brothers Band

b)! The Jonas Brothers

c) One Direction

d) The Beatles

119. Trivia Question: Steve Jobs is known for wearing a black…

a) button-down shirt

b) t-shirt

c)! turtleneck

d) blazer

120. Trivia Question: In which movie does Anne Hathaway play a poor, homeless woman?

a) The Devil Wears Prada

b)! Les Miserables

c) The Princess Diaries

d) Ella Enchanted 

121. Trivia Question: The movie 10 Things I Hate About You was based on which play by Shakespeare:

a)! Taming of the Shrew

b) Hamlet

c) Romeo and Juliet

d) A Midsummer Night’s Dream

122. Trivia Question: Where does Nathan’s Hot Dog Eating Contest take place?

a)! Coney Island

b) Miami Beach

c) Mall of America

d) Orlando

123. Trivia Question: What age did Amy Winehouse, Janis Joplin, and Jimi Hendrix die?

a) 26

b)! 27

c) 29

d) 30

124. Trivia Question: Which two planets in our solar system are known as “ice giants”?

a) Neptune and Jupiter

b) Uranus and Pluto

c) Pluto and Jupiter

d)! Neptune and Uranus

125. Trivia Question: What country is Prague in?

a) Hungary

b) Austria

c)! Czech Republic

d) Germany

126. Trivia Question: What is the actress’s name in Funny Face, Sabrina, and Roman Holiday?

a)! Audrey Hepburn

b) Natalie Wood

c) Marilyn Monroe

d) Grace Kelly

127. Trivia Question: Which poet wrote the poem “The Raven”? 

a) Robert Frost

b)! Edgar Allen Poe

c) Walt Whitman

d) Sylvia Plath

128. Trivia Question: How many ribs are in the human body?

a) 16

b)! 24

c) 19

d) 29

129. Trivia Question: Who was the 16th president of the United States?

a)! Lincoln

b) Nixon

c) Jackson

d) Madison

130. Trivia Question: Who wrote the novel Slaughterhouse-Five?

a)! Kurt Vonnegut

b) Stephen King

c) J.D. Salinger

d) Harper Lee

131. Trivia Question: In The Office, what was the food that Dwight grew on his farm?

a) pumpkins

b)! beets

c) onions

d) potatoes

132. Trivia Question: What animal is associated with ancient Egypt?

a) lion

b)! cat

c) hummingbird

d) rabbit

133. Trivia Question: In 2016, a musician won the Nobel Peace Prize for Literature. Who was it?

a) Lenny Kravitz

b) Eric Clapton

c)! Bob Dylan

d) Elton John

134. Trivia Question: How many time zones are there in the world?

a) 7

b)! 24

c) 23

d) 9

135. Trivia Question: What was the movie’s name that featured Matthew McConaughey, Michael Caine, Anne Hathaway, John Lithgow, and Matt Damon?

a) Flight Plan

b) The Martian 

c)! Interstellar

d) Ad Astra

136. Trivia Question: How many rings are there in the Olympic symbol?

a)! 5

b) 7

c) 4

d) 9

137. Trivia Question: Twilight was both a book and a movie, with the main character Bella Swan being pulled into two different love directions with Edward Cullen and…

a) Jasper Hale

b)! Jacob Black

c) Billy Black

d) Dr. Cullen

138. Trivia Question: What is celebrated on December 26th?

a) the day after Christmas

b) Harvest Day

c)! Boxing Day

d) National Dog Day

139. Trivia Question: What is the name of the second American astronaut to step foot on the moon?

a)! Buzz Aldrin

b) Neil Armstrong

c) Alan Bean

d) James Irwin

140. Trivia Question: How many eyes does a spider have?

a)! 8

b) 9

c) 10

d) 2

141. Trivia Question: What is the first book of the Old Testament in the Bible?

a) Matthew

b) Proverbs

c)! Genesis

d) Exodus

142. Trivia Question: Which founding father is known for his large handwriting on the Declaration of Independence?

a)! John Hancock

b) Thomas Jefferson

c) John Adams

d) Alexander Hamilton

143. Trivia Question: What was the first Disney film that was produced in color?

a) Cinderella

b)! Snow White and the Seven Dwarfs

c) Sleeping Beauty

d) Pocahontas

144. Trivia Question: Sodium bicarbonate is used in the kitchen as what?

a) salt

b) sugar

c)! baking soda

d) vinegar

145. Trivia Question: In the 1983 movie National Lampoon’s Vacation, what is the name of the fictional amusement park the Griswold family is trying to go to?

a) Dorney Park

b)! Walley World

c) Six Flags

d) Dollywood

146. Trivia Question: In Ray Bradbury’s novel Fahrenheit 451, what are they burning?

a) clothes

b) houses

c)! books

d) money

147. Trivia Question: What was the first capital of the United States?

a) Washington, D.C.

b) Richmond

c) Boston

d)! Philadelphia

148. Trivia Question: Which actor performs music under the stage name Childish Gambino?

a)! Donald Glover

b) Will Smith

c) Frank Ocean

d) Tyler, The Creator

149. Trivia Question: Which water sport is the official sport for the state of Hawaii?

a) water polo

b) swimming

c) water skiing

d)! surfing

150. Trivia Question: In the movie The Princess Bride, what is Westley’s response to Buttercup’s requests?

a) “Okay.”

b) “Of course, I love you.”

c)! “As you wish.”

d) “Anything for you.”

151. Trivia Question: What is the name of the company that published the Mario Kart video game?

a)! Nintendo

b) Electronic Arts (EA)

c) SEGA

d) Xbox
";
        qs = qsS.Split('\n');
    }
}
