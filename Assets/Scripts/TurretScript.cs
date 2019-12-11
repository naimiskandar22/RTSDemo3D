using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum BuildingType
{
	NONE,
	TURRET,
	SATELLITE,
	BARRACK,
	TOTAL
};

public class TurretScript : MonoBehaviour {

	public PlatformScript myBase;
	public Transform spawnPoint;
	public Transform rallyPoint;

	public float range = 5f;
	public bool hostileTurret = true;
	public Transform target;

	public BuildingType myType;
	public bool isAlive = true;
	public GameObject currBuilding;

	public Transform turretHead;
	public float turretRotateSpeed = 5f;
	public Material headMat;
	public Material bodyMat;
	public int damage = 2;
	public int health;
	int maxHealth;
	public float fireRate = 2f;
	public float fireCountdown;

	public Transform healthBarTransform;
	public Image healthBarImage;
	public Transform clickMeTransform;
	public GameObject clickMeGO;


	void Awake()
	{
		clickMeGO = transform.Find("ClickMeCanvas").gameObject;
		clickMeTransform = transform.Find("ClickMeCanvas").transform;
	}

	// Use this for initialization
	void Start () 
	{
		health = 100;
		maxHealth = health;
		InvokeRepeating("FindTarget", 0f, 0.5f);
		fireCountdown = 1f / fireRate;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameManagerScript.instance.isPaused)
		{
			return;
		}

