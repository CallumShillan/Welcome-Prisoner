using System;
using System.Collections.Generic;

/// <summary>
/// Represents a book with pages and display tracking.
/// </summary>
public class GameBook
{
    /// <summary>
    /// Gets the name of the book.
    /// </summary>
    public string BookName { get; }

    /// <summary>
    /// Represents the collection of pages in the book.
    /// </summary>
    private readonly List<string> bookPages = new List<string>();

    /// <summary>
    /// Gets the pages of the book as a list.
    /// </summary>
    public List<string> ThePages => bookPages;

    /// <summary>
    /// Gets or sets whether the book has yet to be shown.
    /// </summary>
    public bool HasYetToBeShown { get; set; } = true;

    /// <summary>
    /// Gets or sets the time when the book was shown.
    /// </summary>
    public DateTime WhenShown { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameBook"/> class.
    /// </summary>
    /// <param name="bookName">The name of the book.</param>
    public GameBook(string bookName)
    {
        BookName = bookName ?? throw new ArgumentNullException(nameof(bookName));
    }

    /// <summary>
    /// Adds a page to the book.
    /// </summary>
    /// <param name="page">The page content to add.</param>
    public void AddPage(string page)
    {
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }
        bookPages.Add(page);
    }
}
