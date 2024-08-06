using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Velocidad")]
    [SerializeField] float speed = 5.0f;
    [Header("Caida")]
    [SerializeField] KeyCode caidaButton = KeyCode.Space;
    private bool vaVertical = false;
    private bool vaHorizontal = false;
    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private Gusano gusano;
    private bool isAutomaticMovement = false;

    private Vector3 baseScale = new Vector3(0.61109f, 0.61109f, 0.61109f); // Escala base del caracol
    private Vector3 currentScale; // Escala actual que cambia con el tamaño

    //Dialogs
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    private bool didDialogueStart;
    private int lineIndex;
    private float dialogueSpeed = 0.05f;

    private bool canStartDialogue = false; // Variable para controlar si se puede iniciar el diálogo

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        gusano = FindObjectOfType<Gusano>();
        currentScale = baseScale; // Inicializa la escala actual
        UpdateScale(); // Ajustar inicialmente el collider
    }

    void Update()
    {
        if (!isAutomaticMovement)
        { 
            float moveCaracolH = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float moveCaracolV = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            if (vaVertical && vaHorizontal)
            {
                moveCaracolH = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
                moveCaracolV = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            }
            else if (vaVertical)
            {
                moveCaracolH = 0;
            }
            else if (vaHorizontal)
            {
                moveCaracolV = 0;
            }

            if (vaVertical && Input.GetKeyDown(caidaButton))
            {
                rb.gravityScale = 1;
                vaVertical = false;
                vaHorizontal = false;
            }

            // Voltear el sprite cuando se mueve a la izquierda
            if (moveCaracolH < 0)
            {
                if (transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }
            else if (moveCaracolH > 0)
            {
                if (transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }

            // Aplicar el movimiento
            transform.Translate(moveCaracolH, moveCaracolV, 0);

            // Cambiar a animación de movimiento o idle
            if (moveCaracolH != 0 || moveCaracolV != 0)
            {
                ChangeMovementAnimation();
            }
            else
            {
                ChangeIdleAnimation();
            }

            // Manejo de entrada del teclado para el diálogo
            if (Input.GetButtonDown("Fire1") && canStartDialogue)
            {
                if (!didDialogueStart)
                {
                    StartDialogue();
                }
                else if (dialogueText.text == dialogueLines[lineIndex])
                {
                    NextDialogueLine();
                }
                else
                {
                    StopAllCoroutines();
                    dialogueText.text = dialogueLines[lineIndex];
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pared"))
        {
            rb.gravityScale = 0;
            vaVertical = true;
        }
        if (collision.gameObject.CompareTag("Suelo"))
        {
            rb.gravityScale = 1;
            vaHorizontal = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pared"))
        {
            vaVertical = false;
        }
        if (collision.gameObject.CompareTag("Suelo"))
        {
            vaHorizontal = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Manzana"))
        {
            StartCoroutine(PlayEatAnimation("Eat"));
            currentScale += new Vector3(0.5f, 0.5f, 0); // Ajustar la escala actual
            UpdateScale();
            ChangeMovementAnimation();
            StartCoroutine(PlayGusanoAnimation("Agrandar"));
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Pera"))
        {
            StartCoroutine(PlayEatAnimation("Eat"));
            currentScale -= new Vector3(0.5f, 0.5f, 0); // Ajustar la escala actual
            UpdateScale();
            ChangeMovementAnimation();
            StartCoroutine(PlayGusanoAnimation("Reducir"));
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("GusanoActive"))
        {
            Debug.Log("Gusano Activado");
            gusano.ActivateGusanoAnimation(transform);
            Destroy(collision.gameObject);
            canStartDialogue = true; // Permitir el inicio del diálogo

            // Iniciar el diálogo si el caracol no está en movimiento automático
            if (!didDialogueStart)
            {
                StartDialogue();
            }
        }
        if (collision.gameObject.CompareTag("Portal"))
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void StartDialogue()
    {
        if (!didDialogueStart)
        {
            didDialogueStart = true;
            dialoguePanel.SetActive(true);
            lineIndex = 0;
            StartCoroutine(ShowLine());
        }
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(dialogueSpeed);
        }
    }

    private IEnumerator PlayEatAnimation(string animationName)
    {
        animator.Play(animationName);
        yield return new WaitForSeconds(2.0f);
    }

    private IEnumerator PlayGusanoAnimation(string animationName)
    {
        Debug.Log("Attempting to play animation: " + animationName);
        animator.Play(animationName);
        yield return new WaitForSeconds(2.0f);

        // Aquí lanzamos las manzanas y la pera como en el paso anterior
        yield return new WaitForSeconds(2.0f);
        GameObject manzana1 = new GameObject("Manzana1");
        manzana1.tag = "Manzana";
        GameObject manzana2 = new GameObject("Manzana2");
        manzana2.tag = "Manzana";

        // Lanzar las manzanas (ajustar las posiciones según sea necesario)
        Rigidbody2D rb1 = manzana1.AddComponent<Rigidbody2D>();
        rb1.velocity = new Vector2(2.0f, 0.0f); // Ajustar la dirección y velocidad

        yield return new WaitForSeconds(2.0f);

        Rigidbody2D rb2 = manzana2.AddComponent<Rigidbody2D>();
        rb2.velocity = new Vector2(2.0f, 0.0f); // Ajustar la dirección y velocidad

        yield return new WaitForSeconds(5.0f);

        GameObject pera = new GameObject("Pera");
        pera.tag = "Pera";
        Rigidbody2D rbPera = pera.AddComponent<Rigidbody2D>();
        rbPera.velocity = new Vector2(2.0f, 0.0f); // Ajustar la dirección y velocidad

        Debug.Log("Manzanas y pera lanzadas");
    }

    private void UpdateScale()
    {
        // Asegúrate de que la escala x se mantenga positiva para evitar problemas
        float scaleX = Mathf.Abs(currentScale.x);
        float scaleY = currentScale.y;

        // Ajusta el localScale con la escala base y la dirección actual
        transform.localScale = new Vector3(
            scaleX * Mathf.Sign(transform.localScale.x),
            scaleY,
            transform.localScale.z
        );

        UpdateColliderSize();
    }

    private void UpdateColliderSize()
    {
        if (boxCollider != null)
        {
            // Factor de ajuste para reducir el tamaño del collider cuando el sprite se hace más pequeño
            float adjustmentFactor = 0.8f; // Puedes ajustar este valor según sea necesario

            // Actualizar el tamaño del collider con base en la escala del jugador y el factor de ajuste
            float width = Mathf.Abs(transform.localScale.x) * adjustmentFactor;
            float height = Mathf.Abs(transform.localScale.y) * adjustmentFactor;
            boxCollider.size = new Vector2(width, height);

            // Actualizar el offset del collider para mantenerlo centrado en el sprite
            boxCollider.offset = new Vector2(0, height / 2 - 0.5f * adjustmentFactor);
        }
    }

    private void ChangeMovementAnimation()
    {
        float scaleMagnitude = currentScale.magnitude;
        float baseScaleMagnitude = baseScale.magnitude;

        if (scaleMagnitude > baseScaleMagnitude * 1.1f)
        {
            // El jugador está grande
            animator.Play("Move_Grande");
        }
        else if (scaleMagnitude < baseScaleMagnitude * 0.9f)
        {
            // El jugador está pequeño
            animator.Play("Move_Chico");
        }
        else
        {
            // El jugador está en tamaño normal
            animator.Play("Move_Normal");
        }
    }

    private void ChangeIdleAnimation()
    {
        float scaleMagnitude = currentScale.magnitude;
        float baseScaleMagnitude = baseScale.magnitude;

        if (scaleMagnitude > baseScaleMagnitude * 1.1f)
        {
            // El jugador está grande
            animator.Play("Grande_idle");
        }
        else if (scaleMagnitude < baseScaleMagnitude * 0.9f)
        {
            // El jugador está pequeño
            animator.Play("Idle");
        }
        else
        {
            // El jugador está en tamaño normal
            animator.Play("Idle");
        }
    }

    public void SetAutomaticMovement(bool isAutomatic)
    {
        isAutomaticMovement = isAutomatic;
    }
}





//Otro 
/*private void UpdateColliderSize()
{
    if (boxCollider != null)
    {
        // Actualizar el tamaño del collider con base en la escala del jugador
        float width = Mathf.Abs(transform.localScale.x);
        float height = Mathf.Abs(transform.localScale.y);
        boxCollider.size = new Vector2(width - 0.2f, height - 0.2f);

        // Actualizar el offset del collider para mantenerlo centrado en el sprite
        boxCollider.offset = new Vector2(0, height / 2 - 0.5f);
    }
}*/























