using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;


public class PlayAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;
    private Button button;

    private void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetTrigger("Hover"); // Activa la animación de hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animator != null)
        {
            animator.SetTrigger("Idle"); // Regresa a la animación de idle
        }
    }
}

