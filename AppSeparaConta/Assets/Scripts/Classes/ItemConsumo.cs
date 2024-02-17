using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemConsumo
{
    public string Id;
    public int DivididoEm;
    public int PartesPagas;
    public ItemCardapio ItemDoCardapio = new ItemCardapio();

    public ItemConsumo()
    {
        Id = Guid.NewGuid().ToString();
        DivididoEm = 1;
        PartesPagas = 1;
    }
    public ItemConsumo(int _divididoEm, int _partesPagas, ItemCardapio _itemCardapio)
    {
        Id = Guid.NewGuid().ToString();
        DivididoEm = _divididoEm;
        PartesPagas = _partesPagas;
        ItemDoCardapio = _itemCardapio;
    }
    public ItemConsumo(string _id, int _divididoEm, int _partesPagas, ItemCardapio _itemCardapio)
    {
        Id = _id;
        DivididoEm = _divididoEm;
        PartesPagas = _partesPagas;
        ItemDoCardapio = _itemCardapio;
    }

    public double GetSubTotal()
    {
        return ItemDoCardapio.PrecoTotal / (double)DivididoEm * (double)PartesPagas;
    }

    public override string ToString()
    {
        return $"{ItemDoCardapio.Nome} - {PartesPagas}/{DivididoEm} = {GetSubTotal()}";
    }
}
