//using Mono.Cecil.Cil;
using UnityEngine.UIElements;
using System.Collections.Generic;
public static class EBookReaderHelper
{
    private static ScrollView documentNameScrollView = null;
    private static Button referenceDocumentButton = null;
    private static Label labelBookName = null;
    private static Label labelBookPage = null;

    private static List<string> bookPages = null;
    private static int currentPageNumber = 0;
    private static int maxNumberPages = 0;

    private static PrisonerDigitalAssistantEventHandler thePdaEventHandler;

    public static void WireUp(VisualElement rootVisualElement, PrisonerDigitalAssistantEventHandler pdaEventHandler)
    {
        string appIdentifier = string.Empty;
        Button chapterNavigationButton = null;

        thePdaEventHandler = pdaEventHandler;

        string rootVisualElementName = rootVisualElement.name;

        // Wire up the "page turn" buttons
        foreach (string btnName in new string[] { "btn-First", "btn-Previous", "btn-Next", "btn-Last" })
        {
            chapterNavigationButton = rootVisualElement.Q<Button>(btnName);
            if (chapterNavigationButton is null)
            {
                GameLog.ErrorMessage($"Unable to find button '{btnName}' in Root Visual Element '{rootVisualElementName}'");
                continue;
            }

            chapterNavigationButton.RegisterCallback<ClickEvent>(ev => { HandlePageNavigation(btnName); });
        }

        // Get the ScrollView used to house all the buttons for each document
        documentNameScrollView = rootVisualElement.Q<ScrollView>("documentNameScrollView");
        if (documentNameScrollView is null)
        {
            GameLog.ErrorMessage($"Unable to find Scroller 'documentNameScrollView' in Root Visual Element '{rootVisualElementName}'");
            return;
        }

        // Get the reference document button name
        referenceDocumentButton = rootVisualElement.Q<Button>("btn-DocumentName");
        if (referenceDocumentButton is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'btn-DocumentName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        // Clear the scroller of all book name references
        documentNameScrollView.Clear();

        labelBookName = rootVisualElement.Q<Label>("label-BookName");
        if (labelBookName is null)
        {
            GameLog.ErrorMessage($"Unable to find label 'label-BookName' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelBookName.text = "Choose a book to view from the left-hand navigation ...";

        labelBookPage = rootVisualElement.Q<Label>("label-bookPage");
        if (labelBookPage is null)
        {
            GameLog.ErrorMessage($"Unable to find button 'label-bookPage' in Root Visual Element '{rootVisualElementName}'");
            return;
        }
        labelBookPage.text = string.Empty;
    }

    public static void CreateNavigationForKnownBooks()
    {
        documentNameScrollView.Clear();

        foreach (string bookName in GameDocuments.Instance.AllBooks.Keys)
        {
            // Create a Button for each document in the room and add it to the Foldout
            Button viewBookButton = null;

            viewBookButton = new Button();
            viewBookButton.text = bookName;
            viewBookButton.name = bookName;

            // Copy the CSS classes from the reference Document Button
            foreach (string cssClass in referenceDocumentButton.GetClasses())
            {
                viewBookButton.AddToClassList(cssClass);
            }

            viewBookButton.RegisterCallback<ClickEvent>(ev => { DisplayBook(bookName); });

            documentNameScrollView.Add(viewBookButton);
        }
    }

    private static void HandlePageNavigation(string btnName)
    {
        switch(btnName)
        {
            case "btn-First":
                currentPageNumber = 0;
                break;
            case "btn-Previous":
                if(currentPageNumber > 0)
                {
                    currentPageNumber--;
                }
                break;
            case "btn-Next":
                if (currentPageNumber < maxNumberPages-1)
                {
                    currentPageNumber++;
                }
                break;
            case "btn-Last":
                currentPageNumber = maxNumberPages-1;
                break;
        }
        labelBookPage.text = bookPages[currentPageNumber];
    }

    public static void DisplayBook(string bookName)
    {
        // Get the book being read
        bookPages = GameDocuments.Instance.AllBooks[bookName].ThePages;

        // Set the current and maximum pages
        currentPageNumber = 0;
        maxNumberPages = bookPages.Count;

        // Set the book name and page contents
        labelBookName.text = bookName;
        labelBookPage.text = bookPages[currentPageNumber];

        // Set the book's display button to indicate it is selected
        Button bookButton = documentNameScrollView.Q<Button>(bookName);
        bookButton.style.unityFontStyleAndWeight = UnityEngine.FontStyle.Bold;

        // Display the eBook Reader app
        thePdaEventHandler.DisplayPdaApp(PdaAppId.EbookReader);
    }
}
