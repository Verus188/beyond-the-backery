using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject deerPrefab;
    public GameObject santaPrefab;
    public Camera mainCam;
    public float interval = 3f;
    public float offset = 3f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        GameObject prefab = Random.value > 0.5f ? deerPrefab : santaPrefab;
        float height = mainCam.orthographicSize * 2;
        float width = height * mainCam.aspect;
        Vector3 camPos = mainCam.transform.position;
        Vector3 pos = camPos;
        int side = Random.Range(0, 4);

        if (side == 0) pos.y += height/2 + offset;
        else if (side == 1) pos.y -= height/2 - offset;
        else if (side == 2) { pos.x -= width/2 - offset; pos.y += Random.Range(-height/2, height/2); }
        else { pos.x += width/2 + offset; pos.y += Random.Range(-height/2, height/2); }

        if (side < 2) pos.x += Random.Range(-width/2, width/2);
        Instantiate(prefab, pos, Quaternion.identity);
    }
}

