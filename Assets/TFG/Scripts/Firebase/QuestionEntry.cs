using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionEntry
{
    public string questionText, theme;
    public string answerText1, answerText2, answerText3, answerText4, answerText5;
    public bool answer1, answer2, answer3, answer4, answer5;

    public QuestionEntry()
    {
    }

    public QuestionEntry(string tText, string qtext, string answerText1, string answerText2, string answerText3, string answerText4, string answerText5, bool answer1, bool answer2, bool answer3, bool answer4, bool answer5)
    {
        this.theme = tText;
        this.questionText = qtext;
        this.answerText1 = answerText1;
        this.answer1 = answer1;
        this.answerText2 = answerText2;
        this.answer2 = answer2;
        this.answerText3 = answerText3;
        this.answer3 = answer3;
        this.answerText4 = answerText4;
        this.answer4 = answer4;
        this.answerText5 = answerText5;
        this.answer5 = answer5;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        Dictionary<string, object> respuestas = new Dictionary<string, object>();
        Dictionary<string, object> respuesta1 = new Dictionary<string, object>();
        Dictionary<string, object> respuesta2 = new Dictionary<string, object>();
        Dictionary<string, object> respuesta3 = new Dictionary<string, object>();
        Dictionary<string, object> respuesta4 = new Dictionary<string, object>();
        Dictionary<string, object> respuesta5 = new Dictionary<string, object>();

        result["tema"] = theme;
        result["texto"] = questionText;
        
        respuesta1["texto"] = answerText1;
        respuesta1["respuesta_correcta"] = answer1;
        respuesta2["texto"] = answerText2;
        respuesta2["respuesta_correcta"] = answer2;
        respuesta3["texto"] = answerText3;
        respuesta3["respuesta_correcta"] = answer3;
        respuesta4["texto"] = answerText4;
        respuesta4["respuesta_correcta"] = answer4;
        respuesta5["texto"] = answerText5;
        respuesta5["respuesta_correcta"] = answer5;

        respuestas["respuesta1"] = respuesta1;
        respuestas["respuesta2"] = respuesta2;
        respuestas["respuesta3"] = respuesta3;
        respuestas["respuesta4"] = respuesta4;
        respuestas["respuesta5"] = respuesta5;

        result["respuestas"] = respuestas;


        return result;
    }

}
