using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class midi : MonoBehaviour
{
    [Header("Obj Main")]
    public piano p;

    [Header("Obj Midi")]
    public GameObject panel_midi_editor;
    public Image img_recod;
    private bool is_play = false;
    private int sel_index_line = 0;
    private int length_midi_note = 0;
    private List<midi_line> list_midi_line = new();
    public GameObject midi_line_prefab;

    private float count_next_midi_timer = 0f;
    private int index_play_note_in_midi = -1;
    public ScrollRect scroll_all_line;
    public Transform area_all_line;
    public InputField inp_name_midi;

    public Color32 midi_color_nomal_note;
    public Color32 midi_color_play;
    public Color32 midi_color_play_true;
    public Color32 midi_color_ready_play;
    public Color32 midi_color_select_note;
    public Color32 midi_color_select_line;
    public Color32 midi_color_nomal_line;

    public Image img_play_midi;
    public Sprite icon_play;
    public Sprite icon_pause;

    private midi_note sel_midi_recod = null;
    private float speed = 0.2f;
    private int index_midi_edit = -1;

    public GameObject panel_menu_act;
    public GameObject panel_midi_speed;
    public Text txt_timer_speed;
    public Text txt_timer_speed_setting;
    public Slider slider_speed;

    private int type_edit_midi = 0;

    public void Show_editor()
    {
        p.carrot.play_sound_click();
        this.load_midi();
    }

    public void load_midi()
    {
        panel_menu_act.SetActive(true);
        speed = 0.2f;
        txt_timer_speed.text = speed.ToString() + "s";
        index_midi_edit = -1;
        inp_name_midi.text = "";
        panel_midi_speed.SetActive(false);
        clear_midi();

        panel_midi_editor.SetActive(true);
        p.panel_menu_top.SetActive(false);
        p.panel_menu_mini.SetActive(false);
        p.carrot.ads.Destroy_Banner_Ad();
    }

    public void clear_midi()
    {
        clear_all_line();
        add_line_midi();
        add_col_midi();
        select_midi_note(list_midi_line[0].get_midi_note(0));
    }

    private void clear_all_line()
    {
        p.carrot.clear_contain(area_all_line);
        list_midi_line = new List<midi_line>();
        sel_index_line = 0;
        length_midi_note = 0;
    }

    public void add_col_midi()
    {
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].add_note_midi();
        }
        Canvas.ForceUpdateCanvases();
        scroll_all_line.horizontalNormalizedPosition = Mathf.Clamp(scroll_all_line.horizontalNormalizedPosition, 1f, 0f);
        length_midi_note++; 
    }

    public void add_col_midi_by_note()
    {
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].add_note_midi();
        }
        Canvas.ForceUpdateCanvases();
        scroll_all_line.horizontalNormalizedPosition = Mathf.Clamp(scroll_all_line.horizontalNormalizedPosition, 1f, 0f);
        length_midi_note++;
    }

    public void add_line_midi()
    {
        GameObject item_midi_line = Instantiate(midi_line_prefab);
        item_midi_line.transform.SetParent(area_all_line);
        item_midi_line.transform.localScale = new Vector3(1f, 1f, 0f);
        item_midi_line.GetComponent<midi_line>().index_line = list_midi_line.Count;
        item_midi_line.GetComponent<midi_line>().add_note_midi_by_length(length_midi_note);
        list_midi_line.Add(item_midi_line.GetComponent<midi_line>());
    }

    public void delete_line_midi()
    {
        this.list_midi_line[this.sel_index_line].clear();
        Destroy(this.list_midi_line[this.sel_index_line].gameObject);
        list_midi_line.RemoveAt(this.sel_index_line);
        if (this.sel_index_line > 0) this.sel_index_line = this.sel_index_line - 1;
        this.update_data_midi();
        if (this.list_midi_line.Count == 0) this.clear_midi();
    }

    public void delete_col_midi()
    {
        if(this.sel_midi_recod.index_note_midi<= list_midi_line[0].get_midi_note_length())
        {
            for (int i = 0; i < list_midi_line.Count; i++)
            {
                list_midi_line[i].delete_note(this.sel_midi_recod.index_note_midi);
            }

            if (list_midi_line[0].get_midi_note_length() == 0) 
                this.clear_midi();
            else
            {
                if (this.sel_midi_recod.index_note_midi == list_midi_line[0].get_midi_note_length())
                {
                    midi_note m_note_sel = this.list_midi_line[this.sel_index_line].get_midi_note(this.sel_midi_recod.index_note_midi - 1);
                    if(m_note_sel!=null) this.select_midi_note(m_note_sel);
                }
                else
                    this.select_midi_note(this.list_midi_line[this.sel_index_line].get_midi_note(this.sel_midi_recod.index_note_midi));
            }
                
        }
    }

    private void update_data_midi()
    {
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].index_line = i;
            list_midi_line[i].update_data();
        }
    }

    private void add_line_midi_from_data(IList arr_index, IList arr_type)
    {
        GameObject item_midi_line = Instantiate(midi_line_prefab);
        item_midi_line.transform.SetParent(area_all_line);
        item_midi_line.transform.localScale = new Vector3(1f, 1f, 0f);
        item_midi_line.GetComponent<midi_line>().index_line = list_midi_line.Count;
        for (int i = 0; i < arr_index.Count; i++)
        {
            int index_p = int.Parse(arr_index[i].ToString());
            int type_p = int.Parse(arr_type[i].ToString());
            string s_txt_piano = p.get_txt_note_piano(index_p, type_p);
            item_midi_line.GetComponent<midi_line>().add_note_midi(s_txt_piano, index_p, type_p);
        }
        list_midi_line.Add(item_midi_line.GetComponent<midi_line>());
    }

    private void set_no_select_all_midi_note()
    {
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].no_select(midi_color_nomal_note);
        }
    }

    private void set_ready_play_all_midi_note()
    {
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].ready_play(midi_color_ready_play);
        }
    }

    private void play_note_midi_line(int index_note)
    {
        int[] arr_index_note = new int[list_midi_line.Count];
        int[] arr_type_note = new int[list_midi_line.Count];
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            midi_note m_note = list_midi_line[i].get_midi_note(index_note);
            arr_index_note[i] = m_note.index_note_piano;
            arr_type_note[i] = m_note.type_note_piano;
            if (m_note.index_note_piano != -1)
            {
                m_note.play(midi_color_play_true);
            }
            else
            {
                m_note.play(midi_color_play);
            }
        }
        p.play_note_midi(arr_index_note, arr_type_note);
    }

    public void play_midi()
    {

        if (is_play)
        {
            pause_midi();
        }
        else
        {
            set_ready_play_all_midi_note();
            index_play_note_in_midi = -1;
            is_play = true;
            img_play_midi.sprite = icon_pause;
            Canvas.ForceUpdateCanvases();
            reset_color_all_line();
            panel_menu_act.SetActive(false);
            scroll_all_line.horizontalNormalizedPosition = Mathf.Clamp(scroll_all_line.horizontalNormalizedPosition, -1f, 0f);
        }
    }

    private void pause_midi()
    {
        set_no_select_all_midi_note();
        is_play = false;
        img_play_midi.sprite = icon_play;
        panel_menu_act.SetActive(true);
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].reset_note_show();
        }
    }

    private void Update()
    {
        if (is_play)
        {
            count_next_midi_timer += 1f * Time.deltaTime;
            if (count_next_midi_timer > speed)
            {
                index_play_note_in_midi++;
                if (index_play_note_in_midi < list_midi_line[0].get_midi_note_length())
                {
                    play_note_midi_line(index_play_note_in_midi);
                    count_next_midi_timer = 0;
                }
                else
                {
                    pause_midi();
                }
            }
        }
    }

    public void select_midi_note(midi_note m_note)
    {
        if (is_play)
        {
            set_no_select_all_midi_note();
            count_next_midi_timer = 0;
            index_play_note_in_midi = m_note.index_note_midi - 1;
        }
        else
        {
            set_no_select_all_midi_note();
            sel_midi_recod = m_note;
            sel_midi_recod.GetComponent<Image>().color = midi_color_select_note;
            select_line(m_note.index_line);
        }
    }

    private void select_line(int index_line)
    {
        sel_index_line = index_line;
        reset_color_all_line();
        list_midi_line[index_line].GetComponent<Image>().color = midi_color_select_line;
    }

    private void reset_color_all_line()
    {
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            list_midi_line[i].GetComponent<Image>().color = midi_color_nomal_line;
        }
    }

    public void recod_note_midi(string s_name, int index_note_p, int type_note_p)
    {
        if (is_play == false)
        {
            if (sel_midi_recod == null)
            {
                add_col_midi();
                select_midi_note(list_midi_line[sel_index_line].get_midi_note_last());
                sel_midi_recod.txt.text = s_name;
                sel_midi_recod.index_note_piano = index_note_p;
                sel_midi_recod.type_note_piano = type_note_p;
                sel_midi_recod = null;
            }
            else
            {
                sel_midi_recod.txt.text = s_name;
                sel_midi_recod.index_note_piano = index_note_p;
                sel_midi_recod.type_note_piano = type_note_p;
                sel_midi_recod = null;
            }
        }
    }

    public void delete_note_midi_select()
    {
        if (sel_midi_recod != null)
        {
            sel_midi_recod.txt.text = "...";
            sel_midi_recod.index_note_piano = -1;
            sel_midi_recod.type_note_piano = 0;
        }
    }

    public void save_midi()
    {
        List<int[]> data_index_piano_save = new();
        List<int[]> data_type_piano_save = new();
        for (int i = 0; i < list_midi_line.Count; i++)
        {
            data_index_piano_save.Add(list_midi_line[i].get_arr_int_index_note_piano());
            data_type_piano_save.Add(list_midi_line[i].get_arr_type_note_piano());
        }

        string s_data_index = Carrot.Json.Serialize(data_index_piano_save);
        string s_data_type = Carrot.Json.Serialize(data_type_piano_save);

        if (index_midi_edit == -1)
            p.m_list.add_item_midi(inp_name_midi.text, s_data_index, s_data_type, speed, type_edit_midi);
        else
            p.m_list.upadte_item_midi(index_midi_edit, inp_name_midi.text, s_data_index, s_data_type, speed);
    }


    public void open_midi(midi_item item_midi)
    {
        this.panel_midi_editor.SetActive(true);
        p.carrot.play_sound_click();
        p.panel_menu_top.SetActive(false);
        p.panel_menu_mini.SetActive(false);
        gameObject.SetActive(true);
        panel_midi_speed.SetActive(false);
        speed = item_midi.speed;
        type_edit_midi = item_midi.type_edit;
        txt_timer_speed.text = speed.ToString() + "s";
        index_midi_edit = item_midi.index;
        inp_name_midi.text = item_midi.name_midi;
        panel_menu_act.SetActive(true);

        clear_all_line();


        IList data_index = (IList)Carrot.Json.Deserialize(item_midi.s_data_index);
        IList data_type = (IList)Carrot.Json.Deserialize(item_midi.s_data_type);
        IList arr_index_0 = (IList)data_index[0];
        length_midi_note = arr_index_0.Count;
        sel_index_line = 0;
        for (int i = 0; i < data_index.Count; i++)
        {
            IList arr_index = (IList)data_index[i];
            IList arr_type = (IList)data_type[i];
            add_line_midi_from_data(arr_index, arr_type);
        }

        Canvas.ForceUpdateCanvases();
        scroll_all_line.horizontalNormalizedPosition = Mathf.Clamp(scroll_all_line.horizontalNormalizedPosition, 0f, 0f);
    }

    public void Show_by_data(IDictionary data)
    {
        this.Show_editor();
        IList data_index = (IList)Carrot.Json.Deserialize(data["data_index"].ToString());
        IList data_type = (IList)Carrot.Json.Deserialize(data["data_type"].ToString());
        IList arr_index_0 = (IList)data_index[0];
        length_midi_note = arr_index_0.Count;
        sel_index_line = 0;
        for (int i = 0; i < data_index.Count; i++)
        {
            IList arr_index = (IList)data_index[i];
            IList arr_type = (IList)data_type[i];
            add_line_midi_from_data(arr_index, arr_type);
        }
    }

    public void show_speed_setting()
    {
        p.carrot.play_sound_click();
        slider_speed.value = speed;
        panel_midi_speed.SetActive(true);
    }

    public void change_speed_midi()
    {
        txt_timer_speed.text = slider_speed.value.ToString("F2") + "s";
        txt_timer_speed_setting.text = slider_speed.value.ToString("F2") + "s";
        speed = slider_speed.value;
    }

    public void Close()
    {
        this.panel_midi_editor.SetActive(false);
        this.p.panel_menu_top.SetActive(true);
        this.p.panel_menu_mini.SetActive(true);
        p.carrot.play_sound_click();
        p.rest_color_all_note();
        img_play_midi.sprite = icon_play;
        is_play = false;
        p.set_use_keyboar_pc();
        img_recod.color = Color.white;
        p.carrot.ads.show_ads_Interstitial();
        p.carrot.ads.create_banner_ads();
    }

    public void btn_export_midi()
    {
        int length_midi = list_midi_line[0].get_midi_note_length();
        int[] arr_note_index = new int[length_midi];
        int[] arr_note_type = new int[length_midi];

        for (int i = 0; i < length_midi; i++)
        {
            arr_note_index[i] = list_midi_line[0].get_midi_note(i).index_note_piano;
            arr_note_type[i] = list_midi_line[0].get_midi_note(i).type_note_piano;
        }

        Debug.Log(Carrot.Json.Serialize(arr_note_index));
        Debug.Log(Carrot.Json.Serialize(arr_note_type));

        WWWForm frm_export = p.carrot.frm_act("export_file_midi");
        frm_export.AddField("data_midi_index", "[" + Carrot.Json.Serialize(arr_note_index) + "]");
        frm_export.AddField("data_midi_type", "[" + Carrot.Json.Serialize(arr_note_type) + "]");
       // p.carrot.send(frm_export, act_export_midi_file);
    }

    private void act_export_midi_file(string s_data)
    {
        p.carrot.show_msg(PlayerPrefs.GetString("midi_editor", "Midi drafting"), PlayerPrefs.GetString("export_file_midi_success", "Exported midi editor (.midi) file successfully!"), Carrot.Msg_Icon.Success);
        Application.OpenURL(s_data);
    }
}
