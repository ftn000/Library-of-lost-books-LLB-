using UnityEngine;

public class PlayerOcclusionHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private SpriteRenderer highlightSprite;
    [SerializeField] private Camera cam;

    [Header("Anim")]
    [SerializeField] private Animator animator;
    private LookDir lastDir = LookDir.Down;

    private RaycastHit[] hits = new RaycastHit[8];

    private void LateUpdate()
    {
        Vector3 camPos = cam.transform.position;
        Vector3 target = mainSprite.bounds.center;

        Vector3 dir = target - camPos;
        float dist = dir.magnitude;

        int hitCount = Physics.RaycastNonAlloc(
            camPos,
            dir.normalized,
            hits,
            dist
        );

        bool occluded = false;

        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].collider.GetComponentInParent<Occluder>() != null)
            {
                occluded = true;
                break;
            }
        }

        highlightSprite.enabled = occluded;

        if (occluded)
        {
            //highlightSprite.sprite = mainSprite.sprite; 
        }

        
    }

    public void UpdateFromPlayer(Vector2 delta)
    {
        UpdateVisualFromDelta(new Vector2(delta.x, delta.y));
    }
    LookDir Get8Dir(Vector2 dir)
    {
        if (dir.x > 0 && dir.y > 0) return LookDir.Up;        // NE
        if (dir.x < 0 && dir.y > 0) return LookDir.Up;        // NW
        if (dir.x > 0 && dir.y < 0) return LookDir.Down;      // SE
        if (dir.x < 0 && dir.y < 0) return LookDir.Down;      // SW

        if (dir.x > 0) return LookDir.Right;
        if (dir.x < 0) return LookDir.Left;
        if (dir.y > 0) return LookDir.Up;
        return LookDir.Down;
    }


    void UpdateVisualFromDelta(Vector2 delta)
    {
        if (delta.sqrMagnitude < 0.0001f)
            return;

        Vector2 dir = delta.normalized;

        // Округляем к ближайшему из 8 направлений
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);

        animator.SetFloat("InputX", dir.x);
        animator.SetFloat("InputY", dir.y);

        lastDir = Get8Dir(dir);
    }
}
