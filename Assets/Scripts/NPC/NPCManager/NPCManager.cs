using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject DialogUI;
    public Transform NpcSpawn; 
    public List<GameObject> NPCs = new();
    public int NPCCount = 0;
    public DialogUIManagerScript UIManager;
    public int QuestionNum = 2;
    public int CurrentQuestionNum = 0;
    public Animator animator;
    [SerializeField] 
    private GameObject _currentNPC;
    void Start()
    {
        UIManager = GameObject.FindGameObjectWithTag("HUD").GetComponent<DialogUIManagerScript>();
    }

    public void ReloadCount()
    {
        CurrentQuestionNum = 0;
    }
    public void NextNPC()
    {
        animator.Play("Bell_ringing", 0, 0.0f);
        if (CurrentQuestionNum == QuestionNum) return;
        if (!DialogUI.activeInHierarchy)  DialogUI.SetActive(true);
        DeleteNPC();
        var npc = Instantiate(NPCs[NPCCount], NpcSpawn.transform);
        _currentNPC = npc;
        
        UIManager.dialogManager =  npc.GetComponent<DialogManager>();
        UIManager.ShowCurrentDialog();
        //UIManager.NextDialogButton.interactable = false;
        
        NPCCount++;
        CurrentQuestionNum++;
        
    }

    public void DeleteNPC()
    {
        if (_currentNPC != null)
        {
            Destroy(_currentNPC);
        }
    }
}
