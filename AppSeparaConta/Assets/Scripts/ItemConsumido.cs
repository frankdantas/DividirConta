using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions.Must;
using System;
//using static UnityEditor.Progress;

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

    /// <summary>
    /// Recebe o objeto para preencher os dados na tela
    /// </summary>
    /// <param name="item"></param>
    public void SetValores(ItemConsumo item)
    {
        _itemConsumido = item;
        txtNome.text = item.ItemDoCardapio.Nome;
        txtDivisao.text = $"{item.PartesPagas}/{item.DivididoEm}";
        txtSubTotal.text = string.Format("{0:C2}", item.GetSubTotal());
    }

    public void AutoUpdate()
    {
        txtNome.text = _itemConsumido.ItemDoCardapio.Nome;
        txtDivisao.text = $"{_itemConsumido.PartesPagas}/{_itemConsumido.DivididoEm}";
        txtSubTotal.text = string.Format("{0:C2}", _itemConsumido.GetSubTotal());
    }

    /// <summary>
    /// Recupera o item
    /// </summary>
    /// <returns></returns>
    public ItemConsumo GetItem()
    {
        return _itemConsumido;
    }

    /// <summary>
    /// Excluir esse item
    /// </summary>
    public void onclick_excluirItem()
    {
        print("Excluir o item");
        Controle.SINGLETON.deletarItemConsumido(this.gameObject);
    }

    /// <summary>
    /// Editar esse item
    /// </summary>
    public void onclick_editar()
    {
        Controle.SINGLETON.SetItemParaeditar(this.gameObject);
        Controle.SINGLETON.onclick_abrirPainelEditItem();
    }
}
