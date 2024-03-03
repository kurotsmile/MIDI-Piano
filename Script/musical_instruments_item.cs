using UnityEngine;
using UnityEngine.UI;

public class musical_instruments_item : MonoBehaviour
{
    public Image img_icon;
    public Text txt_name;
    public int index;
    public GameObject obj_btn;
    public bool is_buy;

    private Carrot.Carrot_Window_Msg msg_musical_instruments = null;

    public void click()
    {
        if (this.is_buy)
            GameObject.Find("piano").GetComponent<piano>().buy_musical_instruments(index);
        else
            GameObject.Find("piano").GetComponent<musical_instruments_manager>().select_musical_instruments(this.index);
    }

    public void get_rewarded()
    {
        this.msg_musical_instruments=GameObject.Find("piano").GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("musical_instruments","Musical Instruments"),PlayerPrefs.GetString("rewarded_ads_tip","Watch an ad to get a bonus for this instrument?"), get_rewarded_yes, get_rewarded_no);
    }

    private void get_rewarded_yes()
    {
        if (this.msg_musical_instruments != null) this.msg_musical_instruments.close();
        GameObject.Find("piano").GetComponent<musical_instruments_manager>().set_index_ads_rewarded_instruments(this.index);
        GameObject.Find("piano").GetComponent<piano>().carrot.ads.show_ads_Rewarded();
    }

    private void get_rewarded_no()
    {
        if (this.msg_musical_instruments != null) this.msg_musical_instruments.close();
    }
}
