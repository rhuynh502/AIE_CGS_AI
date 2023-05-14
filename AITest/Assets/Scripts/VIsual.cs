using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VIsual : MonoBehaviour
{
    [SerializeField] private AIPlayer player;
    private Network networkToDisplay;
    private List<Neuron> neuronList;
    private List<Connection> connections;

    [SerializeField] private Image node;
    [SerializeField] private Image connection;

    private List<Image> nodesToDraw = new List<Image>();

    // Start is called before the first frame update
    private void Start()
    {
        networkToDisplay = player.GetNetwork();
        neuronList = networkToDisplay.GetNeurons();
        connections = networkToDisplay.GetConnections();
        int howManyInLayer = 0;
        int prevLayer = 0;
        for(int i = 0; i < neuronList.Count; i++)
        {
            Image newNode = Instantiate(node);
            newNode.transform.SetParent(transform, false);
            newNode.transform.localPosition = new Vector3(-920 + 50 * neuronList[i].layer, 480 - 50 * howManyInLayer, 0);

            if (prevLayer == neuronList[i].layer)
                howManyInLayer++;
            else
                howManyInLayer = 0;
            nodesToDraw.Add(newNode);
            prevLayer = neuronList[i].layer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < neuronList.Count; i++)
        {
            if (neuronList[i].outputValue < 0.2f)
                nodesToDraw[i].color = Color.Lerp(nodesToDraw[i].color, Color.red, Time.deltaTime * 3);
            else if (neuronList[i].outputValue < 0.5f)
                nodesToDraw[i].color = Color.Lerp(nodesToDraw[i].color, Color.magenta, Time.deltaTime * 3);
            else if (neuronList[i].outputValue < 0.7f)
                nodesToDraw[i].color = Color.Lerp(nodesToDraw[i].color, Color.yellow, Time.deltaTime * 3);
            else
                nodesToDraw[i].color = Color.Lerp(nodesToDraw[i].color, Color.green, Time.deltaTime * 3);
        }

    }
}
