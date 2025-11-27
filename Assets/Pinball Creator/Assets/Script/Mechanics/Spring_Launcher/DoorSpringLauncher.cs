// DoorSpringLauncher : Description : Open and lock the door to the spring launcher

using UnityEngine;

public class DoorSpringLauncher : MonoBehaviour
{
    #region --- Exposed Fields ---

    public bool b_Exit = true;

    public GameObject obj_Door;

    #endregion

    #region --- Callbacks ---

    private void OnTriggerEnter(Collider other)
    {
        // Function used by Object "Anti_Bug" if the ball go back to the spring launcher
        if (other.tag == "Ball" && !b_Exit)
        {
            // Open the door

            obj_Door.transform.localPosition = new Vector3(
                obj_Door.transform.localPosition.x,
                -1,
                obj_Door.transform.localPosition.z
            );

            obj_Door.GetComponent<Collider>().isTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Function use by the Door_Exit object;
        if (other.tag == "Ball" && b_Exit)
        {
            // Lock the door

            obj_Door.transform.localPosition = new Vector3(
                obj_Door.transform.localPosition.x,
                0,
                obj_Door.transform.localPosition.z
            );

            GetComponent<Collider>().isTrigger = false;
        }
    }

    #endregion
}