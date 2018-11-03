using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Al_AI.Scripts
{
    public class Spavn : MonoBehaviour
    {
        public float min = -2;
        public float max = 3;
        public float cooldown = 5;
        public GameObject enemy;

        public short maximum = 10;

        public List<GameObject> gameObjects = new List<GameObject>();

        // Use this for initialization
        private void Start()
        {
            State();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            
        }


        private Vector3 GetRandomPositionForEidolons()
        {
            float x, z;
            x = UnityEngine.Random.Range(min, max);
            z = UnityEngine.Random.Range(min, max);

            return transform.position + new Vector3(x, 0, z);
        }

        private void State()
        {
            if (gameObjects.Count < maximum)
            {
               gameObjects.Add( Instantiate(enemy, GetRandomPositionForEidolons(), Quaternion.identity));

            }
            else
            {
                for(int i=0;i< gameObjects.Count; i++)
                {
                    if (gameObjects[i] == null)
                    {
                        gameObjects.RemoveAt(i);
                        i--;
                    }
                }
            }
            Invoke("State", cooldown);
        }
    }
}
