using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Utils;
using Observer;

namespace Utils
{
    public class SceneGenerator : MonoBehaviour, ISubject
    {
        public SceneObjectFactory sceneObjectFactory;
        [SerializeField] GameObject cyclicConnectionPrefab;
        [SerializeField] float cyclicConnectionRange = 15f;
        [SerializeField] float maxGScore = 15f;
        [SerializeField] PathFinder pathFinderBuilder;


        [SerializeField] GameObject positiveX;
        [SerializeField] GameObject negativeX;
        [SerializeField] GameObject positiveZ;
        [SerializeField] GameObject negativeZ;


        [SerializeField] bool useEditorValues = false;
        [SerializeField] int minRooms = 5;
        [SerializeField] int maxRooms = 10;
        [SerializeField] int sceneX = 500;
        [SerializeField] int sceneY = 500;
        [SerializeField] int roomSizeMin = 0;
        [SerializeField] int roomSizeMax = 50;
        [SerializeField] int corridorSizeMin = 0;
        [SerializeField] int corridorSizeMax = 50;


        [SerializeField] bool useRandomSeed = false;
        [SerializeField] string seed = "";


        List<Connector> availableRoomConnectors = new List<Connector>();
        List<Connector> availableCorridorConnectors = new List<Connector>();

        List<Room> generatedRooms = new List<Room>();
        List<Corridor> generatedCorridors = new List<Corridor>();
        List<SpecialRoom> generatedSpecialRooms = new List<SpecialRoom>();
        List<PathFinder> pathFinders = new List<PathFinder>();


        bool wasRoomPlaced = false;
        bool wasCorridorPlaced = false;
        public static System.Random pseudoRandom = null;

        void Start()
        {
            StartCoroutine("GenerateScene");
        }

        // Update is called once per frame
        void Update()
        {
            // if (Input.GetMouseButtonDown(0)) {
            //     CleanUp();
            //     StartCoroutine("GenerateScene");
            // }   
        }

        IEnumerator GenerateScene()
        {
            yield return Helpers.startup;
            SetParameters();
            SetSceneSize();
            print("Seed: " + seed);
            if (seed == "")
            {
                print("seed is empty");
                seed = Time.time.ToString();
            }

            print(seed.GetHashCode());
            pseudoRandom = new System.Random(seed.GetHashCode());


            PlaceStartRoom();

            int numberOfIterations = pseudoRandom.Next(minRooms, maxRooms);
            for (int i = 0; i < numberOfIterations - 1; i++)
            {
                wasRoomPlaced = false;
                wasCorridorPlaced = false;
                List<SceneObject> possibleRooms = sceneObjectFactory.GetList("regularRoom");
                List<SceneObject> possibleCorridors = sceneObjectFactory.GetList("corridor");
                List<SceneObject> possibleSpecialRooms = sceneObjectFactory.GetList("specialRoom");


                while (!wasRoomPlaced && possibleSpecialRooms.Count > 0)
                {
                    SpecialRoom specRoom = sceneObjectFactory.Create("specialRoom") as SpecialRoom;
                    possibleSpecialRooms.RemoveAll(r => r.GetName() == specRoom.GetName());
                    if (specRoom.GetSpawnChance() < pseudoRandom.Next(0, 100))
                    {
                        Destroy(specRoom.gameObject);
                        continue;
                    }
                    if (specRoom.GetMaxAmountPerScene() <= CountGeneratedSpecialRooms(specRoom))
                    {
                        Destroy(specRoom.gameObject);
                        continue;
                    }


                    if (specRoom.GetSize() < roomSizeMin || specRoom.GetSize() > roomSizeMax)
                    {
                        Destroy(specRoom.gameObject);
                    }
                    else
                    {
                        PlaceSpecialRoom(specRoom);
                        break;
                    }
                    yield return Helpers.fixedUpdateInterval;
                }


                while (!wasRoomPlaced && possibleRooms.Count > 0)
                {
                    Room currentRoom = sceneObjectFactory.Create("regularRoom") as Room;
                    possibleRooms.RemoveAll(r => r.GetName() == currentRoom.GetName());
                    if (currentRoom.GetSize() < roomSizeMin || currentRoom.GetSize() > roomSizeMax)
                    {
                        Destroy(currentRoom.gameObject);
                    }
                    else
                    {
                        PlaceRoom(currentRoom);
                    }
                    yield return Helpers.fixedUpdateInterval;
                }


                while (!wasCorridorPlaced && possibleCorridors.Count > 0)
                {
                    Corridor currentCorridor = sceneObjectFactory.Create("corridor") as Corridor;
                    possibleCorridors.RemoveAll(r => r.GetName() == currentCorridor.GetName());
                    if (currentCorridor.GetSize() < corridorSizeMin || currentCorridor.GetSize() > corridorSizeMax)
                    {
                        Destroy(currentCorridor.gameObject);
                    }
                    else
                    {
                        PlaceCorridor(currentCorridor);
                    }
                    yield return Helpers.fixedUpdateInterval;
                }
            }
            PlaceEndRoom();

            if (generatedRooms.Count < minRooms)
            {
                this.Notify("Error, try generating with different parameters");
            }
            else
            {
                // Debug.Break();
                yield return StartCoroutine(ConnectEmptyConnectors());

                DeleteUnconnectedCorridors();

                yield return AddWallsToPathFinders();
                BakeNavMesh();
                // Debug.Log("finished");

                this.Notify("Finished");
            }

            StopCoroutine("GenerateScene");
        }

