using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WAILA : MonoBehaviour
{
    //tells you what you are looking at when you hover over something

    public Camera cam;
    public Vector3 mousePos;
    public LayerMask mask;
    public RaycastHit hit;

    public GameObject hoveredObject;
    public GameObject selectedObject;

    public TMP_Text nameTextbox;

    public LayerMask layerMask;

    public RaycastHit hoveredElement;
    public RaycastHit selectedElement;

    public GraphicRaycaster m_Raycaster;
    public PointerEventData m_PointerEventData;
    public EventSystem m_EventSystem;


    int UILayer;

    public GameObject infoPanel;
    public TMP_Text infoPanelName;

    public RaycastResult hitUI;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        nameTextbox = GameObject.Find("WAILAName").GetComponent<TMP_Text>();

        UILayer = LayerMask.NameToLayer("UI");
        infoPanel = GameObject.Find("InfoPanel");
        infoPanelName = infoPanel.transform.Find("Name").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            hoveredObject = hit.transform.gameObject;
            hoveredElement = hit;
            //Debug.Log(hit.name);
        }

        if (Input.GetMouseButtonDown(0))
        {
            nameTextbox.text = hoveredObject.name;
            selectedObject = hoveredObject;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }

        IsPointerOverUIElement(GetEventSystemRaycastResults());

        PositionInfoPanel();
    }

    public void UpdateInfoPanel()
    {
        infoPanelName.text = hitUI.gameObject.name.ToString();
    }

    public void PositionInfoPanel()
    {
        infoPanel.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
        infoPanel.transform.position = Input.mousePosition + new Vector3(1f, 1f, 1f);
    }

    public bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaycastResult = eventSystemRaysastResults[index];
            if (curRaycastResult.gameObject.layer == UILayer)
            {
                hitUI = curRaycastResult;
                infoPanel.SetActive(true);
                UpdateInfoPanel();
                return true;
                    
            }
                    
        }
        infoPanel.SetActive(false);

        return false;
            
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public GameObject HoveredObject()
    {
        return hoveredObject;
    }

    //on mouse down show different selector for grabbing a puck or a card. draw an arrow for card select
}
