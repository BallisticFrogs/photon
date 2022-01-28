using UnityEngine;

public class Atom : MonoBehaviour
{
    public float emissionAngleSpeed = -0.1f;
    public float energy = 0f;

    private float emissionAngle;

    public Transform orbit;
    public GameObject charge;

    void Start()
    {
    }

    void Update()
    {
        var keyDown = Input.anyKeyDown;
        if (keyDown && energy > 0)
        {
            // TODO emit photon
        }

        if (energy > 0)
        {
            var radius = orbit.transform.localScale.x / 2f;
            emissionAngle += emissionAngleSpeed;
            var v = Rotate(Vector2.up, emissionAngle) * radius * 0.95f;
            charge.transform.position = transform.position + new Vector3(v.x, v.y, charge.transform.position.z);
        }

        if (energy <= 0 && charge.activeSelf)
        {
            charge.SetActive(false);
        }

        if (energy > 0 && !charge.activeSelf)
        {
            charge.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // capture photon
        energy += 1; // photon.energy
        emissionAngle =
            Vector2.SignedAngle(Vector2.up, col.gameObject.transform.position - gameObject.transform.position);
        // TODO destroy photon object
    }

    public static Vector2 Rotate(Vector2 v, float angleInDegs)
    {
        float angleInRads = angleInDegs * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(angleInRads) - v.y * Mathf.Sin(angleInRads),
            v.x * Mathf.Sin(angleInRads) + v.y * Mathf.Cos(angleInRads)
        );
    }
}