        public void SetParameters()
        {
            if (useEditorValues) return;
            this.Notify("Loading parameters");
            minRooms = PlayerPrefsController.GetMinRooms();
            maxRooms = PlayerPrefsController.GetMaxRooms();
            sceneX = PlayerPrefsController.GetSceneSizeX();
            sceneY = PlayerPrefsController.GetSceneSizeY();
            roomSizeMin = PlayerPrefsController.GetMinRoomSize();
            roomSizeMax = PlayerPrefsController.GetMaxRoomSize();
            corridorSizeMin = PlayerPrefsController.GetMinCorridorSize();
            corridorSizeMax = PlayerPrefsController.GetMaxCorridorSize();
            seed = PlayerPrefsController.GetSeed();
        }

        public void SetSceneSize()
        {
            positiveX.transform.position = new Vector3(sceneY / 4, 0f, positiveX.transform.position.z);
            positiveX.transform.localScale = new Vector3(positiveX.transform.localScale.x, 10f, sceneX / 2);

            negativeX.transform.position = new Vector3(-sceneY / 4, 0f, negativeX.transform.position.z);
            negativeX.transform.localScale = new Vector3(negativeX.transform.localScale.x, 10f, sceneX / 2);

            positiveZ.transform.position = new Vector3(positiveZ.transform.position.x, 0f, sceneX / 4);
            positiveZ.transform.localScale = new Vector3(sceneY / 2, 10f, positiveZ.transform.localScale.z);

            negativeZ.transform.position = new Vector3(negativeZ.transform.position.x, 0f, -sceneX / 4);
            negativeZ.transform.localScale = new Vector3(sceneY / 2, 10f, negativeZ.transform.localScale.z);
        }

        int CountGeneratedSpecialRooms(SpecialRoom specRoom)
        {
            int count = 0;
            for (int i = 0; i < generatedSpecialRooms.Count; i++)
            {
                SpecialRoom room = generatedSpecialRooms[i];
                if (room.GetName() == specRoom.GetName()) count++;
            }
            // print(count);
            return count;
        }


        void PlaceStartRoom()
        {
            this.Notify("Placing start room");
            // Debug.Log("placed start room");
            Room startRoom = sceneObjectFactory.Create("startRoom") as Room;
            startRoom.transform.parent = this.transform;
            generatedRooms.Add(startRoom);

            AddConnectorsToList(availableRoomConnectors, startRoom);

            startRoom.transform.position = Vector3.zero;
            startRoom.transform.rotation = Quaternion.identity;
        }


