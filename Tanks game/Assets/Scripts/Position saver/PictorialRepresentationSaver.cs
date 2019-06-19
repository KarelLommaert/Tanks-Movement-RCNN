using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PictorialRepresentationSaver : MonoBehaviour
{
    public GameObject SavingDataTextPrefab;
    private Image _savingImage;

    [SerializeField] private bool _generateMapLayout = false;
    [SerializeField] private Texture2D _mapLayout = null;
    [SerializeField] private /*const*/ int _textureWidth = 50; // Must be at least equal to map size?
    [SerializeField] private /*const*/ int _textureHeight = 50;
    [SerializeField] private /*const*/ float _mapSize = 100.0f;
    [SerializeField] private Transform _wallsParent = null;
    [SerializeField] private Color _wallsColor = Color.grey;
    [SerializeField] private string _playerName = "Player1";

    [SerializeField] private bool _resetSaveFileIndex = false;

    private int _saveFileIndex = 0;
    //private int _sessionID = 0;

    private void Awake()
    {
        if (_generateMapLayout)
            _mapLayout = CreateMapLayout();

        if (_resetSaveFileIndex)
        {
            _saveFileIndex = 0;
            PlayerPrefs.SetInt("SaveFileIndex", _saveFileIndex);
        }

        _saveFileIndex = PlayerPrefs.GetInt("SaveFileIndex");
        //else
        //    _mapLayout = 
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("SaveFileIndex", _saveFileIndex);
    }

    private Texture2D CreateTexture2D(ObjectPixelRepresentation[] objects, int sequenceIndex)
    {
        // Create new texture with map layout as initial pixels
        Texture2D texture = new Texture2D(_textureWidth, _textureHeight);
        texture.SetPixels(_mapLayout.GetPixels());

        // Fill in pixels of all objects
        for (int i = 0; i < objects.Length; i++)
        {
            //texture.SetPixel((int)(objects[i].NormalizedPosition.x * _textureWidth), (int)(objects[i].NormalizedPosition.y * _textureHeight), objects[i].PixelColor);
            Vector2 normalizedPosition = new Vector2((objects[i].Position.x / _mapSize) + 0.5f, (objects[i].Position.y / _mapSize) + 0.5f); // +0.5f cause arena is centered and normalized pos must be [0, 1]
            DrawPixelCircle(texture, new Vector2((int)(normalizedPosition.x * _textureWidth), (int)(normalizedPosition.y * _textureHeight)), objects[i].Radius, objects[i].PixelColor);
        }
        texture.Apply();

        return texture;
    }

    private void DrawPixelCircle(Texture2D tex, Vector2 pos, int radius, Color color, bool filled = true)
    {
        //x^2 + y^2 = r^2
        int r2 = radius * radius;

        if (filled)
        {
            // Filled circle
            for (int y = -radius; y < radius; y++)
            {
                for (int x = -radius; x < radius; x++)
                {
                    // Check if pixel is within radius
                    if (x * x + y * y <= r2)
                    {
                        // Check if pixel exists
                        if ((int)pos.x + x >= 0 && (int)pos.y + y >= 0)
                            tex.SetPixel((int)pos.x + x, (int)pos.y + y, color);
                    }
                }
            }
        }
        else
        {
            // Empty circle
            for (int x = 1; x <= radius; x++)
            {
                int y = (int)Mathf.Sqrt(-x * x + r2);

                // Not all pixels in screen, check which ones to draw
                if ((int)pos.x - x < 0 || (int)pos.y - y < 0)
                {
                    if ((int)pos.x - x >= 0)
                        tex.SetPixel((int)pos.x - x, (int)pos.y + y, color);
                    else if ((int)pos.y - y >= 0)
                        tex.SetPixel((int)pos.x + x, (int)pos.y - y, color);

                    // Always drawable
                    tex.SetPixel((int)pos.x + x, (int)pos.y + y, color);
                }
                // All pixels drawable
                else
                {
                    tex.SetPixel((int)pos.x + x, (int)pos.y + y, color);
                    tex.SetPixel((int)pos.x + x, (int)pos.y - y, color);
                    tex.SetPixel((int)pos.x - x, (int)pos.y + y, color);
                    tex.SetPixel((int)pos.x - x, (int)pos.y - y, color);
                }
            }
        }
    }

    public void SaveSequenceToPNG(Texture2D[] textures, string directoryPath, string fileName)
    {
        Directory.CreateDirectory(directoryPath);

        for (int i = 0; i < textures.Length; i++)
        {
            byte[] bytes = textures[i].EncodeToPNG();
            Object.Destroy(textures[i]);
            File.WriteAllBytes(directoryPath + "/" + fileName + i + ".png", bytes);
        }

        ++_saveFileIndex;
    }

    public void SaveAllStoredSequences(bool saveJSon)
    {
        //_playerName = PlayerPrefs.GetString("PlayerName");
        //Sequence[] sequences = PositionSaverUpgrade.Instance.StoredSequences.ToArray();
        //for (int i = 0; i < sequences.Length; i++)
        //{
        //    ObjectPixelRepresentation[][] positions = sequences[i].Positions;
        //    Texture2D[] textures = new Texture2D[positions.Length];
        //    for (int j = 0; j < positions.Length; j++)
        //    {
        //        textures[j] = CreateTexture2D(positions[j], j);
        //    }
        //    string directoryPath;
        //    string fileName;
        //    switch (sequences[i].Type)
        //    {
        //        case SequenceType.Player:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Player/" + _playerName + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.Timer:
        //            directoryPath = Application.dataPath + "/../SavedSequences/GameplayStyle/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.LowHP:
        //            directoryPath = Application.dataPath + "/../SavedSequences/GameplayStyle/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.GameOpening:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.UnlimitedAmmo:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.Kill:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.Death:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.SelfHit:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.EnemyHit:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.GetHit:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        case SequenceType.HealthPickup:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //        default:
        //            directoryPath = Application.dataPath + "/../SavedSequences/Default/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
        //            fileName = "PictorialRepresentation";
        //            break;
        //    }
        //    SaveSequenceToPNG(textures, directoryPath, fileName);
        //}

        //// Also save to JSON?
        ////SequenceWrapper wrapper = new SequenceWrapper(sequences);
        ////string jsonString = JsonUtility.ToJson(wrapper);
        //string jsonString = _playerName + "\n";
        //for (int i = 0; i < sequences.Length; i++)
        //{
        //    jsonString += JsonUtility.ToJson(sequences[i].Type);
        //    for (int j = 0; j < sequences[i].Positions.Length; j++)
        //    {
        //        for (int k = 0; k < sequences[i].Positions[j].Length; k++)
        //        {
        //            jsonString += JsonUtility.ToJson(sequences[i].Positions[j][k]);
        //            jsonString += "\n";
        //        }
        //    }
        //}
        //File.WriteAllText(Application.dataPath + "/../SavedSequences/JSON_" + _saveFileIndex, jsonString);

        StartCoroutine(SaveSequencesOverTime(saveJSon));
    }

    private IEnumerator SaveSequencesOverTime(bool saveJSon)
    {
        Transform prefab = Instantiate(SavingDataTextPrefab).transform;
        _savingImage = prefab.Find("Foreground").GetComponent<Image>();
        _savingImage.fillAmount = 0.0f;
        yield return null;
        //bool saveJson = PlayerPrefs.GetInt("SaveToJSon") > 0;

        _playerName = PlayerPrefs.GetString("PlayerName");
        Sequence[] sequences = PositionSaverUpgrade.Instance.StoredSequences.ToArray();
        for (int i = 0; i < sequences.Length; i++)
        {
            ObjectPixelRepresentation[][] positions = sequences[i].Positions;
            Texture2D[] textures = new Texture2D[positions.Length];
            for (int j = 0; j < positions.Length; j++)
            {
                textures[j] = CreateTexture2D(positions[j], j);
            }
            string directoryPath;
            string fileName;
            switch (sequences[i].Type)
            {
                case SequenceType.Player:
                    directoryPath = Application.dataPath + "/../SavedSequences/Player/" + _playerName + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.NormalMode:
                    directoryPath = Application.dataPath + "/../SavedSequences/GameplayStyle/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.Timer:
                    directoryPath = Application.dataPath + "/../SavedSequences/GameplayStyle/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.LowHP:
                    directoryPath = Application.dataPath + "/../SavedSequences/GameplayStyle/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.GameOpening:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.UnlimitedAmmo:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.Kill:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.Death:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.SelfHit:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.EnemyHit:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.GetHit:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                case SequenceType.HealthPickup:
                    directoryPath = Application.dataPath + "/../SavedSequences/Action/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
                default:
                    directoryPath = Application.dataPath + "/../SavedSequences/Default/" + sequences[i].Type.ToString() + "/Sequence_" + _saveFileIndex;
                    fileName = "PictorialRepresentation";
                    break;
            }
            SaveSequenceToPNG(textures, directoryPath, fileName);

            if (saveJSon)
                _savingImage.fillAmount = ((float)i / (float)sequences.Length) * 0.3f;
            else
                _savingImage.fillAmount = (float)i / (float)sequences.Length;
            yield return null;
        }

        if (saveJSon)
        {
            // Also save to JSON?
            //SequenceWrapper wrapper = new SequenceWrapper(sequences);
            //string jsonString = JsonUtility.ToJson(wrapper);
            string jsonString = _playerName + "\n";
            for (int i = 0; i < sequences.Length; i++)
            {
                jsonString += JsonUtility.ToJson(sequences[i].Type);
                for (int j = 0; j < sequences[i].Positions.Length; j++)
                {
                    for (int k = 0; k < sequences[i].Positions[j].Length; k++)
                    {
                        jsonString += JsonUtility.ToJson(sequences[i].Positions[j][k]);
                        jsonString += "\n";
                    }
                }

                _savingImage.fillAmount = 0.3f + (((float)i / (float)sequences.Length) * 0.7f);
                yield return null;
            }
            File.WriteAllText(Application.dataPath + "/../SavedSequences/JSON_" + _saveFileIndex, jsonString);
        }

        SceneManager.LoadScene(0);
    }

    //private void SaveSequence(int length)
    //{
    //    // Get all paths from all position savers and the saved sequences of destroyed objects
    //    PositionSaver[] savers = FindObjectsOfType<PositionSaver>();
    //    PositionSequence[] seqs = GetComponent<SequenceSaver>().GetSequences(length);
    //    PictorialObjectPath[] paths = new PictorialObjectPath[savers.Length + seqs.Length];
    //    for (int i = 0; i < savers.Length; i++)
    //    {
    //        paths[i] = new PictorialObjectPath(savers[i].Color, savers[i].GetSequencePositions(length), savers[i].Radius);
    //    }

    //    for (int i = 0; i < seqs.Length; i++)
    //    {
    //        paths[savers.Length + i] = new PictorialObjectPath(seqs[i].RepresentationColor, seqs[i].Positions);
    //    }

    //    // Convert paths into an array of textures
    //    Texture2D[] textures = new Texture2D[length];
    //    for (int i = 0; i < length; i++)
    //    {
    //        ObjectPixelRepresentation[] pixels = new ObjectPixelRepresentation[paths.Length];
    //        for (int j = 0; j < pixels.Length; j++)
    //        {
    //            pixels[j] = paths[j].GetPixelAtIndex(i);
    //        }
    //        textures[i] = CreateTexture2D(pixels, i);
    //    }

    //    // Save those textures to PNGs
    //    SaveSequenceToPNG(textures);
    //}

    private void SaveSequenceUpgraded(int steps)
    {
        ObjectPixelRepresentation[][] positions = PositionSaverUpgrade.Instance.GetPositions(steps);
        Texture2D[] textures = new Texture2D[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            textures[i] = CreateTexture2D(positions[i], i);
        }

        // Save those textures to PNGs
        SaveSequenceToPNG(textures, Application.dataPath + "/../SavedSequences/Custom/Sequence_ " + _saveFileIndex, "Custom");
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Texture2D[] tex = new Texture2D[1];
        //    tex[0] = CreateMapLayout();
        //    SaveSequenceToPNG(tex);
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    //SaveSequenceUpgraded(100);
        //    SaveAllStoredSequences();
        //}
    }

    private Texture2D CreateMapLayout()
    {
        Texture2D tex = new Texture2D(_textureWidth, _textureHeight);
        // All walls are simple cubes so we can get their size using their scale
        Transform[] children = _wallsParent.GetComponentsInChildren<Transform>();
        List<Transform> walls = new List<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].CompareTag("wall"))
                walls.Add(children[i]);
        }

        for (int i = 0; i < walls.Count; i++)
        {
            Vector2 normalizedPos = new Vector2(walls[i].position.x / _mapSize, walls[i].position.z / _mapSize);
            int xPos = (int)(normalizedPos.x * _textureWidth);
            int zPos = (int)(normalizedPos.y * _textureHeight);
            float xScale = walls[i].localScale.x;
            float zScale = walls[i].localScale.z;
            float radAngle = -walls[i].localEulerAngles.y * Mathf.Deg2Rad;
            int pixelsPerScale = (int)(_textureWidth / _mapSize);

            // Rotate rect centered around origin
            Vector2 TL = new Vector2((-xScale / 2) * Mathf.Cos(radAngle) - (zScale / 2) * Mathf.Sin(radAngle), (-xScale / 2) * Mathf.Sin(radAngle) + (zScale / 2) * Mathf.Cos(radAngle));
            Vector2 TR = new Vector2((xScale / 2) * Mathf.Cos(radAngle) - (zScale / 2) * Mathf.Sin(radAngle), (xScale / 2) * Mathf.Sin(radAngle) + (zScale / 2) * Mathf.Cos(radAngle));
            Vector2 BL = new Vector2((-xScale / 2) * Mathf.Cos(radAngle) - (-zScale / 2) * Mathf.Sin(radAngle), (-xScale / 2) * Mathf.Sin(radAngle) + (-zScale / 2) * Mathf.Cos(radAngle));
            Vector2 BR = new Vector2((xScale / 2) * Mathf.Cos(radAngle) - (-zScale / 2) * Mathf.Sin(radAngle), (xScale / 2) * Mathf.Sin(radAngle) + (-zScale / 2) * Mathf.Cos(radAngle));

            // Actual rect (in pixels)
            Vector2 topLeftPos = new Vector2((((walls[i].position.x + TL.x) / _mapSize) * _textureWidth) + _textureWidth / 2,
                                            (((walls[i].position.z + TL.y) / _mapSize) * _textureHeight) + _textureHeight / 2);
            Vector2 topRightPos = new Vector2((((walls[i].position.x + TR.x) / _mapSize) * _textureWidth) + _textureWidth / 2,
                                            (((walls[i].position.z + TR.y) / _mapSize) * _textureHeight) + _textureHeight / 2);
            Vector2 botLeftPos = new Vector2((((walls[i].position.x + BL.x) / _mapSize) * _textureWidth) + _textureWidth / 2,
                                            (((walls[i].position.z + BL.y) / _mapSize) * _textureHeight) + _textureHeight / 2);
            Vector2 botRightPos = new Vector2((((walls[i].position.x + BR.x) / _mapSize) * _textureWidth) + _textureWidth / 2,
                                            (((walls[i].position.z + BR.y) / _mapSize) * _textureHeight) + _textureHeight / 2);

            // Square of pixels to check
            Vector2[] corners = new Vector2[] { topLeftPos, topRightPos, botLeftPos, botRightPos };
            int mostLeftPixX = (int)topLeftPos.x;
            int mostRightPixX = (int)topLeftPos.x;
            int highestPixZ = (int)topLeftPos.y;
            int lowestPixZ = (int)topLeftPos.y;

            for (int j = 0; j < corners.Length; j++)
            {
                if (corners[j].x < mostLeftPixX)
                    mostLeftPixX = (int)(corners[j].x);
                if (corners[j].x > mostRightPixX)
                    mostRightPixX = (int)(corners[j].x);
                if (corners[j].y > highestPixZ)
                    highestPixZ = (int)(corners[j].y);
                if (corners[j].y < lowestPixZ)
                    lowestPixZ = (int)(corners[j].y);
            }

            for (int x = mostLeftPixX; x <= mostRightPixX; x++)
            {
                for (int z = lowestPixZ; z <= highestPixZ; z++)
                {
                    // Check if point lies left of all of the rect edges to see if it has to be drawn
                    // if (x2 - x1) * (y - y1) - (x - x1) * (y2 - y1) >= 0, point is on left side of (ccw) edge (where (x1, y1) and (x2, y2) represent the edge and (x, y) represent the point
                    if ((topRightPos.x - topLeftPos.x) * (z - topLeftPos.y) - (x - topLeftPos.x) * (topRightPos.y - topLeftPos.y) > 0)
                        continue;
                    if ((topLeftPos.x - botLeftPos.x) * (z - botLeftPos.y) - (x - botLeftPos.x) * (topLeftPos.y - botLeftPos.y) > 0)
                        continue;
                    if ((botLeftPos.x - botRightPos.x) * (z - botRightPos.y) - (x - botRightPos.x) * (botLeftPos.y - botRightPos.y) > 0)
                        continue;
                    if ((botRightPos.x - topRightPos.x) * (z - topRightPos.y) - (x - topRightPos.x) * (botRightPos.y - topRightPos.y) > 0)
                        continue;

                    // Pixel is in the rectangle, so draw it
                    tex.SetPixel(x, z, _wallsColor);
                }
            }
        }

        return tex;
    }
}

