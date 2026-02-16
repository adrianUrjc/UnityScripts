using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public interface IManager
    {
        public enum GameStartMode { FIRST,EARLY, NORMAL, LATE }//se va a necesitar first para tema de dependecia de datos
        public GameStartMode StartMode{ get; }
        public void StartManager();
        public void OnStartGame();
        public void LoadData();
        public void SaveData();
        public void OnEndGame();
        public void OnEnd();
    }
}