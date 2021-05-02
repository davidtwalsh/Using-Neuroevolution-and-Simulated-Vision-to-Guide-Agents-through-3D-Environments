using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CourseCreator : MonoBehaviour
{
    public int tileObstacleLength = 15;
    public int tileObstacleMaxStraight = 3;
    public int tileObstacleMaxLeftRight = 3;
    public GameObject tilePrefab;
    public float tilePrefabLength = 10f;

    public GameObject flagPrefab;

    List<GameObject> obstacleCourseObjs;
    public GameObject flag;
    public ManagerTileRunner managerTileRunner;

    private void Awake()
    {
        obstacleCourseObjs = new List<GameObject>();
        CreateTileObstacle();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            managerTileRunner.isTraining = false;
        }
    }

    public void CreateTileObstacle()
    {
        int tileL = 0;
        int xx = 0;
        int zz = 0;
        while (tileL < tileObstacleLength)
        {
            int straightL = Random.Range(2, tileObstacleMaxStraight + 1);
            for (int i = 0; i < straightL; i++)
            {
                zz++;
                tileL++;
                obstacleCourseObjs.Add(Instantiate(tilePrefab, new Vector3(xx * tilePrefabLength, 0f, zz * tilePrefabLength),Quaternion.identity));
            }
            int sideL = Random.Range(-tileObstacleMaxLeftRight, tileObstacleMaxLeftRight + 1);
            for (int i = 0; i < System.Math.Abs(sideL); i++)
            {
                if (sideL < 0)
                    xx--;
                else if (sideL > 0)
                    xx++;
                obstacleCourseObjs.Add(Instantiate(tilePrefab, new Vector3(xx * tilePrefabLength, 0f, zz * tilePrefabLength), Quaternion.identity));
            }
        }
        GameObject f = (Instantiate(flagPrefab, new Vector3(xx * tilePrefabLength, 2f, zz * tilePrefabLength), Quaternion.identity));
        obstacleCourseObjs.Add(f);
        flag = f;
    }

    public void ClearObstacles()
    {
        for (int i = 0; i < obstacleCourseObjs.Count; i++)
        {
            Destroy(obstacleCourseObjs[i]);
        }
        obstacleCourseObjs = new List<GameObject>();
    }
}