        void PlaceEndRoom()
        {
            this.Notify("Placing end room");
            Room endRoom = sceneObjectFactory.Create("endRoom") as Room;
            endRoom.transform.parent = this.transform;
            List<Connector> currentRoomConnectors = endRoom.GetConnectors();

            List<Connector> sortedAvailableCorridorConnectors = new List<Connector>(availableCorridorConnectors);
            //sort connectors by distance
            for (int i = 1; i < sortedAvailableCorridorConnectors.Count; i++)
            {
                Connector current = sortedAvailableCorridorConnectors[i];
                int j = i - 1;

                while (j >= 0 &&
                    sortedAvailableCorridorConnectors[j].distanceFromStart < current.distanceFromStart)
                {

                    sortedAvailableCorridorConnectors[j + 1] = sortedAvailableCorridorConnectors[j];
                    j--;
                }

                sortedAvailableCorridorConnectors[j + 1] = current;
            }

            foreach (Connector currentSceneCorridorConnector in sortedAvailableCorridorConnectors)
            {
                foreach (Connector currentRoomConnector in currentRoomConnectors)
                {
                    PositionSceneObjectAtConnector(endRoom, currentRoomConnector, currentSceneCorridorConnector);

                    if (endRoom.CheckOverlap())
                    {
                        continue;
                    }

                    AddConnectorsToList(availableRoomConnectors, endRoom);
                    generatedRooms.Add(endRoom);

                    availableCorridorConnectors.Remove(currentSceneCorridorConnector);
                    availableRoomConnectors.Remove(currentRoomConnector);

                    currentSceneCorridorConnector.SetConnectedTo(currentRoomConnector);
                    currentRoomConnector.SetConnectedTo(currentSceneCorridorConnector);

                    SetConnectorDistance(endRoom, currentSceneCorridorConnector.distanceFromStart + 1);

                    return;
                }
            }
            Destroy(endRoom.gameObject);
        }


        void PlaceRoom(Room currentRoom)
        {
            // Debug.Log("place random room");
            this.Notify("Placing room");
            currentRoom.transform.parent = this.transform;
            List<Connector> currentRoomConnectors = currentRoom.GetConnectors();

            foreach (Connector currentSceneCorridorConnector in availableCorridorConnectors)
            {
                foreach (Connector currentRoomConnector in currentRoomConnectors)
                {
                    PositionSceneObjectAtConnector(currentRoom, currentRoomConnector, currentSceneCorridorConnector);

                    if (currentRoom.CheckOverlap())
                    {
                        continue;
                    }

                    AddConnectorsToList(availableRoomConnectors, currentRoom);
                    generatedRooms.Add(currentRoom);

                    availableCorridorConnectors.Remove(currentSceneCorridorConnector);
                    availableRoomConnectors.Remove(currentRoomConnector);

                    currentSceneCorridorConnector.SetConnectedTo(currentRoomConnector);
                    currentRoomConnector.SetConnectedTo(currentSceneCorridorConnector);

                    SetConnectorDistance(currentRoom, currentSceneCorridorConnector.distanceFromStart + 1);
                    wasRoomPlaced = true;
                    return;
                }
            }
            Destroy(currentRoom.gameObject);
        }

        void PlaceSpecialRoom(SpecialRoom currentRoom)
        {
            // Debug.Log("place random room");
            this.Notify("Placing special room");
            currentRoom.transform.parent = this.transform;
            List<Connector> currentRoomConnectors = currentRoom.GetConnectors();

            foreach (Connector currentSceneCorridorConnector in availableCorridorConnectors)
            {
                if (currentSceneCorridorConnector.distanceFromStart <= currentRoom.GetMinSpawnDistance()) continue;
                foreach (Connector currentRoomConnector in currentRoomConnectors)
                {
                    PositionSceneObjectAtConnector(currentRoom, currentRoomConnector, currentSceneCorridorConnector);

                    if (currentRoom.CheckOverlap())
                    {
                        continue;
                    }

                    AddConnectorsToList(availableRoomConnectors, currentRoom);
                    generatedRooms.Add(currentRoom);
                    generatedSpecialRooms.Add(currentRoom);

                    availableCorridorConnectors.Remove(currentSceneCorridorConnector);
                    availableRoomConnectors.Remove(currentRoomConnector);

                    currentSceneCorridorConnector.SetConnectedTo(currentRoomConnector);
                    currentRoomConnector.SetConnectedTo(currentSceneCorridorConnector);

                    SetConnectorDistance(currentRoom, currentSceneCorridorConnector.distanceFromStart + 1);
                    wasRoomPlaced = true;
                    return;
                }
            }
            Destroy(currentRoom.gameObject);
        }

