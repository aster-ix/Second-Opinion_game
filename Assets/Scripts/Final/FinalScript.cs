using UnityEngine;

public class FinalScript : MonoBehaviour
{
    public GameObject finalchoice;

    public GameObject Hud;

    public NPCManager  npcManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEnd()
    {
        finalchoice.SetActive(true);
        Hud.SetActive(false);
    }

    public void HusbandChoice()
    {
        LoadEnding1();
    }

    public void FanChoice()
    {
        finalchoice.SetActive(false);
        Hud.SetActive(true);
        npcManager.NextNPC();
    }

    public void LoadEnding1()
    {
        
    }
    public void LoadEnding2()
    {
        
    }
    
}
