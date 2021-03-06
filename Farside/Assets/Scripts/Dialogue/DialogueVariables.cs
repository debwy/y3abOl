using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    //reads in globals.ink, initialise variable values here
    public Dictionary<string, Ink.Runtime.Object> variables;

    public Story globalVariablesStory;
    private const string saveVariablesKey = "INK_VARIABLES";

    public void SaveData(GameData data) {
        //Debug.Log("Save data called from dialogue var");
        if (globalVariablesStory != null) {
            //load current state of all variables to globals ink story
            VariablesToStory(globalVariablesStory);

            //replace this with actual save/load -> PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
            if (data.dialogueSave.ContainsKey(saveVariablesKey)) {
                data.dialogueSave.Remove(saveVariablesKey);
            }
            data.dialogueSave.Add(saveVariablesKey, globalVariablesStory.state.ToJson());
        }
    }


    public void LoadData(GameData data) {
        //Debug.Log("Load data called from dialogue var");
        if (data.dialogueSave.ContainsKey(saveVariablesKey)) {
            string jsonState;
            data.dialogueSave.TryGetValue(saveVariablesKey, out jsonState);
            globalVariablesStory.state.LoadJson(jsonState);
        }
    }
    
    
    //this ver should probably never be called
    public DialogueVariables(TextAsset loadGlobalsJSON) {
        //create story
        globalVariablesStory = new Story(loadGlobalsJSON.text);
        /*
        //if have saved data, load
        if (PlayerPrefs.HasKey(saveVariablesKey))
        {
            string jsonState = PlayerPrefs.GetString(saveVariablesKey);
            globalVariablesStory.state.LoadJson(jsonState);
        }
        */
        //initialize dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState) {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            //Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public DialogueVariables(TextAsset loadGlobalsJSON, GameData data) {
        //create story
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        LoadData(data);

        //initialize dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState) {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            //Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }
    
    /*
    public void SaveVariables() {
        if (globalVariablesStory != null) {
            //load current state of all variables to globals ink story
            VariablesToStory(globalVariablesStory);

            //replace this with actual save/load
            PlayerPrefs.SetString(saveVariablesKey, globalVariablesStory.state.ToJson());
        }
    }
    */
    
    //take in story that we want variable observer to listen to
    public void StartListening(Story story) {
        //must send variables to story before assigning the listener
        VariablesToStory(story);

        //VariableChanged is the listener for when the story changes
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story) {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }


    //name is the variable name, value is the value of that variable.
    private void VariableChanged(string name, Ink.Runtime.Object value) {
        //only maintain variables that are initialized from globals ink file
        if (variables.ContainsKey(name)) {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    //send variables back to ink story
    private void VariablesToStory(Story story) {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables) {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

}
