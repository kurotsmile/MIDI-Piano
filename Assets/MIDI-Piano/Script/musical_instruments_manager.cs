using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class musical_instruments_manager : MonoBehaviour
{
    [Header("Obj Main")]
    public piano p;

    [Header("Obj Musical Instruments")]
    public Image img_musical_instruments;
    public GameObject[] obj_musical_instruments_data;
    private int sel_musical_instruments = -1;
    private int index_buy_musical_instruments = -1;
    private int index_rewarded_musical_instruments = -1;

    private string s_name_instruments;
    private Carrot_Box box_musical_instruments;
    private Carrot_Window_Msg msg_musical_instruments = null;

    private bool is_buy_allinstrument = false;
    public int index_buy_musical_all_instruments = 4;
    private Carrot_Box_Btn_Item btn_buy_head = null;

    public void Load_musical_instruments()
    {
        this.sel_musical_instruments = PlayerPrefs.GetInt("sel_musical_instruments",0);
        this.Load_musical_instruments_by_index(this.sel_musical_instruments);

        if (PlayerPrefs.GetInt("is_buy_allinstrument", 0) == 0)
            is_buy_allinstrument = false;
        else
            is_buy_allinstrument = true;
    }

    public void show_list_musical_instruments()
    {
        this.box_musical_instruments=p.carrot.Create_Box(PlayerPrefs.GetString("musical_instruments","Musical Instruments"), img_musical_instruments.sprite);

        if (!this.is_buy_allinstrument)
        {
            btn_buy_head = this.box_musical_instruments.create_btn_menu_header(this.p.carrot.icon_carrot_buy);
            btn_buy_head.set_act(() => this.Buy_all_instrument());
        }

        for (int i = 0; i < obj_musical_instruments_data.Length; i++)
        {
            var index = i;
            musical_instruments_data m_data = obj_musical_instruments_data[i].GetComponent<musical_instruments_data>();
            Carrot_Box_Item item_instruments = this.box_musical_instruments.create_item();
            item_instruments.set_icon(m_data.icon);
            item_instruments.set_title(m_data.s_name);
            
            bool is_buy = m_data.is_buy;
            
            if (this.is_buy_allinstrument)
            {
                is_buy = false;
            }
            else
            {
                if (PlayerPrefs.GetInt("musical_instruments_" + i, 0) == 1) is_buy = false;
            }
            
            if (is_buy)
            {
                item_instruments.set_act(() => p.buy_musical_instruments(index));
                item_instruments.set_tip("Buy to use");

                if (p.carrot.os_app != OS.Window)
                {
                    Carrot_Box_Btn_Item btn_ads = item_instruments.create_item();
                    btn_ads.set_icon(this.p.icon_ads_gift);
                    btn_ads.set_color(p.carrot.color_highlight);
                    btn_ads.set_act(() => Get_rewarded(index));
                }

                Carrot_Box_Btn_Item btn_buy = item_instruments.create_item();
                btn_buy.set_icon(this.p.icon_buy);
                btn_buy.set_color(p.carrot.color_highlight);
                Destroy(btn_buy.GetComponent<Button>());
            }
            else
            {
                item_instruments.set_act(() => Select_musical_instruments(index));
                item_instruments.set_tip("Free");
            }

            if (this.sel_musical_instruments == i)
            {
                item_instruments.GetComponent<Image>().color = p.carrot.color_highlight;
                item_instruments.txt_tip.color = Color.white;
            }
        }
        this.box_musical_instruments.update_color_table_row();
    }

    public void Get_rewarded(int index)
    {
        this.index_rewarded_musical_instruments = index;
        this.msg_musical_instruments =p.carrot.Show_msg(PlayerPrefs.GetString("musical_instruments", "Musical Instruments"), PlayerPrefs.GetString("rewarded_ads_tip", "Watch an ad to get a bonus for this instrument?"), Get_rewarded_yes, Get_rewarded_no);
    }

    private void Get_rewarded_yes()
    {
        if (this.msg_musical_instruments != null) this.msg_musical_instruments.close();
        p.musical_instruments.set_index_ads_rewarded_instruments(this.index_rewarded_musical_instruments);
        p.ads.Show_ads_Interstitial();
    }

    private void Get_rewarded_no()
    {
        if (this.msg_musical_instruments != null) this.msg_musical_instruments.close();
    }

    public void Select_musical_instruments(int index_m)
    {
        PlayerPrefs.SetInt("sel_musical_instruments", index_m);
        this.sel_musical_instruments = index_m;
        this.Load_musical_instruments_by_index(index_m);
        p.carrot.close();
    }

    private void Load_musical_instruments_by_index(int index_m)
    {
        musical_instruments_data m_data = obj_musical_instruments_data[index_m].GetComponent<musical_instruments_data>();
        for(int i = 0; i < p.note_black.Length; i++)
        {
            p.note_black[i].sound_piano.clip = m_data.note_black[i];
        }

        for (int i = 0; i < p.note_white.Length; i++)
        {
            p.note_white[i].sound_piano.clip = m_data.note_white[i];
        }
        this.img_musical_instruments.sprite = m_data.icon;
        this.s_name_instruments = m_data.s_name;
    }

    public void set_index_buy_musical_instruments(int index_instruments)
    {
        this.index_buy_musical_instruments = index_instruments;
    }

    public void set_index_ads_rewarded_instruments(int index_instruments)
    {
        this.index_rewarded_musical_instruments = index_instruments;
    }

    public void on_success_buy_musical_instruments()
    {
        PlayerPrefs.SetInt("musical_instruments_"+this.index_buy_musical_instruments,1);
        if (this.index_buy_musical_instruments != -1)
        {
            this.Load_musical_instruments_by_index(this.index_buy_musical_instruments);
            p.carrot.close();
            this.index_buy_musical_instruments = -1;
        }
    }

    public void On_success_buy_all_musical_instruments()
    {
        PlayerPrefs.SetInt("is_buy_allinstrument", 1);
        this.is_buy_allinstrument = true;
        if (this.btn_buy_head != null) Destroy(this.btn_buy_head.gameObject);
    }

    public void on_success_rewarded_musical_instruments()
    {
        if (this.index_rewarded_musical_instruments != -1)
        {
            this.Load_musical_instruments_by_index(this.index_rewarded_musical_instruments);
            this.index_rewarded_musical_instruments = -1;
            if (this.box_musical_instruments != null) this.box_musical_instruments.close();
        }
    }

    public string get_name_instruments()
    {
        return this.s_name_instruments;
    }

    public void Buy_all_instrument()
    {
        this.p.carrot.buy_product(this.index_buy_musical_all_instruments);
    }
}
