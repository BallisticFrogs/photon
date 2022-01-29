using UnityEngine;

public class Atom : MonoBehaviour
{
    public float energy = 0f;

    public float emissionAngleSpeed = -1f;
    private float emissionAngle;
    private int emissionAngleSign;

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
            emissionAngle += emissionAngleSpeed * emissionAngleSign * Time.deltaTime;
            var v = Rotate(Vector2.up, emissionAngle) * radius * 0.97f;
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
        var photon = col.gameObject.GetComponent<PhotonManager>();
        if (photon == null)
        {
            return;
        }

        // capture photon
        energy += photon.energy;
        var nucleusToCollision = col.gameObject.transform.position - gameObject.transform.position;
        emissionAngle = Vector2.SignedAngle(Vector2.up, nucleusToCollision);
        Destroy(col.gameObject);

        // compute rotation direction from angle and position of the incoming photon relative to the nucleus
        float a = Vector2.SignedAngle(photon.Direction, -nucleusToCollision);
        emissionAngleSign = a > 0 ? 1 : -1;
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