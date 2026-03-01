using UnityEngine;

//to run this code successfully, must setCellSize and theninitialize the code.
public class GridMeshOverlay : MonoBehaviour
{
    public Terrain terrain;
    private int gridWidth, gridLength;
    private float cellSize;
    private Mesh mesh;
    //Vertices are the points in 3D space
    private Vector3[] vertices;
    //This tells unity which 3 vertices make a triangle face.
    //User triangles as unity cannot render squares for 3D.
    private int[] triangles;
    private Color[] colors;   

    private bool colorArrChanged = false; 

    public void Initialize(int gridW, int gridL, float cellSi)
    {
        gridWidth = gridW;
        gridLength = gridL;
        cellSize = cellSi;
        GenerateMesh();
    }
    

    void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //+1 as all cells share vertices (corners) so the number of corners
        //is the length or width + 1
        vertices = new Vector3[(gridWidth+1)*(gridLength+1)];
        ///*6 as 2 triangles * 3 vertices per triangle = 6 indices per cell
        triangles = new int[gridWidth*gridLength*6];
        colors = new Color[vertices.Length];

        CreateVertices();
        CreateTriangles();
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        //Normals determine how light hits a surface, but using
        //Unlit material = lighting is minimal effect. 
        //this just ensures mesh shades correctly
        mesh.RecalculateNormals();
    }
    void CreateVertices()
    {
        float heighOffset = 0.6f;
        for(int z=0; z<=gridLength; z++)
        {
            for(int x=0; x<=gridWidth; x++)
            {
               //z*grid gets the "row", then +x gets the "column"
               //Extra brackets added for clarity 
                int index = (z*(gridWidth+1))+x;
                float worldX = x*cellSize;
                float worldZ = z*cellSize;

                float terrainHeight = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
                vertices[index] = new Vector3(
                    worldX,
                    terrainHeight + heighOffset,
                    worldZ
                );

                //transparent colour (set RGBS to 0000)
                colors[index] = Color.clear;
            }
        }
    }

//???why do we need triangles? why only 2?
    void CreateTriangles()
    {
        int triangleIndex = 0;
        for(int z=0; z<gridLength; z++)
        {
            for(int x=0; x<gridWidth; x++)
            {
                int vertexIndex = z*(gridWidth+1)+x;

                //Math for this section and logic:
                //assume Length, width = 1 (as this is one square with 4 vertices)
                //0  1 <-- 0 = bottomLeft, 1 = bottom right
                //2  3 <-- 2 = topLeft, 3 = top right
                //triangle 1 = 0,1,2 = index, index+length, index+length+2
                //triangle 2 = 1,2,3 = index+1, index+length+1, index+length+2 
                
                //The row is 1 longer than the gridwidth due to the shared vertices
                //So it starts at 0, and will have one more at the end of the row
                int vertexRowLength = gridWidth+1;

                int bottomLeft = vertexIndex;
                int bottomRight = vertexIndex + 1;
                int topLeft = vertexIndex + vertexRowLength;
                int topRight = vertexIndex + vertexRowLength+1;

                //Triangle 1
                // triangles[triangleIndex++] = vertexIndex;
                // triangles[triangleIndex++] = vertexIndex+gridWidth+1;
                // triangles[triangleIndex++] = vertexIndex+1;

                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomRight;

                //Triangle 2
                // triangles[triangleIndex++] = vertexIndex + 1;
                // triangles[triangleIndex++] = vertexIndex + gridWidth + 1;
                // triangles[triangleIndex++] = vertexIndex + gridWidth + 2;

                triangles[triangleIndex++] = bottomRight;
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = topRight;
            }
        }
    }

    public void SetCellColour(int x, int z, Color color)
    {        
        //Color is stored per vertex, so need to get the vertices of 
        //the square to change the colour of a square.
        int bottomLeft = z*(gridWidth+1)+x;
        int bottomRight = bottomLeft + 1;
        int topLeft = bottomLeft + gridWidth + 1;
        int topRight = topLeft + 1;

        colors[bottomLeft] = color;
        colors[bottomRight] = color;
        colors[topLeft] = color;
        colors[topRight] = color;

        mesh.colors = colors;
        //signal that the colours have changed. 
        colorArrChanged = true;
    }

    void LateUpdate()
    {
        //Added so that colours are only updated once per frame as this array is very large. 
        if (colorArrChanged)
        {
            mesh.colors = colors;
            colorArrChanged = false;
        }
    }

}
