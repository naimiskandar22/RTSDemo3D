using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformScript : MonoBehaviour {

	public Image redBar;
	public Image greenBar;
	public float currControl;
	float maxControl = 100f;

	public bool friendly = false;
	public List<TurretScript> myBuildings = new List<TurretScript>();
	public float turretRange = 5f;

	// Use this for initialization
	void Start () 
	{
		greenBar.fillAmount = 0f / 100f;

		if(currControl < 0)
		{
			redBar.fillAmount = (-1f * currControl) / 100f;
		}
		else
		{
			greenBar.fillAmount = currControl / 100f;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameManagerScript.instance.isPaused)
		{
			return;
		}

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3.4f);

		for(int j = 0; j < hitColliders.Length; j++)
		{
			if(hitColliders[j] != null)
			{
				UnitScript unit = hitColliders[j].GetComponent<UnitScript>();

				if(unit != null)
				{
					if(unit.friendly)
					{
						if(currControl < maxControl)
						{
							currControl += 0.1f;

							if(currControl >= maxControl)
							{
								if(!friendly)
								{
									friendly = true;
									UpdateBuildings();
								}
							}
						}
						else
						{
							currControl = maxControl;
						}
					}
					else
					{
						if(currControl > -100f)
						{
							currControl -= 0.1f;

							if(currControl < -100f)
							{
								currControl = -100f;

								if(friendly)
								{
									friendly = false;
									UpdateBuildings();
								}
							}
						}
						else
						{
							currControl = -100f;
						}
					}

					if(currControl < 0)
					{
						redBar.fillAmount = (-1f * currControl) / 100f;
					}
					else
					{
						greenBar.fillAmount = currControl / maxControl;
					}
				}
			}
		}

	}

	public void UpdateBuildings()
	{
		int range = 0;

		for(int i = 0; i < myBuildings.Count; i++)
		{
			if(myBuildings[i].myBase == null)
			{
				myBuildings[i].myBase = this;
			}
            
			if(friendly)
			{
				myBuildings[i].hostileTurret = false;
			}
			else
			{
				myBuildings[i].hostileTurret = true;
			}

			if(myBuildings[i].myType != BuildingType.NONE)
			{
				if(myBuildings[i].currBuilding == null)
				{
					myBuildings[i].GetBuildingGO();
				}
			}

			if(myBuildings[i].myType == BuildingType.SATELLITE)
			{
				range++;
			}

			if(myBuildings[i].myType != BuildingType.NONE)
			{
				myBuildings[i].clickMeGO.SetActive(false);
			}
			else
			{
				myBuildings[i].isAlive = false;

				if(myBuildings[i].hostileTurret)
				{
					myBuildings[i].clickMeGO.SetActive(false);
				}
			}

			myBuildings[i].UpdateMat();
		}

		turretRange = 5f + range*2f;

		for(int i = 0; i < myBuildings.Count; i++)
		{
			if(myBuildings[i].myType == BuildingType.TURRET)
			{
				myBuildings[i].fireRate = 2f;
				myBuildings[i].range = turretRange;
			}
			else if(myBuildings[i].myType == BuildingType.BARRACK)
			{
				myBuildings[i].fireRate = 0.02f;
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, 3.4f);
	}
}
