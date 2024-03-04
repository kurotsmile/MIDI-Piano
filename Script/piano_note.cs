using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class piano_note : MonoBehaviour
{
    public Text txt_piano;
    public AudioSource sound_piano;
    public int index;
    public int type;
    public string s_piano;
    public string s_pc;
    public string s_code_shift_pc;
    public float pos_scroll;

    private UnityAction act_click;
    private UnityAction act_click_key;

    public void Set_Act_Click(UnityAction act)
    {
        this.act_click = act;
    }

    public void Set_Act_Click_Key(UnityAction act)
    {
        this.act_click_key = act;
    }

    public void click()
    {
        sound_piano.Play();
        act_click?.Invoke();
        //GameObject.Find("piano").GetComponent<piano>().play_note(index, type, true);
    }

    public void click_key()
    {
        sound_piano.Play();
        act_click_key?.Invoke();
        //GameObject.Find("piano").GetComponent<piano>().play_note(index, type, false);
    }

    public void play()
    {
        sound_piano.Play();
    }

    public Vector3 get_pos()
    {
        return txt_piano.transform.position;
    }

    public void set_type_text_show(int type_show)
    {
        if (type_show == 0) txt_piano.text = "";
        if (type_show == 1) txt_piano.text = s_piano;
        if (type_show == 2) txt_piano.text = s_pc;
    }
}
