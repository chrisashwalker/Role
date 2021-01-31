using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI : MonoBehaviour, IPointerClickHandler
{
    public static Camera MainCamera {get; set;}
    public static float DefaultCameraSize {get; set;}
    public static GameObject Target {get; set;}
    public static int CellSize {get; set;} = 1;
    public static GameObject ShortcutCanvas {get; set;}
    public static GameObject[] AllShortcutToggles {get; set;}
    public static GameObject HealthBar {get; set;}

    public static void GetMainCamera()
    {
        MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        DefaultCameraSize = MainCamera.orthographicSize;
    }
    public static void RelocateCamera()
    {
        MainCamera.transform.position = new Vector3(Map.Player.Rigidbody.position.x - 7.5f, Map.Player.Rigidbody.position.y + 10f, Map.Player.Rigidbody.position.z - 15.0f);
    }

    public static void RelocateTarget()
    {
        if (Target == null){
            Target = GameObject.Instantiate(Resources.Load<GameObject>("Target"));
        }
        float targetX, targetY, targetZ, cellsAlongX, cellsAlongZ, cellSizeOffset;
        targetY = 0.04f;
        cellsAlongX = Map.Player.Rigidbody.position.x / CellSize;
        cellsAlongZ = Map.Player.Rigidbody.position.z / CellSize;
        cellSizeOffset = CellSize;
        if (Map.Player.Rigidbody.rotation.eulerAngles.y < 90){
            cellsAlongZ = (float) System.Math.Ceiling(cellsAlongZ);
        }
        else if (Map.Player.Rigidbody.rotation.eulerAngles.y >= 90 && Map.Player.Rigidbody.rotation.eulerAngles.y < 180)
        {
            cellsAlongX = (float) System.Math.Ceiling(cellsAlongX);
        }
        cellsAlongX = (float) System.Math.Floor(cellsAlongX);
        cellsAlongZ = (float) System.Math.Floor(cellsAlongZ);
        targetX = (float) System.Math.Floor(cellsAlongX) * CellSize + CellSize;
        targetZ = (float) System.Math.Ceiling(cellsAlongZ) * CellSize + CellSize;
        if (Map.Player.Rigidbody.rotation.eulerAngles.y >= 180 && Map.Player.Rigidbody.rotation.eulerAngles.y < 270)
        {
            targetZ -= cellSizeOffset * 2;
        } 
        else if (Map.Player.Rigidbody.rotation.eulerAngles.y >= 270)
        {
            targetX -= cellSizeOffset * 2;
        }
        Target.transform.position = new Vector3(targetX,targetY,targetZ);
        Target.transform.rotation = Map.Player.Rigidbody.rotation;
    }

    public static void ToggleCamera()
    {
        if (MainCamera.orthographicSize == DefaultCameraSize)
        {
            MainCamera.orthographicSize = DefaultCameraSize * 4;
        } 
        else 
        {
            MainCamera.orthographicSize = DefaultCameraSize;
        }
    }

    public static void LoadHUD()
    {
        ShortcutCanvas = GameObject.FindWithTag("ShortcutCanvas");
        AllShortcutToggles = new GameObject[0];
        Items.UpdateToggles();
        HealthBar = GameObject.FindGameObjectWithTag("Health");
        HealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(10 * Map.Player.MaxHealth, 10);
        HealthBar.GetComponent<Slider>().maxValue = Map.Player.MaxHealth;
        HealthBar.GetComponent<Slider>().value = Map.Player.Health;
    }

    public static void UpdateHUD()
    {
        HealthBar.GetComponent<Slider>().value = Map.Player.Health;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        GameObject clickedToggle = pointerEventData.pointerPress;
        if (clickedToggle.tag == "ShortcutToggle" && !Trading.InTrade){
            Items.Equip(Map.Player, clickedToggle);
        } else if (clickedToggle.tag == "ShortcutToggle" && Trading.InTrade){
            Trading.TradeItem(clickedToggle);
        }
    }

     public static void LaunchTrading()
     {
        foreach (GameObject toggle in AllShortcutToggles)
        {
            toggle.SetActive(false);
            GameObject.Destroy(toggle);
        }
        foreach (Item item in Trading.SaleItems)
        {
            GameObject newToggleObject = GameObject.Instantiate(Resources.Load<GameObject>("ShortcutToggle"));
            newToggleObject.tag = "ShortcutToggle";
            newToggleObject.transform.SetParent(ShortcutCanvas.transform, false);
            string itemLabel;
            itemLabel = item.Name + " : " + item.Value;
            newToggleObject.GetComponentInChildren<Text>().text = itemLabel;
        }
        AllShortcutToggles = GameObject.FindGameObjectsWithTag("ShortcutToggle");
        int toggleCount = AllShortcutToggles.Length;
        foreach (GameObject toggle in AllShortcutToggles)
        {
            int toggleIndex = System.Array.IndexOf(AllShortcutToggles, toggle);
            float positionFromCenter = toggleIndex - ((float) toggleCount / 2) + 0.5f;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionFromCenter * Items.toggleWidth, Items.toggleHeight);
        }
    }
}
