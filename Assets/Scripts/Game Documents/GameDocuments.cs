using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all game books loaded from resources for the current scene.
/// </summary>
public class GameDocuments : Singleton<GameDocuments>
{
    private readonly Dictionary<string, GameBook> allBooks = new();

    /// <summary>
    /// Gets all books loaded for the current scene.
    /// </summary>
    public IReadOnlyDictionary<string, GameBook> AllBooks => allBooks;

    /// <inheritdoc/>
    protected override void Awake()
    {
        base.Awake();
        GameLog.NormalMessage(this, "[GameDocuments] Awake() started");
        try
        {
            // Prepare the document library for the active scene
            string sceneName = SceneManager.GetActiveScene().name;
            if (string.IsNullOrEmpty(sceneName))
            {
                GameLog.ExceptionMessage(this, "[GameDocuments] Scene name is null or empty.");
                return;
            }

            // Load all pages from the files in the Resources folder for the current scene
            // Naming format is {BookName}!{PageNumber}
            TextAsset[] allPages = Resources.LoadAll<TextAsset>($"GameDocuments/{sceneName}");
            if (allPages == null || allPages.Length == 0)
            {
                GameLog.NormalMessage(this, $"[GameDocuments] No pages found for scene '{sceneName}'.");
                return;
            }

            // Iterate through all pages and add them to the relevant books
            foreach (TextAsset page in allPages)
            {
                string[] nameInfo = page.name.Split('!');
                if (nameInfo.Length < 2)
                {
                    GameLog.ExceptionMessage(this, $"[GameDocuments] Invalid page name format: {page.name}");
                    continue;
                }

                string bookName = nameInfo[0];
                //string pageName = nameInfo[1]; // Available if needed but at the moment, the book just has a List<string> of the pages.

                // Check if the book already exists, if not, create a new one
                if (!allBooks.TryGetValue(bookName, out GameBook gameBook))
                {
                    gameBook = new GameBook(bookName);
                    allBooks.Add(bookName, gameBook);
                }
                gameBook.AddPage(page.text);
            }
        }
        catch (Exception ex)
        {
            GameLog.ExceptionMessage(this, $"[GameDocuments] Awake() exception: {ex}");
        }
        GameLog.NormalMessage(this, "[GameDocuments] Awake() finished");
    }
}
