using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class NewBehaviourScript : MonoBehaviour
{
    KeywordRecognizer  KeywordRecognizer;

    Dictionary<string, Action> WordToAction;
        void Start()
    {
        WordToAction = new Dictionary<string, Action>();
        WordToAction.Add("azul", Azul);
        WordToAction.Add("rojo", Rojo);
        WordToAction.Add("mover", Mover);

        KeywordRecognizer = new KeywordRecognizer(WordToAction.Keys.ToArray());
        KeywordRecognizer.OnPhraseRecognized += WordRecognized;
        KeywordRecognizer.Start();
    }

    private void WordRecognized(PhraseRecognizedEventArgs word)
    {
        Debug.Log(word.text);
        WordToAction[word.text].Invoke();
        
    }

    private void Mover()
    {
        transform.Translate(1, 0, 0);
    }

    private void Rojo()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    private void Azul()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
