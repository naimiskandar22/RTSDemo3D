using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TouchInputState
{
	UNITSELECT,
	UNITMOVE,
	TOTAL,
}

public class CameraOperatorScript : MonoBehaviour {

	public static CameraOperatorScript instance;

	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	float baseSpeed = 0.1f;
	public TouchInputState inputState;

	public Texture2D boxHighlight;
	public static Rect selectionbox = new Rect(0, 0, 0, 0);
	public Vector3 startClick = -Vector3.one;
	public List<UnitScript> selectedUnitList = new List<UnitScript>();
	public bool isDragging = false;

	// Use this for initialization
	void Start () {
		inputState = TouchInputState.UNITMOVE;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameManagerScript.instance.isPaused)
		{
			return;
		}

		if(inputState == TouchInputState.UNITSELECT)
		{
			TouchPinch();
		}

		if(inputState == TouchInputState.UNITMOVE)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
				{
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

					if(Physics.Raycast(ray, out hit))
					{
						if(hit.transform.gameObject.isStatic)
						{
							for(int i = 0; i < selectedUnitList.Count; i++)
							{
								selectedUnitList[i].targetPos = hit.point;
								selectedUnitList[i].canMove = true;

								selectedUnitList[i].MoveUnit();

								Collider[] hitColliders = Physics.OverlapSphere(hit.point, selectedUnitList[i].range);

								for(int j = 0; j < hitColliders.Length; j++)
								{
									if(hitColliders[j] != null)
									{
										TurretScript target = hitColliders[j].GetComponent<TurretScript>();

										if(target != null)
										{
											selectedUnitList[i].target = target.gameObject;
										}
									}
								}

							}
						}
					}
				}
			}
		}
	}

	void LateUpdate()
	{
		if(inputState != TouchInputState.UNITSELECT)
		{
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
				transform.Translate(-touchDeltaPosition.x * baseSpeed, -touchDeltaPosition.y * baseSpeed, 0);
			}
		}
	}

	void CheckCamera()
	{
		if(Input.GetMouseButtonDown(0))
		{
			for(int i = 0; i < selectedUnitList.Count; i++)
			{
				selectedUnitList[i].selected = false;

				selectedUnitList[i].objRenderer.material.color = Color.white;
			}

			selectedUnitList.Clear();
			startClick = Input.mousePosition;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			startClick = -Vector3.one;
		}

		if(Input.GetMouseButton(0))
		{
			selectionbox = new Rect(startClick.x, InvertMouseY(startClick.y), 
				Input.mousePosition.x - startClick.x, 
				InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));

			if(selectionbox.width < 0)
			{
				selectionbox.x += selectionbox.width;
				selectionbox.width = -selectionbox.width;
			}
			if(selectionbox.height < 0)
			{
				selectionbox.y += selectionbox.height;
				selectionbox.height = -selectionbox.height;
			}
		}

	}

	void TouchPinch()
	{
		if(Input.touchCount == 2)
		{
			if(!isDragging) isDragging = true;

			if(Input.touches[0].phase == TouchPhase.Began)
			{
				for(int i = 0; i < selectedUnitList.Count; i++)
				{
					selectedUnitList[i].selected = false;

					selectedUnitList[i].objRenderer.material.color = Color.white;
				}

				selectedUnitList.Clear();
				startClick = Input.mousePosition;
			}

			Vector2 point2;

			startClick = Input.touches[0].position;

			point2 = Input.touches[1].position;

			selectionbox = new Rect(startClick.x, InvertMouseY(startClick.y), 
				point2.x - startClick.x, 
				InvertMouseY(point2.y) - InvertMouseY(startClick.y));

			if(selectionbox.width < 0)
			{
				selectionbox.x += selectionbox.width;
				selectionbox.width = -selectionbox.width;
			}
			if(selectionbox.height < 0)
			{
				selectionbox.y += selectionbox.height;
				selectionbox.height = -selectionbox.height;
			}
		}
		else
		{
			isDragging = false;
			startClick = -Vector3.one;
			selectionbox = new Rect(0, 0, 0, 0);
		}
	}

	void OnGUI()
	{
		if(startClick != -Vector3.one)
		{
			GUI.color = new Color(1, 1, 1, 0.5f);
			GUI.DrawTexture(selectionbox, boxHighlight);
		}
	}

	public static float InvertMouseY(float yPos)
	{
		return Screen.height - yPos;
	}

	public void UnitSelectPhase()
	{
		inputState = TouchInputState.UNITSELECT;
	}

	public void UnitMovePhase()
	{
		inputState = TouchInputState.UNITMOVE;
	}
}
