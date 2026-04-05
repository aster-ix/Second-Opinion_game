using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class DocumentPickup : MonoBehaviour, IPointerClickHandler
{
    public DocumentData     data;
    public DocumentReaderUI readerUI;
    public Transform        readingAnchor;

    [Header("Панели которые блокируют откладывание документа")]

    public List<RectTransform> blockingPanels = new();

    public float          animDuration = 0.35f;
    public AnimationCurve animCurve    = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Animator animator;

    private Vector3    _originPos;
    private Quaternion _originRot;
    private bool       _isReading;
    private bool       _isAnimating;


    private readonly List<RaycastResult> _raycastResults = new();

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
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();


        var pointerData = new PointerEventData(EventSystem.current) { position = mousePos };
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, _raycastResults);


        foreach (var hit in _raycastResults)
        {
            foreach (var panel in blockingPanels)
            {
                if (panel == null || !panel.gameObject.activeInHierarchy) continue;
                if (hit.gameObject.transform.IsChildOf(panel) || hit.gameObject.transform == panel.transform)
                    return;
            }
        }

        // Не кладём если клик попал в сам документ
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit physHit) && physHit.transform == transform)
            return;

        StartCoroutine(PutDown());
    }

    IEnumerator PickUp()
    {
        _isAnimating = true;
        _isReading   = true;
        animator.Play("Doc_opening", 0, 0.0f);
        yield return AnimateTo(readingAnchor.position, readingAnchor.rotation);
        readerUI.Open(data);
        _isAnimating = false;
    }

    IEnumerator PutDown()
    {
        _isAnimating = true;
        readerUI.Close();
        animator.Play("Doc_closing", 0, 0.0f);
        yield return AnimateTo(_originPos, _originRot);
        _isReading   = false;
        _isAnimating = false;
    }

    IEnumerator AnimateTo(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3    startPos = transform.position;
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