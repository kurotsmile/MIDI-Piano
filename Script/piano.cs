using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class piano : MonoBehaviour
{
    public Carrot.Carrot carrot;
    private int sel_show_text_type = 0;
    private bool is_lock_key = false;
    private bool is_show_effect = true;
    public GameObject panel_menu_top;
    public GameObject panel_menu_mini;
    private string id_midi_online_buy;

    [Header("Icon")]
    public Sprite icon_hide_txt;
    public Sprite icon_piano_txt;
    public Sprite icon_buy_list;
    public Sprite icon_pc_txt;
    public Sprite icon_lock_key;
    public Sprite icon_unlock_key;
    public Sprite icon_effect_show;
    public Sprite icon_effect_hide;

    [Header("Piano")]
    public piano_note[] note_white;
    public piano_note[] note_black;
    public Scrollbar scroll_piano;
    public ScrollRect rec_view_piano;
    public Image img_lock_key;
    public Image img_text_key;
    public Image img_recod;
    public Image img_effect;
    public GameObject panel_midi_editor;
    public Text txt_show_name_note;
    public GameObject effect_play;
    public metronome metro;

    [Header("color")]
    public Color32 color_nomal_white_note;
    public Color32 color_nomal_black_note;
    public Color32 color_play_note;

    private string link_deep_app;
    private bool is_open_list_after_login = false;
#pragma warning disable 414
    private bool is_user_keyboar = true;
#pragma warning restore 414


    private void Start()
    {
        link_deep_app = Application.absoluteURL;
        Application.deepLinkActivated += onDeepLinkActivated;

        carrot.Load_Carrot(check_exit_app);
        carrot.act_after_close_all_box = set_use_keyboar_pc;
        carrot.shop.onCarrotPaySuccess += buy_carrot_pay_success;
        carrot.shop.onCarrotRestoreSuccess += restore_carrot_pay_success;
        carrot.ads.set_act_Rewarded_Success(act_Rewarded_Success);
        this.carrot.act_after_delete_all_data=this.delete_all_data;

        panel_menu_top.SetActive(true);
        panel_menu_mini.SetActive(true);
        panel_midi_editor.SetActive(false);
        metro.panel_metronome.SetActive(false);

        for (int i = 0; i < note_white.Length; i++)
        {
            note_white[i].index = i;
            note_white[i].type = 0;
        }

        for (int i = 0; i < note_black.Length; i++)
        {
            note_black[i].index = i;
            note_black[i].type = 1;
        }

        sel_show_text_type = PlayerPrefs.GetInt("sel_show_text_type", 1);
        check_show_or_hide_txt_note();
        if (PlayerPrefs.GetFloat("pos_scroll_piano", 0) != 0) rec_view_piano.horizontalNormalizedPosition = PlayerPrefs.GetFloat("pos_scroll_piano");
        if (PlayerPrefs.GetInt("is_lock_key", 0) == 0) is_lock_key = false; else is_lock_key = true;
        act_unclock_or_lock_scroll_piano();
        if (PlayerPrefs.GetInt("is_show_effect", 0) == 0) is_show_effect = false; is_show_effect = true;
        act_show_hide_effect_click();
        GetComponent<midi_list>().check_midi();
        this.GetComponent<musical_instruments_manager>().Load_musical_instruments();
    }

    public void check_link_deep_app()
    {
        if (link_deep_app.Trim() != "")
        {
            if (carrot.is_online())
            {
                if (link_deep_app.Contains("show"))
                {
                    string data_link = link_deep_app.Replace("midi://show/", "");
                    GetComponent<midi_list>().get_midi_by_id(data_link);
                    link_deep_app = "";
                }
            }
        }
    }

    public void load_app_where_online()
    {

    }

    public void load_app_where_offline()
    {

    }

    private void check_exit_app()
    {
        if (panel_midi_editor.activeInHierarchy)
        {
            close_midi_editor();
            carrot.set_no_check_exit_app();
        }
        else
        {
            if (metro.panel_metronome.activeInHierarchy)
            {
                metro.panel_metronome.SetActive(false);
                carrot.set_no_check_exit_app();
            }
        }
    }

#if !UNITY_ANDROID
    private void Update()
    {
        if (this.is_user_keyboar)
        {

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                for (int i = 0; i < this.note_black.Length; i++)
                {
                    if (Input.GetKeyDown(this.note_black[i].s_code_shift_pc)) this.note_black[i].click_key();
                }
            }
            else
            {
                for (int i = 0; i < this.note_white.Length; i++) { if (Input.GetKeyDown(this.note_white[i].s_pc)) this.note_white[i].click_key(); }
            }
        }
  
    }
