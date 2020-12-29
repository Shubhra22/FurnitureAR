using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    private GameObject furniture;
    
    [SerializeField] private ButtonManager buttonPrefab;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private List<Item> _items;
    
    private int id = 0;
    
    private static DataHandler instance;
    public static DataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>();
            }
            return instance;
        }
        
    }
    
    private void Start()
    {
        _items = new List<Item>();
        LoadItems();
        CreateButtons();
    }

    void LoadItems()
    {
        var items_obj =Resources.LoadAll("Items",typeof(Item));
        foreach (var item in items_obj)
        {
            _items.Add(item as Item);
        }
    }
    void CreateButtons()
    {
        foreach (Item i in _items)
        {
            ButtonManager b = Instantiate(buttonPrefab, buttonContainer.transform);
            b.ItemId = id;
            b.ButtonTexture = i.itemImage;
            id++;
        }
    }

    public void SetFurinute(int id)
    {
        furniture = _items[id].itemPrefab;
    }

    public GameObject GetFurniture()
    {
        return furniture;
    }
}
