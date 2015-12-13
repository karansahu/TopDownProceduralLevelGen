using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//[System.Serializable]
//public class MapSection
//{
//    public GameObject section;
//    public GameObject[] connections;
//}

//[CustomEditor(typeof(Maps))]
//public class RenameConnectionsArrayList
//{
//    public override void OnInspectorGUI()
//    {
//        var obj = ScriptableObject.CreateInstance<Maps>();
//        var serializedObject = new UnityEditor.SerializedObject(obj);
//        serializedObject.Update();
//        var controller = target as Maps;
//        EditorGUIUtility.LookLikeInspector();
//        SerializedProperty tps = serializedObject.FindProperty("targetPoints");
//        EditorGUI.BeginChangeCheck();
//        EditorGUILayout.PropertyField(tps, true);
//        if (EditorGUI.EndChangeCheck())
//            serializedObject.ApplyModifiedProperties();
//        EditorGUIUtility.LookLikeControls();
//    }
//}

public class MapConnections : MonoBehaviour
{    
    //public List<MapSection> mapSections = new List<MapSection>();
    //private List<List<RoomConnections.Room>> allSectionsRooms;
    //private List<RoomConnections.Room> connectionRooms;

    void Start()
    {

    }
    /*
    void GetAllRegions()
    {
        foreach(MapSection m in mapSections)
        {
            //Contains all roomRegions of all mapSections
            allSectionsRooms[mapSections.IndexOf(m)] = m.section.GetComponent<RoomConnections>().roomRegions;

            foreach (List<RoomConnections.Room> sectionRooms in allSectionsRooms)
            {
                sectionRooms[allSectionsRooms.IndexOf(sectionRooms)].edgeTiles
            }
            int index = mapSections.IndexOf(m);
            roomConnections[index] = m.section.GetComponent<RoomConnections>();
            //get rooms from section + connections
            //Rooms of the section
            foreach (RoomConnections.Room baseMapRoom in roomConnections[index].roomRegions)
            {

                //if (!forceMainRoomConnection)
                //{
                //    connectionFound = false;
                //    //This room has more than one connection, break
                //    if (roomA.connectedRooms.Count > 0)
                //        break;
                //}

                //Rooms of the connections
                foreach (RoomConnections.Room roomsOfConnections in roomConnections[index].roomRegions)
                {
                    //If the rooms are the same or if they are already connected, skip to the next room.
                    if (baseMapRoom == roomsOfConnections || baseMapRoom.IsConnectedTo(roomsOfConnections))
                        continue;

                    for (int i = 0; i < baseMapRoom.edgeTiles.Count; i++)
                    {
                        for (int j = 0; j < roomsOfConnections.edgeTiles.Count; j++)
                        {

                            Coord tileRoomA = baseMapRoom.edgeTiles[i];
                            Coord tileRoomB = roomsOfConnections.edgeTiles[j];

                            int distance = (int)(Mathf.Pow((tileRoomA.posX - tileRoomB.posX), 2) + Mathf.Pow((tileRoomA.posY - tileRoomB.posY), 2));
                            if (distance < shortestDistance || !connectionFound)
                            {
                                shortestDistance = distance;
                                connectionFound = true;
                                roomA_ConnectFrom = baseMapRoom;
                                roomB_ConnectTo = roomsOfConnections;
                                tileA_ConnectFrom = tileRoomA;
                                tileB_ConnectTo = tileRoomB;
                            }
                        }
                    }
                }
                if (connectionFound && !forceMainRoomConnection)
                    CreatePassage(roomA_ConnectFrom, roomB_ConnectTo, tileA_ConnectFrom, tileB_ConnectTo);
            }

            foreach (GameObject c in m.connections)
            {

            }
            //find shortest distance between section and each connection
        }
    }

    */
}
