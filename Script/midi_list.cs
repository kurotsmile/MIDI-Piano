using Carrot;
using System.Collections;
using UnityEngine;

public class midi_list : MonoBehaviour
{
    [Header("Main Obj")]
    public piano p;
    [Header("List Midi Obj")]
    public Sprite icon;
    public Sprite icon_list_online;
    public Sprite icon_list_category;
    public Sprite icon_midi_buy;
    private int leng_midi = 0;
    public GameObject item_midi_prefab;
    private midi_item midi_item_temp;
    private Carrot_Box box_list_midi;

    private string s_title_box;

    public void Check_midi()
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
        leng_midi++;
        PlayerPrefs.SetInt("leng_m", leng_midi);
        p.m.Close();
        Check_midi();
        show_list();
    }

    public void upadte_item_midi(int index_update, string name_midi, string _data_index, string _data_type, float speed)
    {
        if (name_midi == "") name_midi = "Midi " + (leng_midi + 1);
        PlayerPrefs.SetString("m_" + index_update + "_name", name_midi);
        PlayerPrefs.SetString("m_" + index_update + "_data_index", _data_index);
        PlayerPrefs.SetString("m_" + index_update + "_data_type", _data_type);
        PlayerPrefs.SetFloat("m_" + index_update + "_speed", speed);
        p.m.Close();
        Check_midi();
        show_list();
    }

    public void show_list()
    {
        p.carrot.play_sound_click();
        if (leng_midi > 0)
        {
            this.box_list_midi=p.carrot.Create_Box(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), icon);
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
                    if (p.carrot.is_online())
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
            if (!is_list_true) p.carrot.show_msg(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), PlayerPrefs.GetString("none_midi_save", "No midi compositions are archived yet"), Carrot.Msg_Icon.Alert);
        }
        else
        {
            if (this.box_list_midi != null) this.box_list_midi.close();
            p.carrot.show_msg(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), PlayerPrefs.GetString("none_midi_save", "No midi compositions are archived yet"), Carrot.Msg_Icon.Alert);
        }
    }

    public void func_search()
    {
        p.set_no_use_keyboar_pc();
        p.carrot.show_search( Act_done_search, PlayerPrefs.GetString("search_midi_tip", "You can Search for midi songs online, for reference and use for composition purposes"));
    }


    private void Act_done_search(string s_data)
    {
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("name", Query_OP.EQUAL, s_data);
        p.carrot.server.Get_doc(q.ToJson(), this.Act_get_list_midi_online_done);
    }

    public void delete(int index)
    {
        PlayerPrefs.DeleteKey("m_" + index + "_name");
        PlayerPrefs.DeleteKey("m_" + index + "_data_index");
        PlayerPrefs.DeleteKey("m_" + index + "_data_type");
        PlayerPrefs.DeleteKey("m_" + index + "_speed");
        PlayerPrefs.DeleteKey("m_" + index + "_type");
        check_after_delete();
        Check_midi();
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

    public void Upload_midi(midi_item m_item)
    {
        midi_item_temp = m_item;
        string s_id_user = p.carrot.user.get_id_user_login();
        string s_id_midi = "midi"+p.carrot.generateID();
        IDictionary data_midi = (IDictionary)Json.Deserialize("{}");
        data_midi["speed"] = m_item.speed.ToString();
        data_midi["data_type"] = m_item.s_data_type;
        data_midi["data_index"] = m_item.s_data_index;
        data_midi["name"] = m_item.txt_name.text;
        data_midi["status"] = "pending";
        
        if (s_id_user != "")
        {
            data_midi["user_id"] = s_id_user;
            data_midi["user_lang"] = p.carrot.user.get_lang_user_login();
        }

        p.carrot.show_loading();
        string s_json=p.carrot.server.Convert_IDictionary_to_json(data_midi);
        p.carrot.server.Add_Document_To_Collection(p.carrot.Carrotstore_AppId, s_id_midi,s_json, Act_upload_midi_done, Act_updload_midi_fail);
    }

    private void Act_upload_midi_done(string s_data)
    {
        p.carrot.show_msg(PlayerPrefs.GetString("midi_public", "Publishing Midi"), PlayerPrefs.GetString("midi_public_success", "Thank you for contributing the draft midi piano, We will review and release to the world in the fastest time possible."), Msg_Icon.Success);
        midi_item_temp.btn_upload.SetActive(false);
        p.carrot.hide_loading();
    }

    private void Act_updload_midi_fail(string s_error)
    {
        p.carrot.show_msg(PlayerPrefs.GetString("midi_public", "Publishing Midi"), s_error, Msg_Icon.Error);
        p.carrot.hide_loading();
    }

    public void show_list_download_midi()
    {
        p.carrot.ads.show_ads_Interstitial();
        s_title_box = PlayerPrefs.GetString("list_midi_online", "List Midi Online");
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        p.carrot.show_loading();
        p.carrot.server.Get_doc(q.ToJson(), Act_get_list_midi_online_done);
    }

    public void show_list_buy_user_id(string s_user_id)
    {
        s_title_box = PlayerPrefs.GetString("list_midi_online", "List Midi Online");
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("user_id", Query_OP.EQUAL, s_user_id);
        p.carrot.server.Get_doc(q.ToJson(), Act_get_list_midi_online_done);
    }

    private void Act_get_list_midi_online_done(string s_data)
    {
        p.carrot.hide_loading();
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.box_list_midi = p.carrot.Create_Box(s_title_box, icon_list_online);
            Carrot_Box_Btn_Item btn_cat=this.box_list_midi.create_btn_menu_header(p.carrot.icon_carrot_all_category);
            btn_cat.set_act(() => this.Btn_show_list_category());

            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data = fc.fire_document[i].Get_IDictionary();
                var id_midi = data["id"].ToString();

                Carrot_Box_Item item_midi = this.box_list_midi.create_item("item_midi_" + i);
                item_midi.set_icon(icon);
                item_midi.set_title(data["name"].ToString());
                item_midi.set_tip(data["id"].ToString());
                item_midi.set_act(() => Open_midi_editor_by_data(data));

                if (p.carrot.model_app == ModelApp.Develope)
                {
                    Carrot_Box_Btn_Item btn_del = item_midi.create_item();
                    btn_del.set_icon(p.carrot.sp_icon_del_data);
                    btn_del.set_color(p.carrot.color_highlight);
                    btn_del.set_act(() => Delete_midi_from_server(id_midi));
                }
            }
            Carrot_Box_Btn_Item btn_search = this.box_list_midi.create_btn_menu_header(p.carrot.icon_carrot_search);
            btn_search.set_act(func_search);
            this.box_list_midi.update_color_table_row();
        }
        else
        {
            p.carrot.show_msg(PlayerPrefs.GetString("list_midi_online", "List Midi Online"), PlayerPrefs.GetString("none_midi_search", "No related midi compositions found"), Msg_Icon.Alert);
        }
    }

    private void Open_midi_editor_by_data(IDictionary data)
    {
        p.carrot.play_sound_click();
        p.m.Show_by_data(data);
        if (box_list_midi != null) box_list_midi.close();
    }

    public void get_midi_by_id(string s_midi_id)
    {
        if (this.box_list_midi != null) this.box_list_midi.close();
        p.carrot.ads.Destroy_Banner_Ad();
        p.carrot.server.Get_doc_by_path(p.carrot.Carrotstore_AppId,s_midi_id, act_get_midi);
    }

    private void act_get_midi(string s_data)
    {
        IDictionary data_midi = (IDictionary)Json.Deserialize(s_data);
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
            p.m.Show_by_data(data_midi);
            Destroy(m_obj);
        }
    }

    public void Btn_show_list_category()
    {
        p.carrot.play_sound_click();
        p.ShowInterstitialAd();
        StructuredQuery q = new("midi-category");
        p.carrot.server.Get_doc(q.ToJson(), Act_show_list_category);
    }

    private void Act_show_list_category(string s_data)
    {
        Fire_Collection fc = new(s_data);
        Carrot_Box box_list_category =p.carrot.Create_Box(PlayerPrefs.GetString("midi_category", "List of midi genres"), icon_list_category);

        if (p.carrot.model_app == ModelApp.Develope)
        {
            Carrot_Box_Btn_Item btn_add_cat = box_list_category.create_btn_menu_header(this.p.carrot.icon_carrot_add);
            btn_add_cat.set_act(() => this.Box_add_cat_midi());
        }

        if (!fc.is_null)
        {
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_cat = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_cat = box_list_category.create_item("item_cat_" + i);
                item_cat.set_title(data_cat["name"].ToString());
                item_cat.set_tip(data_cat["name"].ToString());
                item_cat.set_icon(icon_list_category);
            }
        }
    }

    private void Box_add_cat_midi()
    {
        p.set_no_use_keyboar_pc();

        Carrot_Box box_add_cat = p.carrot.Create_Box();
        box_add_cat.set_title("Add Category");
        box_add_cat.set_icon(p.carrot.icon_carrot_add);

        Carrot_Box_Item item_name=box_add_cat.create_item();
        item_name.set_type(Box_Item_Type.box_value_input);
        item_name.check_type();

        Carrot_Box_Btn_Panel panel_btn=box_add_cat.create_panel_btn();

        Carrot_Button_Item btn_done=panel_btn.create_btn("btn_add");
        btn_done.set_icon(p.carrot.icon_carrot_done);
        btn_done.set_label(PlayerPrefs.GetString("done", "Done"));
        btn_done.set_act_click(() => Act_add_cat(item_name.get_val()));

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_icon(p.carrot.icon_carrot_cancel);
        btn_cancel.set_label(PlayerPrefs.GetString("cancel", "Cancel"));
        btn_cancel.set_act_click(() => box_add_cat.close());
    }

    private void Act_add_cat(string s_name_cat)
    {
        p.set_use_keyboar_pc();
        p.carrot.show_loading();
        IDictionary data_cat = (IDictionary) Json.Deserialize("{}");
        data_cat["id"] = "cat"+p.carrot.generateID();
        data_cat["name"] = s_name_cat;
        p.carrot.server.Add_Document_To_Collection("midi-category", data_cat["id"].ToString(),p.carrot.server.Convert_IDictionary_to_json(data_cat),Act_add_cat_done);
    }

    private void Act_add_cat_done(string s_data)
    {
        p.carrot.hide_loading();
        p.carrot.show_msg("Category", "Add category success!!!", Msg_Icon.Success);
    }

    public void show_list_midi_by_category(string name_category)
    {
        s_title_box = name_category;
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("category", Query_OP.EQUAL, name_category);
        p.carrot.server.Get_doc(q.ToJson(), this.Act_get_list_midi_online_done);
    }

    private void Delete_midi_from_server(string s_id_midi)
    {
        p.carrot.show_loading();
        p.carrot.server.Delete_Doc(p.carrot.Carrotstore_AppId, s_id_midi, Act_delete_midi_done);
    }

    private void Act_delete_midi_done(string s_data)
    {
        p.carrot.hide_loading();
        p.carrot.show_msg("Midi", "Delete Midi Success!", Msg_Icon.Success);
    }
}
