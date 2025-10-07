using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace Marvin.PoolingSystem
{
    public class PoolingSystem : MonoBehaviour
    {
        public static List<PoolObjectInfo> poolObjectInfos = new List<PoolObjectInfo>();

        public static GameObject rootGameObject;
        public enum PoolingParentGameObject
        {
            Default,
            Player,
            Enemy,
        }

        private void Awake()
        {
            rootGameObject = gameObject;
            poolObjectInfos.Clear();

            int enumLength = Enum.GetNames(typeof(PoolingParentGameObject)).Length;

            for (int i = 0; i < enumLength; i++)
            {
                GameObject obj = new GameObject(Enum.GetName(typeof(PoolingParentGameObject), i));
                obj.transform.parent = transform;
            }
        }

        public static GameObject SpawnObject(GameObject objToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolingParentGameObject projectileType = PoolingParentGameObject.Default)
        {
            PoolObjectInfo pool = poolObjectInfos.Find(p => p.objectName == objToSpawn.name);

            if (pool == null)
            {
                pool = new PoolObjectInfo();
                poolObjectInfos.Add(pool);
                pool.objectName = objToSpawn.name;
            }

            GameObject obj = pool.inactiveObjects.FirstOrDefault();

            if (obj == null)
            {
                obj = Instantiate(objToSpawn, spawnPosition, spawnRotation);

                obj.GetComponent<IPoolingList>().poolingList = pool;
            }
            else
            {
                obj.transform.position = spawnPosition;
                obj.transform.rotation = spawnRotation;

                obj.SetActive(true);
                pool.inactiveObjects.Remove(obj);
            }

            obj.gameObject.transform.parent = rootGameObject.transform.GetChild((int)projectileType);

            return obj;
        }
        public static void ReturnObjectToPool(GameObject obj, PoolObjectInfo poolObjectInfo)
        {
            if (poolObjectInfo.inactiveObjects.Contains(obj))
            {
                //Debug.Log("Object is multiple times in the list");
                obj.SetActive(false);
                return;
            }

            poolObjectInfo.inactiveObjects.Add(obj);
            obj.SetActive(false);
        }

        public static void PurgePool()
        {
            int enumLength = Enum.GetNames(typeof(PoolingParentGameObject)).Length;

            for (int i = 0; i < enumLength; i++)
            {
                var child = rootGameObject.transform.GetChild(i);
                for (int j = child.childCount - 1; j >= 0; j--)
                {
                    var obj = child.GetChild(j).gameObject;
                    Destroy(obj);
                }
            }
        }

        public class PoolObjectInfo
        {
            public string objectName;
            public List<GameObject> inactiveObjects = new List<GameObject>();
        }
    }
}
