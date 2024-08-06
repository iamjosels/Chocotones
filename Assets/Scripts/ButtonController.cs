using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] GameObject door;

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //CloseDoor();
        }
    }

    private void OpenDoor()
    {
        Debug.Log("Puerta abierta");
        door.transform.position = door.transform.position + new Vector3(0, 2, 0);
        // Aquí puedes activar o desactivar la puerta, moverla, etc.
        //door.SetActive(false); // Desactiva la puerta para abrirla
    }

    private void CloseDoor()
    {
        Debug.Log("Puerta cerrada");
        // Aquí puedes revertir la acción de abrir la puerta
        //door.SetActive(true); // Activa la puerta para cerrarla
    }
}

