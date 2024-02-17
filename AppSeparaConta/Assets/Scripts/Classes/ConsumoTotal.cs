using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class ConsumoTotal
{
    public string Id;
    public string Nome;
    public DateTime DataHora;
    public List<ItemConsumo> ListaConsumo = new List<ItemConsumo>();
    public List<ItemCardapio> Cardapio = new List<ItemCardapio>();

    public ConsumoTotal()
    {
        Id = Guid.NewGuid().ToString();
        Nome = String.Empty;
        DataHora = DateTime.Now;
        ListaConsumo = new List<ItemConsumo>();
    }

    public ConsumoTotal(string _nome)
    {
        Id = Guid.NewGuid().ToString();
        Nome = _nome;
        DataHora = DateTime.Now;
        ListaConsumo = new List<ItemConsumo>();
    }

    public double GetConsumoTotal()
    {
        return ListaConsumo.Sum(x => x.GetSubTotal());
    }
}
