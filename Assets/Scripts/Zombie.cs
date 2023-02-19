using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{

    GameObject player;
    Rigidbody2D rb2D;
    List<Node2D> path;
    Animator animator;
    SpriteRenderer spriteRenderer;


    float speed = 1;
    bool pauseMovement = false;

    int health = 100;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(Pathfinding());
    }

    IEnumerator Pathfinding()
    {
        while (true)
        {
            path = Pathfinding2D.FindPath(transform.position, player.transform.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = (int)(-transform.position.y * 10);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(path != null && path.Count > 0 && !pauseMovement)
        {
           Vector2 newPosition =  Vector2.MoveTowards(
                transform.position,
                path[0].worldPosition,
                speed * Time.deltaTime);

            Vector3 direction =
                (new Vector3(newPosition.x, newPosition.y) - transform.position).normalized;

            rb2D.MovePosition(newPosition);
            
            animator.SetBool("moving", true);
            animator.SetFloat("lastMovementX", direction.x);
            animator.SetFloat("lastMovementY", direction.y);
        }
    }

    public void OnHit(Vector2 direction)
    {
        rb2D.velocity = direction * 3;
        health -= 30;
        StartCoroutine(HitCooldown());
    }

    IEnumerator HitCooldown()
    {
        pauseMovement = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.4f);
        pauseMovement = false;
    }

    /*public void OnDrawGizmos()
    {
        Pathfinding2D.DrawGizmoz(transform, path);
    }*/
}
