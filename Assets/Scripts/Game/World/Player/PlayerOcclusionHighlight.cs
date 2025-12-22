using UnityEngine;

public class PlayerOcclusionHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private SpriteRenderer highlightSprite;
    [SerializeField] private Camera cam;

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
            highlightSprite.sprite = mainSprite.sprite;
    }
}
