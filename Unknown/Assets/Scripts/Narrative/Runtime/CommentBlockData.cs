using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentBlockData {
    public List<string> ChildNodes = new List<string>();
    public Vector2 Position;
    public string Title= "Comment Block";
}