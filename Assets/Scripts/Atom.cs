using DefaultNamespace;
using UnityEngine;

public class Atom : MonoBehaviour
{
    public float energy = 0f;

    private float emissionAngle;
    private int emissionAngleSign;

    public Transform orbit;
    public GameObject charge;

    private bool keyDown;
    private bool keyWasDown;

    void Update()
    {
        keyWasDown = keyDown;
        keyDown = Input.anyKey;

        var keyUp = !keyDown && keyWasDown;
        if (keyUp && energy > 0)
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
            float bonusSpeed = keyDown ? 3 : 1;
            var radius = orbit.transform.localScale.x / 2f;
            emissionAngle += GameManager.INSTANCE.emissionAngleSpeed * emissionAngleSign * Time.deltaTime * bonusSpeed;
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

    public void GenerateBonusPhoton(float energy)
    {
        var pos = FindCorrectPopPosition();
        var velocity = (transform.position - pos).normalized * GameManager.INSTANCE.emissionSpeed * 2;
        GameManager.INSTANCE.CreateNewPhoton(null, pos, velocity, energy, false);
        GameManager.INSTANCE.RegisterAtom(this);
    }

    private Vector3 FindCorrectPopPosition()
    {
        var sourcePos = transform.position;

        int attempts = 0;
        while (attempts < 10)
        {
            attempts++;
            var v = GenerateRandomPosition();
            var pos = sourcePos + v;
            var hitCheck = Physics2D.CircleCast(pos, 1 + 3, -v, float.PositiveInfinity,
                Masks.ATOMS | Masks.OBSTACLES);
            if (hitCheck.collider && hitCheck.rigidbody.gameObject == gameObject)
            {
                return pos;
            }
        }

        return sourcePos + GenerateRandomPosition();
    }

    private static Vector3 GenerateRandomPosition()
    {
        return Vector2.up.Rotate(Random.Range(0, 360)) * GameManager.INSTANCE.checkpointPhotonPopDistance;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var photon = col.gameObject.GetComponentInParent<Photon>();
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