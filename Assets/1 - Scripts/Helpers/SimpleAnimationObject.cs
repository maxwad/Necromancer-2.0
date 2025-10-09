using UnityEngine;

public class SimpleAnimationObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SimpleAnimator animator;

    public void SetSettings(
        float size = -1,
        Color color = default(Color),
        int sortingOrder = -1,
        string sortingLayer = "",
        float animationSpeed = 0f,
        MonoBehaviour prefabSource = null
        )
    {
        if(size != -1)
        {
            transform.localScale = new Vector3(size, size, size);
        }

        if(color != Color.clear && sprite != null)
        {
            sprite.color = color;
        }

        if(sortingOrder != -1 && sprite != null)
        {
            sprite.sortingOrder = sortingOrder;
        }

        if(sortingLayer != "" && sprite != null)
        {
            sprite.sortingLayerName = sortingLayer;
        }

        if(animationSpeed != 0 && animator != null)
        {
            animator.SetSpeed(animationSpeed);
        }

        if(prefabSource != null && animator != null)
        {
            animator.SerPrefabSource(prefabSource, this);
        }
    }
}
