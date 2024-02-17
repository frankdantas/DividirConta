using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class ConsumoTotal
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataHora { get; set; }
    public List<ItemConsumo> ListaConsumo { get; set; }

    public ConsumoTotal()
    {
        Nome = String.Empty;
        DataHora = DateTime.Now;
        ListaConsumo = new List<ItemConsumo>();
    }

    public ConsumoTotal(string _nome)
    {
        Nome = _nome;
        DataHora = DateTime.Now;
        ListaConsumo = new List<ItemConsumo>();
    }

    public double GetConsumoTotal()
    {
        return ListaConsumo.Sum(x => x.GetSubTotal());
    }
}
