using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI : MonoBehaviour, IPointerClickHandler
{
    public static Camera MainCamera {get; set;}
    public static float DefaultCameraSize {get; set;}
    public static GameObject Target {get; set;}
    public static int cellSize = 1;
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
        float targetX, targetY, targetZ;
        targetY = 0.04f;
        if (Map.Player.Rigidbody.rotation.eulerAngles.y < 90){
            targetX = (float) System.Math.Floor(Map.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Ceiling(Map.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if  (Map.Player.Rigidbody.rotation.eulerAngles.y < 180){
            targetX = (float) System.Math.Ceiling(Map.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(Map.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if (Map.Player.Rigidbody.rotation.eulerAngles.y < 270){
            targetX = (float) System.Math.Floor(Map.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(Map.Player.Rigidbody.position.z / cellSize) * cellSize - cellSize;
        } else {
            targetX = (float) System.Math.Floor(Map.Player.Rigidbody.position.x / cellSize) * cellSize - cellSize;
            targetZ = (float) System.Math.Floor(Map.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
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
        AllShortcutToggles = GameObject.FindGameObjectsWithTag("ShortcutToggle");
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
