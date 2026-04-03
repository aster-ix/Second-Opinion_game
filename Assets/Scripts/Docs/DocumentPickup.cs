using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Collider))]
public class DocumentPickup : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform ReaderPanel;
    [SerializeField] private RectTransform NotebookPanel;
    [SerializeField] private Canvas canvas;
    public DocumentData data;
    public DocumentReaderUI readerUI;
    public Transform readingAnchor;


    public float animDuration = 0.35f;
    public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _originPos;
    private Quaternion _originRot;
    private bool _isReading;
    private bool _isAnimating;

    void Start()
    {
        _originPos = transform.position;
        _originRot = transform.rotation;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isAnimating || _isReading) return;
        StartCoroutine(PickUp());
    }

    void Update()
    {
        if (!_isReading || _isAnimating) return;

        // клик мимо документа
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;


        Vector2 mousePos = Mouse.current.position.ReadValue();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;
        
        bool insideReader = RectTransformUtility.RectangleContainsScreenPoint(ReaderPanel, mousePos, cam);
        bool insideNoteBook = RectTransformUtility.RectangleContainsScreenPoint(NotebookPanel, mousePos, cam);

     

        if (!insideReader && !insideNoteBook) 
            StartCoroutine(PutDown());
    }

    IEnumerator PickUp()
    {
        _isAnimating = true;
        _isReading = true;

        yield return AnimateTo(readingAnchor.position, readingAnchor.rotation);

        readerUI.Open(data);
        _isAnimating = false;
    }

    IEnumerator PutDown()
    {
        _isAnimating = true;
        readerUI.Close();

        yield return AnimateTo(_originPos, _originRot);

        _isReading = false;
        _isAnimating = false;
    }

    IEnumerator AnimateTo(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        for (float t = 0f; t < 1f; t += Time.deltaTime / animDuration)
        {
            float curved = animCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, targetPos, curved);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, curved);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}