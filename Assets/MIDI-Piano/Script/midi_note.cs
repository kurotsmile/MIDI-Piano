using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class midi_note : MonoBehaviour
{
    public int index_line = -1;
    public int index_note_midi = -1;
    public int index_note_piano = -1;
    public int type_note_piano = 0;
    public Text txt;
    public void click()
    {
        GameObject.Find("piano").GetComponent<midi>().select_midi_note(this);
    }

    public void play(Color32 colr)
    {
        txt.color = Color.black;
        GetComponent<Image>().color = colr;
        StartCoroutine(LateCall());
    }

    public void no_select(Color32 colr)
    {
        GetComponent<Image>().color = colr;
    }

    private float sec = 2f;

    IEnumerator LateCall()
    {
        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);
    }

    public void rest_note_show()
    {
        txt.color = Color.white;
        StopAllCoroutines();
        gameObject.SetActive(true);
    }
}
