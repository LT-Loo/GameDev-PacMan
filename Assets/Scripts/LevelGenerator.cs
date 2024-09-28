using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    public GameObject manualLayout;
    private Transform layout;

    private int[,] levelMap = {
        { 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 4 },
        { 2, 6, 4, 0, 0, 4, 5, 4, 0, 0, 0, 4, 5, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 4, 4, 4, 3, 5, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 },
        { 2, 5, 3, 4, 4, 3, 5, 3, 3, 5, 3, 4, 4, 4 },
        { 2, 5, 3, 4, 4, 3, 5, 4, 4, 5, 3, 4, 4, 3 },
        { 2, 5, 5, 5, 5, 5, 5, 4, 4, 5, 5, 5, 5, 4 },
        { 1, 2, 2, 2, 2, 1, 5, 4, 3, 4, 4, 3, 0, 4 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 3, 4, 4, 3, 0, 3 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 2, 5, 4, 4, 0, 3, 4, 4, 0 },
        { 2, 2, 2, 2, 2, 1, 5, 3, 3, 0, 4, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 4, 0, 0, 0 }
    };

    public GameObject innerCorner, innerWall, jointWall, outerCorner, outerWall, path;
    public GameObject pellet, powerPellet;

    private Tile[,] tiles;
    int layerMask;

    void Start()
    {
        layerMask = LayerMask.NameToLayer("Layout");
        
        Destroy(manualLayout);

        layout = new GameObject("Layout Generate").GetComponent<Transform>();
        layout.position = Vector3.zero;
        layout.rotation = Quaternion.identity;
        layout.gameObject.layer = layerMask;

        tiles = new Tile[levelMap.GetLength(0), levelMap.GetLength(1)];

        for (int r = 0; r < levelMap.GetLength(0); r++) {
            GameObject row = new GameObject("R" + r);
            row.transform.position = new Vector3(0, -r, 0);
            row.transform.rotation = Quaternion.identity;
            row.transform.parent = layout;

            for (int c = 0; c < levelMap.GetLength(1); c++) {
                Vector3 position = new Vector3(c, -r, 0);
                Quaternion rotation = Quaternion.identity;
                GameObject tile = path;
                List<int> match = new List<int>();

                if (levelMap[r, c] == 1) {tile = outerCorner; match.Add(2);} 
                if (levelMap[r, c] == 2) {tile = outerWall; match.Add(2); match.Add(1); match.Add(7);} 
                if (levelMap[r, c] == 3) {tile = innerCorner; match.Add(3); match.Add(4);} 
                if (levelMap[r, c] == 4) {tile = innerWall; match.Add(3); match.Add(4); match.Add(7);}
                if (levelMap[r, c] == 7) {tile = jointWall; match.Add(2); match.Add(4);} 

                string matchStr = "";
                foreach (int m in match) {
                    matchStr += m.ToString() + " ";
                }
                Debug.Log(r + ", " + c + ", Match: " + matchStr);

                bool checkL = false; bool checkR = false; 
                bool checkU = false; bool checkB = false;

                if (r == 0) {
                    checkB = true;
                    if (c == 0) {checkR = true;} 
                    else if (c == levelMap.GetLength(1) - 1) {checkL = true;}
                    else {checkL = true; checkR = true;}
                }
                else if (r == levelMap.GetLength(0) - 1) {
                    checkU = true;
                    if (c == 0) {checkR = true;} 
                    else if (c == levelMap.GetLength(1) - 1) {checkL = true;}
                    else {checkL = true; checkR = true;}
                } 
                else {
                    checkU = true;
                    checkB = true;
                    if (c < levelMap.GetLength(1) - 1) {checkR = true;}
                    if (c > 0) {checkL = true;}
                }

                Debug.Log("To check R:" + checkR + ", L: " + checkL + ", U: " + checkU + ", B: " + checkB);

                bool Left = false; bool Right = false;
                bool Up = false; bool Bottom = false;

                if (checkL) {Left = checkLeft(r, c, match);}
                if (checkR) {Right = checkRight(r, c, match);}
                if (checkU) {Up = checkUp(r, c, match);}
                if (checkB) {Bottom = checkBottom(r, c, match);}

                Debug.Log("After check R:" + Right + ", L: " + Left + ", U: " + Up + ", B: " + Bottom);

                int trueCount = getTrueCount(Left, Right, Up, Bottom);

                match.Clear();
                match.Add(0); match.Add(5); match.Add(6);

                if (levelMap[r, c] == 1 || levelMap[r, c] == 3 || levelMap[r, c] == 7) {
                    
                    if (trueCount == 4) {
                        if (checkDiagonal(r - 1, c - 1, match)) {Bottom = false; Right = false;}
                        if (checkDiagonal(r - 1, c + 1, match)) {Bottom = false; Left = false;}
                        if (checkDiagonal(r + 1, c - 1, match)) {Up = false; Right = false;}
                        if (checkDiagonal(r + 1, c + 1, match)) {Up = false; Left = false;}
                    }

                    if (trueCount == 3) {
                        if (Up && Bottom) {
                            if (Left) {
                                if (checkDiagonal(r - 1, c - 1, match)) {Bottom = false;}
                                else {Up = false;}
                            }
                            if (Right) {
                                if (checkDiagonal(r - 1, c + 1, match)) {Bottom = false;}
                                else {Up = false;}
                            }
                        }
                        else if (Left && Right) {
                            if (Up) {
                                if (checkDiagonal(r - 1, c + 1, match)) {Left = false;}
                                else {Right = false;}
                            }
                            if (Bottom) {
                                if (checkDiagonal(r + 1, c + 1, match)) {Left = false;}
                                else {Right = false;}
                            }
                        }
                    }

                    if (trueCount == 1) {
                        if (Up || Bottom) {
                            if (!checkR) {Right = true;}
                            else {Left = true;}
                        }
                        else if (Left || Right) {
                            if (!checkU) {Up = true;}
                            else {Bottom = true;}
                        }
                    }

                    Debug.Log("After adjust R:" + Right + ", L: " + Left + ", U: " + Up + ", B: " + Bottom);

                    trueCount = getTrueCount(Left, Right, Up, Bottom);

                    if (trueCount == 2) {
                        if (Right && Up) {rotation = Quaternion.Euler(0f, 0f, 90f);}
                        else if (Right && Bottom) {rotation = Quaternion.identity;}
                        else if (Left && Up) {rotation = Quaternion.Euler(0f, 0f, 180f);}
                        else if (Left && Bottom) {rotation = Quaternion.Euler(0f, 0f, -90f);}
                    }
                }

                if (levelMap[r, c] == 2 || levelMap[r, c] == 4) {   
                    if (trueCount > 1) {
                        if (trueCount == 2) {
                            if ((Left != Right) && (Up != Bottom)) {
                                bool changed = false;
                                if (checkL && !Left && !changed) {Right = !checkLeft(r, c, match); changed = true;} 
                                if (checkR && !Right && !changed) {Left = !checkRight(r, c, match); changed = true;}
                                if (checkU && !Up && !changed) {Bottom = !checkUp(r, c, match); changed = true;}
                                if (checkB && !Bottom && !changed) {Up = !checkBottom(r, c, match);}
                                Debug.Log("Wall Change:" + Right + ", L: " + Left + ", U: " + Up + ", B: " + Bottom);
                            }
                        }
                        if (Left && Right) {rotation = Quaternion.Euler(0f, 0f, 90f);}
                    }

                    trueCount = getTrueCount(Left, Right, Up, Bottom);

                    if (trueCount == 1) {
                        if (Left || Right) {rotation = Quaternion.Euler(0f, 0f, 90f);}
                    }
                }
                
                tiles[r, c] = new Tile(levelMap[r, c], position, rotation, r, c);
                GameObject newTile = Instantiate(tile, position, rotation, row.transform);
                newTile.layer = layerMask;

                if (levelMap[r, c] == 5) {
                    GameObject newPellet = Instantiate(pellet, position, Quaternion.identity, newTile.transform);
                    newPellet.tag = "Pellet";
                    newPellet.layer = layerMask;
                }

                if (levelMap[r, c] == 6) {
                    GameObject newPellet = Instantiate(powerPellet, position, Quaternion.identity, newTile.transform);
                    newPellet.tag = "Pellet";
                    newPellet.layer = layerMask;
                }

            }
        }

        Vector3 layoutPos = new Vector3(layout.position.x + 2 * levelMap.GetLength(1) - 1, layout.position.y, layout.position.z);
        Transform layoutUR = Instantiate(layout, layoutPos, Quaternion.Euler(0f, 180f, 0f));

        layoutPos = new Vector3(layout.position.x + 2 * levelMap.GetLength(1) - 1, layout.position.y - 2 * levelMap.GetLength(0) + 2, layout.position.z);
        Transform layoutBR = Instantiate(layout, layoutPos, Quaternion.Euler(-180f, 180f, 0f));
        Destroy(layoutBR.transform.Find("R" + (levelMap.GetLength(0) - 1)).gameObject);

        layoutPos = new Vector3(layout.position.x, layout.position.y - 2 * levelMap.GetLength(0) + 2, layout.position.z);
        Transform layoutBL = Instantiate(layout, layoutPos, Quaternion.Euler(-180f, 0f, 0f));
        Destroy(layoutBL.transform.Find("R" + (levelMap.GetLength(0) - 1)).gameObject);

        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        foreach (GameObject pellet in pellets) {
            pellet.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

    }


    bool checkRight(int r, int c, List<int> match) {
        return match.Contains(levelMap[r, c + 1]);
    }

    bool checkLeft(int r, int c, List<int> match) {
        return match.Contains(levelMap[r, c - 1]);
    }

    bool checkUp(int r, int c, List<int> match) {
        return match.Contains(levelMap[r - 1, c]);
    }

    bool checkBottom(int r, int c, List<int> match) {
        return match.Contains(levelMap[r + 1, c]);
    }

    bool checkDiagonal(int r, int c, List<int> match) {
        return match.Contains(levelMap[r, c]);
    }

    int getTrueCount(bool left, bool right, bool up, bool bottom) {
        int trueCount = 0;
        if (left) {trueCount++;}
        if (right) {trueCount++;}
        if (up) {trueCount++;}
        if (bottom) {trueCount++;}

        return trueCount;
    }

}
