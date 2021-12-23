using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class APIManager : MonoBehaviour
{
    //Variables

    //Public Variables
    public RawImage pokemonRawImage;
    public ArrayList pokemonTypeTextArray;
    public TMP_InputField inputField;


    //Private Variables
    private int currentIndex;
    private string currentPokemonName;
    private string currentPokemonType;
    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    //Start Function -- Set Raw Image to null.
    private void Start()
    {
        pokemonRawImage.texture = Texture2D.blackTexture;
    }

    //OnButtonPress Function -- Set PokemonIndex to a random number, set currentIndex to be the same as randomIndex and calls API to get the required data.
    public void OnButtonRandomPokemon()
    {
        int randomPokemonIndex = Random.Range(1, 808);
        currentIndex = randomPokemonIndex;

        pokemonRawImage.texture = Texture2D.blackTexture;
        
        StartCoroutine(GetPokemonAtIndex(randomPokemonIndex));
    }

    //GetPokemonAtIndex Function -- Get pokemon data from API based on generated random number.
    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        string pokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokemonInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);
        JSONNode pokemonTypes = pokemonInfo["types"];

        string pokemonName = pokemonInfo["name"];
        string pokemonSpriteURL = pokemonInfo["sprites"]["front_default"];
        string[] pokemonTypeName = new string[pokemonTypes.Count];

        for (int i = 0, j = pokemonTypes.Count - 1; i < pokemonTypes.Count; i++, j--)
        {
            pokemonTypeName[j] = pokemonTypes[i]["type"]["name"];
        }

        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokemonSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }
            
        pokemonRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokemonRawImage.texture.filterMode = FilterMode.Point;

        currentPokemonName = CapitalizeFirstLetter(pokemonName);
        currentPokemonType = CapitalizeFirstLetter(pokemonTypeName[0]);

        inputField.text = "Pokemon Name: " + currentPokemonName + "\r\nPokemon Type: " + currentPokemonType + "\r\nPokemon ID Numer: " + currentIndex;
    }

    //CapitalizeFirstLetter Function -- Capitalize first character of string
    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
