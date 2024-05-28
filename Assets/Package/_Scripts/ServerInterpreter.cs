using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class ServerInterpreter
{  public static List<TrainerData> InterpretData(string[] args)
    {
        List<TrainerData> ret = new List<TrainerData>();

        if (args.Length < 1)
        {
            Debug.Log("No Server Objects To Recieve");
            return ret;
        }

        for (int i = 0; i < args.Length; i++)
        {
            string[] properties = args[i].Split(" ");

            if (properties.Length < 2)
            {
                continue;
            }
            int propertyIndex = 0;
            int serverID = int.Parse(properties[propertyIndex++]);
            //Debug.Log(serverID);

            string name = properties[propertyIndex++];

            int tID = int.Parse(properties[propertyIndex++]);

            string[] split = properties[propertyIndex++].Split(",");//position
            Vector3 position = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));

            split = properties[propertyIndex++].Split(",");//rotation
            Vector3 rotation = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));

            split = properties[propertyIndex++].Split(",");//scale
            Vector3 scale = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));

            float speed = float.Parse(properties[propertyIndex++]);

            bool jump = bool.Parse(properties[propertyIndex++]);

            bool grounded = bool.Parse(properties[propertyIndex++]);

            bool freefall = bool.Parse(properties[propertyIndex++]);

            bool crouch = bool.Parse(properties[propertyIndex++]);

            bool throwball = bool.Parse(properties[propertyIndex++]);

            bool emote = bool.Parse(properties[propertyIndex++]);

            int emoteState = int.Parse(properties[propertyIndex++]);

            int modelSelection = int.Parse(properties[propertyIndex++]);

            TrainerData obj = new TrainerData(name, serverID, tID, position, rotation, scale, speed, jump, grounded, freefall, crouch, throwball, emote, emoteState, modelSelection);

            ret.Add(obj);
        }

        return ret;
    }

    public static string StringifyData(TrainerData data)
    {
        string str = $"NetworkTrainerData|";

        string toAdd =
            //trainer data
            $"{data.serverID} " +
            $"{data.name} " +
            $"{data.tID} " +
            //position and rotation data
            $"{data.position.x},{data.position.y},{data.position.z} " +
            $"{data.rotation.x},{data.rotation.y},{data.rotation.z} " +
            $"{data.scale.x},{data.scale.y},{data.scale.z} " +
            //animation data
            $"{data.speed} " +
            $"{data.jump} " +
            $"{data.grounded} " +
            $"{data.freefall} " +
            $"{data.crouch} " +
            $"{data.throwball} " +
            $"{data.emote} " +
            $"{data.emoteState} " +
            //model data
            $"{data.modelSelection}";

        toAdd += "|";

        str += toAdd;

        return str;
    }
}

[System.Serializable]
public class TrainerData
{
    public string name;
    public int serverID;
    public int tID;

    //world info
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    //animation info
    public float speed;
    public bool jump;
    public bool grounded;
    public bool freefall;
    public bool crouch;
    public bool throwball;
    public bool emote;
    public int emoteState;

    public int modelSelection;

    public TrainerData(string name, int serverID, int tID, Vector3 position, Vector3 rotation, Vector3 scale, float speed, bool jump, bool grounded, bool freefall, bool crouch, bool throwball, bool emote, int emoteState, int modelSelection)
    {
        this.name = name;
        this.serverID = serverID;
        this.tID = tID;

        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.speed = speed;
        this.jump = jump;
        this.grounded = grounded;
        this.freefall = freefall;
        this.crouch = crouch;
        this.throwball = throwball;

        this.emote = emote;
        this.emoteState = emoteState;
        this.modelSelection = modelSelection;
    }
}