#endif

    public void btn_change_type_note_txt()
    {
        this.carrot.ads.show_ads_Interstitial();
        sel_show_text_type++;
        if (sel_show_text_type > 2) sel_show_text_type = 0;
        PlayerPrefs.SetInt("sel_show_text_type", sel_show_text_type);
        check_show_or_hide_txt_note();
    }

    private void check_show_or_hide_txt_note()
    {
        if (sel_show_text_type == 0) img_text_key.sprite = icon_hide_txt;
        if (sel_show_text_type == 1) img_text_key.sprite = icon_piano_txt;
        if (sel_show_text_type == 2) img_text_key.sprite = icon_pc_txt;
        for (int i = 0; i < note_white.Length; i++) note_white[i].set_type_text_show(sel_show_text_type);
        for (int i = 0; i < note_black.Length; i++) note_black[i].set_type_text_show(sel_show_text_type);
    }

    public void show_hide_effect_click()
    {
        this.carrot.ads.show_ads_Interstitial();
        if (is_show_effect)
        {
            is_show_effect = false;
            PlayerPrefs.SetInt("is_show_effect", 1);
        }
        else
        {
            is_show_effect = true;
            PlayerPrefs.SetInt("is_show_effect", 0);
        }
        act_show_hide_effect_click();
    }

    private void act_show_hide_effect_click()
    {
        if (is_show_effect)
            img_effect.sprite = icon_effect_show;
        else
            img_effect.sprite = icon_effect_hide;
    }

    public void unclock_or_lock_scroll_piano()
    {
        if (is_lock_key)
        {
            PlayerPrefs.SetInt("is_lock_key", 0);
            is_lock_key = false;
        }
        else
        {
            PlayerPrefs.SetInt("is_lock_key", 1);
            is_lock_key = true;
        }
        act_unclock_or_lock_scroll_piano();
    }

    private void act_unclock_or_lock_scroll_piano()
    {
        if (is_lock_key)
        {
            img_lock_key.sprite = icon_lock_key;
            scroll_piano.interactable = false;
            rec_view_piano.enabled = false;
        }
        else
        {
            img_lock_key.sprite = icon_unlock_key;
            scroll_piano.interactable = true;
            rec_view_piano.enabled = true;
        }
    }

    public void show_or_hide_midi_editor()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        if (panel_midi_editor.activeInHierarchy)
            close_midi_editor();
        else
            show_midi_editor();
    }

    public void close_midi_editor()
    {
        panel_midi_editor.GetComponent<midi>().close();
        panel_midi_editor.SetActive(false);
        panel_menu_top.SetActive(true);
        panel_menu_mini.SetActive(true);
        img_recod.color = Color.white;
        ShowInterstitialAd();
        this.carrot.ads.create_banner_ads();
        is_user_keyboar = true;
    }

    private void show_midi_editor()
    {
        panel_midi_editor.SetActive(true);
        panel_menu_top.SetActive(false);
        panel_midi_editor.GetComponent<midi>().load_midi();
        img_recod.color = Color.red;
        panel_menu_mini.SetActive(false);
        this.carrot.ads.Destroy_Banner_Ad();
    }

    public void play_note(int index, int type_note, bool is_pos_mouse)
    {

        if (type_note == 0)
            txt_show_name_note.text = note_white[index].s_piano;
        else
            txt_show_name_note.text = note_black[index].s_piano.Replace("\n", "");

        if (panel_midi_editor.activeInHierarchy) panel_midi_editor.GetComponent<midi>().recod_note_midi(txt_show_name_note.text, index, type_note);

        if (!is_lock_key && !is_pos_mouse)
        {
            if (type_note == 0)
                rec_view_piano.horizontalNormalizedPosition = note_white[index].pos_scroll;
            else
                rec_view_piano.horizontalNormalizedPosition = note_black[index].pos_scroll;
        }

        if (is_show_effect)
        {
            Vector3 clickedPosition;
            if (is_pos_mouse)
                clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else
            {
                if (type_note == 0)
                    clickedPosition = note_white[index].get_pos();
                else
                    clickedPosition = note_black[index].get_pos();
            }
            effect_play.transform.position = new Vector3(clickedPosition.x, clickedPosition.y, -12.5f);
            effect_play.SetActive(false);
            effect_play.SetActive(true);
        }

    }

    public void play_note_midi(int[] index_note_piano, int[] type_note)
    {
        rest_color_all_note();
        for (int i = 0; i < index_note_piano.Length; i++)
        {
            int i_note_piano = index_note_piano[i];
            if (i_note_piano != -1)
            {
                if (type_note[i] == 0)
                {
                    note_white[i_note_piano].play();
                    note_white[i_note_piano].GetComponent<Image>().color = color_play_note;
                }

                if (type_note[i] == 1)
                {
                    note_black[i_note_piano].play();
                    note_black[i_note_piano].GetComponent<Image>().color = color_play_note;
                }

                if (!is_lock_key)
                {
                    if (type_note[i] == 0)
                        rec_view_piano.horizontalNormalizedPosition = note_white[i_note_piano].pos_scroll;
                    else
                        rec_view_piano.horizontalNormalizedPosition = note_black[i_note_piano].pos_scroll;
                }
            }
        }
    }

    public void rest_color_all_note()
    {
        for (int i = 0; i < note_white.Length; i++)
        {
            note_white[i].GetComponent<Image>().color = color_nomal_white_note;
        }

        for (int i = 0; i < note_black.Length; i++)
        {
            note_black[i].GetComponent<Image>().color = color_nomal_black_note;
        }
    }

    public void save_pos_scroll()
    {
        PlayerPrefs.SetFloat("pos_scroll_piano", rec_view_piano.horizontalNormalizedPosition);
    }

    public void load_midi(midi_item item_midi)
    {
        panel_midi_editor.GetComponent<midi>().open_midi(item_midi);
        carrot.close();
    }

    public string get_txt_note_piano(int index_note, int type_note)
    {
        if (index_note != -1)
        {
            if (type_note == 0)
                return note_white[index_note].txt_piano.text;
            else
                return note_black[index_note].txt_piano.text.Replace("\n", ""); ;
        }
        else
        {
            return "...";
        }
    }

    private void delete_all_data()
    {
        PlayerPrefs.DeleteAll();
        this.carrot.delay_function(2f, this.Start);
    }

    public void show_metronome()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.carrot.play_sound_click();
        metro.show_setting();
    }

    public void ShowInterstitialAd()
    {
        carrot.ads.show_ads_Interstitial();
    }

    public void show_setting()
    {
        this.carrot.ads.show_ads_Interstitial();
        Carrot.Carrot_Box box_setting=this.carrot.Create_Setting();

        Carrot.Carrot_Box_Item setting_instruments = box_setting.create_item_of_top("list_midi");
        setting_instruments.set_icon(this.GetComponent<musical_instruments_manager>().img_musical_instruments.sprite);
        setting_instruments.set_title(PlayerPrefs.GetString("musical_instruments", "Musical Instruments"));
        setting_instruments.set_tip(this.GetComponent<musical_instruments_manager>().get_name_instruments());
        setting_instruments.set_key_lang_title("musical_instruments");
        setting_instruments.set_act(() => this.GetComponent<musical_instruments_manager>().show_list_musical_instruments());

        if (PlayerPrefs.GetInt("buy_list", 0) == 0)
        {
            Carrot.Carrot_Box_Item setting_list_midi = box_setting.create_item_of_top("list_midi");
            setting_list_midi.set_icon(this.icon_buy_list);
            setting_list_midi.set_title(PlayerPrefs.GetString("buy_list_midi", "Use all midi from online list"));
            setting_list_midi.set_tip(PlayerPrefs.GetString("buy_list_midi_tip", "Use all midi from online listings without having to buy midi in the future"));
            setting_list_midi.set_lang_data("buy_list_midi","buy_list_midi_tip");
            setting_list_midi.set_act(() => this.carrot.buy_product(2));
        }
    }

    public void game_rate()
    {
        carrot.show_rate();
    }

    public void show_list_online_by_user_login()
    {
        this.carrot.play_sound_click();
        this.GetComponent<midi_list>().show_list_buy_user_id(carrot.user.get_id_user_login());
    }

    public void btn_show_account_login()
    {
        this.carrot.ads.show_ads_Interstitial();
        is_user_keyboar = false;
        is_open_list_after_login = false;
        carrot.show_login();
    }

    public void btn_buy_product(int index)
    {
        carrot.buy_product(index);
    }

    public void btn_show_list_app_carrot()
    {
        carrot.show_list_carrot_app();
    }

    public void btn_share_app()
    {
        carrot.show_share();
    }

    public void btn_show_list_change_lang()
    {
        carrot.show_list_lang();
    }

    public void buy_success(Product product)
    {
        buy_carrot_pay_success(product.definition.id);
    }

    public void buy_carrot_pay_success(string s_id_product)
    {
        if (s_id_product == carrot.shop.get_id_by_index(0))
        {
            GameObject.Find("piano").GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("in_app_midi_success", "Thank you for your purchase, now you can use MIDI"));
            GameObject.Find("piano").GetComponent<midi_list>().get_midi_by_id(id_midi_online_buy);
        }

        if (s_id_product == carrot.shop.get_id_by_index(2))
        {
            carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("in_app_list_success", "Thank you for your purchase, Now you can use all the midi songs in the online playlist"));
            act_inapp_list_midi();
        }

        if (s_id_product == carrot.shop.get_id_by_index(3))
        {
            carrot.show_msg(PlayerPrefs.GetString("shop", "Shop"), PlayerPrefs.GetString("in_app_instruments_success", "Thank you for your purchase, Now you can use all the midi songs in the online playlist"));
            act_inapp_instruments();
        }
    }

    public void restore_carrot_pay_success(string[] arr_id)
    {
        for (int i = 0; i < arr_id.Length; i++)
        {
            string s_id_product = arr_id[i];
            if (s_id_product == carrot.shop.get_id_by_index(2)) act_inapp_list_midi();
        }
    }

    private void act_inapp_list_midi()
    {
        PlayerPrefs.SetInt("buy_list", 1);
    }

    private void act_inapp_instruments()
    {
        this.GetComponent<musical_instruments_manager>().on_success_buy_musical_instruments();
    }

    public void set_id_midi_buy(string s_id)
    {
        id_midi_online_buy = s_id;
    }

    public void event_click_field_user_customer()
    {
        show_list_online_by_user_login();
    }

    public void btn_show_list_midi_public_by_me()
    {
        is_user_keyboar = false;
        if (carrot.user.get_id_user_login() == "")
        {
            carrot.show_login();
            is_open_list_after_login = true;
        }
        else
        {
            ShowInterstitialAd();
            show_list_online_by_user_login();
        }
    }

    public void event_where_after_login_user()
    {
        if (is_open_list_after_login)
        {
            carrot.delay_function(1f, show_list_online_by_user_login);
            is_open_list_after_login = false;
        }
    }

    public void delete_midi_by_id(string id_midi)
    {
        this.carrot.show_loading();
        this.carrot.server.Delete_Doc(carrot.Carrotstore_AppId, id_midi,Act_delete_midi);
    }

    private void Act_delete_midi(string s_data)
    {
        carrot.show_msg(PlayerPrefs.GetString("list_midi_your_public", "Your published midi list"), PlayerPrefs.GetString("delete_success", "Deleted successfully!"));
        this.carrot.hide_loading();
    }

    public void set_use_keyboar_pc()
    {
        is_user_keyboar = true;
    }

    public void set_no_use_keyboar_pc()
    {
        is_user_keyboar = false;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) carrot.delay_function(1f, check_link_deep_app);
    }

    private void onDeepLinkActivated(string url)
    {
        link_deep_app = url;
        if (carrot != null) carrot.delay_function(1f, check_link_deep_app);
    }

    public void buy_musical_instruments(int index_tool)
    {
        this.GetComponent<musical_instruments_manager>().set_index_buy_musical_instruments(index_tool);
        this.carrot.buy_product(3);
    }

    [ContextMenu("Test Rewarded Ads Success")]
    private void act_Rewarded_Success()
    {
        this.GetComponent<musical_instruments_manager>().on_success_rewarded_musical_instruments();
    }

}
