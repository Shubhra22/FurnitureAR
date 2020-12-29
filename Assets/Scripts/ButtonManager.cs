using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    [SerializeField] private RawImage buttonImage;
    private Button btn;
    private int _itemId;
    private Sprite _buttonTexture;
    
    public int ItemId
    {
        set => _itemId = value;
    }
    public Sprite ButtonTexture 
    {
        set
        {
            _buttonTexture = value;
            buttonImage.texture = _buttonTexture.texture;
        }
    }
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
        }
        else
        {
            transform.DOScale(Vector3.one, 0.2f);
        }
    }

    void SelectObject()
    {
        DataHandler.Instance.SetFurinute(_itemId);
    }
}
