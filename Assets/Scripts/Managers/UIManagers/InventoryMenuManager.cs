using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenuManager : MonoBehaviour, IInitializeScript, ICloseAllMenus
{
    public static InventoryMenuManager instance;

    [Header("Script References")]
    [SerializeField] PlayerToUI plrToUI;
    [SerializeField] InventoryComms invComms;
    [SerializeField] UIInputDetectir UIinp;

    [Header("Canvas")]
    [SerializeField] GameObject canvas;
    [SerializeField] InventorySystem inventorySystem;

    [Header("Components")]
    [SerializeField] GameObject inventoryMenuPrefab;
    [SerializeField] GameObject inventoryMenuWorld;
    [SerializeField] GameObject inventorySlots;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDesc;
    [SerializeField] Image healthStatus;
    [SerializeField] GameObject currentDraggingItem;
    [SerializeField] GameObject hoveredItem;
    [SerializeField] private GameObject currentItemUI;
    [SerializeField] private GameObject currentItem;


    [Header("Variables")]
    [SerializeField] List<GameObject> listOfItems = new List<GameObject>();
    private float maxHealthStat = 150f;


    private void Awake()
    {
        if(instance != null & instance != this)
        {

        }
        else
        {
            instance = this;
        }
    }

    public void DeInitializeScript()
    {
        inventorySystem.OnAddItem -= OnAddItemReceiver;
        invComms.OnHoveredItemUI -= OnHoveredItemUIReceiver;
        UIinp.OnEInputEventSender -= OnEInputEventSenderReceiver;
        invComms.OnExitItemUI -= OnExitItemUIReceiver;
    }

    public void InitializeScript()
    {

        inventorySystem.OnAddItem += OnAddItemReceiver;
        invComms.OnHoveredItemUI += OnHoveredItemUIReceiver;
        UIinp.OnEInputEventSender += OnEInputEventSenderReceiver;
        invComms.OnExitItemUI += OnExitItemUIReceiver;
    }

    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }




    public void OpenInventoryScreenReceiver(object sender, System.EventArgs e)
    {
        InventoryMenuController();
    }
    IEnumerator IsInventoryDebounce;
    IEnumerator InventoryDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        var isPaused = GameManagers.instance.GetIsPaused();
        if (isPaused == false)
        {
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
            GameManagers.instance.PauseGame();
            GameManagers.instance.SetStateToOnMenu();
            IsInventoryDebounce = null;
        }
        else if (isPaused == true)
        {
            GameManagers.instance.UnpauseGame();
            GameManagers.instance.SetStateToPlaying();
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
            IsInventoryDebounce = null;
        }
    }
    public void InventoryMenuController()
    {
        if(GameManagers.instance.GetGameState() == GameManagers.GameState.OnMenu && inventoryMenuWorld.activeSelf == false)
        {

        }
        else
        {
            if (IsInventoryDebounce == null)
            {
                if (GameManagers.instance.GetGameState() == GameManagers.GameState.Playing)
                {
                    if (inventoryMenuWorld == null)
                    {
                        IsInventoryDebounce = InventoryDebounce();
                        StartCoroutine(IsInventoryDebounce);
                        inventoryMenuWorld = Instantiate(inventoryMenuPrefab, canvas.transform);
                        inventoryMenuWorld.SetActive(true);
                        ListSlots();
                        //HealthStatusUpdate();
                    }
                    else if (inventoryMenuWorld != null)
                    {
                        IsInventoryDebounce = InventoryDebounce();
                        StartCoroutine(IsInventoryDebounce);
                        inventoryMenuWorld.SetActive(true);
                        ListSlots();
                        HealthStatusUpdate();
                    }
                }
                else if(GameManagers.instance.GetGameState() == GameManagers.GameState.OnMenu && inventoryMenuWorld.activeSelf == true)
                {
                    IsInventoryDebounce = InventoryDebounce();
                    StartCoroutine(IsInventoryDebounce);
                    CloseMenu();
                }
            }
        }  
    }




    public void CloseMenu()
    {
        if (inventoryMenuWorld != null)
        {
            inventoryMenuWorld.SetActive(false);
            listOfItems.Clear();
            DeleteText();
            NullHoveredItem();
        }
    }



    IEnumerator IsAddItemDebounce;
    IEnumerator AddItemDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        var isPaused = GameManagers.instance.GetIsPaused();
        if (isPaused == false)
        {
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
        }
        else if (isPaused == true)
        {

        }
        IsAddItemDebounce = null;
    }
    private void OnAddItemReceiver(object sender, InventorySystem.OnAddItemArgs e)
    {
        ItemSlot slot;
        if(IsAddItemDebounce == null)
        {
            IsAddItemDebounce = AddItemDebounce();
            StartCoroutine(IsAddItemDebounce);
            for (int i = 0; i < inventorySlots.transform.childCount; i++)
            {
                if (inventorySlots.transform.GetChild(i).transform.TryGetComponent(out slot) && slot.GetItemHeld() == null)
                {
                    slot.AddItem(e.item);
                    break;
                }
            }
        }
        
    }

    private void ListSlots()
    {
        ItemSlot itemSlot;
        for (int i = 0;inventorySlots.transform.childCount > i; i++)
        {
            if(inventorySlots.transform.GetChild(i).TryGetComponent(out itemSlot))
            {
                if(itemSlot.GetItemHeld() == null)
                {
                    listOfItems.Add(null);
                }
                else
                {
                    listOfItems.Add(itemSlot.GetItemHeld());
                }
            }
        }
    }


    private void OnHoveredItemUIReceiver(object sender, InventoryComms.OnHoveredItemUIArgs e)
    {
        ChangeHoveredItem(e.item);
        ChangeText(e.itemName, e.itemDesc);
    }


    private void OnExitItemUIReceiver(object sender, EventArgs e)
    {
        hoveredItem = null;
    }

    private void ChangeText(string name, string desc)
    {
        itemName.text = name;
        itemDesc.text = desc;
    }

    private void DeleteText()
    {
        itemName.text = string.Empty;
        itemDesc.text = string.Empty;
    }

    private void ChangeHoveredItem(GameObject item)
    {
        hoveredItem = item;
    }
    private void NullHoveredItem()
    {
        hoveredItem = null;
    }

    private void OnEInputEventSenderReceiver(object sender, EventArgs e)
    {
        E_Equip();
    }
    public event EventHandler<EquipItemEventArgs> EquipItemEvent;
    public class EquipItemEventArgs : EventArgs { public GameObject item; }
    private void E_Equip()
    {
        if (hoveredItem != null)
        {
            EquipItemEvent?.Invoke(this, new EquipItemEventArgs { item = hoveredItem });
            var itemUsesScr = hoveredItem.TryGetComponent(out ItemUses itemUses) ? itemUses : null;
            var itemUI = Instantiate(itemUsesScr.GetItemUI(), currentItemUI.transform);
            itemUI.GetComponent<RectTransform>().position = currentItemUI.GetComponent<RectTransform>().position;
            var itemUIScr = itemUI.TryGetComponent(out ItemIcon itemIcon) ? itemIcon : null;
            itemUIScr.SetUnInteractable();
        }
    }



    private void HealthStatusUpdate()
    {
        var healthAlpha = PlayerUIManager.instance.GetHealthAlpha();
        if(healthAlpha == 0)
        {
            healthStatus.color = new Color(healthStatus.color.r, healthStatus.color.g, healthStatus.color.b, healthAlpha);
        }
        else
        {
            var calculateAlpha = (healthAlpha / 100) * 150;
            var calculateRelativeToMaxAlpha = calculateAlpha / 225;
            healthStatus.color = new Color(healthStatus.color.r, healthStatus.color.g, healthStatus.color.b, calculateRelativeToMaxAlpha);
        }
    }
}