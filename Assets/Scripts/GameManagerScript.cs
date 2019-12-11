using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

	public static GameManagerScript instance;

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

	public GameObject turretPrefab;
	public GameObject barrackPrefab;
	public GameObject satellitePrefab;
	public GameObject unitPrefab;

	public List<GameObject> turretPool = new List<GameObject>();
	public List<GameObject> barrackPool = new List<GameObject>();
	public List<GameObject> satellitePool = new List<GameObject>();
	public List<GameObject> unitPool = new List<GameObject>();

	public List<UnitScript> unitList = new List<UnitScript>();
	public List<UnitScript> enemyList = new List<UnitScript>();
	public PlatformScript[] platforms;

	public GameObject BuildingUI;
	public TurretScript currBuilding;

	public bool isPaused = false;
	public GameObject pauseGO;

	// Use this for initialization
	void Start () {

		for(int i = 0; i < 15; i++)
		{
			GameObject turretGO = Instantiate(turretPrefab, transform.position, Quaternion.identity);
			turretGO.SetActive(false);
			turretPool.Add(turretGO);
		}

		for(int i = 0; i < 15; i++)
		{
			GameObject barrackGO = Instantiate(barrackPrefab, transform.position, Quaternion.identity);
			barrackGO.SetActive(false);
			barrackPool.Add(barrackGO);
		}

		for(int i = 0; i < 15; i++)
		{
			GameObject satelliteGO = Instantiate(satellitePrefab, transform.position, Quaternion.identity);
			satelliteGO.SetActive(false);
			satellitePool.Add(satelliteGO);
		}

		for(int i = 0; i < 30; i++)
		{
			GameObject unitGO = Instantiate(unitPrefab, platforms[1].transform.position, Quaternion.identity);
			unitGO.SetActive(false);
			unitPool.Add(unitGO);
		}

		GameObject[] units = GameObject.FindGameObjectsWithTag("Player");

		foreach(GameObject unit in units)
		{
			unitList.Add(unit.GetComponent<UnitScript>());
		}
			
		for(int i = 0; i < platforms.Length; i++)
		{
			platforms[i].UpdateBuildings();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void BuildTurret()
	{
		if(!currBuilding.isAlive)
		{
			currBuilding.myType = BuildingType.TURRET;
			currBuilding.myBase.UpdateBuildings();
			CloseUI();
		}

	}

	public void BuildBarrack()
	{
		if(!currBuilding.isAlive)
		{
			currBuilding.myType = BuildingType.BARRACK;
			currBuilding.myBase.UpdateBuildings();
			CloseUI();
		}

	}

	public void BuildSatellite()
	{
		if(!currBuilding.isAlive)
		{
			currBuilding.myType = BuildingType.SATELLITE;
			currBuilding.myBase.UpdateBuildings();
			CloseUI();
		}

	}

	public void CloseUI()
	{
		BuildingUI.SetActive(false);
		currBuilding = null;
	}

	public void PauseGame()
	{
		if(!isPaused)
		{
			pauseGO.SetActive(true);
			isPaused = true;
			Time.timeScale = 0f;
		}
		else
		{
			pauseGO.SetActive(false);
			isPaused = false;
			Time.timeScale = 1f;
		}

	}

	public void ResetScene()
	{
		SceneManager.LoadScene("RTSScene");
	}
}
