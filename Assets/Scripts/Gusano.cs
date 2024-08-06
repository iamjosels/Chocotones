using System.Collections;
using UnityEngine;

public class Gusano : MonoBehaviour
{
    public GameObject peraPrefab; // Prefab de la pera que se lanzará
    public GameObject manzanaPrefab; // Prefab de la manzana que se lanzará
    public float launchForce = 10f; // Fuerza base con la que se lanzará la pera y la manzana
    public float manzanaInterval = 2.0f; // Intervalo entre el lanzamiento de manzanas
    public float peraInterval = 2.0f; // Intervalo entre el lanzamiento de peras

    private Animator animator;
    private Transform caracolTransform; // Referencia al transform del caracol
    private bool hasLaunchedFruits = false; // Controla si las frutas ya han sido lanzadas

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ActivateGusanoAnimation(Transform caracol)
    {
        if (!hasLaunchedFruits) // Verifica si las frutas ya han sido lanzadas
        {
            caracolTransform = caracol;
            animator.SetTrigger("Gusano");
            StartCoroutine(LaunchFruitsAfterDelay());
        }
    }

    private IEnumerator LaunchFruitsAfterDelay()
    {
        // Lanzar las dos manzanas con intervalo
        yield return new WaitForSeconds(2.0f);
        LaunchManzana();
        yield return new WaitForSeconds(manzanaInterval);
        LaunchManzana();

        // Lanzar las dos peras con intervalo después de las manzanas
        yield return new WaitForSeconds(peraInterval);
        LaunchPera();
        yield return new WaitForSeconds(peraInterval);
        LaunchPera();
    }

    private void LaunchManzana()
    {
        if (caracolTransform != null && manzanaPrefab != null)
        {
            // Crear la manzana en la posición del gusano
            GameObject manzana = Instantiate(manzanaPrefab, transform.position, Quaternion.identity);

            // Calcular la dirección y fuerza necesarias para alcanzar la posición del caracol
            Vector2 direction = (caracolTransform.position - transform.position).normalized;

            // Calcular la distancia entre el gusano y el caracol
            float distance = Vector2.Distance(caracolTransform.position, transform.position);

            // Aplicar la fuerza al objeto manzana
            manzana.GetComponent<Rigidbody2D>().AddForce(direction * launchForce * distance, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogError("Caracol transform o manzana prefab no está asignado.");
        }
    }

    private void LaunchPera()
    {
        if (caracolTransform != null && peraPrefab != null)
        {
            // Crear la pera en la posición del gusano
            GameObject pera = Instantiate(peraPrefab, transform.position, Quaternion.identity);

            // Calcular la dirección y fuerza necesarias para alcanzar la posición del caracol
            Vector2 direction = (caracolTransform.position - transform.position).normalized;

            // Calcular la distancia entre el gusano y el caracol
            float distance = Vector2.Distance(caracolTransform.position, transform.position);

            // Aplicar la fuerza al objeto pera
            pera.GetComponent<Rigidbody2D>().AddForce(direction * launchForce * distance, ForceMode2D.Impulse);

            // Marcar como lanzadas las frutas para evitar múltiples lanzamientos
            hasLaunchedFruits = true;
        }
        else
        {
            Debug.LogError("Caracol transform o pera prefab no está asignado.");
        }
    }
}










