using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ActiveObjects : MonoBehaviour
{
    public GameObject monster;
 
    void Start()
    {
        monster.SetActive(false);
    }
 
    /*IEnumerator OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            monster.SetActive(true);
            yield return new WaitForSeconds(3);
            monster.SetActive(false);
        }
    }*/
}
 

