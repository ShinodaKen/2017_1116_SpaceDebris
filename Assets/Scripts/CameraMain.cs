using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGDebrisPool
{
    protected Queue<GameObject> m_objects = new Queue<GameObject>();

    public BGDebrisPool(int num, GameObject prefab, GameObject parent, Vector3 offset)
    {
        for ( int i = 0; i < num; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-4, 4), Random.Range(-4, 4), Random.Range(-2, 2));
            Quaternion rot = Quaternion.Euler(new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
            GameObject obj = GameObject.Instantiate(prefab, pos, rot, parent.transform);

            m_objects.Enqueue(obj);
        }

        AddOffset(offset);
    }
    public void AddOffset(Vector3 offset)
    {
        foreach (var o in m_objects)
        {
            Vector3 pos = o.transform.position;
            pos += offset;
            o.transform.position = pos;
        }
    }
}

public class CameraMain : MonoBehaviour {
    [SerializeField]
    protected GameObject m_playerObject;
    [SerializeField]
    protected GameObject m_background;
    [SerializeField]
    protected GameObject m_backgroundDebris;
    [SerializeField]
    protected GameObject m_backgroundRoot;

    protected Vector3 m_currentAnchor;
    protected LinkedList<LinkedList<BGDebrisPool>> m_BGDebrisPools = new LinkedList<LinkedList<BGDebrisPool>>();
    protected float m_poolSize = 8;

    protected Vector3 m_positionOffset = Vector3.zero;
    // Use this for initialization
    void Start()
    {
        m_currentAnchor = transform.position;
        m_positionOffset = transform.position - m_playerObject.transform.position;

        for (int i = 0; i < 5; ++i)
        {
            LinkedList<BGDebrisPool> pools = new LinkedList<BGDebrisPool>();
            for ( int j = 0; j < 5; ++j)
            {
                BGDebrisPool pool = new BGDebrisPool(8, m_backgroundDebris, m_backgroundRoot, new Vector3(m_poolSize * (j - 2), m_poolSize * (i - 2), -6.0f));
                pools.AddLast(pool);
            }
            m_BGDebrisPools.AddLast(pools);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pos = m_positionOffset + m_playerObject.transform.position;
        pos.z = m_positionOffset.z;
        transform.position = pos;

        pos.z = m_background.transform.position.z;
        m_background.transform.position = pos;

        Vector3 offset = Vector3.zero;
        if (pos.x > m_currentAnchor.x + m_poolSize)
        {
            m_currentAnchor.x += m_poolSize;
            offset.x = m_poolSize * 5;
            foreach(var pools in m_BGDebrisPools)
            {
                LinkedListNode<BGDebrisPool> node = pools.First;
                pools.RemoveFirst();
                pools.AddLast(node);
                node.Value.AddOffset(offset);
            }
        }
        else if (pos.x < m_currentAnchor.x - m_poolSize)
        {
            m_currentAnchor.x -= m_poolSize;
            offset.x = -m_poolSize * 5;
            foreach (var pools in m_BGDebrisPools)
            {
                LinkedListNode<BGDebrisPool> node = pools.Last;
                pools.RemoveLast();
                pools.AddFirst(node);
                node.Value.AddOffset(offset);
            }
        }

        if (pos.y > m_currentAnchor.y + m_poolSize)
        {
            m_currentAnchor.y += m_poolSize;
            offset.y = m_poolSize * 5;
            LinkedListNode<LinkedList<BGDebrisPool>> node = m_BGDebrisPools.First;
            m_BGDebrisPools.RemoveFirst();
            m_BGDebrisPools.AddLast(node);
            foreach (var pool in node.Value)
            {
                pool.AddOffset(offset);
            }
        }
        else if (pos.y < m_currentAnchor.y - m_poolSize)
        {
            m_currentAnchor.y -= m_poolSize;
            offset.y = -m_poolSize * 5;
            LinkedListNode<LinkedList<BGDebrisPool>> node = m_BGDebrisPools.Last;
            m_BGDebrisPools.RemoveLast();
            m_BGDebrisPools.AddFirst(node);
            foreach (var pool in node.Value)
            {
                pool.AddOffset(offset);
            }
        }
    }
}
