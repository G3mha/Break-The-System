using UnityEngine;
using UnityEngine.InputSystem;

public class Arm : MonoBehaviour
{
    private GameObject player;
    private PlayerInput input;
    private Player playerScript;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        input = GameObject.FindGameObjectWithTag("input").GetComponent<PlayerInput>();
        input.actions["Aim"].started += OnAimEnter;
        input.actions["Aim"].canceled += OnAimExit;
        input.actions["AimUp"].started += OnAimUpEnter;
        input.actions["AimUp"].canceled += OnAimUpExit;
        input.actions["AimDown"].started += OnAimDownEnter;
        input.actions["AimDown"].canceled += OnAimDownExit;
    }

    void OnAimEnter(InputAction.CallbackContext context)
    {
        AimAtAngle(30f);
    }

    void OnAimUpEnter(InputAction.CallbackContext context)
    {
        AimAtAngle(45f);
    }

    void OnAimDownEnter(InputAction.CallbackContext context)
    {
        AimAtAngle(-45f);
    }

    void OnAimExit(InputAction.CallbackContext context)
    {
        AimAtAngle(-30f);
    }

    void OnAimUpExit(InputAction.CallbackContext context)
    {
        AimAtAngle(-45f);
    }

    void OnAimDownExit(InputAction.CallbackContext context)
    {
        AimAtAngle(45f);
    }

    void AimAtAngle(float angle)
    {
        if(!playerScript.getIsDead()){
            transform.Rotate(new Vector3(0f,0f,angle));
        }
    }

    public void Deactivate(){
        gameObject.SetActive(false);
    }

    public void UnsubscribeInput()
    {
        input.actions["Aim"].started -= OnAimEnter;
        input.actions["Aim"].canceled -= OnAimExit;
        input.actions["AimUp"].started -= OnAimUpEnter;
        input.actions["AimUp"].canceled -= OnAimUpExit;
        input.actions["AimDown"].started -= OnAimDownEnter;
        input.actions["AimDown"].canceled -= OnAimDownExit;
    }

    private void OnDestroy()
    {
        if (input != null) {
            input.actions["Aim"].started -= OnAimEnter;
            input.actions["Aim"].canceled -= OnAimExit;
            input.actions["AimUp"].started -= OnAimUpEnter;
            input.actions["AimUp"].canceled -= OnAimUpExit;
            input.actions["AimDown"].started -= OnAimDownEnter;
            input.actions["AimDown"].canceled -= OnAimDownExit;
        }
    }

    public void SubscribeInput()
    {
        input.actions["Aim"].started += OnAimEnter;
        input.actions["Aim"].canceled += OnAimExit;
        input.actions["AimUp"].started += OnAimUpEnter;
        input.actions["AimUp"].canceled += OnAimUpExit;
        input.actions["AimDown"].started += OnAimDownEnter;
        input.actions["AimDown"].canceled += OnAimDownExit;
    }
}
