using System.Collections;
using UnityEngine;

public class midi_list : MonoBehaviour
{
    public Sprite icon;
    public Sprite icon_list_online;
    public Sprite icon_list_category;
    public Sprite icon_midi_buy;
    private int leng_midi = 0;
    public GameObject item_midi_prefab;
    private midi_item midi_item_temp;
    private Carrot.Carrot_Box box_list_midi;

    private string s_title_box;
    public void check_midi()
    {
        leng_midi = PlayerPrefs.GetInt("leng_m");
    }

    public void add_item_midi(string name_midi, string _data_index, string _data_type, float speed, int type)
    {
        if (name_midi == "") name_midi = "Midi " + (leng_midi + 1);
        PlayerPrefs.SetString("m_" + leng_midi + "_name", name_midi);
        PlayerPrefs.SetString("m_" + leng_midi + "_data_index", _data_index);
        PlayerPrefs.SetString("m_" + leng_midi + "_data_type", _data_type);
        PlayerPrefs.SetFloat("m_" + leng_midi + "_speed", speed);
        PlayerPrefs.SetInt("m_" + leng_midi + "_type", type);
        leng_midi = leng_midi + 1;
        PlayerPrefs.SetInt("leng_m", leng_midi);
        GetComponent<piano>().close_midi_editor();
        check_midi();
        show_list();
    }

    public void upadte_item_midi(int index_update, string name_midi, string _data_index, string _data_type, float speed)
    {
        if (name_midi == "") name_midi = "Midi " + (leng_midi + 1);
        PlayerPrefs.SetString("m_" + index_update + "_name", name_midi);
        PlayerPrefs.SetString("m_" + index_update + "_data_index", _data_index);
        PlayerPrefs.SetString("m_" + index_update + "_data_type", _data_type);
        PlayerPrefs.SetFloat("m_" + index_update + "_speed", speed);
        GetComponent<piano>().close_midi_editor();
        check_midi();
        show_list();
    }

