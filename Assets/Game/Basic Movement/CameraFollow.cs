using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
	public GameObject CameraFollowObj;
	public GameObject CameraObj;

	[Header("Camera Properties")]
	public float CameraMoveSpeed = 120.0f;
	public float clampAngle = 80.0f;
	[Range(10f, 200f)] public float inputSensitivity = 150.0f;
	public float minDistance = 1.0f;
	public float maxDistance = 4.0f;
	public float smooth = 10.0f;
	Vector3 dollyDir;
	public float distance;

	[Header("Preferences")]
	[SerializeField] private bool invertX = false;
	[SerializeField] private bool invertY = false;
	
	private float mouseX;
	private float mouseY;
	
	private float rotY = 0.0f;
	private float rotX = 0.0f;


	void Awake () {
		dollyDir = CameraObj.transform.localPosition.normalized;
		distance = CameraObj.transform.localPosition.magnitude;
	}

	// Use this for initialization
	void Start () {
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;
		
		LockCursor();
	}

	void LockCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;		
	}

	void Update()
	{
		InputUpdate();
		CollisionUpdate();
	}
	
	// Update is called once per frame
	void InputUpdate () {

		mouseX = Input.GetAxis ("Mouse X");
		mouseY = Input.GetAxis ("Mouse Y");

		rotY += invertX ? -mouseX * inputSensitivity * Time.deltaTime : mouseX * inputSensitivity * Time.deltaTime;
		rotX += invertY ? -mouseY * inputSensitivity * Time.deltaTime : mouseY * inputSensitivity * Time.deltaTime;

		rotX = Mathf.Clamp (rotX, -clampAngle, clampAngle);

		Quaternion localRotation = Quaternion.Euler (rotX, rotY, 0.0f);
		transform.rotation = localRotation;
	}

	void CollisionUpdate () {

		Vector3 desiredCameraPos = CameraObj.transform.parent.TransformPoint (dollyDir * maxDistance);
		RaycastHit hit;

		if (Physics.Linecast (CameraObj.transform.parent.position, desiredCameraPos, out hit)) {
			distance = Mathf.Clamp ((hit.distance * 0.87f), minDistance, maxDistance);
				
				} else {
					distance = maxDistance;
				}

				CameraObj.transform.localPosition = Vector3.Lerp (CameraObj.transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
	}


	void LateUpdate () {
		CameraUpdate ();
	}

	void CameraUpdate() {
		Transform target = CameraFollowObj.transform;

		float step = CameraMoveSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, target.position, step);
	}
}
