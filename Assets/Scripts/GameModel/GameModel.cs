
using UnityEngine;

public class GameModel
{
    public ReactiveProperty<int> Score0 = new(0);
    public ReactiveProperty<int> Score1 = new(0);

    public ReactiveProperty<int> RebirthTime = new(0);


    public bool HasSaveData()
    {
        //return PlayerPrefs.HasKey(nameof(Day));
        return false;
    }

    public void Load()
    {
        //Day.Value = PlayerPrefs.GetInt(nameof(Day));
    }

    public void Save()
    {
        //PlayerPrefs.SetInt(nameof(Day), Day.Value);
    }
}
