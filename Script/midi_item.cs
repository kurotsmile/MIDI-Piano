using UnityEngine;
using UnityEngine.UI;

public class midi_item : MonoBehaviour
{
    public int index;
    public Text txt_name;
    public string s_data_index;
    public string s_data_type;
    public float speed;
    public int sell = 0;
    public GameObject btn_upload;
    public GameObject btn_delete;
    public GameObject btn_view;
    public GameObject btn_share;
    public Image btn_buy;
    public string link;
    public string id_midi;
    public string name_midi;
    public int type_edit;
    public Image icon;
    private Carrot.Carrot_Window_Msg msg;

    public void check_type()
    {
        if (sell == 0)
        {
            btn_buy.gameObject.SetActive(false);
            btn_upload.SetActive(true);
            btn_delete.SetActive(true);
        }
        else
        {
            btn_upload.SetActive(false);
            btn_delete.SetActive(false);
        }
    }
    public void delete()
    {
        if (type_edit == 0)
            this.msg=GameObject.Find("piano").GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("list_midi_your", "Your Midi list"), PlayerPrefs.GetString("del_question_item", "Are you sure you want to delete this selected item?"), act_delete_list_midi_item_yes, act_delete_midi_item_no);
        else
            this.msg=GameObject.Find("piano").GetComponent<piano>().carrot.show_msg(PlayerPrefs.GetString("midi_public", "Publishing Midi"), PlayerPrefs.GetString("del_question_item", "Are you sure you want to delete this selected item?"), act_delete_acc_midi_item_yes, act_delete_midi_item_no);
    }

    private void act_delete_list_midi_item_yes()
    {
        if (this.msg != null) this.msg.UI.close();
        GameObject.Find("piano").GetComponent<midi_list>().delete(index);
    }

    private void act_delete_acc_midi_item_yes()
    {
        if (this.msg != null) this.msg.UI.close();
        GameObject.Find("piano").GetComponent<piano>().delete_midi_by_id(id_midi);
    }

    private void act_delete_midi_item_no()
    {
        if (this.msg != null) this.msg.UI.close();
    }

    public void click()
    {
        if (sell == 0)
            load_midi();
        else if (sell == 1)
            load_midi();
        else if (sell == -1)
            GameObject.Find("piano").GetComponent<midi_list>().show_list_midi_by_category(name_midi);
        else
        {
            GameObject.Find("piano").GetComponent<piano>().set_id_midi_buy(id_midi);
            GameObject.Find("piano").GetComponent<piano>().btn_buy_product(0);
        }
    }

    private void load_midi()
    {
        if (id_midi != "")
        {
            GameObject.Find("piano").GetComponent<midi_list>().get_midi_by_id(id_midi);
        }
        else
        {
            GameObject.Find("piano").GetComponent<piano>().load_midi(this);
        }
    }

    public void upload()
    {
        GameObject.Find("piano").GetComponent<midi_list>().Upload_midi(this);
    }


    public void go_link_store()
    {
        Application.OpenURL(link);
    }

    public void share_link()
    {
        string url_midi = GameObject.Find("piano").GetComponent<piano>().carrot.mainhost + "/piano/" + id_midi;
        GameObject.Find("piano").GetComponent<piano>().carrot.show_share(url_midi, PlayerPrefs.GetString("share_your_midi_tip", "Share this work with everyone or your friends to enjoy"));
    }

}
