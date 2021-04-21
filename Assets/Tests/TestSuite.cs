using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Utils;

public class TestSuite
{
    [UnityTest]
    public IEnumerator CheckSceneObjectCollision()
    {
        GameObject gameGameObjectOne =
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Newer/Rooms/Room1"));
        Room roomOne = gameGameObjectOne.GetComponent<Room>();

        GameObject gameGameObjectTwo =
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Newer/Rooms/Room1"));
        Room roomTwo = gameGameObjectTwo.GetComponent<Room>();

        yield return new WaitForSeconds(0.1f);

        Assert.True(roomOne.CheckOverlap());
        Object.Destroy(gameGameObjectOne.gameObject);
        Object.Destroy(gameGameObjectTwo.gameObject);
    }

    [UnityTest]
    public IEnumerator CheckSceneObjectPlaceAlignment()
    {
        GameObject gameGameObjectOne =
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Newer/Rooms/Room1"));
        Room roomOne = gameGameObjectOne.GetComponent<Room>();

        GameObject gameGameObjectTwo =
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Newer/Rooms/Room2"));
        Room roomTwo = gameGameObjectTwo.GetComponent<Room>();

        SceneGenerator.PositionSceneObjectAtConnector(roomOne, roomOne.GetConnectors()[0], roomTwo.GetConnectors()[0]);
        yield return new WaitForSeconds(0.1f);

        Assert.False(roomOne.CheckOverlap());
        Assert.True(roomOne.GetConnectors()[0].transform.position == roomTwo.GetConnectors()[0].transform.position);
        Object.Destroy(gameGameObjectOne.gameObject);
        Object.Destroy(gameGameObjectTwo.gameObject);
    }

    [UnityTest]
    public IEnumerator CheckSceneObjectSize()
    {
        GameObject gameGameObjectOne =
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Newer/Rooms/Room1"));
        Room roomOne = gameGameObjectOne.GetComponent<Room>();

        Debug.Log(roomOne.GetSize());
        yield return new WaitForSeconds(0.1f);

        Assert.True(roomOne.GetSize() == 720);
        Object.Destroy(gameGameObjectOne.gameObject);
    }
}
