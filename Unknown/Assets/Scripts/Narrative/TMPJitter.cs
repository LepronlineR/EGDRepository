using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPJitter : MonoBehaviour
{
    TMP_Text textMesh;
    Mesh mesh;
    Vector3[] vertices;
    List<int> wordIndexes;
    List<int> wordLengths;
    private bool activate = false;
    public bool ignore = true;

    [SerializeField] [Range(0.0f, 5.0f)] float editableValueOne;
    [SerializeField] [Range(0.0f, 5.0f)] float editableValueTwo;
 
    // Start is called before the first frame update
    void Start(){
        textMesh = GetComponent<TMP_Text>();
    }

    public void AllowOtherActivate(bool click){
        activate = click;
    }
 
    // Update is called once per frame
    void Update(){
        if(activate || ignore){
            textMesh.ForceMeshUpdate();
            mesh = textMesh.mesh;
            vertices = mesh.vertices;
    
            for (int w = 0; w < textMesh.textInfo.characterCount; w++){
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[w];
                int index = c.vertexIndex;
                
                Vector3 offset = Jitter(Time.time + w);
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }
 
            mesh.vertices = vertices;
            textMesh.canvasRenderer.SetMesh(mesh);
        }
    }
 
    Vector2 Jitter(float time) {
        return new Vector2(Mathf.Sin(time*editableValueOne), Mathf.Cos(time*editableValueTwo));
    }
}
