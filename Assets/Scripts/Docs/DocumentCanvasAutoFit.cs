using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(Canvas))]
public class DocumentCanvasAutoFit : MonoBehaviour
{
    public float pixelsPerUnit = 200f;

    void Awake()   => Fit();
    void OnValidate() => Fit(); // пересчитывается при изменении в Inspector

    void Fit()
    {
        var mr = GetComponentInParent<MeshRenderer>();
        if (mr == null) return;

        //  canvas в World Space
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        var rt = GetComponent<RectTransform>();


        Bounds b = mr.localBounds;
        rt.sizeDelta = new Vector2(b.size.x * pixelsPerUnit, b.size.y * pixelsPerUnit);


        Vector3 ps = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
        float s = 1f / pixelsPerUnit;
        rt.localScale = new Vector3(s / ps.x, s / ps.y, s / ps.z);

  
        rt.localPosition = new Vector3(0f, 0f, -0.01f);
        rt.localRotation = Quaternion.identity;

     
        foreach (var tmp in GetComponentsInChildren<TMPro.TMP_Text>())
            tmp.isOrthographic = false;
    }
}
