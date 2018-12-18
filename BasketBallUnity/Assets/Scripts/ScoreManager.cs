using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// this script is attached to the basketball hoop and detects the balls
public class ScoreManager : MonoBehaviour {

    private int _score;
    public Text scoretext;
    public GameObject effectObject;
    private ParticleSystem ps;

    // Use this for initialization
    void Start () {
    
        _score = 0;
        ps = effectObject.GetComponent<ParticleSystem>();

	}

    private void OnTriggerEnter(Collider col)
    {

        _score++;
        scoretext.text = "Score : " + _score;

        // every time the user mark, the animation is play
        ps.Play();

    }

}
