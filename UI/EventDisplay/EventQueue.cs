using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventQueue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MainText = null;
    [SerializeField] private TextMeshProUGUI SecondaryText = null;
    [SerializeField] private GameObject Panel = null;
    [SerializeField] private UIController uiControl = null;
    [SerializeField] private AudioClip notificationSound = null;
    private AudioSource audioSource;

    private Queue<EventData> que = new Queue<EventData>();
    private EventData ActiveEvent;
    private float time_remaining;

    void Start()
    {
        audioSource = GameObject.Find("Master Object").GetComponent<AudioSource>();
        time_remaining = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if(uiControl.current_UI_mode == UI_Mode.Normal && time_remaining <= 0 && que.Count > 0)
        {
            if (notificationSound) { audioSource.PlayOneShot(notificationSound); }
            GetComponent<Animator>().Play("Panel In");
            ActiveEvent = que.Dequeue();
            time_remaining = GetDelay(ActiveEvent.eventType);
            MainText.text = GetMainText(ActiveEvent.eventType);
            SecondaryText.text = ActiveEvent.SecondaryText;
        }
        else if (time_remaining > 0)
        {
            Panel.SetActive(true);
            time_remaining -= Time.deltaTime;
        }
        else if (Panel.activeSelf)
        {
            GetComponent<Animator>().Play("Panel Out");
            StartCoroutine(TurnOffPanel());
        }
    }

    private IEnumerator TurnOffPanel()
    {
        yield return new WaitForSecondsRealtime(0.25f);

        Panel.SetActive(false);
    }

    public void AddEvent(EventData event_in)
    {
        que.Enqueue(event_in);
    }

    private string GetMainText(EventTypeEnum eventType)
    {
        string[] text_array = { "Quest Updated", "Quest Complete", "Quest Failed", "Quest Started", "Level Up", "Ability Level Up", "Items Received", "Location Discovered" };
        return text_array[(int)eventType];
    }

    private float GetDelay(EventTypeEnum eventType)
    {
        float[] delay_array = { 3f, 3f, 3f, 3f, 3f, 3f, 3f, 3f };
        return delay_array[(int)eventType];
    }
}
