
/*
This RPG data streaming assignment was created by Fernando Restituto with 
pixel RPG characters created by Sean Browning.
*/

using System.Collections.Generic;
using UnityEngine;
using System.IO;

#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion

#region Assignment Part 1

static public class SaveDataSignifiers
{
    public const int PartyCharacter = 1;
    public const int Equipment = 2;
}

static public class AssignmentPart1
{
    //For creation process, see: https://youtu.be/VVULsmtWco8?si=4MFPVt6ZPzK9Wrna

    const char SepChar = ',';
    const string SaveFileName = "PartySaveData.txt";

    static public void SavePartyButtonPressed()
    {
        LinkedList<string> serializedSaveData = SerializeSaveData();

        #region Save Data To HD

        StreamWriter sw = new StreamWriter(SaveFileName);

        foreach (string line in serializedSaveData)
            sw.WriteLine(line);

        sw.Close();

        #endregion
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();

        #region Load Data From HD

        LinkedList<string> serializedData = new LinkedList<string>();
        StreamReader sr = new StreamReader(SaveFileName);

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            serializedData.AddLast(line);
        }

        sr.Close();

        #endregion

        DeserializeSaveData(serializedData);

        GameContent.RefreshUI();
    }

    static private LinkedList<string> SerializeSaveData()
    {
        LinkedList<string> serializedData = new LinkedList<string>();

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            string concatenatedString = Concatenate(SaveDataSignifiers.PartyCharacter.ToString(),
                pc.classID.ToString(), pc.health.ToString(),
                pc.mana.ToString(), pc.strength.ToString(),
                pc.agility.ToString(), pc.wisdom.ToString());

            serializedData.AddLast(concatenatedString);

            foreach (int e in pc.equipment)
            {
                concatenatedString = Concatenate(SaveDataSignifiers.Equipment.ToString(), e.ToString());
                serializedData.AddLast(concatenatedString);
            }
        }

        return serializedData;
    }

    static private void DeserializeSaveData(LinkedList<string> serializedData)
    {
        PartyCharacter pc = null;

        foreach (string line in serializedData)
        {
            string[] csv = line.Split(SepChar);
            int signifier = int.Parse(csv[0]);

            if (signifier == SaveDataSignifiers.PartyCharacter)
            {
                pc = new PartyCharacter(int.Parse(csv[1]),
                    int.Parse(csv[2]), int.Parse(csv[3]),
                    int.Parse(csv[4]), int.Parse(csv[5]),
                    int.Parse(csv[6]));

                GameContent.partyCharacters.AddLast(pc);
            }
            else if (signifier == SaveDataSignifiers.Equipment)
            {
                pc.equipment.AddLast(int.Parse(csv[1]));
            }
        }
    }

    static private string Concatenate(params string[] stringsToJoin)
    {
        string joinedString = "";

        foreach (string s in stringsToJoin)
        {
            if (joinedString != "")
                joinedString += SepChar;
            joinedString += s;
        }

        return joinedString;
    }
}


#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 1;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    static List<string> listOfPartyNames;
    private static string saveDirectoryPath = "SavedParties/";

    static public void GameStart()
    {
        listOfPartyNames = new List<string>();

        if (!Directory.Exists(saveDirectoryPath))
        {
            Directory.CreateDirectory(saveDirectoryPath);
        }

        foreach (string filePath in Directory.GetFiles(saveDirectoryPath, "*.txt"))
        {
            listOfPartyNames.Add(Path.GetFileNameWithoutExtension(filePath));
        }

        GameContent.RefreshUI();
        Debug.Log("Loaded existing party names.");
    }

    static public List<string> GetListOfPartyNames()
    {
        return listOfPartyNames;
    }

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        GameContent.RefreshUI();
    }

    static public void SavePartyButtonPressed()
    {
        string partyName = GameContent.GetPartyNameFromInput()?.Trim();

        if (string.IsNullOrEmpty(partyName))
        {
            Debug.LogError("Party name cannot be empty!");
            return;
        }

        // Replace invalid characters
        partyName = string.Join("_", partyName.Split(Path.GetInvalidFileNameChars()));

        string filePath = Path.Combine(saveDirectoryPath, $"{partyName}.txt");

        // Handle duplicate names
        if (listOfPartyNames.Contains(partyName))
        {
            partyName = $"{partyName}_{listOfPartyNames.Count}";
            filePath = Path.Combine(saveDirectoryPath, $"{partyName}.txt");
        }

        // Backup existing file
        if (File.Exists(filePath))
        {
            string backupFile = filePath + ".bak";
            File.Copy(filePath, backupFile, true);
            Debug.Log($"Backup created for existing party: {partyName}");
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (PartyCharacter pc in GameContent.partyCharacters)
            {
                writer.WriteLine($"{pc.classID},{pc.health},{pc.mana},{pc.strength},{pc.agility},{pc.wisdom}");
                writer.WriteLine(string.Join(" ", pc.equipment));
            }
        }

        if (!listOfPartyNames.Contains(partyName))
        {
            listOfPartyNames.Add(partyName);
            listOfPartyNames.Sort();
        }

        Debug.Log($"Party '{partyName}' saved successfully!");
        GameContent.RefreshUI();
    }

    static public void DeletePartyButtonPressed()
    {
        GameContent.RefreshUI();
    }

}

#endregion