        void AddConnectorsToList(List<Connector> list, SceneObject sceneObject)
        {
            foreach (Connector connector in sceneObject.GetConnectors())
            {
                int randomExitPoint = pseudoRandom.Next(0, list.Count);
                list.Insert(randomExitPoint, connector);
            }
        }

        void SetConnectorDistance(SceneObject sceneObject, int distance)
        {
            foreach (Connector connector in sceneObject.GetConnectors())
            {
                connector.distanceFromStart = distance;
            }
        }

        public static void PositionSceneObjectAtConnector(SceneObject sceneObject, Connector sceneObjectConnector, Connector targetConnector)
        {
            sceneObject.transform.position = Vector3.zero;
            sceneObject.transform.rotation = Quaternion.identity;

            Vector3 targetConnectorEuler = targetConnector.transform.eulerAngles;
            Vector3 sceneObjectConnectorEuler = sceneObjectConnector.transform.eulerAngles;
            float deltaAngle = Mathf.DeltaAngle(sceneObjectConnectorEuler.y, targetConnectorEuler.y);
            Quaternion sceneObjectTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
            sceneObject.transform.rotation = sceneObjectTargetRotation * Quaternion.Euler(0, 180f, 0);

            Vector3 sceneObjectPositionOffset = sceneObjectConnector.transform.position - sceneObject.transform.position;
            sceneObject.transform.position = targetConnector.transform.position - sceneObjectPositionOffset;
        }

        void PlaceCorridor(Corridor currentCorridor)
        {
            this.Notify("Placing corridor");
            currentCorridor.transform.parent = this.transform;
            List<Connector> currentCorridorConnectors = currentCorridor.GetConnectors();

            foreach (Connector currentSceneRoomConnector in availableRoomConnectors)
            {
                foreach (Connector currentCorridorConnector in currentCorridorConnectors)
                {
                    PositionSceneObjectAtConnector(currentCorridor, currentCorridorConnector, currentSceneRoomConnector);

                    if (currentCorridor.CheckOverlap())
                    {
                        continue;
                    }

                    AddConnectorsToList(availableCorridorConnectors, currentCorridor);
                    generatedCorridors.Add(currentCorridor);

                    availableRoomConnectors.Remove(currentSceneRoomConnector);
                    availableCorridorConnectors.Remove(currentCorridorConnector);

                    currentSceneRoomConnector.SetConnectedTo(currentCorridorConnector);
                    currentCorridorConnector.SetConnectedTo(currentSceneRoomConnector);

                    SetConnectorDistance(currentCorridor, currentSceneRoomConnector.distanceFromStart + 1);

                    return;
                }
            }
            Destroy(currentCorridor.gameObject);
        }








        IEnumerator ConnectEmptyConnectors()
        {
            this.Notify("Attempting cyclic connection");
            foreach (Connector corridorConnector in availableCorridorConnectors)
            {
                if (corridorConnector.GetConnectedTo() != null) continue;
                List<Connector> foundConnectors = FindClosestConnectors(corridorConnector);
                foreach (Connector foundConnector in foundConnectors)
                {
                    PathFinder newBuilder = Instantiate(pathFinderBuilder);
                    newBuilder.transform.parent = this.transform;
                    newBuilder.SetMaxGScore(maxGScore);
                    newBuilder.SetConnectionPoints(corridorConnector, foundConnector);
                    yield return StartCoroutine(newBuilder.StartConnecting());

                    if (newBuilder.isEndFound)
                    {
                        corridorConnector.SetConnectedTo(foundConnector);
                        foundConnector.SetConnectedTo(corridorConnector);
                        pathFinders.Add(newBuilder);
                        break;
                    }
                    else
                    {
                        Destroy(newBuilder.gameObject);
                    }
                }
            }
        }

        IEnumerator AddWallsToPathFinders()
        {
            this.Notify("Adding walls to paths");
            foreach (PathFinder pathFinder in pathFinders)
            {
                yield return StartCoroutine(pathFinder.AddWallsToPath());
            }
        }

