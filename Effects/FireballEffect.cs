using System.Collections;
using UnityEngine;

public static class FireballEffect
{
    private const string FireballExplosionResource = "Effects/FireballExplosionEffect";
    private const string FireballSpellResource = "Effects/FireballSpellEffect";
    private const float FireballSpeed = 2f;

    public static void Play(UnitManager context, Vector3 from, Vector3 to)
    {
        var fireball = GameObject.Instantiate(
            Resources.Load(FireballSpellResource) as GameObject,
            from,
            Quaternion.identity);

        fireball.transform.LookAt(to);

        var explosion = GameObject.Instantiate(
            Resources.Load(FireballExplosionResource) as GameObject,
            to,
            Quaternion.identity);
        explosion.SetActive(false);

        context.StartCoroutine(MoveFireballTowards(fireball, explosion, from, to));
    }

    public static IEnumerator MoveFireballTowards(GameObject fireball, GameObject explosion, Vector3 start, Vector3 destination)
    {
        float timeToComplete = Mathf.Floor(Vector3.Distance(start, destination)) / FireballSpeed;
        float timeElapsed = 0;

        while (timeElapsed < timeToComplete)
        {
            fireball.transform.position = Vector3.Lerp(start, destination, timeElapsed / timeToComplete);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fireball.SetActive(false);
        GameObject.Destroy(fireball);

        explosion.SetActive(true);
        GameObject.Destroy(explosion, 2.5f);
    }
}
