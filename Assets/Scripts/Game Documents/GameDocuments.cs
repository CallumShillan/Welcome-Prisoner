using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBook
{
    private string bookName = string.Empty;
    private List<string> bookPages = null;
    private bool hasYetToBeShown = true;
    private DateTime whenShown = DateTime.MinValue;

    public GameBook(string bookName)
    {
        this.bookName = bookName;
        this.bookPages = new List<string>();
        hasYetToBeShown = true;
    }

    public string BookName { get => bookName; }
    public List<string> ThePages { get => bookPages; }
    public bool HasYetToBeShown { get => hasYetToBeShown; set => hasYetToBeShown = value; }
    public DateTime WhenShown { get => whenShown; set => whenShown = value; }
}

public class GameDocuments : MonoBehaviour
{
    private Dictionary<string, GameBook> allBooks = new System.Collections.Generic.Dictionary<string, GameBook>();
    public static GameDocuments Instance { get; private set; }
    public Dictionary<string, GameBook> AllBooks { get => allBooks; set => allBooks = value; }

    void Awake()
    {
        try
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            // Prime the Document Library by preparing (a) an empty sorted lists of all rooms in the scene that has books and (b) an empty sorted list of books within that room
            // These are then ready for population with the book contents, when needed

            // Remember the Active Scene name
            string sceneName = SceneManager.GetActiveScene().name;
            
            object[] allPages = Resources.LoadAll<TextAsset>("GameDocuments/" + sceneName);
            
            string[] nameInfo;
            string bookName = string.Empty;
            string pageName = string.Empty;

            foreach ( TextAsset page in allPages)
            {
                nameInfo = page.name.Split("!");
                bookName = nameInfo[0];
                pageName = nameInfo[1];

                if ( allBooks.ContainsKey(bookName))
                {
                    allBooks[bookName].ThePages.Add(page.text);
                }
                else
                {
                    GameBook aGameBook = new GameBook(bookName);
                    aGameBook.ThePages.Add(page.text);
                    allBooks.Add(bookName, aGameBook);
                }
            }
        }
        catch ( Exception ex)
        {
            GameLog.ExceptionMessage(this, "GameDocuments Awake() exception: {0}", ex.ToString());
        }
        GameLog.NormalMessage(this, "GameDocuments Awake() finished");
    }
}
