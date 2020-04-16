using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    [Header("Player Inputs")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string turnAxis = "Mouse X";
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode rollKey = KeyCode.Tab;

    [Header("Camera")]
    public Transform target;
    [HideInInspector] public float h;
    [HideInInspector] public float v;
    [HideInInspector] public float t;
    public bool sprint = false;

    private CharacterMovement cm;

    private void Awake() {
        cm = GetComponent<CharacterMovement>();
    }
    

    // Update is called once per frame
    void Update()
    {
        sprint = (Input.GetKey(sprintKey)) ? true : false;    

        if(sprint)
        {
            if(Input.GetKeyDown(rollKey)) cm.RollTrigger();
        }     

        h = Input.GetAxis(horizontalAxis);
        v = Input.GetAxis(verticalAxis);
        t = Input.GetAxis(turnAxis);   
    }
}
