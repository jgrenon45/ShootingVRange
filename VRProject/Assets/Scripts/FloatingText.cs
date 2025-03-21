using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float lifetime = 1.5f; // Time before disappearing
    public float floatSpeed = 1.0f; // How fast it moves up
    public TextMeshPro textMesh;
    private Color startColor;

    void Start()
    {
        startColor = textMesh.color;
        Destroy(gameObject, lifetime); // Automatically destroy after lifetime
    }

    void Update()
    {
        // Make text float upwards over time
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Gradually fade out
        float fadeAmount = Mathf.Clamp01(lifetime - Time.timeSinceLevelLoad);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fadeAmount);
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
