using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Dialogue/Node")]
public class DialogObject : ScriptableObject
{
    [TextArea]
    public string text; // текст который рассказывает нпс

    public List<Choice> choices; // список вопросов игрока
    
    public bool isActive =  true; // для деиалогов которые надо открывать по условию
    public bool wasSelected = false; // был ли вариант диалога выбран ранее (мб для отображения в ui иначе)
}



[System.Serializable]
public class Choice
{
    public string text; // вопрос игрока
    public DialogObject nextNode; // следующая ветка диалога
    public int order = 0;
    public bool isLast = false;
}