public struct PictorialObjectPath
{
    public PictorialObjectPath(Color color, Vector2[] positions, int radius = 1)
    {
        PixelColor = color;
        PositionsOfObject = positions;
        Radius = radius;
    }

    public ObjectPixelRepresentation GetPixelAtIndex(int index)
    {
        return new ObjectPixelRepresentation(PixelColor, PositionsOfObject[index], Vector3.zero, Radius);
    }

    public Color PixelColor;
    public Vector2[] PositionsOfObject;
    public int Radius;
}

[System.Serializable]
public struct ObjectPixelRepresentation
{
    public ObjectPixelRepresentation(Color color, Vector2 position, Vector3 rot, int radius = 1)
    {
        PixelColor = color;
        Position = position;
        Radius = radius;
        Rotation = rot;
    }

    public Color PixelColor;
    public Vector2 Position;
    public Vector3 Rotation;
    public int Radius;
}

[System.Serializable]
public enum SequenceType
{
    Player,
    NormalMode,
    Timer,
    LowHP,
    GameOpening,
    UnlimitedAmmo,
    Kill,
    Death,
    SelfHit,
    EnemyHit,
    GetHit,
    HealthPickup
}
[System.Serializable]
public struct Sequence
{
    public Sequence(ObjectPixelRepresentation[][] positions, SequenceType type/*, bool win*/)
    {
        Positions = positions;
        Type = type;
        //Win = win;
    }
    public ObjectPixelRepresentation[][] Positions;
    public SequenceType Type;
    //public bool Win;
}

[System.Serializable]
public class SequenceWrapper
{
    public SequenceWrapper(Sequence[] sequences)
    {
        Sequences = sequences;
    }
    public Sequence[] Sequences;
}

//enum ActionType
//{
//    None,
//    GameOpening,
//    UnlimitedAmmo,
//    Kill,
//    SelfHit,
//    HealthPickup
//}