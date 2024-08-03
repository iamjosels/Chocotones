using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5.0f;
    private bool vaVertical = false;
    private bool vaHorizontal = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float MoveCaracolH = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float MoveCaracolV = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        if (vaVertical && vaHorizontal)
        {
            MoveCaracolH = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            MoveCaracolV = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        }
        else if (vaVertical)
        {
            MoveCaracolH = 0;
        }
        else if (vaHorizontal)
        {
            MoveCaracolV = 0;
        }

        transform.Translate(MoveCaracolH, MoveCaracolV, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pared")
        {
            rb.gravityScale = 0;
            vaVertical = true;
        }
        if (collision.gameObject.tag == "Suelo")
        {
            rb.gravityScale = 1;
            vaHorizontal = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pared")
        {
            vaVertical = false;
        }
        if (collision.gameObject.tag == "Suelo")
        {
            vaHorizontal = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Manzana")
        {
            transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Pera")
        {
            transform.localScale = transform.localScale - new Vector3(0.1f, 0.1f, 0);
            Destroy(collision.gameObject);
        }
    }
}



