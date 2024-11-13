using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileView : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Image _imgPlayerHead;
    [SerializeField] private Button _buttonPhoto;
    [SerializeField] private GameObject _avatarItemPrefab;
    [SerializeField] private InputField _inputFieldName;
    [SerializeField] private InputField _inputFieldSignature;
    [SerializeField] private Button _backgroundButton;
    [SerializeField] private Image _imgBackground;
    [SerializeField] private Text _textNameCount;
    [SerializeField] private Text _textSignatureCount;
    [SerializeField] private Button _backButton;

    private Sprite _photoButtonSprite;
    
    private void Awake()
    {
        UserData.Instance.OnDataChanged += OnDataChanged;
        _photoButtonSprite = _imgBackground.sprite;
        Utils.RefreshListItems(_scrollRect,_avatarItemPrefab,Utils.HEAD_MAX,((i, o) =>
        {
            var headId = i + 1;
            o.transform.Find("Icon").GetComponent<Image>().sprite = Utils.GetUserHead(headId);
            o.GetComponent<Button>().onClick.AddListener(() =>
            {
                UserData.Instance.UserHead = headId;
            });
        }));
        _buttonPhoto.onClick.AddListener(() =>
        {
            Utils.PickImage((t) =>
            {
                if (t)
                {
                    UserData.Instance.CustomerHead = new SerializableTexture(t);
                    RefreshUI();
                }
            });
        });
        _backgroundButton.onClick.AddListener(() =>
        {
            Utils.PickImage((t) =>
            {
                if (t)
                {
                    UserData.Instance.Background = new SerializableTexture(t);
                    RefreshUI();
                }
            });
        });
        _inputFieldName.onEndEdit.AddListener((s) =>
        {
            UserData.Instance.UserName = s;
        });
        
        _inputFieldSignature.onEndEdit.AddListener((s) =>
        {
            UserData.Instance.UserDescription = s;
        });
        _backButton.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        RefreshUI();
    }
    
    private void OnDestroy()
    {
        UserData.Instance.OnDataChanged -= OnDataChanged;
    }

    private void RefreshUI()
    {
        _imgPlayerHead.sprite = Utils.GetMyHead(); //Utils.GetUserHead(UserData.Instance.UserHead);
        _inputFieldName.text = UserData.Instance.UserName;
        _textNameCount.text = UserData.Instance.UserName.Length + "/" + Utils.NAME_MAX;
        _inputFieldSignature.text = UserData.Instance.UserDescription;
        _textSignatureCount.text = UserData.Instance.UserDescription.Length + "/" + Utils.DESCRIPTION_MAX;
        if (UserData.Instance.Background != null && UserData.Instance.Background.textureData != null && UserData.Instance.Background.textureData.Length > 0)
        {
            _imgBackground.sprite = Sprite.Create(UserData.Instance.Background.ToTexture2D(),
                new Rect(Vector2.zero,
                    new Vector2(UserData.Instance.Background.width, UserData.Instance.Background.height)),
                new Vector2(0.5f, 0.5f));   
        }
        else
        {
            _imgBackground.sprite = _photoButtonSprite;
        }
    }
    
    private void OnDataChanged()
    {
        RefreshUI();
    }
    
    
    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/EditProfileView"),Main.Instance.canvas.transform);
    }
}
