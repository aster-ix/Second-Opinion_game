using UnityEngine;
using System.Collections;

public class Shiza : MonoBehaviour
{
    private SanitySystem sanitySystem;
    private ShizaMonologue shizaMonologue;
    private DialogUIManagerScript dialogUIManagerScript;

    public GameObject Shiza_obj;

    [SerializeField] private Transform spawnPointA; // Корридор
    [SerializeField] private Transform spawnPointB; // Комната
    public bool AppearedA = false;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private float checkInterval = 300f;

    float SanityChance;

    private bool DialogOver = false;
    private bool isOnCooldown = false;

    private GameObject currentShizaClone;

    void Start()
    {
        shizaMonologue = FindObjectOfType<ShizaMonologue>();
        sanitySystem = FindObjectOfType<SanitySystem>();
        dialogUIManagerScript = FindObjectOfType<DialogUIManagerScript>();

        StartCoroutine(CheckRoutine());
    }

    void Update()
    {
        SanityChance = sanitySystem.GetCurrentSanity();
    }


    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            DialogOver = dialogUIManagerScript.isOver;

            if (DialogOver)
            {
                TryAppear();
                DialogOver = false;

                StartCoroutine(Cooldown());
            }
        }
    }

    public void OnDialogFinished()
    {
        DialogOver = dialogUIManagerScript.isOver;

        if (DialogOver)
        {
            TryAppear();
            DialogOver = false;

            StartCoroutine(Cooldown());
        }
    }

    private void TryAppear()
    {
        float SpawnChance = Random.Range(50f, 100f);

        if (SanityChance < SpawnChance)
        {
            Transform spawnPoint = ChooseSpawn();
            currentShizaClone = Instantiate(Shiza_obj, spawnPoint.position, spawnPoint.rotation);

            shizaMonologue.StartMonologue();
        }
    }

    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(checkInterval);
        isOnCooldown = false;
    }

    Transform ChooseSpawn()
    {
        float cameraAngle = playerCamera.transform.eulerAngles.y;

        if (cameraAngle == 180.0f)
        {
            AppearedA = false;
            return spawnPointB;
        }
        else
        {
            AppearedA = true;
            return spawnPointA;
        }
            
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