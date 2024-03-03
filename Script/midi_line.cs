using System.Collections.Generic;
using UnityEngine;

public class midi_line : MonoBehaviour
{
    public int index_line = -1;
    public GameObject midi_note_prefab;
    private List<midi_note> list_midi_note = new List<midi_note>();
    public void clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        list_midi_note = new List<midi_note>();
    }

    public void add_note_midi()
    {
        GameObject midi_note = Instantiate(midi_note_prefab);
        midi_note.transform.SetParent(transform);
        midi_note.transform.localScale = new Vector3(1f, 1f, 0f);
        midi_note.GetComponent<midi_note>().index_line = index_line;
        midi_note.GetComponent<midi_note>().index_note_midi = list_midi_note.Count;
        midi_note.GetComponent<midi_note>().type_note_piano = 0;
        midi_note.GetComponent<midi_note>().index_note_piano = -1;
        list_midi_note.Add(midi_note.GetComponent<midi_note>());
    }

    public void add_note_midi_by_length(int length_add_note)
    {
        for (int i = 0; i < length_add_note; i++)
        {
            GameObject midi_note = Instantiate(midi_note_prefab);
            midi_note.transform.SetParent(transform);
            midi_note.transform.localScale = new Vector3(1f, 1f, 0f);
            midi_note.GetComponent<midi_note>().index_line = index_line;
            midi_note.GetComponent<midi_note>().index_note_midi = list_midi_note.Count;
            midi_note.GetComponent<midi_note>().type_note_piano = 0;
            midi_note.GetComponent<midi_note>().index_note_piano = -1;
            list_midi_note.Add(midi_note.GetComponent<midi_note>());
        }
    }

    public void add_note_midi(string s_txt, int index_note_p, int type_note_p)
    {
        GameObject midi_note = Instantiate(midi_note_prefab);
        midi_note.transform.SetParent(transform);
        midi_note.transform.localScale = new Vector3(1f, 1f, 0f);
        midi_note.GetComponent<midi_note>().txt.text = s_txt;
        midi_note.GetComponent<midi_note>().index_note_piano = index_note_p;
        midi_note.GetComponent<midi_note>().index_line = index_line;
        midi_note.GetComponent<midi_note>().type_note_piano = type_note_p;
        midi_note.GetComponent<midi_note>().index_note_midi = list_midi_note.Count;
        list_midi_note.Add(midi_note.GetComponent<midi_note>());
    }

    public void no_select(Color32 colr)
    {
        for (int i = 0; i < list_midi_note.Count; i++)
        {
            list_midi_note[i].no_select(colr);
        }
    }

    public void ready_play(Color32 colr)
    {
        for (int i = 0; i < list_midi_note.Count; i++)
        {
            list_midi_note[i].no_select(colr);
        }
    }

    public void select_note(int index, Color32 colr)
    {
        list_midi_note[index].no_select(colr);
    }

    public midi_note get_midi_note(int index)
    {
        return list_midi_note[index];
    }

    public midi_note get_midi_note_last()
    {
        return list_midi_note[list_midi_note.Count - 1];
    }

    public int get_midi_note_length()
    {
        return list_midi_note.Count;
    }

    public int[] get_arr_int_index_note_piano()
    {
        int[] arr_p = new int[list_midi_note.Count];
        for (int i = 0; i < arr_p.Length; i++)
        {
            arr_p[i] = list_midi_note[i].index_note_piano;
        }
        return arr_p;
    }

    public int[] get_arr_type_note_piano()
    {
        int[] arr_type = new int[list_midi_note.Count];
        for (int i = 0; i < arr_type.Length; i++)
        {
            arr_type[i] = list_midi_note[i].type_note_piano;
        }
        return arr_type;
    }

    public void reset_note_show()
    {
        for (int i = 0; i < list_midi_note.Count; i++)
        {
            list_midi_note[i].rest_note_show();
        }
    }

    public void update_data()
    {
        for (int i = 0; i < list_midi_note.Count; i++)
        {
            list_midi_note[i].index_line = this.index_line;
        }
    }

    public void delete_note(int index_midi_edit)
    {
        if (this.list_midi_note[index_midi_edit] != null)
        {
            Destroy(this.list_midi_note[index_midi_edit]);
            Destroy(this.list_midi_note[index_midi_edit].gameObject);
            this.list_midi_note.RemoveAt(index_midi_edit);
        }
    }
}
