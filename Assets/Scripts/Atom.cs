using DefaultNamespace;
using UnityEngine;

public class Atom : MonoBehaviour
{
    public float energy = 0f;

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
            EmitPhoton();
        }

        UpdateEmissionAngle();
    }

    private void EmitPhoton()
    {
        GameManager.INSTANCE.CreateNewPhoton(this);
        GameManager.INSTANCE.UnregisterAtom(this);
        energy = 0;
    }

    private void UpdateEmissionAngle()
    {
        if (energy > 0)
        {
            var radius = orbit.transform.localScale.x / 2f;
            emissionAngle += GameManager.INSTANCE.emissionAngleSpeed * emissionAngleSign * Time.deltaTime;
            var v = Vector2.up.Rotate(emissionAngle) * radius * 0.97f;
            charge.transform.position = transform.position + new Vector3(v.x, v.y, charge.transform.position.z);
        }

        if (energy <= 0 && charge.activeSelf)
        {
            charge.SetActive(false);
        }
        else if (energy > 0 && !charge.activeSelf)
        {
            charge.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var photon = col.gameObject.GetComponent<Photon>();
        if (!photon || (photon.source && photon.source.gameObject == gameObject))
        {
            return;
        }

        // capture photon
        energy += photon.energy;
        var nucleusToCollision = col.gameObject.transform.position - gameObject.transform.position;
        emissionAngle = Vector2.SignedAngle(Vector2.up, nucleusToCollision);
        GameManager.INSTANCE.PhotonLost(photon);
        GameManager.INSTANCE.RegisterAtom(this);

        // compute rotation direction from angle and position of the incoming photon relative to the nucleus
        float a = Vector2.SignedAngle(photon.Velocity, -nucleusToCollision);
        emissionAngleSign = a > 0 ? 1 : -1;
    }
}