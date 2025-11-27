using UnityEngine;

public class PlungerBallDetector : MonoBehaviour
{
    #region --- Exposed Fields ---

    public bool Ball_Collision;

    // PlungerBallDetector.js : Description : Use to know if a ball is on the launcher
    public GameObject obj_Spring;
    public Rigidbody rb_Ball;
    public SpringLauncher spring_Launcher;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        spring_Launcher = obj_Spring.GetComponent<SpringLauncher>();
    }

    #endregion

    #region --- Methods ---

    public bool ReturnCollision()
    {
        return Ball_Collision;
    }

    #endregion

    #region --- Callbacks ---

    private void OnCollisionExit(Collision collision)
    {
        // Ball exit the launcher
        if (collision.transform.tag == "Ball")
        {
            //Debug.Log(collision.transform.name);
            rb_Ball = null;
            spring_Launcher.BallOnPlunger(rb_Ball);
            Ball_Collision = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Ball is on the launcher
        if (collision.transform.tag == "Ball")
        {
            rb_Ball = collision.transform.GetComponent<Rigidbody>();
            spring_Launcher.BallOnPlunger(rb_Ball);
            Ball_Collision = true;
        }
    }

    #endregion
}