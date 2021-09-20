using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CharacterCreationScript : MonoBehaviour
{
    //initialize variables -- keeping everything directly related to the model in this script
    public Transform modelTransform;
    public Animator myAnim;
    public ParticleSystem myParticles;

    public GameObject shirtObj;
    public GameObject hairObj;
    public GameObject eyeObj;
    public GameObject mouthObj;

    Material shirtMat;
    public MeshFilter hairMesh;
    Material hairMat;
    public Material eyeDecal;
    public Material mouthDecal;

    public Texture2D[] shirtOptions;
    public Mesh[] hairOptions;
    public Sprite[] eyeOptions;
    public Sprite[] mouthOptions;

    int currentShirt = 0;
    int currentHair = 0;
    int currentEyes = 0;
    int currentMouth = 0;

    int currentShirtColor = 0;
    int currentHairColor = 0;
    int currentEyeColor = 0;

    //initializing possible color schemes for traits that could accept them, i.e. shirt, skin, hair
    public Color[][] possibleColorSchemes = new Color[][] {
        new Color[] { Color.white, Color.red, Color.blue, Color.green, Color.yellow, Color.gray, Color.magenta, Color.black },
        new Color[] { Color.white, new Color(142/255f, 91/255f, 5/255f), new Color(238/255f, 234/255f, 100/255f), new Color(20 / 255f, 20 / 255f, 20 / 255f), Color.gray, new Color(2/255f, 141/255f, 255/255f) },
        new Color[] { },
        new Color[] { }
    };



    private void Start() //ensure we're working with material instances, not the base materials
    {
        shirtMat = shirtObj.GetComponent<SkinnedMeshRenderer>().materials[0];
        hairMat = hairObj.GetComponent<MeshRenderer>().material;

        //these are necessary because unity doesn't process decal materials the same way as normal materials
        eyeDecal = eyeObj.GetComponent<DecalProjector>().material;
        eyeObj.GetComponent<DecalProjector>().material = new Material(eyeDecal);
        eyeDecal = eyeObj.GetComponent<DecalProjector>().material;

        mouthDecal = mouthObj.GetComponent<DecalProjector>().material;
        mouthObj.GetComponent<DecalProjector>().material = new Material(mouthDecal);
        mouthDecal = mouthObj.GetComponent<DecalProjector>().material;
    }



    public int[][] RetrieveCharacterTraits()
    {
        int[][] traits = new int[4][]; //Each int[] represents which selection and which color option
        
        traits[0] = new int[2] { currentShirt, currentShirtColor };
        traits[1] = new int[2] { currentHair, currentHairColor };
        traits[2] = new int[2] { currentEyes, currentEyeColor };
        traits[3] = new int[2] { currentMouth, 0 };

        return traits;
    }

    public void ApplyCharacterTraitChange(Traits whichToChange, int toChangeTo) //changes one of the main traits (shirt, hair, eye, etc.)
    {
        switch (whichToChange)
        {
            case Traits.Shirt:
                currentShirt = toChangeTo;
                shirtMat.mainTexture = shirtOptions[toChangeTo];
                break;
            case Traits.Hair:
                currentHair = toChangeTo;
                hairMesh.mesh = hairOptions[toChangeTo];
                break;
            case Traits.Eye:
                currentEyes = toChangeTo;
                eyeDecal.SetTexture("_BaseColorMap", eyeOptions[toChangeTo].texture);
                break;
            case Traits.Mouth:
                currentMouth = toChangeTo;
                mouthDecal.SetTexture("_BaseColorMap", mouthOptions[toChangeTo].texture);
                break;
        }
        myAnim.SetTrigger("Inspect");
        myParticles.Play(); //anim and particles for a little flair
    }

    public void ApplyCharacterColorChange(Traits whichToChange, int toChangeTo) //changes the color of the trait
    {
        switch (whichToChange)
        {
            case Traits.Shirt:
                currentShirtColor = toChangeTo;
                shirtMat.color = possibleColorSchemes[0][toChangeTo];
                break;
            case Traits.Hair:
                currentHairColor = toChangeTo;
                hairMat.color = possibleColorSchemes[1][toChangeTo];
                break;
            case Traits.Eye:
                break;
            case Traits.Mouth:
                break;
        }
    }

    public void UpdateCharacterModel() //Apply everything to make sure the character is up to date
    {
        shirtMat.mainTexture = shirtOptions[currentShirt];
        shirtMat.color = possibleColorSchemes[0][currentShirtColor];

        hairMesh.mesh = hairOptions[currentHair];
        hairMat.color = possibleColorSchemes[1][currentHairColor];

        eyeDecal.SetTexture("_BaseColorMap", eyeOptions[currentEyes].texture);

        mouthDecal.SetTexture("_BaseColorMap", mouthOptions[currentMouth].texture);
    }
}


public enum Traits //easier to understand than a random int
{
    Shirt,
    Hair,
    Eye,
    Mouth
};