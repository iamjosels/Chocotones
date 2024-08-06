using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public Camera mainCamera;
    public float transitionDuration = 1.0f;
    public Vector3 targetPosition;
    public Transform caracol;
    public float caracolSpeed = 2.0f;
    private Transform gusanoActiveTarget;

    private void Start()
    {
        // Buscar el objeto con el tag "GusanoActive"
        GameObject gusanoActiveObject = GameObject.FindWithTag("GusanoActive");
        if (gusanoActiveObject != null)
        {
            gusanoActiveTarget = gusanoActiveObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró el objeto con el tag 'GusanoActive'");
        }
    }

    public void PlayGame()
    {
        StartCoroutine(MoveCameraToPosition(targetPosition, transitionDuration));
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Controles");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator MoveCameraToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = mainCamera.transform.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPos;

        // Después de que la cámara haya llegado a su posición objetivo, mover el caracol
        if (gusanoActiveTarget != null)
        {
            StartCoroutine(MoveCaracolToGusano());
        }
    }

    private IEnumerator MoveCaracolToGusano()
    {
        // Desactivar el control manual
        Player caracolScript = caracol.GetComponent<Player>();
        caracolScript.SetAutomaticMovement(true);

        Vector3 targetPosition = gusanoActiveTarget.position;
        Vector3 startPosition = caracol.position;

        // Mantener la posición en el eje Y igual durante el movimiento
        float startY = startPosition.y;
        float targetX = targetPosition.x;

        while (Mathf.Abs(caracol.position.x - targetX) > 0.1f)
        {
            float newX = Mathf.MoveTowards(caracol.position.x, targetX, caracolSpeed * Time.deltaTime);
            caracol.position = new Vector3(newX, startY, caracol.position.z);
            yield return null;
        }

        // Asegurarse de que el caracol esté exactamente en la posición del objetivo si el objetivo aún existe
        if (gusanoActiveTarget != null)
        {
            caracol.position = new Vector3(targetX, startY, caracol.position.z);

            // Iniciar el diálogo automáticamente
            caracolScript.StartDialogue();
        }

        // Reactivar el control manual
        caracolScript.SetAutomaticMovement(false);
    }


}



