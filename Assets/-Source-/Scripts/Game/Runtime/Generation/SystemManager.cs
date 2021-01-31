using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    public class SystemManager : PersistentLazySingleton<SystemManager>
    {
        #region Variables

        #region Public

        public delegate void PlayerExitedSystemBounds();
        public PlayerExitedSystemBounds onPlayerExitedSystemBounds;

        public delegate void PlayerEnteredSystemBounds();
        public PlayerExitedSystemBounds onPlayerEnteredSystemBounds;

        #endregion

        #region Private


        #region Exposed

        //EWWWW
        [SerializeField] private GameObject circleDrawerPrefab;

        [SerializeField] private List<GameObject> planets;
        [SerializeField] private GameObject asteroid;
        [SerializeField] private GameObject enemy;

        [SerializeField] private int _levelRadius = 50;
        [SerializeField] private int minimumObjectDistance = 3;
        [SerializeField] private int minimumOrbitDistance = 12;
        [SerializeField] private int enemySpawnRadius = 2;

        [SerializeField] private int enemySpawnChance = 50;
        [SerializeField] private int planetSpawnChance = 60;

        [SerializeField] private int minNeutroniumAsteroids = 3, maxNeutroniumAsteroids = 6;
        [SerializeField] private int minAsteroids = 5, maxAsteroids = 16;

        [SerializeField] private int minPlanets = 0, maxPlanets = 4;
        [SerializeField] private int minEnemies = 0, maxEnemies = 5;

        #endregion

        private int retryAttempts = 10;

        private struct PlanetOrbit
        {
            public CircleDrawer drawer;
            public int radius;

            public PlanetOrbit(CircleDrawer _drawer, int _radius)
            {
                drawer = _drawer;
                radius = _radius;
            }

            public void Draw()
            {
                drawer.DrawCircle(radius);
            }
        }

        private List<PlanetOrbit> planetOrbits = new List<PlanetOrbit>();

        private bool playerIsOutside;
        private CircleDrawer borderDrawer;
        private int currentRadius;

        #endregion

        #endregion

        #region Methods

        #region Unity

        private void Start()
        {
            borderDrawer = Instantiate(circleDrawerPrefab, transform).GetComponent<CircleDrawer>();
            borderDrawer.Setup(.2f, Color.green);
        }

        void Update()
        {
            CheckIfOutside(PlayerController.Instance.transform.position);
            borderDrawer.DrawCircle(currentRadius);

            foreach(PlanetOrbit orbit in planetOrbits)
            {
                orbit.Draw();
            }
        }

        #endregion

        #region Public

        public void GenerateLevel()
        {
            maxPlanets = (_levelRadius - 45) / minimumOrbitDistance + 1;
            
            if(minPlanets > maxPlanets)
            {
                minPlanets = maxPlanets - 1;
            }

            //Todo: calculate if a starbase spawns
            int requiredAsteroids = Random.Range(minAsteroids, maxAsteroids);
            int requiredNeutronium = Random.Range(minNeutroniumAsteroids, maxNeutroniumAsteroids);
            int requiredPlanets = Random.Range(minPlanets, maxPlanets);
            int requiredEnemies = Random.Range(minEnemies, maxEnemies);

            GenerateLevel(_levelRadius, requiredAsteroids, requiredNeutronium, requiredPlanets, requiredEnemies);
        }


        public void GenerateLevel(int levelRadius, int requiredAsteroids, int requiredNeutronium, int requiredPlanets, int requiredEnemies)
        {
            

            if (levelRadius == -1) levelRadius = _levelRadius;
            if (requiredAsteroids == -1) requiredAsteroids = Random.Range(minAsteroids, maxAsteroids);
            if (requiredNeutronium == -1) requiredNeutronium = Random.Range(minNeutroniumAsteroids, maxNeutroniumAsteroids);
            if (requiredPlanets == -1) requiredPlanets = Random.Range(minPlanets, maxPlanets);
            if (requiredEnemies == -1) requiredEnemies = Random.Range(minEnemies, maxEnemies);

            if ((levelRadius - 45) / minimumOrbitDistance + 1 < requiredPlanets)
            {
                Debug.LogError("Cant fit that many planets mate.");
                requiredPlanets = (levelRadius - 45) / minimumOrbitDistance + 1;
            }

            currentRadius = levelRadius;
            SetPlayerStartPosition();

            List<GameObject> spawnedObjects = new List<GameObject>();

            #region Spawning Planets

            if (Random.Range(0, 101) <= planetSpawnChance)
            {
                for (int i = 0; i < requiredPlanets; i++)
                {
                    int orbitRadius, attempts = 0;
                    Vector3 dir;

                    do
                    {
                        attempts++;
                        dir = Random.insideUnitCircle.normalized;
                        orbitRadius = Random.Range(35, levelRadius - 10);

                        if (attempts > retryAttempts) break;
                    }
                    while (IsOrbitTooClose(orbitRadius, planetOrbits) || IsObjectTooClose(dir * orbitRadius, spawnedObjects));

                    CircleDrawer newDrawer = Instantiate(circleDrawerPrefab, transform).GetComponent<CircleDrawer>();
                    newDrawer.Setup(.1f, Color.white);

                    planetOrbits.Add(new PlanetOrbit(newDrawer, orbitRadius));


                    int planetType = Random.Range(0, planets.Count);

                    spawnedObjects.Add(Instantiate(planets[planetType], dir * orbitRadius, Quaternion.identity));
                }
            }

            #endregion

            #region Spawning Asteroids

            for (int i = 0; i < requiredAsteroids; i++)
            {
                int attempts = 0;
                Vector3 pos;
                do
                {
                    attempts++;
                    pos = Random.insideUnitCircle * levelRadius;
                    if (attempts > retryAttempts) break;
                }
                while (IsObjectTooClose(pos, spawnedObjects));

                GameObject obj = Instantiate(asteroid, pos, Quaternion.identity);
                spawnedObjects.Add(obj);

                obj.GetComponent<Asteroid>().Initialize(i < requiredNeutronium);

            }

            #endregion

            #region Spawning Enemies

            if (Random.Range(0, 101) <= enemySpawnChance)
            {
                for (int i = 0; i < requiredEnemies; i++)
                {
                    int objectToSpawnOn = Random.Range(0, spawnedObjects.Count);

                    Vector3 pos = (Random.insideUnitSphere * enemySpawnRadius) + spawnedObjects[objectToSpawnOn].transform.position;
                    pos.z = 0;
                    Instantiate(enemy, pos, new Quaternion(0, 0, Random.Range(0, 360), 0));
                }
            }

            #endregion
        }

        public void SetPlayerStartPosition()
        {
            Vector3 dir = Random.insideUnitCircle.normalized;
            PlayerController.Instance.transform.position = dir * (currentRadius - 10);
            PlayerController.Instance.transform.up = -dir;
        }

        #endregion

        #region Private

        private bool IsObjectTooClose(Vector3 obj, List<GameObject> otherObjs)
        {
            foreach(GameObject otherObj in otherObjs)
            {
                //TODO [Wybren]: factor in size of objects

                if(Vector3.Distance(obj, otherObj.transform.position) < minimumObjectDistance)
                {
                    return true;
                }

                if(Vector3.Distance(obj, PlayerController.Instance.transform.position) < minimumObjectDistance)
                {
                    //return true;
                }
            }

            return false;
        }

        private bool IsOrbitTooClose(int orbit, List<PlanetOrbit> otherOrbits)
        {
            foreach(PlanetOrbit otherOrbit in otherOrbits)
            {
                if(Mathf.Abs(orbit - otherOrbit.radius) < minimumOrbitDistance)
                {
                    return true;
                }
            }

            return false;
        }

        private void CheckIfOutside(Vector3 position)
        {
            if (Vector3.Distance(position, transform.position) > currentRadius && !playerIsOutside)
            {
                Debug.Log("Player Exited System!");
                onPlayerExitedSystemBounds?.Invoke();
                playerIsOutside = true;
            }
            else if(Vector3.Distance(position, transform.position) < currentRadius && playerIsOutside)
            {
                Debug.Log("Player Entered System!");
                onPlayerEnteredSystemBounds?.Invoke();
                playerIsOutside = false;
            }
        }

        #endregion

        #endregion
    }
}
