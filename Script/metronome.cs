using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class metronome : MonoBehaviour
{
    private bool is_run = false;
    public Sprite icon_run;
    public Sprite icon_stop;
    public Image img_metronome_icon;
    public Image img_metronome_play;
    public GameObject panel_metronome;
    public AudioSource sound_metronome;
    private float speed = 1f;
    public Slider slider_metronome;
    public Text txt_metronome_speed;

    private IEnumerator coroutine;
    public void show_setting()
    {
        this.GetComponent<piano>().carrot.play_sound_click();
        panel_metronome.SetActive(true);
        slider_metronome.value = speed;
        txt_metronome_speed.text = speed.ToString("");
    }

    void Start()
    {
        coroutine = TempoMake();
    }


    public void run_or_stop()
    {
        this.GetComponent<piano>().carrot.play_sound_click();
        if (is_run)
        {
            is_run = false;
            img_metronome_play.sprite = icon_run;
            img_metronome_icon.color = Color.white;
            img_metronome_play.color = Color.white;
            StopCoroutine(coroutine);
        }
        else
        {
            is_run = true;
            img_metronome_play.sprite = icon_stop;
            StartCoroutine(coroutine);
        }
    }

    private void changed_color_metronome()
    {
        if (img_metronome_icon.color == Color.green)
        {
            img_metronome_icon.color = Color.yellow;
            img_metronome_play.color = Color.yellow;
        }
        else
        {
            img_metronome_icon.color = Color.green;
            img_metronome_play.color = Color.green;
        }
    }

    public void changed_speed_metronome()
    {
        speed = slider_metronome.value;
        txt_metronome_speed.text = speed.ToString();
    }

    IEnumerator TempoMake()
    {
        while (true)
        {
            yield return
             new WaitForSecondsRealtime(speed);
            sound_metronome.Play();
            changed_color_metronome();
        }
    }

    public void close()
    {
        this.GetComponent<piano>().carrot.play_sound_click();
        this.panel_metronome.SetActive(false);
    }
}
