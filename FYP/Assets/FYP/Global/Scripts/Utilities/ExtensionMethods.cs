using UnityEngine;

public static partial class ExtensionMethods
{
    public static float GetHorizontalDistance(this Transform self, Transform other)
    {
        Vector3 dist = self.position - other.transform.position;
        dist.y = 0f;
        return dist.magnitude;
    }
    public static float GetHorizontalDistance(this Transform self, Vector3 other)
    {
        Vector3 dist = self.position - other;
        dist.y = 0f;
        return dist.magnitude;
    }

    public static float GetVerticalDistance(this Transform self, Transform other)
    {
        return (self.position.y - other.transform.position.y);
    }
    public static float GetVerticalDistance(this Transform self, Vector3 other)
    {
        return (self.position.y - other.y);
    }

    /// <summary>
    /// Guesses the HDR intensity of the given color by taking square root method
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static float GetHDRIntensity(this Color color)
    {
        return Mathf.Sqrt(color.r*color.r +color.g*color.g +color.b*color.b);
    }

    public static void ClearTrailImmediate(this TrailRenderer trailRenderer)
    {
        trailRenderer.Clear();
        trailRenderer.enabled = false;
        trailRenderer.enabled = true;
    }
}
