using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2D;
    SpriteRenderer spriteRenderer;

    float speed = 4;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = (int)(-transform.position.y * 10);

        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("attack");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < 1.8f)
                {
                    Vector3 direction = (enemy.transform.position - transform.position).normalized;
                    enemy.GetComponent<Zombie>().OnHit(direction);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"),
            0
            );
        rb2D.MovePosition(transform.position + input * speed * Time.deltaTime);

        bool moving = input.x != 0 || input.y != 0;
        animator.SetBool("moving", moving);
        if (moving)
        {
            animator.SetFloat("lastMovementX", input.x);
            animator.SetFloat("lastMovementY", input.y);
        }
    }
}
