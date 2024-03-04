using Carrot;
using System.Collections;
using UnityEngine;

public class midi_list : MonoBehaviour
{
    [Header("Main Obj")]
    public piano p;
    [Header("List Midi Obj")]
    public Sprite icon;
    public Sprite icon_public;
    public Sprite icon_pendding;
    public Sprite icon_list_online;
    public Sprite icon_list_category;
    public Sprite icon_midi_buy;
    public Sprite icon_upload;
    private int leng_midi = 0;
    private Carrot_Box box_list_midi;

    private string s_title_box;

    public void Check_midi()
    {
        leng_midi = PlayerPrefs.GetInt("leng_m");
    }

    public void Save_midi(IDictionary data)
    {
        if (data["name"].ToString().Trim()=="") data["name"]= "Midi " + (leng_midi + 1);
        PlayerPrefs.SetString("m_data_" + leng_midi, Json.Serialize(data));
        leng_midi++;
        PlayerPrefs.SetInt("leng_m", leng_midi);
        p.m.Close();
        Check_midi();
        show_list();
    }

    public void Upadte_item_midi(int index_update, IDictionary data)
    {
        if (data["name"].ToString().Trim() == "") data["name"] = "Midi " + (leng_midi + 1);
        PlayerPrefs.SetString("m_data_" + index_update, Json.Serialize(data));
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
                string s_data = PlayerPrefs.GetString("m_data_" + i, "");
                if (s_data != "")
                {
                    IDictionary data_midi = (IDictionary)Json.Deserialize(s_data);
                    data_midi["index"] = i.ToString();
                    Add_Item_to_box(this.box_list_midi, data_midi);
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

    private Carrot_Box_Item Add_Item_to_box(Carrot_Box box,IDictionary data)
    {
        var id_midi = data["id"].ToString();

        Carrot_Box_Item item_m = box.create_item("Item_m_"+id_midi);
        item_m.set_title(data["name"].ToString());
        item_m.set_tip(data["id"].ToString());
        item_m.set_act(() => Open_midi_editor_by_data(data));

        string s_status = data["status"].ToString();
        if (s_status == "offline")
        {
            item_m.set_icon(this.icon);

            Carrot_Box_Btn_Item btn_upload = item_m.create_item();
            btn_upload.set_icon(this.icon_upload);
            btn_upload.set_color(p.carrot.color_highlight);
            btn_upload.set_act(() => Upload_midi(data));

            Carrot_Box_Btn_Item btn_del = item_m.create_item();
            btn_del.set_icon(p.carrot.sp_icon_del_data);
            btn_del.set_color(p.carrot.color_highlight);
            btn_del.set_act(() => delete(int.Parse(data["index"].ToString())));
        }
        else
        {
            if (s_status == "public")
                item_m.set_icon(this.icon_public);
            else
                item_m.set_icon(this.icon_pendding);

            Carrot_Box_Btn_Item btn_download = item_m.create_item();
            btn_download.set_icon(this.p.carrot.icon_carrot_download);
            btn_download.set_color(p.carrot.color_highlight);
            btn_download.set_act(() => Download_midi(data));
        }

        if (p.carrot.model_app == ModelApp.Develope)
        {
            if (s_status != "offline")
            {
                Carrot_Box_Btn_Item btn_change_status = item_m.create_item();
                btn_change_status.set_icon(p.carrot.icon_carrot_write);
                btn_change_status.set_color(p.carrot.color_highlight);
                btn_change_status.set_act(() => Change_Status_midi(data));

                Carrot_Box_Btn_Item btn_del = item_m.create_item();
                btn_del.set_icon(p.carrot.sp_icon_del_data);
                btn_del.set_color(Color.red);
                btn_del.set_act(() => Delete_midi_from_server(id_midi));
            }
        }

        return item_m;
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
        PlayerPrefs.DeleteKey("m_data_"+index);
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

    public void Upload_midi(IDictionary data)
    {
        string s_id_user = p.carrot.user.get_id_user_login();
        data["status"] = "pending";
        
        if (s_id_user != "")
        {
            data["user_id"] = s_id_user;
            data["user_lang"] = p.carrot.user.get_lang_user_login();
        }

        p.carrot.show_loading();
        string s_json=p.carrot.server.Convert_IDictionary_to_json(data);
        p.carrot.server.Add_Document_To_Collection(p.carrot.Carrotstore_AppId, data["id"].ToString(),s_json, Act_upload_midi_done, Act_fail);
    }

    private void Act_upload_midi_done(string s_data)
    {
        p.carrot.show_msg(PlayerPrefs.GetString("midi_public", "Publishing Midi"), PlayerPrefs.GetString("midi_public_success", "Thank you for contributing the draft midi piano, We will review and release to the world in the fastest time possible."), Msg_Icon.Success);
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
                Add_Item_to_box(this.box_list_midi, data);
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
        p.m.Open_midi(data);
        if (box_list_midi != null) box_list_midi.close();
    }

    public void get_midi_by_id(string s_midi_id)
    {
        p.carrot.show_loading();
        if (this.box_list_midi != null) this.box_list_midi.close();
        p.carrot.ads.Destroy_Banner_Ad();
        p.carrot.server.Get_doc_by_path(p.carrot.Carrotstore_AppId,s_midi_id, Act_get_midi, Act_fail);
    }

    private void Act_get_midi(string s_data)
    {
        p.carrot.hide_loading();
        Fire_Document fd = new(s_data);
        p.m.Open_midi(fd.Get_IDictionary());
    }

    private void Act_fail(string s_error)
    {
        p.carrot.hide_loading();
        p.carrot.show_msg("Error", s_error, Msg_Icon.Error);
        p.carrot.play_vibrate();
    }

    public void Btn_show_list_category()
    {
        p.carrot.play_sound_click();
        p.ShowInterstitialAd();
        StructuredQuery q = new("midi-category");
        p.carrot.server.Get_doc(q.ToJson(), Act_show_list_category,Act_fail);
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
                item_cat.set_tip(data_cat["id"].ToString());
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
        item_name.set_icon(p.carrot.user.icon_user_name);
        item_name.set_title("Name Category");
        item_name.set_tip("Enter new name catgeory");
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
        p.carrot.server.Add_Document_To_Collection("midi-category", data_cat["id"].ToString(),p.carrot.server.Convert_IDictionary_to_json(data_cat),Act_add_cat_done,Act_fail);
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
        p.carrot.server.Delete_Doc(p.carrot.Carrotstore_AppId, s_id_midi, Act_delete_midi_done,Act_fail);
    }

    private void Act_delete_midi_done(string s_data)
    {
        p.carrot.hide_loading();
        p.carrot.show_msg("Midi", "Delete Midi Success!", Msg_Icon.Success);
    }

    private void Download_midi(IDictionary data)
    {
        p.carrot.play_sound_click();
        p.carrot.play_vibrate();
        data["status"] = "offline";
        this.Save_midi(data);
        p.carrot.show_msg("Midi", "Download success!", Msg_Icon.Alert);
    }

    private void Change_Status_midi(IDictionary data)
    {
        Carrot_Box box_public = p.carrot.Create_Box();
        box_public.set_title("Change status MIDI (" + data["name"].ToString() + ")");
        box_public.set_icon(this.p.carrot.user.icon_user_status);

        Carrot_Box_Item item_category = box_public.create_item();
        item_category.set_icon(this.p.carrot.icon_carrot_all_category);
        item_category.set_title("Category");
        item_category.set_tip("Select category for MIDI");
        item_category.set_type(Box_Item_Type.box_value_input);
        item_category.check_type();

        Carrot_Box_Item item_status = box_public.create_item();
        item_status.set_icon(this.p.carrot.user.icon_user_info);
        item_status.set_title("Status");
        item_status.set_tip("Select status for MIDI");
        item_status.set_type(Box_Item_Type.box_value_dropdown);
        item_status.check_type();

        item_status.dropdown_val.ClearOptions();
        item_status.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("pedding"));
        item_status.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("public"));
        item_status.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("sell"));

        string s_status = data["status"].ToString();
        if (s_status == "pending") item_status.set_val("0");
        if (s_status == "public") item_status.set_val("1");
        if (s_status == "sell") item_status.set_val("2");

        Carrot_Box_Btn_Panel panel_btn = box_public.create_panel_btn();
        Carrot_Button_Item btn_done=panel_btn.create_btn();
        btn_done.set_act_click(() => Act_change_status_midi(data, item_status.get_val(),item_category.get_val()));
        btn_done.set_label(PlayerPrefs.GetString("done", "Done"));
        btn_done.set_bk_color(p.carrot.color_highlight);
        btn_done.set_icon(p.carrot.icon_carrot_done);

        Carrot_Button_Item btn_cancel=panel_btn.create_btn();
        btn_cancel.set_act_click(() => box_public.close());
        btn_cancel.set_label(PlayerPrefs.GetString("cancel", "Cancel"));
        btn_cancel.set_bk_color(p.carrot.color_highlight);
        btn_cancel.set_icon(p.carrot.icon_carrot_cancel);
    }

    private void Act_change_status_midi(IDictionary data,string s_status,string s_category)
    {
        string s_val_status = "pending";
        if (s_status == "0") s_val_status = "pending";
        if (s_status == "1") s_val_status = "public";
        if (s_status == "2") s_val_status = "sell";
        data["status"] = s_val_status;
        data["category"] = s_category;
        string s_json = p.carrot.server.Convert_IDictionary_to_json(data);
        p.carrot.show_loading();
        p.carrot.server.Add_Document_To_Collection(p.carrot.Carrotstore_AppId, data["id"].ToString(), s_json, Act_change_status_done, Act_fail);
    }

    private void Act_change_status_done(string s_data)
    {
        p.carrot.hide_loading();
        p.carrot.show_msg("Midi (Dev)", "Change status success!", Msg_Icon.Success);
    }
}
