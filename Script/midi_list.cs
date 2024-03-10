using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum Type_List_MIDI {Pending,Public,Search}

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
    public Sprite icon_upload;
    public Sprite icon_export;

    private int leng_midi = 0;
    private Carrot_Box box_list_midi;
    private UnityAction<string> act_sel_category;
    private Carrot_Box_Item item_edit_name;
    private Carrot_Box_Item item_edit_category;
    private Carrot_Box_Item item_edit_buy;
    private Carrot_Box_Item item_edit_author;
    private Carrot_Box_Item item_edit_status;
    private Carrot_Box box_category;
    private Carrot_Box box_edit;
    private Carrot_Window_Input box_search;
    private Type_List_MIDI type;

    private IDictionary data_buy_midi =null;
    private string s_data_list_midi_public = "";
    private string s_title_box;

    public void Check_midi()
    {
        leng_midi = PlayerPrefs.GetInt("leng_m");
        if (p.carrot.is_offline()) s_data_list_midi_public = PlayerPrefs.GetString("s_data_list_midi_public");
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
            for (int i = leng_midi-1; i >=0; i--)
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

        bool buy=false;
        Carrot_Box_Item item_m = box.create_item("Item_m_"+id_midi);
        item_m.set_title(data["name"].ToString());
        if (data["author"] != null)
            item_m.set_tip(data["author"].ToString());
        else
            item_m.set_tip(data["id"].ToString());

        if (data["buy"] != null)
        {
            if (data["buy"].ToString() == "0")
            {
                buy = false;
            }
            else
            {
                if (PlayerPrefs.GetInt("buy_midi_" + id_midi, 0) == 1)
                    buy = false;
                else
                    buy = true;
            }
        }

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
            btn_del.set_act(() => Delete(int.Parse(data["index"].ToString())));

            item_m.set_act(() => Open_midi_editor_by_data(data));
        }
        else
        {
            if (s_status == "public")
                item_m.set_icon(this.icon_public);
            else
                item_m.set_icon(this.icon_pendding);

            if (buy)
            {
                Carrot_Box_Btn_Item btn_buy = item_m.create_item();
                btn_buy.set_icon(p.icon_buy);
                btn_buy.set_color(p.carrot.color_highlight);
                btn_buy.set_act(() =>Buy_midi(data));

                item_m.set_act(() => Buy_midi(data));
            }
            else
            {
                Carrot_Box_Btn_Item btn_download = item_m.create_item();
                btn_download.set_icon(this.p.carrot.icon_carrot_download);
                btn_download.set_color(p.carrot.color_highlight);
                btn_download.set_act(() => Download_midi(data));

                item_m.set_act(() => Open_midi_editor_by_data(data));
            }

            if (data["user_id"] != null)
            {
                string user_id = data["user_id"].ToString();
                string user_lang = data["user_lang"].ToString();
                Carrot_Box_Btn_Item btn_user = item_m.create_item();
                btn_user.set_icon(p.carrot.user.icon_user_login_true);
                btn_user.set_color(p.carrot.color_highlight);
                btn_user.set_act(() => p.carrot.user.show_user_by_id(user_id, user_lang));
            }

        }

        Carrot_Box_Btn_Item btn_export = item_m.create_item();
        btn_export.set_icon(icon_export);
        btn_export.set_color(p.carrot.color_highlight);
        btn_export.set_act(() => p.m.Export_midi_by_data(data));

        if (p.carrot.model_app == ModelApp.Develope)
        {
            if (s_status != "offline")
            {               
                Carrot_Box_Btn_Item btn_search = item_m.create_item();
                btn_search.set_icon(p.carrot.icon_carrot_search);
                btn_search.set_color(p.carrot.color_highlight);
                btn_search.set_act(() => Open_link_search_name(data["name"].ToString()));

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

    private void Open_link_search_name(string s_name)
    {
        string s_search = "https://www.google.com/search?q=" +s_name;
        p.carrot.play_sound_click();
        Application.OpenURL(s_search);
    }

    public void func_search()
    {
        p.set_no_use_keyboar_pc();
        this.box_search=p.carrot.show_search( Act_done_search, PlayerPrefs.GetString("search_midi_tip", "You can Search for midi songs online, for reference and use for composition purposes"));
    }

    private void Act_done_search(string s_data)
    {
        s_title_box = s_data;
        if (this.box_search != null) this.box_search.close();
        p.carrot.show_loading();
        type = Type_List_MIDI.Search;
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("name", Query_OP.EQUAL, s_data.ToLower());
        p.carrot.server.Get_doc(q.ToJson(), this.Act_get_list_midi_online_done,Act_fail);
    }

    public void Delete(int index)
    {
        PlayerPrefs.DeleteKey("m_data_"+index);
        Check_after_delete();
        Check_midi();
        show_list();
    }

    private void Check_after_delete()
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
        type = Type_List_MIDI.Public;
        p.carrot.ads.show_ads_Interstitial();
        s_title_box = PlayerPrefs.GetString("list_midi_online", "List Midi Online");
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("status", Query_OP.EQUAL, "public");
        p.carrot.show_loading();
        p.carrot.server.Get_doc(q.ToJson(), Act_get_list_midi_online_done);
    }

    private void show_list_pending_midi()
    {
        type = Type_List_MIDI.Pending;
        p.carrot.ads.show_ads_Interstitial();
        s_title_box = PlayerPrefs.GetString("list_midi_online", "List Midi Online");
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("status", Query_OP.EQUAL, "pending");
        p.carrot.show_loading();
        p.carrot.server.Get_doc(q.ToJson(), Act_get_list_midi_online_done);
    }

    public void show_list_by_user_id(string s_user_id)
    {
        p.carrot.show_loading();
        s_title_box = PlayerPrefs.GetString("list_midi_online", "List Midi Online");
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("user_id", Query_OP.EQUAL, s_user_id);
        p.carrot.server.Get_doc(q.ToJson(), Act_get_list_midi_online_done);
    }

    private void Act_get_list_midi_online_done(string s_data)
    {
        p.carrot.hide_loading();
        if (type == Type_List_MIDI.Public)
        {
            if (this.s_data_list_midi_public == "")
            {
                this.s_data_list_midi_public = s_data;
                PlayerPrefs.SetString("s_data_list_midi_public", s_data);
                this.Show_List_By_data(s_data);
            }
            else
            {
                this.Show_List_By_data(s_data_list_midi_public);
            }
        }
        else
        {
            this.Show_List_By_data(s_data);
        }
    }

    private void Show_List_By_data(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            if (this.box_list_midi != null) this.box_list_midi.close();
            this.box_list_midi = p.carrot.Create_Box(s_title_box, icon_list_online);
            Carrot_Box_Btn_Item btn_cat = this.box_list_midi.create_btn_menu_header(p.carrot.icon_carrot_all_category);
            btn_cat.set_act(() => this.Btn_show_list_category());

            if (type == Type_List_MIDI.Public)
            {
                Carrot_Box_Btn_Item btn_pending = this.box_list_midi.create_btn_menu_header(icon_pendding);
                btn_pending.set_act(() => this.show_list_pending_midi());
            }

            if (type == Type_List_MIDI.Pending)
            {
                Carrot_Box_Btn_Item btn_pending = this.box_list_midi.create_btn_menu_header(icon_public);
                btn_pending.set_act(() => this.show_list_download_midi());
            }

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
        Show_list_category(null);
    }

    private void Show_list_category(UnityAction<string> act_done_sel)
    {
        this.act_sel_category = act_done_sel;
        p.ShowInterstitialAd();
        StructuredQuery q = new("midi-category");
        p.carrot.server.Get_doc(q.ToJson(), Act_show_list_category, Act_fail);
    }

    private void Act_show_list_category(string s_data)
    {
        Fire_Collection fc = new(s_data);
        box_category =p.carrot.Create_Box(PlayerPrefs.GetString("midi_category", "List of midi genres"), icon_list_category);

        if (p.carrot.model_app == ModelApp.Develope)
        {
            Carrot_Box_Btn_Item btn_add_cat = box_category.create_btn_menu_header(this.p.carrot.icon_carrot_add);
            btn_add_cat.set_act(() => this.Box_add_cat_midi());
        }

        if (!fc.is_null)
        {
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_cat = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item item_cat = box_category.create_item("item_cat_" + i);
                var name_cat = data_cat["name"].ToString();
                var id_cat = data_cat["id"].ToString();
                item_cat.set_title(data_cat["name"].ToString());
                item_cat.set_tip(data_cat["id"].ToString());
                item_cat.set_icon(icon_list_category);
                if (this.act_sel_category != null) item_cat.set_act(() => act_sel_category(name_cat));
                else item_cat.set_act(() => Show_list_midi_by_category(name_cat));

                if (p.carrot.model_app == ModelApp.Develope)
                {
                    Carrot_Box_Btn_Item btn_del=item_cat.create_item();
                    btn_del.set_icon(p.carrot.sp_icon_del_data);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() => Delete_category_from_server(id_cat));
                }
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

    private void Show_list_midi_by_category(string name_category)
    {
        s_title_box = name_category;
        StructuredQuery q = new(p.carrot.Carrotstore_AppId);
        q.Add_where("category", Query_OP.EQUAL, name_category);
        p.carrot.server.Get_doc(q.ToJson(), this.Act_get_list_midi_online_done);
    }

    private void Delete_category_from_server(string s_id_cat)
    {
        p.carrot.show_loading();
        p.carrot.server.Delete_Doc(PlayerPrefs.GetString("midi_category", "List of midi genres"), s_id_cat, Act_delete_category_done, Act_fail);
    }

    private void Act_delete_category_done(string s_data)
    {
        p.carrot.hide_loading();
        p.carrot.show_msg("Midi", "Delete category Success!", Msg_Icon.Success);
        if (box_category != null) box_category.close();
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
        this.p.set_no_use_keyboar_pc();
        box_edit = p.carrot.Create_Box();
        box_edit.set_title("Change status MIDI (" + data["name"].ToString() + ")");
        box_edit.set_icon(this.p.carrot.user.icon_user_status);

        item_edit_name = box_edit.create_item();
        item_edit_name.set_icon(p.carrot.user.icon_user_name);
        item_edit_name.set_title("Name");
        item_edit_name.set_tip("Name for midi");
        item_edit_name.set_type(Box_Item_Type.box_value_input);
        item_edit_name.check_type();
        if(data["name"] != null) item_edit_name.set_val(data["name"].ToString());

        item_edit_category = box_edit.create_item();
        item_edit_category.set_icon(this.p.carrot.icon_carrot_all_category);
        item_edit_category.set_title("Category");
        item_edit_category.set_tip("Select category for MIDI");
        item_edit_category.set_type(Box_Item_Type.box_value_input);
        item_edit_category.check_type();
        if (data["category"] != null) item_edit_category.set_val(data["category"].ToString());

        Carrot_Box_Btn_Item btn_sel_cat = item_edit_category.create_item();
        btn_sel_cat.set_icon(this.icon_list_category);
        btn_sel_cat.set_color(p.carrot.color_highlight);
        btn_sel_cat.set_act(() => this.Show_list_category(this.Select_Category_for_Edit));

        item_edit_status = box_edit.create_item();
        item_edit_status.set_icon(this.p.carrot.user.icon_user_info);
        item_edit_status.set_title("Status");
        item_edit_status.set_tip("Select status for MIDI");
        item_edit_status.set_type(Box_Item_Type.box_value_dropdown);
        item_edit_status.check_type();

        item_edit_status.dropdown_val.ClearOptions();
        item_edit_status.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("pedding"));
        item_edit_status.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("public"));

        string s_status = data["status"].ToString();
        if (s_status == "pending") item_edit_status.set_val("0");
        if (s_status == "public") item_edit_status.set_val("1");

        item_edit_buy = box_edit.create_item();
        item_edit_buy.set_icon(p.icon_buy);
        item_edit_buy.set_title("Sell");
        item_edit_buy.set_tip("Set buy Midi");
        item_edit_buy.set_type(Box_Item_Type.box_value_dropdown);
        item_edit_buy.check_type();

        item_edit_buy.dropdown_val.ClearOptions();
        item_edit_buy.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("Free"));
        item_edit_buy.dropdown_val.options.Add(new UnityEngine.UI.Dropdown.OptionData("Buy"));

        item_edit_author = box_edit.create_item();
        item_edit_author.set_icon(p.icon_buy);
        item_edit_author.set_title("Author");
        item_edit_author.set_tip("Author of the music composition");
        item_edit_author.set_type(Box_Item_Type.box_value_input);
        item_edit_author.check_type();

        if (data["author"] != null) item_edit_author.set_val(data["author"].ToString());

        if (data["buy"] != null)
        {
            string s_buy = data["buy"].ToString();
            if (s_buy == "0") item_edit_buy.set_val("0");
            if (s_buy == "1") item_edit_buy.set_val("1");
        }

        Carrot_Box_Btn_Panel panel_btn = box_edit.create_panel_btn();
        Carrot_Button_Item btn_done=panel_btn.create_btn();
        btn_done.set_act_click(() => Act_change_status_midi(data));
        btn_done.set_label(PlayerPrefs.GetString("done", "Done"));
        btn_done.set_bk_color(p.carrot.color_highlight);
        btn_done.set_icon(p.carrot.icon_carrot_done);

        Carrot_Button_Item btn_cancel=panel_btn.create_btn();
        btn_cancel.set_act_click(() => box_edit.close());
        btn_cancel.set_label(PlayerPrefs.GetString("cancel", "Cancel"));
        btn_cancel.set_bk_color(p.carrot.color_highlight);
        btn_cancel.set_icon(p.carrot.icon_carrot_cancel);
    }

    private void Select_Category_for_Edit(string s_name_category)
    {
        p.carrot.show_msg(s_name_category);
        p.carrot.play_sound_click();
        this.item_edit_category.set_val(s_name_category);
        if (box_category != null) box_category.close();
    }

    private void Act_change_status_midi(IDictionary data)
    {
        string s_val_status = "pending";
        if (item_edit_status.get_val() == "0") s_val_status = "pending";
        if (item_edit_status.get_val() == "1") s_val_status = "public";

        data["name"] = item_edit_name.get_val();
        data["status"] = s_val_status;
        data["category"] = item_edit_category.get_val();
        data["buy"] = item_edit_buy.get_val();
        data["author"] = item_edit_author.get_val();
        string s_json = p.carrot.server.Convert_IDictionary_to_json(data);
        p.carrot.show_loading();
        p.carrot.server.Add_Document_To_Collection(p.carrot.Carrotstore_AppId, data["id"].ToString(), s_json, Act_change_status_done, Act_fail);
    }

    private void Act_change_status_done(string s_data)
    {
        p.set_use_keyboar_pc();
        s_data_list_midi_public = "";
        p.carrot.hide_loading();
        p.carrot.show_msg("Midi (Dev)", "Change status success!", Msg_Icon.Success);
        if (box_list_midi != null) box_list_midi.close();
        if (box_edit != null) box_edit.close();
    }

    public void Buy_midi(IDictionary data_midi)
    {
        this.data_buy_midi = data_midi;
        p.carrot.shop.buy_product(0);
    }

    public void On_buy_success()
    {
        if (this.data_buy_midi != null)
        {
            PlayerPrefs.SetInt("buy_midi_" + data_buy_midi["id"].ToString(), 1);
            this.Open_midi_editor_by_data(this.data_buy_midi);
            this.data_buy_midi=null;
            if (box_list_midi != null) box_list_midi.close();
        }
    }
}
