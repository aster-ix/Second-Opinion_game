using UnityEngine;

public class Shiza : MonoBehaviour
{
    private SanitySystem sanitySystem;
    public GameObject Shiza_obj;
    float SanityChance;
    
    void Start()
    {
        sanitySystem = FindObjectOfType<SanitySystem>();
        SanityChance = sanitySystem.GetCurrentSanity() / 100;
    }

    //добавлю 2 спота для шизы, и если камера повернута на кровать буду спавнить в прохходе. Если камера на проходе, буду спавнить рядом с кроватью. Сделаю привязку к концу диалога

    public void Appearing()
    {
        float SpawnChance = Random.Range(0f, 1f);
        if (SanityChance < SpawnChance)
        {
            Shiza_obj.SetActive(true);
        }
        
    }
    
}
