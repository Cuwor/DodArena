using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Al_AI.Scripts
{
    public class Spavn : MonoBehaviour
    {
        public float radius = 3;
        public float cooldown = 10;
        public GameObject enemy;

        public short maximum = 10;

        public List<GameObject> gameObjects = new List<GameObject>();

        public ServivalManagerScript SMS;

        // Use this for initialization
        private void Start()
        {
            Invoke("Start2",5f);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            
        }

        private void Start2()
        {
            if(SMS==null)
                State();
        }
        

        private Vector3 GetRandomPositionForEidolons()
        {
            float x, z;
            x = UnityEngine.Random.Range(-radius, radius);
            z = UnityEngine.Random.Range(-radius, radius);

            return transform.position + new Vector3(x, 0, z);
        }

        public void State()
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
