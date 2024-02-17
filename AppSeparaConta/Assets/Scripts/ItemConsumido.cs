using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions.Must;
using System;

public class ItemConsumido : MonoBehaviour
{
    public TMP_Text txtNome;
    public TMP_Text txtDivisao;
    public TMP_Text txtSubTotal;

    private ItemConsumo _itemConsumido;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValores(ItemConsumo item)
    {
        _itemConsumido = item;
        txtNome.text = item.ItemDoCardapio.Nome;
        txtDivisao.text = $"{item.PartesPagas}/{item.DivididoEm}";
        txtSubTotal.text = string.Format("{0:C2}", item.GetSubTotal());
    }

    public ItemConsumo GetItem()
    {
        return _itemConsumido;
    }

    public void onclick_excluirItem()
    {
        Controle.SINGLETON.deletarItemConsumido(this.gameObject);
    }

    public void onclick_editar()
    {
        Controle.SINGLETON.SetItemParaeditar(this.gameObject);
        Controle.SINGLETON.onclick_abrirPainelEditItem();
    }
}