		if(!isAlive)
		{
			clickMeTransform.LookAt(clickMeTransform.position + Camera.main.transform.rotation * Vector3.back,
				Camera.main.transform.rotation *Vector3.down);
		}
		else
		{
			fireCountdown -= Time.deltaTime;

			healthBarTransform.LookAt(healthBarTransform.position + Camera.main.transform.rotation * Vector3.back,
				Camera.main.transform.rotation *Vector3.down);

			if(myType == BuildingType.TURRET)
			{
				if(target != null)
				{
					Vector3 dir = target.position - transform.position;
					Quaternion lookRotation = Quaternion.LookRotation(dir);
					Vector3 rotation = Quaternion.Lerp(turretHead.rotation, lookRotation, Time.deltaTime * turretRotateSpeed).eulerAngles;
					turretHead.rotation = Quaternion.Euler(0f, rotation.y, 0f);

					if(fireCountdown <= 0f)
					{
						fireCountdown = 1f / fireRate;

						Collider[] hitColliders = Physics.OverlapSphere(target.position, 1f);

						for(int j = 0; j < hitColliders.Length; j++)
						{
							if(hitColliders[j] != null)
							{
								UnitScript unit = hitColliders[j].GetComponent<UnitScript>();

								if(unit != null)
								{
									if(unit.target == null)
									{
										if(hostileTurret && unit.friendly)
										{
											unit.target = this.gameObject;
											unit.TakeDamage(damage);
										}
										else if(!hostileTurret && !unit.friendly)
										{
											unit.target = this.gameObject;
											unit.TakeDamage(damage);
										}
									}
								}

							}
						}
					}
				}
			}
			else if(myType == BuildingType.BARRACK)
			{
				if(fireCountdown <= 0f)
				{
					fireCountdown = 1f / fireRate;
                    

					for(int i = 0; i < GameManagerScript.instance.unitPool.Count; i++)
					{
						if(GameManagerScript.instance.unitPool[i].activeSelf == false)
						{

							UnitScript unitScript = GameManagerScript.instance.unitPool[i].GetComponent<UnitScript>();

							if(hostileTurret)
							{
								unitScript.friendly = false;
								GameManagerScript.instance.enemyList.Add(unitScript);
								if(unitScript.myMat != null)
								{
									unitScript.myMat.color = Color.blue;
								}
								else
								{
									unitScript.GetComponent<Renderer>().material.color = Color.blue;
								}

							}
							else
							{
								unitScript.friendly = true;
								GameManagerScript.instance.unitList.Add(unitScript);
								if(unitScript.myMat != null)
								{
									unitScript.myMat.color = Color.red;
								}
								else
								{
									unitScript.GetComponent<Renderer>().material.color = Color.red;
								}
							}

							unitScript.targetPos = rallyPoint.position;

							unitScript.canMove = true;

							GameManagerScript.instance.unitPool[i].transform.position = spawnPoint.position + new Vector3(0f, 1f, 0f);

							GameManagerScript.instance.unitPool[i].SetActive(true);

							unitScript.MoveUnit();

							break;
						}
					}
				}
			}
		}
	}

	public void TakeDamage(int damage)
	{
		health -= damage;

		healthBarImage.fillAmount = (float)health / (float)maxHealth;

		if(health <= 0)
		{
			isAlive = false;
			myType = BuildingType.NONE;
			if(!hostileTurret)
			{
				clickMeGO.SetActive(true);
			}

			DiscardGO();

			myBase.UpdateBuildings();
		}
	}

	public void UpdateMat()
	{
		if(myType != BuildingType.NONE)
		{
			if(healthBarTransform == null)
			{
				healthBarTransform = currBuilding.transform.Find("HealthCanvas").transform;
				healthBarImage = healthBarTransform.Find("HealthBar").GetComponent<Image>();
			}
		}
		else
		{
			if(!hostileTurret)
			{
				clickMeGO.SetActive(true);
			}
			else
			{
				clickMeGO.SetActive(false);
			}
		}

		if(myType == BuildingType.TURRET)
		{
			if(bodyMat == null || headMat == null)
			{
				bodyMat = currBuilding.transform.Find("Body").GetComponent<Renderer>().material;
				headMat = currBuilding.transform.Find("Head").GetComponent<Renderer>().material;
				turretHead = currBuilding.transform.Find("Head").transform;
			}

			if(hostileTurret)
			{
				bodyMat.color = Color.red;
				headMat.color = Color.red;
			}
			else
			{
				bodyMat.color = Color.white;
				headMat.color = Color.white;
			}
		}
		else if(myType == BuildingType.BARRACK)
		{
			if(bodyMat == null || headMat == null)
			{
				bodyMat = currBuilding.transform.Find("Body").GetComponent<Renderer>().material;
				headMat = currBuilding.transform.Find("Head").GetComponent<Renderer>().material;
			}

			if(hostileTurret)
			{
				bodyMat.color = Color.red;
				headMat.color = Color.red;

			}
			else
			{
				bodyMat.color = Color.white;
				headMat.color = Color.white;
			}
		}
		else if(myType == BuildingType.SATELLITE)
		{
			if(bodyMat == null || headMat == null)
			{
				bodyMat = currBuilding.transform.Find("Body").GetComponent<Renderer>().material;
			}

			if(hostileTurret)
			{
				bodyMat.color = Color.red;

			}
			else
			{
				bodyMat.color = Color.white;
			}
		}

	}

	public void GetBuildingGO()
	{
		if(myType == BuildingType.NONE)
		{
			return;
		}

		if(myType == BuildingType.TURRET)
		{
			for(int i = 0; i < GameManagerScript.instance.turretPool.Count; i++)
			{
				if(GameManagerScript.instance.turretPool[i].activeSelf == false)
				{
					currBuilding = GameManagerScript.instance.turretPool[i];
					currBuilding.transform.parent = this.transform;
					currBuilding.transform.localPosition = Vector3.zero;

					break;
				}
			}
		}
		else if(myType == BuildingType.BARRACK)
		{
			for(int i = 0; i < GameManagerScript.instance.barrackPool.Count; i++)
			{
				if(GameManagerScript.instance.barrackPool[i].activeSelf == false)
				{
					currBuilding = GameManagerScript.instance.barrackPool[i];
					currBuilding.transform.parent = this.transform;
					currBuilding.transform.localPosition = Vector3.zero;

					break;
				}
			}
		}
		else if(myType == BuildingType.SATELLITE)
		{
			for(int i = 0; i < GameManagerScript.instance.satellitePool.Count; i++)
			{
				if(GameManagerScript.instance.satellitePool[i].activeSelf == false)
				{
					currBuilding = GameManagerScript.instance.satellitePool[i];
					currBuilding.transform.parent = this.transform;
					currBuilding.transform.localPosition = Vector3.zero;

					break;
				}
			}
		}

		isAlive = true;
		currBuilding.SetActive(true);
		UpdateMat();

		maxHealth = 100;
		health = maxHealth;
		healthBarImage.fillAmount = (float)health / (float)maxHealth;
	}

	public void DiscardGO()
	{
		currBuilding.SetActive(false);
		bodyMat = null;
		headMat = null;
		currBuilding = null;
	}

	void FindTarget()
	{
		float shortestDistance = Mathf.Infinity;
		GameObject nearestEnemy = null;

		if(hostileTurret)
		{
			for(int i = 0; i < GameManagerScript.instance.unitList.Count; i++)
			{
				float distanceToEnemy = Vector3.Distance(transform.position, GameManagerScript.instance.unitList[i].transform.position);
				if(distanceToEnemy < shortestDistance)
				{
					shortestDistance = distanceToEnemy;
					nearestEnemy = GameManagerScript.instance.unitList[i].gameObject;
				}

				if(nearestEnemy != null && shortestDistance <= range)
				{
					target = nearestEnemy.transform;
				}
				else if(nearestEnemy != null && shortestDistance > range)
				{
					target = null;
				}
			}
		}
	}

	public void OnMouseDown()
	{
		if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
		{
			if(!isAlive && !hostileTurret)
			{
				GameManagerScript.instance.BuildingUI.SetActive(true);
				GameManagerScript.instance.currBuilding = this;
			}
		}

	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}

}
