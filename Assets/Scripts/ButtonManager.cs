using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Button btn;
    [SerializeField]private RawImage buttonImage;

    private int _itemId;
    private Sprite _buttonTexture;
    
    public Sprite ButtonTexture
    {
        set
        {
            _buttonTexture = value;
            buttonImage.texture = _buttonTexture.texture;

        }
        
    }

    public int ItemId
    {
        set { _itemId = value; }
    }
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SelectObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.OnEntered(gameObject))
        {
            transform.DOScale(Vector3.one * 2, 0.2f);
            //transform.localScale = Vector3.one * 2;
        }
        else
        {
            transform.DOScale(Vector3.one, 0.2f);
            //transform.localScale = Vector3.one;
        }
    }

    void SelectObject()
    {
        DataHandler.Instance.SetFurniture(_itemId);
    }
}