        List<Connector> FindClosestConnectors(Connector corridorConnector)
        {
            List<Connector> foundConnectors = new List<Connector>();
            foreach (Connector roomConnector in availableRoomConnectors)
            {
                if (roomConnector.GetConnectedTo() != null) continue;

                float distanceBetweenConnectors = Vector3.Distance(
                    corridorConnector.transform.position,
                    roomConnector.transform.position);

                if (distanceBetweenConnectors <= cyclicConnectionRange)
                {
                    if (!CheckIfSameRoom(corridorConnector, roomConnector))
                    {
                        foundConnectors.Add(roomConnector);
                    }

                }
            }

            //insertion sort
            for (int i = 1; i < foundConnectors.Count; i++)
            {
                Connector current = foundConnectors[i];
                float currentDist = Vector3.Distance(
                    corridorConnector.transform.position,
                    current.transform.position);
                int j = i - 1;

                while (j >= 0 &&
                    Vector3.Distance(corridorConnector.transform.position,
                    foundConnectors[j].transform.position) > currentDist)
                {

                    foundConnectors[j + 1] = foundConnectors[j];
                    j--;
                }

                foundConnectors[j + 1] = current;
            }

            return foundConnectors;
        }

        bool CheckIfSameRoom(Connector startCorridorConnector, Connector endRoomConnector)
        {
            GameObject corridorGameObject = Helpers.GetRootGameObject(startCorridorConnector.transform);
            Corridor currentCorridor = corridorGameObject.GetComponent<Corridor>();
            List<Connector> corridorConnectors = currentCorridor.GetConnectors();

            foreach (Connector corridorConnector in corridorConnectors)
            {

                if (corridorConnector == startCorridorConnector) continue;
                if (corridorConnector.GetConnectedTo() == null) continue;
                GameObject roomGameObject = Helpers.GetRootGameObject(corridorConnector.GetConnectedTo().transform);
                Room currentRoom = roomGameObject.GetComponent<Room>();
                List<Connector> roomConnectors = currentRoom.GetConnectors();


                foreach (Connector roomConnector in roomConnectors)
                {
                    if (roomConnector.GetConnectedTo() != null) continue;
                    if (roomConnector.transform.position == endRoomConnector.transform.position)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        void DeleteUnconnectedCorridors()
        {
            List<Corridor> deletedCorridors = new List<Corridor>();
            foreach (Corridor corridor in generatedCorridors)
            {
                List<Connector> connectors = corridor.GetConnectors();
                int connections = 0;
                foreach (Connector connector in connectors)
                {
                    if (connector.GetConnectedTo() != null) connections++;
                }

                if (connections < 2)
                {
                    foreach (Connector connector in connectors)
                    {
                        if (connector.GetConnectedTo() != null)
                        {
                            connector.GetConnectedTo().SetConnectedTo(null);
                        }
                    }
                    deletedCorridors.Add(corridor);
                    Destroy(corridor.gameObject);
                }
            }

            foreach (Corridor remove in deletedCorridors)
            {
                generatedCorridors.Remove(remove);
            }
        }

        void BakeNavMesh()
        {
            this.Notify("Baking NavMesh");

            GetComponent<NavMeshSurface>().BuildNavMesh();
            Helpers.navBaked = true;

        }



        void CleanUp()
        {
            generatedCorridors.Clear();
            generatedRooms.Clear();
            generatedSpecialRooms.Clear();
            availableRoomConnectors.Clear();
            availableCorridorConnectors.Clear();
            pathFinders.Clear();

            foreach (Transform child in this.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private List<IObserver> _observers = new List<IObserver>();
        // Attach an observer to the subject.
        public void Attach(IObserver observer)
        {
            Console.WriteLine("Subject: Attached an observer.");
            this._observers.Add(observer);
        }

        // Detach an observer from the subject.
        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
            Console.WriteLine("Subject: Detached an observer.");
        }

        // Notify all observers about an event.
        public void Notify(string state)
        {
            Console.WriteLine("Subject: Notifying observers...");

            foreach (var observer in _observers)
            {
                observer.UpdateObserver(state);
            }
        }

    }

}
