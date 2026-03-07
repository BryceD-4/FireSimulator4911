using UnityEngine;

//to run this code successfully, must setCellSize and theninitialize the code.
public class GridMeshOverlay : MonoBehaviour
{
    //Need access to the terrain for vertex heights.
    public Terrain terrain;
    //Used next items for iterating through grid cells
    private int gridWidth, gridLength;
    private float cellSize;

    //mesh contains vertices and multiple triangle arrays. 
    private Mesh mesh;
    //Vertices are the points in 3D space
    private Vector3[] vertices;
    //This tells unity which 3 vertices make a triangle face.
    //User triangles as unity cannot render squares for 3D.
    private int[] triangles;

    //The color array will hold the colour of each vertex
    private Color[] colors;   

    //This was added as only want the colour array to update onces per cycle. 
    //initially it was updating at each point it was changed and this was 
    //noticeably impacting performance.
    private bool colorArrChanged = false; 

    //Called from main controller (FireSimMain) following grid creation
    public void Initialize(int gridW, int gridL, float cellSi)
    {
        gridWidth = gridW;
        gridLength = gridL;
        cellSize = cellSi;
        GenerateMesh();
    }
    
    void GenerateMesh()
    {
        //Mesh creation following outline provided here: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Mesh.html
        mesh = new Mesh();
        //Gets the MeshFilter object of the object this is applied to and sets its mesh to the mesh object we are creating. 
        //Followed examplsprovided here: REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/MeshFilter-mesh.html
        GetComponent<MeshFilter>().mesh = mesh;
        //+1 as all cells share vertices (corners) so the number of corners
        //is the length or width + 1
        vertices = new Vector3[(gridWidth+1)*(gridLength+1)];
        ///*6 as 2 triangles * 3 vertices per triangle = 6 indices per cell
        triangles = new int[gridWidth*gridLength*6];
        colors = new Color[vertices.Length];

        //As outlined in the reference above, we build the vertices and then the triangles from those vertices
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
        //we want the mesh to sit just above the terrain
        //0.6f was chosen with trial and error, anything lower, the mesh was spotty over steep terrain
        float heightOffset = 0.6f;
        for(int z=0; z<=gridLength; z++)
        {
            for(int x=0; x<=gridWidth; x++)
            {
               //z*grid gets the "row", then +x gets the "column"
               //Extra brackets added for clarity 
                int index = (z*(gridWidth+1))+x;
                float worldX = x*cellSize;
                float worldZ = z*cellSize;
                //Get the height of the terrain at the point of interest
                float terrainHeight = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
                //Add a vertex at that point that is just above the terrain
                vertices[index] = new Vector3(
                    worldX,
                    terrainHeight + heightOffset,
                    worldZ
                );

                //transparent colour (set RGBS to 0000)
                //Colouring is added to vertex points, making them all translucent initially to just see the terrain itself.
                colors[index] = Color.clear;
            }
        }
    }

    //from the vertices created, we create the triangles from the squares which make up our grid
    void CreateTriangles()
    {
        //track the triangle creation with indexing
        int triangleIndex = 0;
        for(int z=0; z<gridLength; z++)
        {
            for(int x=0; x<gridWidth; x++)
            {
                // z*grid gets the "row", then +x gets the "column"
                int vertexIndex = z*(gridWidth+1)+x;

                //Math/ logic for this section:
                //assume Length & width = 1 (as this is one square with 4 vertices)
                //0  1 <-- 0 = bottomLeft, 1 = bottom right
                //2  3 <-- 2 = topLeft, 3 = top right
                //triangle 1 = 0,1,2 = index, index+length, index+length+1
                //triangle 2 = 1,2,3 = index+1, index+length+1, index+length+2 
                
                //The row is 1 longer than the gridwidth due to the shared vertices
                //So it starts at 0, and will have one more at the end of the row
                int vertexRowLength = gridWidth+1;
                //Get all vertices for the given "square
                int bottomLeft = vertexIndex;
                int bottomRight = vertexIndex + 1;
                int topLeft = vertexIndex + vertexRowLength;
                int topRight = vertexIndex + vertexRowLength+1;

                //create the two triangles from this square
                //Triangle 1
                triangles[triangleIndex++] = bottomLeft; //0
                triangles[triangleIndex++] = topLeft; //2
                triangles[triangleIndex++] = bottomRight; //1

                //Triangle 2
                triangles[triangleIndex++] = bottomRight; //1
                triangles[triangleIndex++] = topLeft; //2
                triangles[triangleIndex++] = topRight; //3
            }
        }
    }

    //Called by BurningCellManager when a cell is burning or is extinguished
    public void SetCellColour(int x, int z, Color color)
    {        
        //Color is stored per vertex, so need to get the vertices of 
        //the square to change the colour of the entire square.
        //Squares are used as want to ignite the entire cell of the grid which is being changed.
        int bottomLeft = z*(gridWidth+1)+x;
        int bottomRight = bottomLeft + 1;
        int topLeft = bottomLeft + gridWidth + 1;
        int topRight = topLeft + 1;

        //Update the colours array
        colors[bottomLeft] = color;
        colors[bottomRight] = color;
        colors[topLeft] = color;
        colors[topRight] = color;

        //signal that the colours have changed. 
        colorArrChanged = true;
    }

    //LateUpdate runs after all other updates() have run. 
    //REF: https://discussions.unity.com/t/what-is-the-difference-between-update-lateupdate-fixedupdate-and-when-i-should-use-them/26258/2
    void LateUpdate()
    {
        //Added so that colours are only updated once per frame as this array is very large.
        //Only performed if the colours array has been changed
        if (colorArrChanged)
        {
            mesh.colors = colors;
            colorArrChanged = false;
        }
    }

}
