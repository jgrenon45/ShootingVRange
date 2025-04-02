using UnityEngine;

public class StableTarget : Target
{
    [Header("Hit Zone Parameters")]
    [SerializeField] private float maxRadius = 0.5f;

    private int ringCount = 10;

    public override void TargetHit(Vector3 hitPoint)
    {
        base.TargetHit(hitPoint);
    }

    protected override int CalculateScore(Vector3 hitPosition)
    {
        // Measure distance between the hit and the target center
        float distance = Vector3.Distance(targetCenter.position, hitPosition);

        // Normalize distance (0 = center, 1 = max radius)
        float normalizedDistance = Mathf.Clamp01(distance / maxRadius);

        if (distance < maxRadius * 0.1f) return maxScore; // Bullseye
        if (distance < maxRadius * 0.3f) return 9;
        if (distance < maxRadius * 0.5f) return 7;
        if (distance < maxRadius * 0.7f) return 5;
        if (distance < maxRadius * 0.9f) return 3;

        return minScore; 
    }

    protected override void ShowFloatingText(Vector3 hitPos, int score)
    {
        if (floatingTextPrefab)
        {
            hitPos = new Vector3(hitPos.x, hitPos.y + 0.1f, hitPos.z);
            GameObject textObj = Instantiate(floatingTextPrefab, hitPos, targetCenter.rotation);
            textObj.GetComponent<FloatingText>().SetText(score.ToString());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 1; i <= ringCount; i++)
        {
            float radius = (i / (float)ringCount) * maxRadius;
            DrawCircle(targetCenter.position, radius, targetCenter.rotation);
        }
    }

    void DrawCircle(Vector3 center, float radius, Quaternion rotation)
    {
        int segments = 100; // Smoothness of the circle
        Vector3 prevPoint = center + rotation * new Vector3(radius, 0, 0); // Apply rotation

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * 2 * Mathf.PI;
            Vector3 newPoint = center + rotation * new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