    public void show_list()
    {
        this.GetComponent<piano>().carrot.play_sound_click();
        if (leng_midi > 0)
        {
            this.box_list_midi=GetComponent<piano>().carrot.Create_Box(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), icon);
            bool is_list_true = false;
            for (int i = 0; i < leng_midi; i++)
            {
                if (PlayerPrefs.GetString("m_" + i + "_name", "") != "")
                {
                    GameObject i_midi = Instantiate(item_midi_prefab);
                    i_midi.GetComponent<midi_item>().index = i;
                    i_midi.GetComponent<midi_item>().txt_name.text = PlayerPrefs.GetString("m_" + i + "_name");
                    i_midi.GetComponent<midi_item>().name_midi = PlayerPrefs.GetString("m_" + i + "_name");
                    i_midi.GetComponent<midi_item>().s_data_index = PlayerPrefs.GetString("m_" + i + "_data_index");
                    i_midi.GetComponent<midi_item>().s_data_type = PlayerPrefs.GetString("m_" + i + "_data_type");
                    i_midi.GetComponent<midi_item>().speed = PlayerPrefs.GetFloat("m_" + i + "_speed");
                    i_midi.GetComponent<midi_item>().check_type();
                    i_midi.GetComponent<midi_item>().btn_view.SetActive(false);
                    i_midi.GetComponent<midi_item>().btn_share.SetActive(false);
                    if (GetComponent<piano>().carrot.is_online())
                        i_midi.GetComponent<midi_item>().btn_upload.SetActive(true);
                    else
                        i_midi.GetComponent<midi_item>().btn_upload.SetActive(false);
                    i_midi.GetComponent<midi_item>().type_edit = 0;
                    i_midi.transform.SetParent(this.box_list_midi.area_all_item);
                    i_midi.transform.localScale = new Vector3(1f, 1f, 0f);
                    if (PlayerPrefs.GetInt("m_" + i + "_type") == 1) i_midi.GetComponent<midi_item>().btn_upload.SetActive(false);
                    is_list_true = true;
                }
            }
            if (!is_list_true) GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), PlayerPrefs.GetString("none_midi_save", "No midi compositions are archived yet"), Carrot.Msg_Icon.Alert);
        }
        else
        {
            if (this.box_list_midi != null) this.box_list_midi.close();
            GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), PlayerPrefs.GetString("none_midi_save", "No midi compositions are archived yet"), Carrot.Msg_Icon.Alert);
        }
    }

    public void func_search()
    {
        GetComponent<piano>().set_no_use_keyboar_pc();
        WWWForm frm_search = GetComponent<piano>().carrot.frm_act("list_midi_online");
        GetComponent<piano>().carrot.show_search(frm_search, act_done_search, PlayerPrefs.GetString("search_midi_tip", "You can Search for midi songs online, for reference and use for composition purposes"));
    }


    private void act_done_search(string s_data)
    {
        act_get_list_midi_online(s_data);
        Debug.Log("Data search:" + s_data);
    }

    public void delete(int index)
    {
        PlayerPrefs.DeleteKey("m_" + index + "_name");
        PlayerPrefs.DeleteKey("m_" + index + "_data_index");
        PlayerPrefs.DeleteKey("m_" + index + "_data_type");
        PlayerPrefs.DeleteKey("m_" + index + "_speed");
        PlayerPrefs.DeleteKey("m_" + index + "_type");
        check_after_delete();
        check_midi();
        show_list();
    }

    private void check_after_delete()
    {
        int count_item = 0;
        for(int i = 0; i < this.leng_midi; i++)
        {
            if(PlayerPrefs.GetString("m_" + i + "_name")!="") count_item++;
        }

        if (count_item == 0) {
            PlayerPrefs.DeleteKey("leng_m");
            if (this.box_list_midi != null) this.box_list_midi.close();
        } 
    }

    public void upload_midi(midi_item m_item)
    {
        WWWForm frm = GetComponent<piano>().carrot.frm_act("upload_midi");
        string s_id_user = GetComponent<piano>().carrot.user.get_id_user_login();
        if (s_id_user != "")
        {
            frm.AddField("user_id", s_id_user);
            frm.AddField("user_lang", GetComponent<piano>().carrot.user.get_lang_user_login());
        }

        frm.AddField("name_midi", m_item.txt_name.text);

        frm.AddField("data_index", m_item.s_data_index);
        frm.AddField("data_type", m_item.s_data_type);

        frm.AddField("speed", m_item.speed.ToString());
        midi_item_temp = m_item;
        GetComponent<piano>().carrot.send(frm, act_upload_midi);
    }

    private void act_upload_midi(string s_data)
    {
        GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("midi_public", "Publishing Midi"), PlayerPrefs.GetString("midi_public_success", "Thank you for contributing the draft midi piano, We will review and release to the world in the fastest time possible."), Carrot.Msg_Icon.Success);
        midi_item_temp.btn_upload.SetActive(false);
    }

    public void show_list_download_midi()
    {
        GetComponent<piano>().ShowInterstitialAd();
        s_title_box = PlayerPrefs.GetString("list_midi_online", "List Midi Online");
        WWWForm frm = GetComponent<piano>().carrot.frm_act("list_midi_online");
        GetComponent<piano>().carrot.send(frm, act_get_list_midi_online);
    }

    public void show_list_buy_user_id(string s_user_id)
    {
        WWWForm frm = GetComponent<piano>().carrot.frm_act("list_midi_online");
        frm.AddField("user_id", s_user_id);
        GetComponent<piano>().carrot.send(frm, act_get_list_midi_public_your);
    }

    private void act_get_list_midi_online(string s_data)
    {
        IList list_midi = (IList)Carrot.Json.Deserialize(s_data);
        if (list_midi.Count > 0)
        {
            this.box_list_midi = GetComponent<piano>().carrot.Create_Box(s_title_box, icon_list_online);
            for (int i = 0; i < list_midi.Count; i++)
            {
                IDictionary m_dimi = (IDictionary)list_midi[i];
                GameObject i_midi = Instantiate(item_midi_prefab);
                i_midi.GetComponent<midi_item>().id_midi = m_dimi["id_midi"].ToString();
                i_midi.GetComponent<midi_item>().index = -1;
                i_midi.GetComponent<midi_item>().txt_name.text = m_dimi["name"].ToString();
                i_midi.GetComponent<midi_item>().name_midi = m_dimi["name"].ToString();
                i_midi.GetComponent<midi_item>().link = m_dimi["link"].ToString();
                i_midi.GetComponent<midi_item>().speed = float.Parse(m_dimi["speed"].ToString());
                i_midi.GetComponent<midi_item>().type_edit = 1;
                if (m_dimi["sell"].ToString() == "1")
                {
                    i_midi.GetComponent<midi_item>().sell = 1;
                    i_midi.GetComponent<midi_item>().btn_buy.sprite = icon_list_online;
                }
                else
                {
                    if (PlayerPrefs.GetInt("buy_list", 0) == 0)
                    {
                        i_midi.GetComponent<midi_item>().sell = 2;
                    }
                    else
                    {
                        i_midi.GetComponent<midi_item>().sell = 1;
                    }
                    i_midi.GetComponent<midi_item>().btn_buy.sprite = icon_midi_buy;
                }
                i_midi.GetComponent<midi_item>().check_type();
                i_midi.GetComponent<midi_item>().btn_view.SetActive(true);
                i_midi.transform.SetParent(this.box_list_midi.area_all_item);
                i_midi.transform.localScale = new Vector3(1f, 1f, 0f);
            }
            Carrot.Carrot_Box_Btn_Item btn_search= this.box_list_midi.create_btn_menu_header(this.GetComponent<piano>().carrot.icon_carrot_search);
            btn_search.set_act(func_search);
            this.box_list_midi.update_color_table_row();
        }
        else
        {
            GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("list_midi_online", "List Midi Online"), PlayerPrefs.GetString("none_midi_search", "No related midi compositions found"), Carrot.Msg_Icon.Alert);
        }
    }

    private void act_get_list_midi_public_your(string s_data)
    {
        IList list_midi = (IList)Carrot.Json.Deserialize(s_data);
        if (list_midi.Count > 0)
        {
            Carrot.Carrot_Box box_midi_public_your=this.GetComponent<piano>().carrot.Create_Box(PlayerPrefs.GetString("list_midi_your_public", "Your published midi list"), icon_list_online);
            for (int i = 0; i < list_midi.Count; i++)
            {
                IDictionary m_dimi = (IDictionary)list_midi[i];
                GameObject i_midi = Instantiate(item_midi_prefab);
                i_midi.GetComponent<midi_item>().id_midi = m_dimi["id_midi"].ToString();
                i_midi.GetComponent<midi_item>().index = -1;
                i_midi.GetComponent<midi_item>().txt_name.text = m_dimi["name"].ToString();
                i_midi.GetComponent<midi_item>().name_midi = m_dimi["name"].ToString();
                i_midi.GetComponent<midi_item>().link = m_dimi["link"].ToString();
                i_midi.GetComponent<midi_item>().speed = float.Parse(m_dimi["speed"].ToString());
                i_midi.GetComponent<midi_item>().check_type();
                i_midi.GetComponent<midi_item>().btn_upload.SetActive(false);
                i_midi.GetComponent<midi_item>().btn_view.SetActive(true);
                i_midi.GetComponent<midi_item>().type_edit = 1;
                if (m_dimi["sell"].ToString() == "2") i_midi.GetComponent<midi_item>().btn_delete.SetActive(false);
                i_midi.transform.SetParent(box_midi_public_your.area_all_item);
                i_midi.transform.localScale = new Vector3(1f, 1f, 0f);
            }
            box_midi_public_your.update_color_table_row();
        }
        else
        {
            GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("list_midi_your_public", "Your published midi list"), PlayerPrefs.GetString("none_midi_public", "You don't have any published midi compositions yet !"), Carrot.Msg_Icon.Alert);
        }
    }

    public void get_midi_by_id(string s_midi_id)
    {
        if (this.box_list_midi != null) this.box_list_midi.close();
        this.GetComponent<piano>().carrot.ads.Destroy_Banner_Ad();
        WWWForm frm = GetComponent<piano>().carrot.frm_act("get_midi");
        frm.AddField("id_midi", s_midi_id);
        GetComponent<piano>().carrot.send(frm, act_get_midi);
    }

    private void act_get_midi(string s_data)
    {
        IDictionary data_midi = (IDictionary)Carrot.Json.Deserialize(s_data);
        if (data_midi != null)
        {
            GameObject m_obj = Instantiate(item_midi_prefab);
            midi_item m = m_obj.GetComponent<midi_item>();
            m.index = -1;
            m.name_midi = data_midi["name"].ToString();
            m.s_data_index = data_midi["data_index"].ToString();
            m.s_data_type = data_midi["data_type"].ToString();
            m.speed = float.Parse(data_midi["speed"].ToString());
            m.type_edit = 1;
            GameObject.Find("piano").GetComponent<piano>().load_midi(m);
            Destroy(m_obj);
        }

    }

    public void btn_show_list_category()
    {
        GetComponent<piano>().ShowInterstitialAd();
        WWWForm frm_category = GetComponent<piano>().carrot.frm_act("category_midi");
        GetComponent<piano>().carrot.send(frm_category, act_show_list_category);
    }

    private void act_show_list_category(string s_data)
    {
        IList list_cat = (IList)Carrot.Json.Deserialize(s_data);
        Carrot.Carrot_Box box_list_category=this.GetComponent<piano>().carrot.Create_Box(PlayerPrefs.GetString("midi_category", "List of midi genres"), icon_list_category);
        for (int i = 0; i < list_cat.Count; i++)
        {
            IDictionary m_cat = (IDictionary)list_cat[i];
            GameObject cat_midi = Instantiate(item_midi_prefab);
            cat_midi.GetComponent<midi_item>().id_midi = m_cat["name"].ToString();
            cat_midi.GetComponent<midi_item>().txt_name.text = m_cat["name"].ToString();
            cat_midi.GetComponent<midi_item>().name_midi = m_cat["name"].ToString();
            cat_midi.GetComponent<midi_item>().icon.sprite = icon_list_category;
            cat_midi.GetComponent<midi_item>().check_type();
            cat_midi.GetComponent<midi_item>().btn_upload.SetActive(false);
            cat_midi.GetComponent<midi_item>().btn_view.SetActive(false);
            cat_midi.GetComponent<midi_item>().btn_delete.SetActive(false);
            cat_midi.GetComponent<midi_item>().btn_share.SetActive(false);
            cat_midi.GetComponent<midi_item>().btn_buy.gameObject.SetActive(false);
            cat_midi.GetComponent<midi_item>().sell = -1;
            cat_midi.transform.SetParent(box_list_category.area_all_item);
            cat_midi.transform.localScale = new Vector3(1f, 1f, 0f);
        }
    }

    public void show_list_midi_by_category(string name_category)
    {
        s_title_box = name_category;
        WWWForm frm = GetComponent<piano>().carrot.frm_act("list_midi_online");
        frm.AddField("category", name_category);
        GetComponent<piano>().carrot.send(frm, act_get_list_midi_online);
    }

}
