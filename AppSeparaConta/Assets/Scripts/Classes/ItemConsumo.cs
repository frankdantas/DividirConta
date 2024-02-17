using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ItemConsumo
{
    public Guid Id { get; set; }
    public int DivididoEm { get; set; }
    public int PartesPagas { get; set; }
    public ItemCardapio ItemDoCardapio { get; set; }

    public ItemConsumo()
    {
        Id = Guid.NewGuid();
        DivididoEm = 1;
        PartesPagas = 1;
    }
    public ItemConsumo(int _divididoEm, int _partesPagas, ItemCardapio _itemCardapio)
    {
        Id = Guid.NewGuid();
        DivididoEm = _divididoEm;
        PartesPagas = _partesPagas;
        ItemDoCardapio = _itemCardapio;
    }
    public ItemConsumo(Guid _id, int _divididoEm, int _partesPagas, ItemCardapio _itemCardapio)
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
