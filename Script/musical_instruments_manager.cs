using UnityEngine;
using UnityEngine.UI;

public class musical_instruments_manager : MonoBehaviour
{
    public Image img_musical_instruments;
    public GameObject[] obj_musical_instruments_data;
    public GameObject musical_instruments_item_prefab;
    private int sel_musical_instruments = -1;
    private int index_buy_musical_instruments = -1;
    private int index_rewarded_musical_instruments = -1;

    private string s_name_instruments;
    private Carrot.Carrot_Box box_musical_instruments;

    public void Load_musical_instruments()
    {
        this.sel_musical_instruments = PlayerPrefs.GetInt("sel_musical_instruments",0);
        this.load_musical_instruments_by_index(this.sel_musical_instruments);
    }

    public void show_list_musical_instruments()
    {
        this.box_musical_instruments=GetComponent<piano>().carrot.Create_Box(PlayerPrefs.GetString("musical_instruments","Musical Instruments"), img_musical_instruments.sprite);
        for (int i = 0; i < obj_musical_instruments_data.Length; i++)
        {
            GameObject item_m = Instantiate(this.musical_instruments_item_prefab);
            item_m.transform.SetParent(box_musical_instruments.area_all_item);
            item_m.transform.localPosition = new Vector3(item_m.transform.localPosition.x, item_m.transform.localPosition.y, item_m.transform.localPosition.z);
            item_m.transform.localScale = new Vector3(1f, 1f, 1f);
            musical_instruments_data m_data = obj_musical_instruments_data[i].GetComponent<musical_instruments_data>();
            musical_instruments_item m_emp = item_m.GetComponent<musical_instruments_item>();
            m_emp.img_icon.sprite = m_data.icon;
            m_emp.txt_name.text= m_data.s_name;
            m_emp.index = i;
            m_emp.is_buy = m_data.is_buy;
            if (this.sel_musical_instruments == i) item_m.GetComponent<Image>().color = this.GetComponent<piano>().carrot.color_highlight;
            if (m_data.is_buy)
            {
                if(PlayerPrefs.GetInt("musical_instruments_"+i, 0) == 0)
                {
                    m_emp.obj_btn.SetActive(true);
                }
                else
                {
                    m_emp.obj_btn.SetActive(false);
                }   
            }
            else
                m_emp.obj_btn.SetActive(false);
        }
    }

    public void select_musical_instruments(int index_m)
    {
        PlayerPrefs.SetInt("sel_musical_instruments", index_m);
        this.sel_musical_instruments = index_m;
        this.load_musical_instruments_by_index(index_m);
        GameObject.Find("piano").GetComponent<piano>().carrot.close();
    }

    private void load_musical_instruments_by_index(int index_m)
    {
        musical_instruments_data m_data = obj_musical_instruments_data[index_m].GetComponent<musical_instruments_data>();
        for(int i = 0; i < this.GetComponent<piano>().note_black.Length; i++)
        {
            this.GetComponent<piano>().note_black[i].sound_piano.clip = m_data.note_black[i];
        }

        for (int i = 0; i < this.GetComponent<piano>().note_white.Length; i++)
        {
            this.GetComponent<piano>().note_white[i].sound_piano.clip = m_data.note_white[i];
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
            this.load_musical_instruments_by_index(this.index_buy_musical_instruments);
            this.GetComponent<piano>().carrot.close();
        }
    }

    public void on_success_rewarded_musical_instruments()
    {
        if (this.index_rewarded_musical_instruments != -1)
        {
            this.load_musical_instruments_by_index(this.index_rewarded_musical_instruments);
            if (this.box_musical_instruments != null) this.box_musical_instruments.close();
        }
    }

    public string get_name_instruments()
    {
        return this.s_name_instruments;
    }
}
