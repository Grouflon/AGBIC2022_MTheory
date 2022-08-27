using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public Renderer renderer
    {
        get { return m_renderer; }
    }

    public bool grabbed = false;

    public void setData(BoardDefinition _definition, Color _backgroundColor)
    {
        definition = _definition;
        backgroundColor = _backgroundColor;
        updateData();
    }

    public void rotate(int _amount)
    {
        m_rotation = (((m_rotation + _amount) % 4) + 4) % 4;
        transform.rotation = Quaternion.Euler(new Vector3(m_rotation * 90f, 0f, 0f));
    }

    private struct Node
    {
        public int coord;
    }

    [SerializeField]
    private BoardDefinition definition;
    [SerializeField]
    private Color backgroundColor;

    private List<Node> m_nodes;
    private List<Link> m_links;

    private Renderer m_renderer;
    private MaterialPropertyBlock m_propertyBlock;
    private int m_rotation = 0;

    void updateData()
    {
        m_renderer = GetComponentInChildren<Renderer>();
        m_propertyBlock = new MaterialPropertyBlock();

        SanitizeLinkList(in definition.links, out m_nodes, out m_links);

        // NODES
        {
            int nodeCount = m_nodes.Count;
            Vector4[] nodesArray = new Vector4[nodeCount];
            for (int i = 0; i < nodeCount; ++i)
            {
                nodesArray[i] = new Vector4(m_nodes[i].coord, 0, 0, 0);
            }
            if (nodeCount > 0)
            {
                m_propertyBlock.SetVectorArray("_nodes", nodesArray);
            }
            m_propertyBlock.SetInt("_nodeCount", nodeCount);
        }

        // LINKS
        {
            int linkCount = m_links.Count;
            Vector4[] linksArray = new Vector4[linkCount];
            for (int i = 0; i < linkCount; ++i)
            {
                linksArray[i] = new Vector4(m_links[i].begin, m_links[i].end, 0, 0);
            }
            if (linkCount > 0)
            {
                m_propertyBlock.SetVectorArray("_links", linksArray);
            }
            m_propertyBlock.SetInt("_linkCount", linkCount);
        }
        
        m_propertyBlock.SetColor("_nodeColor", definition.nodeColor);
        m_propertyBlock.SetColor("_Color", definition.nodeColor); // edge
        m_propertyBlock.SetColor("_bgColor", backgroundColor);

        m_renderer.SetPropertyBlock(m_propertyBlock);
    }

    static void SanitizeLinkList(in Link[] _links, out List<Node> _outNodes, out List<Link> _outLinks)
    {
        const int gridSize = 9;

        _outNodes = new List<Node>();
        _outLinks = new List<Link>();

        if (_links == null)
            return;

        Dictionary<int, Link> uniqueLinks = new Dictionary<int, Link>();
        Dictionary<int, Node> uniqueNodes = new Dictionary<int, Node>();

        for (int i = 0; i < _links.Length; ++i)
        {
            Link inLink = _links[i];
            bool isBeginValid = inLink.begin >= 0 && inLink.begin < gridSize;
            if (isBeginValid)
            {
                Node node = new Node();
                node.coord = inLink.begin;
                uniqueNodes[inLink.begin] = node;
            }

            bool isEndValid = inLink.end >= 0 && inLink.end < gridSize;
            if (isEndValid)
            {
                Node node = new Node();
                node.coord = inLink.end;
                uniqueNodes[inLink.end] = node;
            }

            if (!isBeginValid || !isEndValid)
                continue;

            uniqueLinks[inLink.id] = inLink;
        }

        foreach (KeyValuePair<int, Node> pair in uniqueNodes)
        {
            _outNodes.Add(pair.Value);
        }
        foreach (KeyValuePair<int, Link> pair in uniqueLinks)
        {
            _outLinks.Add(pair.Value);
        }
    }

    void Awake()
    {
        updateData();
    }

    void Update()
    {
    }

    void OnValidate()
    {
        Awake();
    }
}
