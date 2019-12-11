using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour {

	public bool selected = false;
	public Renderer objRenderer;
	public Vector3 targetPos;
	public TurretScript enemy;
	public bool canMove;
	NavMeshAgent navMeshAgent;
	public Material myMat;
	public GameObject target;
	public float range = 4f;
	float fireRate = 3f;
	float fireCountdown;
	public int damage = 10;
	public bool friendly = false;

	public int health;
	int maxHealth;
	public Transform healthBarTransform;
	public Image healthBarImage;

	void Start()
	{
		objRenderer = GetComponent<Renderer>();
		myMat = objRenderer.material;
		fireCountdown = 1f / fireRate;
		maxHealth = health;
		navMeshAgent = GetComponent<NavMeshAgent>();
		StartCoroutine(RepathRoutine());
	}

	// Update is called once per frame
	void Update () 
	{
		if(GameManagerScript.instance.isPaused)
		{
			return;
		}

		healthBarImage.fillAmount = (float)health / (float)maxHealth;

		healthBarTransform.LookAt(healthBarTransform.position + Camera.main.transform.rotation * Vector3.back,
			Camera.main.transform.rotation * Vector3.down);

		if(objRenderer.isVisible && Input.GetMouseButton(0) && friendly && (Mathf.Abs(CameraOperatorScript.selectionbox.width) > 0.5f || Mathf.Abs(CameraOperatorScript.selectionbox.height) > 0.5f))
		{
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraOperatorScript.InvertMouseY(camPos.y);
			selected = CameraOperatorScript.selectionbox.Contains(camPos);

			if(selected)
			{
				objRenderer.material.color = Color.red;
				if(!CameraOperatorScript.instance.selectedUnitList.Contains(this))
				{
					CameraOperatorScript.instance.selectedUnitList.Add(this);
				}
			}
			else
			{
				if(CameraOperatorScript.instance.selectedUnitList.Contains(this))
				{
					CameraOperatorScript.instance.selectedUnitList.Remove(this);
				}
				objRenderer.material.color = Color.white;
			}
		}


		if(CameraOperatorScript.instance.inputState == TouchInputState.UNITSELECT)
		{
			if(friendly)
			{
				if(Input.GetMouseButtonDown(0))
				{
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

					if(Physics.Raycast(ray, out hit))
					{
						if(hit.transform == this.transform)
						{
							for(int i = 0; i < CameraOperatorScript.instance.selectedUnitList.Count; i++)
							{
								CameraOperatorScript.instance.selectedUnitList[i].selected = false;

								CameraOperatorScript.instance.selectedUnitList[i].objRenderer.material.color = Color.white;
							}

							CameraOperatorScript.instance.selectedUnitList.Clear();

							if(!selected)
							{
								selected = true;
								objRenderer.material.color = Color.red;

								CameraOperatorScript.instance.selectedUnitList.Add(this);

							}
						}
					}
				}
			}
		}

		fireCountdown -= Time.deltaTime;

		if(target != null)
		{
			if(enemy == null)
			{
				enemy = target.GetComponent<TurretScript>();
			}

			if(!enemy.isAlive)
			{
				target = null;
				enemy = null;
				return;
			}

			targetPos = target.transform.position;

			if(fireCountdown <= 0f)
			{
				fireCountdown = 1f / fireRate;
				if(Vector3.Distance(transform.position, targetPos) <= range)
				{
					navMeshAgent.isStopped = true;
					canMove = false;
					StopCoroutine(RepathRoutine());

					enemy.TakeDamage(damage);
				}
			}

		}

	}

	public void TakeDamage(int damage)
	{
		health -= damage;

		myMat.color = Color.white;

		if(health <= 0)
		{
			if(friendly)
			{
				GameManagerScript.instance.unitList.Remove(this);
				gameObject.SetActive(false);
			}
			else
			{
				GameManagerScript.instance.enemyList.Remove(this);
				gameObject.SetActive(false);
			}
		}

		Invoke("ResetColour", 1);
	}

	void ResetColour()
	{
		if(friendly)
		{
			myMat.color = Color.white;

		}
		else
		{
			myMat.color = Color.blue;

		}
	}

	public void MoveUnit()
	{
		StopCoroutine(RepathRoutine());
		canMove = true;
		StartCoroutine(RepathRoutine());
	}

	IEnumerator RepathRoutine()
	{
		while(canMove)
		{
			yield return new WaitForSeconds(0.5f);
			if(navMeshAgent.isStopped) navMeshAgent.isStopped = false;
			navMeshAgent.SetDestination(targetPos);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}

	void OnMouseDown()
	{

		//		if(Input.touches[0].phase == TouchPhase.Began)
		//		{

		//		for(int i = 0; i < CameraOperatorScript.instance.selectedUnitList.Count; i++)
		//		{
		//			CameraOperatorScript.instance.selectedUnitList[i].selected = false;
		//
		//			CameraOperatorScript.instance.selectedUnitList[i].objRenderer.material.color = Color.white;
		//		}
		//
		//		CameraOperatorScript.instance.selectedUnitList.Clear();
		//
		////		}
		//
		//		if(!selected)
		//		{
		//			selected = true;
		//			objRenderer.material.color = Color.red;
		//
		//		}
		//		else
		//		{
		//			selected = false;
		//			objRenderer.material.color = Color.white;
		//		}


	}

	void OnMouseUp()
	{
		//		if(selected)
		//		{
		//			if(!CameraOperatorScript.instance.selectedUnitList.Contains(this))
		//			{
		//				CameraOperatorScript.instance.selectedUnitList.Add(this);
		//			}
		//		}
		//		else
		//		{
		//			if(CameraOperatorScript.instance.selectedUnitList.Contains(this))
		//			{
		//				CameraOperatorScript.instance.selectedUnitList.Remove(this);
		//			}
		//		}
	}
}
