using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] protected int maxScore = 10;
    [SerializeField] protected int minScore = 1;

    [Header("Points UI")]
    [SerializeField] protected Transform pointsTransform;
    [SerializeField] protected GameObject floatingTextPrefab;
    [SerializeField] protected Transform targetCenter;

    private TestManager tests = TestManager.GetInstance();

    public virtual void TargetHit(Vector3 hitPoint)
    {
        int score = CalculateScore(hitPoint);
        ShowFloatingText(hitPoint, score);
        ScoreManager.instance.AddScore(score);
        tests.IncreaseScore(score);
        tests.WriteToJSONIfFinished();
    }

    protected virtual int CalculateScore(Vector3 hitPosition)
    {       
        return maxScore;
    }

    protected virtual void ShowFloatingText(Vector3 hitPos, int score)
    {
        if (floatingTextPrefab)
        {
            hitPos = new Vector3(hitPos.x, hitPos.y + 0.1f, hitPos.z);
            GameObject textObj = Instantiate(floatingTextPrefab, pointsTransform.position, pointsTransform.rotation);
            textObj.GetComponent<FloatingText>().SetText(score.ToString());
        }
    } 
}
