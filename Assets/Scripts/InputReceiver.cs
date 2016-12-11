using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class InputReceiver : MonoBehaviour {


	// TODO:
	// adjust score receiving to include other factors such as visual effects
	// add restart button
	// add new levels and words to database

	public int nr_levels;
	public int strikes;
	private List<List<string>> words;//cate o lista de cuvinte pentru fiecare nivel
	private Text display_text;
	private string input_text;
	private int p;
	private int level;
    private int wordsPerlevel;
    private bool active;
	private int current,previous;
	private int score;
	private int written_words;
	private bool game_over;
    private System.DateTime startTime;
    private int secondsLimit;

    #region Effects
    void Effect1()
    {
        EffectRotateRandomZ();
    }
    void Effect2()
    {
        EffectRotateRandomAll();
    }
    void Effect3()
    {
        EffectScaleRandomAll();
        EffectRotateRandomAll();
    }
    void Effect4()
    {
        EffectMirror();
    }
    void Effect5()
    {
        EffectMirrorAndRotateRandomXZ();
        EffectScaleRandomAll();
    }
    void ClearEffects()
    {
        var input = GameObject.Find("Input");
        input.transform.localScale = new Vector3(1, 1, 1);
        input.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    void EffectMirror()
    {
        var input = GameObject.Find("Input");
        input.transform.localEulerAngles = new Vector3(0, 180, 0);
    }
    void EffectMirrorAndRotateRandomXZ()
    {
        var angleX = Random.Range(-60, 60);
        var angleZ = Random.Range(0, 360);
        var input = GameObject.Find("Input");
        input.transform.localEulerAngles = new Vector3(angleX, 180, angleZ);
    }
    #region EffectRotate X/Y/Z/All
    void EffectRotateX(int x_angle = 1)
    {
        var input = GameObject.Find("Input");
        input.transform.Rotate(new Vector3(x_angle, 0, 0));
    }
    void EffectRotateY(int y_angle = 1)
    {
        var input = GameObject.Find("Input");
        input.transform.Rotate(new Vector3(0, y_angle, 0));
    }
    void EffectRotateZ(int y_angle = 1)
    {
        var input = GameObject.Find("Input");
        input.transform.Rotate(new Vector3(0, 0, y_angle));
    }
    void EffectRotateAll(int angle = 1)
    {
        EffectRotateX(angle);
        EffectRotateY(angle);
        EffectRotateZ(angle);
    }
    #endregion
    #region EffectRotateRandom X/Y/Z/All
    void EffectRotateRandomX()
    {
        var angle = Random.Range(0, 360);
        var input = GameObject.Find("Input");
        input.transform.localEulerAngles = new Vector3(angle, 0, 0);
    }
    void EffectRotateRandomY()
    {
        var angle = Random.Range(-60, 60);
        var input = GameObject.Find("Input");
        input.transform.localEulerAngles = new Vector3(0, angle, 0);
    }
    void EffectRotateRandomZ()
    {
        var angle = Random.Range(-60, 60);
        var input = GameObject.Find("Input");
        input.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    void EffectRotateRandomAll()
    {
        var angleX = Random.Range(-60, 60);
        var angleY = Random.Range(-60, 60);
        var angleZ = Random.Range(0, 360);
        var input = GameObject.Find("Input");
        input.transform.localEulerAngles = new Vector3(angleX, angleY, angleZ);
    }
    #endregion
    #region EffectScaleRandom X/Y/Z/All
    void EffectScaleRandomX()
    {
        var scale = Random.Range(0.2F, 2.0F);
        var input = GameObject.Find("Input");
        input.transform.localScale = new Vector3(scale, 1, 1);
    }
    void EffectScaleRandomY()
    {
        var scale = Random.Range(0.2F, 2.0F);
        var input = GameObject.Find("Input");
        input.transform.localScale = new Vector3(1, scale, 1);
    }
    void EffectScaleRandomZ()
    {
        var scale = Random.Range(0.2F, 2.0F);
        var input = GameObject.Find("Input");
        input.transform.localScale = new Vector3(1, 1, scale);
    }
    void EffectScaleRandomAll()
    {
        var scaleX = Random.Range(0.2F, 2.0F);
        var scaleY = Random.Range(0.2F, 2.0F);
        var scaleZ = Random.Range(0.2F, 2.0F);
        var input = GameObject.Find("Input");
        input.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
    #endregion
    #endregion


    void SetNewWord(){
		previous = current;
		while (current==previous)
			current = Random.Range (0, words[level-1].Count);

		input_text = words[level-1][current];
		display_text.text = input_text;
		display_text.color = Color.white;
		p = 0;//reset position in text

        ClearEffects();
        switch (level)
        {
            case 1:
                Effect1();
                break;
            case 2:
                Effect2();
                break;
            case 3:
                Effect3();
                break;
            case 4:
                Effect4();
                break;
            case 5:
                Effect5();
                break;
        }
        startTime = System.DateTime.Now;
    }

    void readFromFile(string file){
		for (int i = 0; i < nr_levels; i++)
			words.Add (new List<string> ());
		
		StreamReader sr = new StreamReader(file);
		string line;
		List<string> list;
		int l = 0;

		while(!sr.EndOfStream){
			line = sr.ReadLine( );
			list = new List<string>(line.Split(' '));
			foreach (string word in list) {
				words[l].Add (word);
			}
			l++;
		}

		sr.Close( );
	}

	// Use this for initialization
	void Start () {
        ClearEffects();
        wordsPerlevel = 7;
        secondsLimit = 5;
        strikes = 3;
        nr_levels = 5;
        display_text = GetComponent<Text> ();
		display_text.supportRichText=true;

		GameObject.Find ("Score").GetComponent<Text> ().text = "";

		words = new List<List<string>> ();
		active = true;
		game_over = false;
		level = 1;
		score = 0;
		written_words = 0;
		current = -1; previous=-1;
		readFromFile ("words-database");
		SetNewWord ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!active || game_over)
			return;
		string aux;
		int i;
		if (Input.anyKeyDown && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            if (Input.GetKeyDown(input_text[p].ToString()))
            {
                aux = input_text[p].ToString();
                i = 26 * p;
                display_text.text = display_text.text.Remove(i, 1);
                display_text.text = display_text.text.Insert(i, "?");
                display_text.text = display_text.text.Replace("?", "<color=#00ff00ff>" + aux + "</color>");


                p++;
                if (p == input_text.Length)
                {//when player successfully writes the word
                    written_words += 1;
                    score += level * input_text.Length;

                    if (written_words == level * wordsPerlevel)
                    {//when player writes enough words he advances to the next level
                        level++;
                        previous = -1;
                        current = -1;
                        if (level > nr_levels)
                        {
                            ClearEffects();
                            display_text.text = "YOU WON!";
                            display_text.color = Color.green;
                            display_text.fontSize = 100;
                            GameObject.Find("Score").GetComponent<Text>().text = "Final score: " + score;
                            GameObject.Find("Score").GetComponent<Text>().color = Color.green;
                            game_over = true;
                            return;
                        }
                    }
                    SetNewWord();
                }
            }
            else
            {
                StartCoroutine(ErrorWait());
            }
		}
        if ((System.DateTime.Now - startTime).TotalSeconds > secondsLimit)
        {
            StartCoroutine(ErrorWait());
        }
	}
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 70, 30), "Restart"))
           Start();
    }


    IEnumerator ErrorWait() {
		strikes -= 1;
		if (strikes == 0) {
			game_over = true;
            ClearEffects();
            display_text.text = "YOU LOST!";
			display_text.color = Color.red;
			display_text.fontSize = 100;
			GameObject.Find ("Score").GetComponent<Text> ().text = "Final score: "+score;
			GameObject.Find ("Score").GetComponent<Text> ().color = Color.red;

			print ("Done");
		}
		else {
			active = false;
			display_text.text = input_text;
			display_text.color = Color.red;
			yield return new WaitForSeconds (1);
			SetNewWord ();
			active = true;
		}
	}
}
