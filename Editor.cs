using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Editor : MonoBehaviour {
	private static Editor _instance;

	public static Editor instance
	{
		get
		{
			if(_instance == null)
			{_instance = GameObject.FindObjectOfType<Editor>();}
			return _instance;
		}
	}
	
	public bool m_EnabledEditor;
	public bool m_Deserialize;
	public GameObject[] m_HexTypePrefabs;

	private HexType m_SelectedType = HexType.Test;
	private Dictionary<int, HexTileSerializable> m_SavedHex;
	private List<HexTileSerializable> m_ToSerialize;

	// Use this for initialization
	void Start () 
	{
		if(m_EnabledEditor)
			m_SavedHex = new Dictionary<int, HexTileSerializable>();
		m_ToSerialize = new List<HexTileSerializable>();

		if(m_Deserialize)
		{
			DeSerializeHexes();
		}
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(20,40,80,20), "Serial"))
		{
			SerializeHexes();
		}
	}

	public void LeftClickedHex(GameObject hex)
	{
		HexTileSerializable toSave = new HexTileSerializable(hex.transform, m_SelectedType, hex.transform.GetInstanceID());
		m_SavedHex[hex.transform.GetInstanceID()] = toSave;
		Color transparent = hex.renderer.material.color;
		transparent.a = 0f;
		hex.renderer.material.color = transparent;
		GameObject child = GameObject.Instantiate(m_HexTypePrefabs[(int)m_SelectedType], hex.transform.position, hex.transform.rotation) as GameObject;
		child.transform.parent = hex.transform;
	}

	public void RightClickHex(GameObject hex)
	{
		m_SavedHex.Remove(hex.transform.GetInstanceID());
	}

	private void SerializeHexes()
	{
		m_ToSerialize.Clear();
		foreach(KeyValuePair<int, HexTileSerializable> kvp in m_SavedHex)
		{
			Collider[] hitColliders = Physics.OverlapSphere(kvp.Value.GetPosition(),1.5f);
			List<int> partners = new List<int>();
			foreach(Collider c in hitColliders)
			{
				int id = c.gameObject.transform.GetInstanceID();
				if(id != kvp.Key && m_SavedHex.ContainsKey(id))
				{
					partners.Add(id);
				}
			}
			var toAdd = kvp.Value;
			toAdd.AddNeighborList(partners);
			m_ToSerialize.Add(toAdd);
		}
		UnityXMLSerializer.SerializeToXMLFile<List<HexTileSerializable>>("Test.xml", m_ToSerialize, true);
	}

	private void DeSerializeHexes()
	{
		List<HexTileSerializable> test = UnityXMLSerializer.DeserializeFromXMLFile<List<HexTileSerializable>>(@"C:\Test.xml");
		foreach(HexTileSerializable hex in test)
		{
			GameObject spawnedHex = GameObject.Instantiate(m_HexTypePrefabs[(int)hex.type], hex.GetPosition(), Quaternion.identity) as GameObject;
			spawnedHex.transform.Rotate(new Vector3(0f,270f,0f));
		}
	}
}

[System.Serializable]
public enum HexType
{
	Test = 0,
	Gym = 1
};
