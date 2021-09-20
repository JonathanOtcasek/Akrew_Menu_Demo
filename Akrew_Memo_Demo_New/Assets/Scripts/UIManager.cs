using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Declare variables up here -- most of these are public so that they can show up and be assigned easily in the inspector
    public CharacterCreationScript toCharacter;
    public AudioScript audioManager;

    public GameObject topSelector;
    public GameObject objectSelector;
    public GameObject colorSelector;

    IEnumerator[] ongoingCoroutines = new IEnumerator[3];

    int whichCategorySelected = 0;
    int page = 0;
    public Text pageText;

    public GameObject[] TopButtons;
    public GameObject[] OptionButtons;
    public GameObject[] ColorButtons;

    public Sprite[] TorsoThumbnails;
    public Sprite[] HairThumbnails;
    public Sprite[] EyeThumbnails;
    public Sprite[] MouthThumbnails;

    Sprite[][] Thumbs;

    public Sprite nullColorIndicator;

    Vector3 dragOrigin;



    void Start()
    {
        //You can't assign arrays of arrays in the inspector directly, so this builds one on start. Makes it easier to work with further down
        Thumbs = new Sprite[][] { TorsoThumbnails, HairThumbnails, EyeThumbnails, MouthThumbnails };

        //Make sure our button choices are up to date
        ReconfigureChoicesAndColors();
    }



    void ReconfigureChoicesAndColors()
    {
        ReconfirgureColors();
        BuildCurrentPage();
    }

    void ReconfirgureColors()
    {
        int i = 0;
        if(toCharacter.possibleColorSchemes[whichCategorySelected].Length == 0)
        {
            //if no colors are assigned for this character feature (ie mouths which will always be black) show a "n/a" box for color
            ColorButtons[0].SetActive(true);
            ColorButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = nullColorIndicator;
            ColorButtons[0].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            i = 1;
        }
        else
        {
            //otherwise make sure the sprite is set correctly
            ColorButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = ColorButtons[1].transform.GetChild(0).GetComponent<Image>().sprite;
        }

        for (; i < 8; i++)
        {
            //for each color button, go through and assign colors, or turn them off if they're not being used
            if(i < toCharacter.possibleColorSchemes[whichCategorySelected].Length)
            {
                ColorButtons[i].SetActive(true);
                ColorButtons[i].transform.GetChild(0).GetComponent<Image>().color = toCharacter.possibleColorSchemes[whichCategorySelected][i];
            } else
            {
                ColorButtons[i].SetActive(false);
            }
        }
    }

    void BuildCurrentPage() //colors don't have multiple pages but traits (i.e. eyes) can. Therefore traits need to be displayed on a page by page basis
    {
        for (int i = 0; i < 12; i++)
        {
            //go through each button and assign it the appropriate preview thumbnail. If there are fewer buttons than thumbnails for this page, turn the buttons off
            if(i + (page*12) < Thumbs[whichCategorySelected].Length)
            {
                OptionButtons[i].SetActive(true);
                OptionButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = Thumbs[whichCategorySelected][i + (page * 12)];
            } else
            {
                OptionButtons[i].SetActive(false);
            }
        }

        //set the page text so you know which page you're working with
        pageText.text = (page+1) + "/" + (int)((Thumbs[whichCategorySelected].Length / 12)+1);

        //when you move pages, move the highlighter to your previously selected choice. If your selection isn't on this page, turn the highlighter off
        int[][] currentCharacter = toCharacter.RetrieveCharacterTraits();
        if(currentCharacter[whichCategorySelected][0] >= (page*12) && currentCharacter[whichCategorySelected][0] < ((page + 1) * 12))
        {
            objectSelector.SetActive(true);
            objectSelector.transform.position = OptionButtons[currentCharacter[whichCategorySelected][0] - (page * 12)].transform.position;
            colorSelector.SetActive(true);
            colorSelector.transform.position = ColorButtons[currentCharacter[whichCategorySelected][1]].transform.position;
        } else
        {
            objectSelector.SetActive(false);
            colorSelector.SetActive(false);
        }
    }



    public void TopButtonClicked(int whichButton)
    {
        whichCategorySelected = whichButton;
        page = 0;

        ReconfigureChoicesAndColors();

        //this is to ensure the coroutine that moves the highlighter stops before we tell it to move again - otherwise it will get stuck
        if (ongoingCoroutines[0] != null) StopCoroutine(ongoingCoroutines[0]);
        if (ongoingCoroutines[1] != null) StopCoroutine(ongoingCoroutines[1]); //little visual fix
        IEnumerator co = MoveHighlighter(topSelector, TopButtons[whichButton].transform.position, true);
        ongoingCoroutines[0] = co;
        StartCoroutine(co);
    }

    public void OptionButtonClicked(int whichButton)
    {
        if (!objectSelector.activeSelf) //this looks a little nicer
        {
            objectSelector.transform.position = OptionButtons[whichButton].transform.position;
            objectSelector.SetActive(true);
        }

        toCharacter.ApplyCharacterTraitChange((Traits)whichCategorySelected, whichButton + (page * 12));

        if (ongoingCoroutines[1] != null) StopCoroutine(ongoingCoroutines[1]);
        IEnumerator co = MoveHighlighter(objectSelector, OptionButtons[whichButton].transform.position, false);
        ongoingCoroutines[1] = co;
        StartCoroutine(co);
    }

    public void ColorButtonClicked(int whichButton)
    {
        if (!colorSelector.activeSelf)
        {
            colorSelector.transform.position = ColorButtons[whichButton].transform.position;
            colorSelector.SetActive(true);
        }

        toCharacter.ApplyCharacterColorChange((Traits)whichCategorySelected, whichButton);

        if (ongoingCoroutines[2] != null) StopCoroutine(ongoingCoroutines[2]);
        IEnumerator co = MoveHighlighter(colorSelector, ColorButtons[whichButton].transform.position, false);
        ongoingCoroutines[2] = co;
        StartCoroutine(co);
    }

    public void ChangePage(bool forward) //controls the page arrows
    {
        if (forward)
        {
            if (page < (int)((Thumbs[whichCategorySelected].Length / 12))) page++;
        }
        else
        {
            if (page > 0) page--;
        }
        BuildCurrentPage();
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            audioManager.PlaySound(Sounds.Click);
        }

        if (Input.GetMouseButton(0)) //to rotate the character when the mouse is dragged
        {
            toCharacter.modelTransform.Rotate(0f, (dragOrigin.x - Input.mousePosition.x)/3, 0f);
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }



    IEnumerator MoveHighlighter(GameObject whatToMove, Vector3 whereToMove, bool yLocked)
    {
        audioManager.PlaySound(Sounds.ButtonPress); //we play the sound here just because it's convenient, since this coroutine is only activated when you hit a button

        float yTarget = yLocked ? whatToMove.transform.position.y : whereToMove.y; //don't move along the y axis for the top indicator
        while(Vector3.Distance(whatToMove.transform.position, whereToMove) > .001f)
        {
            whatToMove.transform.position = Vector3.Lerp(whatToMove.transform.position, new Vector3(whereToMove.x, yTarget, whatToMove.transform.position.z), .05f);
            yield return null;
        }

        yield return null;
    }
}