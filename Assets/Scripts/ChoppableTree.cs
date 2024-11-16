using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{
    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth;
    public float treeHealth;

    public Animator animator;

    private void Start() {
        treeHealth = treeMaxHealth;
        animator = transform.parent.transform.parent.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }


    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit(float Damage)
    {
        StartCoroutine(Hit(Damage));
    }

    public IEnumerator Hit (float Damage)
    {
        animator.SetTrigger("shake");
        treeHealth -= Damage;

        if(treeHealth < 0)
            TreeIsDead();

        yield return null;
    }

    private void TreeIsDead()
    {
        Vector3 treePostion = transform.position; 

        Destroy(transform.parent.transform.parent.gameObject); // 할아버지 오브젝트를 삭제(Tree_Parent)
        
        GameObject brokenTree  = Instantiate(Resources.Load<GameObject>("ChoppedTree"),
        new Vector3 (treePostion.x , treePostion.y, treePostion.z), Quaternion.identity);
    }
}
