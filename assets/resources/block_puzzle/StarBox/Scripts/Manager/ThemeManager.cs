using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OneSoftGame.Tools;

public class ThemeManager : PersistentSingleton<ThemeManager>
{

    protected internal HashSet<IChangable> listItemTheme = new HashSet<IChangable>();

    /// <summary>
    /// Make the change and reload the loading scene
    /// </summary>
    /// <param name="themeToChange"></param>
    public void ChangeAndReload(Theme themeToChange)
    {
        SoundManager.Instance.ChangeResourceSound(needReBgm: true);
        SceneManager.LoadScene(Global.LOADING_SCENE);
    }

    /// <summary>
    /// Change instantly at run-time.
    /// </summary>
    /// <param name="themeToChange"></param>
    public void ChangeThemeInstantly(Theme themeToChange)
    {
        SoundManager.Instance.ChangeResourceSound(needReBgm: true);

        using (var e = listItemTheme.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var current = e.Current;

                if (current != null)
                    current.Change();
            }
        }
    }

    /// <summary>
    /// Add item to manager
    /// </summary>
    /// <param name="item"></param>
    public void AddToManager(IChangable item)
    {
        if (!listItemTheme.Contains(item))
        {
            listItemTheme.Add(item);
        }
    }

    /// <summary>
    /// Remove item from manager
    /// </summary>
    /// <param name="item"></param>
    public void RemoveFromManager(IChangable item)
    {
        if (!Global.FLAG_RUNNING) return;

        if (listItemTheme.Contains(item))
        {
            listItemTheme.Remove(item);
        }
    }


    private void OnApplicationPause(bool pause)
    {
        Global.FLAG_RUNNING = !pause;
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Global.FLAG_RUNNING = false;
    }
#endif
}
