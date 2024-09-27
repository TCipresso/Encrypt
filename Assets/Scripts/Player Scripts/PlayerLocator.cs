using UnityEngine;

public class PlayerLocator : MonoBehaviour
{
    public LayerMask floorLayer;
    public Node currentNode;

    private void Update()
    {
        LocateCurrentNode();
    }

    private void LocateCurrentNode()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        // Raycast downwards to find the floor node
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayer))
        {
            currentNode = hit.transform.GetComponent<Node>();
            if (currentNode != null)
            {
                Debug.Log("Current Node: " + currentNode.GetCoordinates());
            }
            else
            {
                Debug.LogWarning("Raycast hit an object without a Node component.");
                currentNode = null;
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any object on the specified floor layer.");
            currentNode = null;
        }
    }

    public Node GetCurrentNode()
    {
        // Check if the node is still null
        if (currentNode == null)
        {
            Debug.LogError("Current Node is null. Check if the player is above a valid node.");
        }
        return currentNode;
    }
}
