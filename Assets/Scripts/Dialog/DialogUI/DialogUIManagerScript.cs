using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUIManagerScript : MonoBehaviour
{
    public TMP_Text choiceText;
    public Transform choiceParent;
    public GameObject buttonPrefab;
    public DialogManager dialogManager;
    public DialogObject lockedNode;
    private List<GameObject> _buttons = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateChoices();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreviousDialog()
    {
        dialogManager.PreviousDialog();
        UpdateChoices();
    }
    private void NextDialog(int id)
    {
        dialogManager.NextDialog(id);
        UpdateChoices();
        choiceText.text = dialogManager.currentDialog.text;
    }
    
    

    public void UpdateChoices()
    {
        if (_buttons.Count > 0)
        {
            foreach (GameObject button in _buttons)
            {
                Destroy(button);
            }
        }
        int count = 0;
        float currPos = 238;
        foreach (Choice choice in dialogManager.choices)
        {
            currPos -= 128;
            GameObject button = Instantiate(buttonPrefab, choiceParent);
            button.GetComponent<Button>().onClick.AddListener(() => NextDialog(choice.order));
            button.GetComponentInChildren<TMP_Text>().text = choice.text;
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,currPos);
            button.GetComponent<Button>().enabled = choice.nextNode.isActive;
            if (!choice.nextNode.isActive) lockedNode = choice.nextNode;
            _buttons.Add(button);
            count++;
        }
    }
}
