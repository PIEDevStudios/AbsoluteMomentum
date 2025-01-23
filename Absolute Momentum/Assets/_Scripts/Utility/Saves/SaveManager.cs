using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : Singleton<SaveManager>
{
    public void NewGame()
    {
        
    }

    public void LoadGame()
    {
        
    }

    public void SaveGame()
    {
        
    }

    protected override void OnApplicationQuit()
    {
        SaveGame();
    }
}

public interface IDataPersistent
{
    
}