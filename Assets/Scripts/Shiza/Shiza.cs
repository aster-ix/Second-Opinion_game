using UnityEngine;

public class Shiza : MonoBehaviour
{
    private SanitySystem sanitySystem;
    private ShizaMonologue shizaMonologue;
    private DialogUIManagerScript dialogUIManagerScript;
    public GameObject Shiza_obj;
    [SerializeField] private Transform spawnPointA; // Корридор
    [SerializeField] private Transform spawnPointB; // Комната
    [SerializeField] private Camera playerCamera; // камера со скриптом поворота

    float SanityChance;
    private bool DialogOver = false;
    private GameObject currentShizaClone;
    void Start()
    {
        shizaMonologue = FindObjectOfType<ShizaMonologue>();
        sanitySystem = FindObjectOfType<SanitySystem>();
        dialogUIManagerScript = FindObjectOfType<DialogUIManagerScript>();
    }

    void Update()
    {
        DialogOver = dialogUIManagerScript.isOver;
        SanityChance = sanitySystem.GetCurrentSanity();
    }

    public void Appearing()
    {

        if (!DialogOver) return;
        float SpawnChance = Random.Range(50f, 100f);
        if (SanityChance < SpawnChance)
        {
            Transform spawnPoint = ChooseSpawn();
            currentShizaClone = Instantiate(Shiza_obj, spawnPoint.position, spawnPoint.rotation);
            shizaMonologue.StartMonologue();
        }

    }

    Transform ChooseSpawn()
    {
        float cameraAngle = playerCamera.transform.eulerAngles.y;


        if (cameraAngle == 180.0)
            return spawnPointB;
        else
            return spawnPointA;
    }
    public void DestroyCurrentShiza()
    {
        if (currentShizaClone != null)
        {
            Destroy(currentShizaClone);
            currentShizaClone = null;
            
        }
    }
}
